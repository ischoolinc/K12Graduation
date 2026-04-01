using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using FISCA.UDT;
using FISCA.DSAUtil;
using System.Xml;

namespace K12.Graduation.Modules
{
    public partial class TagView : NavView
    {
        private AccessHelper _AccessHelper = new AccessHelper();

        //依學生分類為單位
        //第一層文字 / 第二層文字 /學生資料
        private Dictionary<string, Dictionary<string, List<string>>> TagDic = new Dictionary<string, Dictionary<string, List<string>>>();

        //以學生為單位
        private Dictionary<string, GraduateUDT> TestDic1 = new Dictionary<string, GraduateUDT>();

        private BackgroundWorker _loadBGW = new BackgroundWorker();
        private string[] _pendingSource = null;

        string NoTag = "未分類";

        public TagView()
        {
            InitializeComponent();

            NavText = "依學生類別檢視";

            _loadBGW.WorkerSupportsCancellation = true;
            _loadBGW.DoWork += new DoWorkEventHandler(LoadBGW_DoWork);
            _loadBGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadBGW_RunWorkerCompleted);

            SourceChanged += new EventHandler(TagView_SourceChanged);
        }

        void TagView_SourceChanged(object sender, EventArgs e)
        {
            string[] snapshot = Source.ToArray();
            if (_loadBGW.IsBusy)
            {
                _pendingSource = snapshot;
                _loadBGW.CancelAsync();
                return;
            }
            _pendingSource = null;
            _loadBGW.RunWorkerAsync(snapshot);
        }

        void LoadBGW_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            string[] sourceArray = (string[])e.Argument;

            var tagDic = new Dictionary<string, Dictionary<string, List<string>>>();
            var testDic1 = new Dictionary<string, GraduateUDT>();

            if (sourceArray.Length == 0)
            {
                e.Result = Tuple.Create(tagDic, testDic1);
                return;
            }

            // Deduplicate source UIDs with O(1) HashSet
            var uniqueList = new HashSet<string>(sourceArray).ToList();

            if (worker.CancellationPending) { e.Cancel = true; return; }

