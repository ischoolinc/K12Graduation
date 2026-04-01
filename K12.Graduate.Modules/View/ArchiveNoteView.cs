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

namespace K12.Graduation.Modules
{
    //依索引分類檢視
    public partial class ArchiveNoteView : NavView
    {
        private AccessHelper _AccessHelper = new AccessHelper();

        //依班及為單位
        private Dictionary<string, List<GraduateUDT>> TestDic1 = new Dictionary<string, List<GraduateUDT>>();

        //依分類為單位
        private Dictionary<string, NoteObj> CategoryDic = new Dictionary<string, NoteObj>();

        //以學生為單位
        private Dictionary<string, GraduateUDT> TestDic2 = new Dictionary<string, GraduateUDT>();

        private BackgroundWorker _loadBGW = new BackgroundWorker();
        private string[] _pendingSource = null;

        string NoArchive = "未分類";

        public ArchiveNoteView()
        {
            InitializeComponent();

            NavText = "依索引分類檢視";

            _loadBGW.WorkerSupportsCancellation = true;
            _loadBGW.DoWork += new DoWorkEventHandler(LoadBGW_DoWork);
            _loadBGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadBGW_RunWorkerCompleted);

            SourceChanged += new EventHandler(NoteView_SourceChanged);
        }

        void NoteView_SourceChanged(object sender, EventArgs e)
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

            var categoryDic = new Dictionary<string, NoteObj>();
            var testDic1 = new Dictionary<string, List<GraduateUDT>>();
            var testDic2 = new Dictionary<string, GraduateUDT>();
            var testList = new List<GraduateUDT>();

            if (sourceArray.Length > 0)
            {
                // Use Distinct() to deduplicate source UIDs before querying
                var uniqueList = sourceArray.Distinct().ToList();

                if (worker.CancellationPending) { e.Cancel = true; return; }

                testList = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("UID", uniqueList));

                if (worker.CancellationPending) { e.Cancel = true; return; }

                testList.Sort(SortClassName);

                foreach (GraduateUDT obj in testList)
                {
                    #region 建立依分類記錄
                    if (!string.IsNullOrEmpty(obj.ArchiveNote))
                    {
                        if (!categoryDic.ContainsKey(obj.ArchiveNote))
                            categoryDic.Add(obj.ArchiveNote, new NoteObj(obj.ArchiveNote));
                        if (!categoryDic[obj.ArchiveNote]._ClassNameList.ContainsKey(obj.ClassName))
                            categoryDic[obj.ArchiveNote]._ClassNameList.Add(obj.ClassName, new List<GraduateUDT>());
                        categoryDic[obj.ArchiveNote]._ClassNameList[obj.ClassName].Add(obj);
                    }
                    else
                    {
                        if (!categoryDic.ContainsKey(NoArchive))
                            categoryDic.Add(NoArchive, new NoteObj(obj.ArchiveNote));
                        if (!categoryDic[NoArchive]._ClassNameList.ContainsKey(obj.ClassName))
                            categoryDic[NoArchive]._ClassNameList.Add(obj.ClassName, new List<GraduateUDT>());
                        categoryDic[NoArchive]._ClassNameList[obj.ClassName].Add(obj);
                    }
                    #endregion

                    #region 建立依班級學生記錄
                    if (!testDic1.ContainsKey(obj.ClassName))
                        testDic1.Add(obj.ClassName, new List<GraduateUDT>());
                    testDic1[obj.ClassName].Add(obj);
                    #endregion

                    #region 建立所有學生記錄
                    if (!testDic2.ContainsKey(obj.UID))
                        testDic2.Add(obj.UID, obj);
                    #endregion
                }
            }

            categoryDic = SortCategoryDic(categoryDic);

            e.Result = Tuple.Create(categoryDic, testDic1, testDic2, testList);
        }

        void LoadBGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (_pendingSource != null)
                {
                    string[] next = _pendingSource;
                    _pendingSource = null;
                    _loadBGW.RunWorkerAsync(next);
                }
                return;
            }

            var result = (Tuple<Dictionary<string, NoteObj>, Dictionary<string, List<GraduateUDT>>, Dictionary<string, GraduateUDT>, List<GraduateUDT>>)e.Result;
            CategoryDic = result.Item1;
            TestDic1 = result.Item2;
            TestDic2 = result.Item3;
            List<GraduateUDT> testList = result.Item4;

            BuildTree();

            // Use Distinct() instead of O(n²) List.Contains() dedup
            List<string> _Source = testList.Select(x => x.UID).Distinct().ToList();
            SetListPaneSource(_Source, false, false);
        }

        private void BuildTree()
        {
            advTree1.BeginUpdate();
            advTree1.Nodes.Clear();

            #region 增加Node
            DevComponents.AdvTree.Node Node1 = new DevComponents.AdvTree.Node();
            Node1.Text = "依索引分類(" + TestDic2.Count + ")";
            Node1.Tag = "All";
            advTree1.Nodes.Add(Node1);

            foreach (string each1 in CategoryDic.Keys)
            {
                int ClassStudentCount = 0;
                foreach (string each2 in CategoryDic[each1]._ClassNameList.Keys)
                    ClassStudentCount += CategoryDic[each1]._ClassNameList[each2].Count;

                if (each1 != NoArchive)
                {
                    DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                    Node2.Text = each1 + "(" + ClassStudentCount + ")";
                    Node2.Tag = each1;
                    Node1.Nodes.Add(Node2);

                    foreach (string each3 in CategoryDic[each1]._ClassNameList.Keys)
                    {
                        DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                        Node3.Text = each3 + "(" + CategoryDic[each1]._ClassNameList[each3].Count() + ")";
                        Node3.Tag = each3;
                        Node2.Nodes.Add(Node3);
                    }
                }
            }

            if (CategoryDic.ContainsKey(NoArchive))
            {
                int ClassStudentCount = 0;
                foreach (string each2 in CategoryDic[NoArchive]._ClassNameList.Keys)
                    ClassStudentCount += CategoryDic[NoArchive]._ClassNameList[each2].Count;

                DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                Node2.Text = NoArchive + "(" + ClassStudentCount + ")";
                Node2.Tag = NoArchive;
                Node1.Nodes.Add(Node2);

                foreach (string each3 in CategoryDic[NoArchive]._ClassNameList.Keys)
                {
                    DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                    Node3.Text = each3 + "(" + CategoryDic[NoArchive]._ClassNameList[each3].Count() + ")";
                    Node3.Tag = each3;
                    Node2.Nodes.Add(Node3);
                }
            }
            #endregion

            advTree1.EndUpdate();
        }

        private Dictionary<string, NoteObj> SortCategoryDic(Dictionary<string, NoteObj> CategoryDic)
        {
            Dictionary<string, NoteObj> dic = new Dictionary<string, NoteObj>();
            List<string> stringList = new List<string>();
            foreach (string each in CategoryDic.Keys)
                stringList.Add(each);
            stringList.Sort();
            foreach (string each in stringList)
                dic.Add(each, CategoryDic[each]);
            return dic;
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

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            bool SelectedAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            bool AddToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

            if (!CategoryDic.ContainsKey("" + e.Node.Tag))
            {
                #region 當使用者是選取標頭(所有學生)
                if ("" + e.Node.Tag == "All")
                {
                    SetListPaneSource(TestDic2.Keys, SelectedAll, AddToTemp);
                }
                else if (TestDic1.ContainsKey("" + e.Node.Tag))
                {
                    if (CategoryDic.ContainsKey("" + e.Node.Parent.Tag))
                    {
                        string StringX = (string)e.Node.Parent.Tag;
                        if (CategoryDic.ContainsKey(StringX))
                        {
                            if (CategoryDic[StringX]._ClassNameList.ContainsKey("" + e.Node.Tag))
                            {
                                List<string> list = CategoryDic[StringX]._ClassNameList["" + e.Node.Tag]
                                    .Select(x => x.UID).Distinct().ToList();
                                SetListPaneSource(list, SelectedAll, AddToTemp);
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region 如果是數字(是學年度Node)
                List<string> ClassStudent = new List<string>();
                string StringX = (string)e.Node.Tag;
                if (CategoryDic.ContainsKey(StringX))
                {
                    foreach (string each in CategoryDic[StringX]._ClassNameList.Keys)
                    {
                        List<string> list = CategoryDic[StringX]._ClassNameList[each].Select(x => x.UID).ToList();
                        ClassStudent.AddRange(list);
                    }
                }
                SetListPaneSource(ClassStudent, SelectedAll, AddToTemp);
                #endregion
            }
        }
    }
}