            List<GraduateUDT> testList = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("UID", uniqueList));

            if (worker.CancellationPending) { e.Cancel = true; return; }

            testList.Sort(SortClassName);

            foreach (GraduateUDT obj in testList)
            {
                #region 依學生類別
                if (!string.IsNullOrEmpty(obj.Tag))
                {
                    XmlElement xmlelement = DSXmlHelper.LoadXml(obj.Tag);
                    foreach (XmlElement xmlE in xmlelement.SelectNodes("Tag"))
                    {
                        string Prefix = xmlE.GetAttribute("Prefix");
                        string Name = xmlE.GetAttribute("Name");
                        if (!tagDic.ContainsKey(Prefix))
                            tagDic.Add(Prefix, new Dictionary<string, List<string>>());
                        if (!tagDic[Prefix].ContainsKey(Name))
                            tagDic[Prefix].Add(Name, new List<string>());
                        tagDic[Prefix][Name].Add(obj.UID);
                    }
                }
                else
                {
                    if (!tagDic.ContainsKey(NoTag))
                        tagDic.Add(NoTag, new Dictionary<string, List<string>>());
                    if (!tagDic[NoTag].ContainsKey(NoTag))
                        tagDic[NoTag].Add(NoTag, new List<string>());
                    tagDic[NoTag][NoTag].Add(obj.UID);
                }
                #endregion

                #region 建立所有學生記錄
                if (!testDic1.ContainsKey(obj.UID))
                    testDic1.Add(obj.UID, obj);
                #endregion
            }

            e.Result = Tuple.Create(tagDic, testDic1);
        }

        void LoadBGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // SourceChanged fired again while loading; start the queued request
                if (_pendingSource != null)
                {
                    string[] next = _pendingSource;
                    _pendingSource = null;
                    _loadBGW.RunWorkerAsync(next);
                }
                return;
            }

            var result = (Tuple<Dictionary<string, Dictionary<string, List<string>>>, Dictionary<string, GraduateUDT>>)e.Result;
            TagDic = result.Item1;
            TestDic1 = result.Item2;
            BuildTree();
        }

        private void BuildTree()
        {
            advTree1.BeginUpdate();
            advTree1.Nodes.Clear();

            DevComponents.AdvTree.Node Node1 = new DevComponents.AdvTree.Node();
            Node1.Text = "依學生類別(" + TestDic1.Count + ")";
            Node1.Tag = "All";
            advTree1.Nodes.Add(Node1);

            foreach (string each in TagDic.Keys)
            {
                if (each == NoTag)
                    continue;

                if (!string.IsNullOrEmpty(each))
                {
                    // Use HashSet for O(1) deduplication
                    var uniqueSet = new HashSet<string>();
                    foreach (string each2 in TagDic[each].Keys)
                        foreach (string each3 in TagDic[each][each2])
                            uniqueSet.Add(each3);

                    DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                    Node2.Text = each + "(" + uniqueSet.Count + ")";
                    Node2.Tag = each;
                    Node1.Nodes.Add(Node2);

                    foreach (string each2 in TagDic[each].Keys)
                    {
                        TagViewObj obj = new TagViewObj(each, each2);
                        DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                        Node3.Text = each2 + "(" + TagDic[each][each2].Count + ")";
                        Node3.Tag = obj;
                        Node2.Nodes.Add(Node3);
                    }
                }
                else
                {
                    // 未分群組
                    var uniqueSet = new HashSet<string>();
                    foreach (string each2 in TagDic[each].Keys)
                        foreach (string each3 in TagDic[each][each2])
                            uniqueSet.Add(each3);

                    foreach (string each2 in TagDic[each].Keys)
                    {
                        TagViewObj obj = new TagViewObj(each, each2);
                        DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                        Node3.Text = each2 + "(" + TagDic[each][each2].Count + ")";
                        Node3.Tag = obj;
                        Node1.Nodes.Add(Node3);
                    }
                }
            }

            if (TagDic.ContainsKey(NoTag))
            {
                if (TagDic[NoTag].ContainsKey(NoTag))
                {
                    DevComponents.AdvTree.Node Node4 = new DevComponents.AdvTree.Node();
                    Node4.Text = NoTag + "(" + TagDic[NoTag][NoTag].Count + ")";
                    Node4.Tag = NoTag;
                    Node1.Nodes.Add(Node4);
                }
            }

            advTree1.EndUpdate();
        }

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            bool SelectedAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            bool AddToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

            if (e.Node.Tag is string)
            {
                string tag = "" + e.Node.Tag;
                if (TagDic.ContainsKey(tag))
                {
                    // Use HashSet to deduplicate UIDs across sub-tags
                    var uniqueSet = new HashSet<string>();
                    foreach (string each1 in TagDic[tag].Keys)
                        foreach (string each2 in TagDic[tag][each1])
                            uniqueSet.Add(each2);
                    SetListPaneSource(uniqueSet.ToList(), SelectedAll, AddToTemp);
                }
                else if (tag == NoTag)
                {
                    SetListPaneSource(TagDic[NoTag][NoTag], SelectedAll, AddToTemp);
                }
                else if (tag == "ALL")
                {
                    SetListPaneSource(TestDic1.Keys, SelectedAll, AddToTemp);
                }
            }
            else if (e.Node.Tag is TagViewObj)
            {
                TagViewObj obj = (TagViewObj)e.Node.Tag;
                foreach (string each1 in TagDic.Keys)
                {
                    if (each1 != obj._Prefix)
                        continue;
                    foreach (string each2 in TagDic[each1].Keys)
                    {
                        if (each2 != obj._Name)
                            continue;
                        SetListPaneSource(TagDic[each1][each2], SelectedAll, AddToTemp);
                    }
                }
            }
        }

        private int SortClassName(GraduateUDT obj1, GraduateUDT obj2)
        {
            string aaaa1 = obj1.ClassName.PadLeft(10, '0');
            string aaaa2 = obj1.SeatNo.HasValue ? obj1.SeatNo.Value.ToString().PadLeft(10, '0') : "0000000000";
            aaaa1 += aaaa2;
            string bbbb1 = obj2.ClassName.PadLeft(10, '0');
            string bbbb2 = obj2.SeatNo.HasValue ? obj2.SeatNo.Value.ToString().PadLeft(10, '0') : "0000000000";
            bbbb1 += bbbb2;
            return aaaa1.CompareTo(bbbb1);
        }
    }
}
