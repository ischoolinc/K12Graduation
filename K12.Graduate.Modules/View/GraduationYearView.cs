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
    //依畢業年度檢視
    public partial class GraduationYearView : NavView
    {
        private AccessHelper _AccessHelper = new AccessHelper();

        //依學年為單位
        private Dictionary<int, SchoolYearObj> SchoolYearDic = new Dictionary<int, SchoolYearObj>();
        //依班及為單位
        private Dictionary<string, List<GraduateUDT>> TestDic1 = new Dictionary<string, List<GraduateUDT>>();
        //以學生為單位
        private Dictionary<string, GraduateUDT> TestDic2 = new Dictionary<string, GraduateUDT>();

        private BackgroundWorker _loadBGW = new BackgroundWorker();
        private string[] _pendingSource = null;

        public GraduationYearView()
        {
            InitializeComponent();

            NavText = "依畢業年檢視";

            _loadBGW.WorkerSupportsCancellation = true;
            _loadBGW.DoWork += new DoWorkEventHandler(LoadBGW_DoWork);
            _loadBGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadBGW_RunWorkerCompleted);

            SourceChanged += new EventHandler(ExtracurricularActivitiesView_SourceChanged);
        }

        void ExtracurricularActivitiesView_SourceChanged(object sender, EventArgs e)
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

            var schoolYearDic = new Dictionary<int, SchoolYearObj>();
            var testDic1 = new Dictionary<string, List<GraduateUDT>>();
            var testDic2 = new Dictionary<string, GraduateUDT>();
            var testList = new List<GraduateUDT>();

            if (sourceArray.Length > 0)
            {
                var uniqueList = sourceArray.Distinct().ToList();

                if (worker.CancellationPending) { e.Cancel = true; return; }

                testList = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("UID", uniqueList));

                if (worker.CancellationPending) { e.Cancel = true; return; }

                testList.Sort(SortClassName);

                foreach (GraduateUDT obj in testList)
                {
                    #region 建立依畢業學年度記錄
                    if (obj.GraduateSchoolYear.HasValue)
                    {
                        if (!schoolYearDic.ContainsKey(obj.GraduateSchoolYear.Value))
                            schoolYearDic.Add(obj.GraduateSchoolYear.Value, new SchoolYearObj(obj.GraduateSchoolYear.Value));
                        if (!schoolYearDic[obj.GraduateSchoolYear.Value]._ClassNameList.ContainsKey(obj.ClassName))
                            schoolYearDic[obj.GraduateSchoolYear.Value]._ClassNameList.Add(obj.ClassName, new List<GraduateUDT>());
                        schoolYearDic[obj.GraduateSchoolYear.Value]._ClassNameList[obj.ClassName].Add(obj);
                    }
                    else
                    {
                        if (!schoolYearDic.ContainsKey(0))
                            schoolYearDic.Add(0, new SchoolYearObj(0));
                        if (!schoolYearDic[0]._ClassNameList.ContainsKey(obj.ClassName))
                            schoolYearDic[0]._ClassNameList.Add(obj.ClassName, new List<GraduateUDT>());
                        schoolYearDic[0]._ClassNameList[obj.ClassName].Add(obj);
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

            schoolYearDic = SortSchoolYearDic(schoolYearDic);

            e.Result = Tuple.Create(schoolYearDic, testDic1, testDic2, testList);
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

            var result = (Tuple<Dictionary<int, SchoolYearObj>, Dictionary<string, List<GraduateUDT>>, Dictionary<string, GraduateUDT>, List<GraduateUDT>>)e.Result;
            SchoolYearDic = result.Item1;
            TestDic1 = result.Item2;
            TestDic2 = result.Item3;
            List<GraduateUDT> testList = result.Item4;

            BuildTree();

            List<string> _Source = testList.Select(x => x.UID).ToList();
            SetListPaneSource(_Source, false, false);
        }

        private void BuildTree()
        {
            advTree1.BeginUpdate();
            advTree1.Nodes.Clear();

            #region 增加Node
            DevComponents.AdvTree.Node Node1 = new DevComponents.AdvTree.Node();
            Node1.Text = "依畢業年(" + TestDic2.Count + ")";
            Node1.Tag = "All";
            advTree1.Nodes.Add(Node1);

            foreach (int each1 in SchoolYearDic.Keys)
            {
                int ClassStudentCount = 0;
                foreach (string each2 in SchoolYearDic[each1]._ClassNameList.Keys)
                    ClassStudentCount += SchoolYearDic[each1]._ClassNameList[each2].Count;

                if (each1 != 0)
                {
                    DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                    Node2.Text = each1 + "學年度畢業生(" + ClassStudentCount + ")";
                    Node2.Tag = each1;
                    Node1.Nodes.Add(Node2);

                    foreach (string each3 in SchoolYearDic[each1]._ClassNameList.Keys)
                    {
                        DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                        Node3.Text = each3 + "(" + SchoolYearDic[each1]._ClassNameList[each3].Count() + ")";
                        Node3.Tag = each3;
                        Node2.Nodes.Add(Node3);
                    }
                }
            }

            if (SchoolYearDic.ContainsKey(0))
            {
                int ClassStudentCount = 0;
                foreach (string each2 in SchoolYearDic[0]._ClassNameList.Keys)
                    ClassStudentCount += SchoolYearDic[0]._ClassNameList[each2].Count;

                DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                Node2.Text = "未分學年度(" + ClassStudentCount + ")";
                Node2.Tag = 0;
                Node1.Nodes.Add(Node2);

                foreach (string each3 in SchoolYearDic[0]._ClassNameList.Keys)
                {
                    DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                    Node3.Text = each3 + "(" + SchoolYearDic[0]._ClassNameList[each3].Count() + ")";
                    Node3.Tag = each3;
                    Node2.Nodes.Add(Node3);
                }
            }
            #endregion

            advTree1.EndUpdate();
        }

        private Dictionary<int, SchoolYearObj> SortSchoolYearDic(Dictionary<int, SchoolYearObj> SchoolYearDic)
        {
            Dictionary<int, SchoolYearObj> dic = new Dictionary<int, SchoolYearObj>();
            List<int> intList = new List<int>();
            foreach (int each in SchoolYearDic.Keys)
                intList.Add(each);
            intList.Sort();
            foreach (int each in intList)
                dic.Add(each, SchoolYearDic[each]);
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

            if (e.Node.Tag is string)
            {
                #region 當使用者是選取標頭(所有學生)
                if ("" + e.Node.Tag == "All")
                {
                    SetListPaneSource(TestDic2.Keys, SelectedAll, AddToTemp);
                }
                else if (TestDic1.ContainsKey("" + e.Node.Tag))
                {
                    if (e.Node.Parent.Tag is int)
                    {
                        int CountX = (int)e.Node.Parent.Tag;
                        if (SchoolYearDic.ContainsKey(CountX))
                        {
                            if (SchoolYearDic[CountX]._ClassNameList.ContainsKey("" + e.Node.Tag))
                                SetListPaneSource(SchoolYearDic[CountX]._ClassNameList["" + e.Node.Tag].Select(x => x.UID), SelectedAll, AddToTemp);
                        }
                    }
                }
                #endregion
            }
            else if (e.Node.Tag is int)
            {
                #region 如果是數字(是學年度Node)
                List<string> ClassStudent = new List<string>();
                int CountX = (int)e.Node.Tag;
                if (SchoolYearDic.ContainsKey(CountX))
                {
                    foreach (string each in SchoolYearDic[CountX]._ClassNameList.Keys)
                        ClassStudent.AddRange(SchoolYearDic[CountX]._ClassNameList[each].Select(x => x.UID));
                }
                SetListPaneSource(ClassStudent, SelectedAll, AddToTemp);
                #endregion
            }
        }
    }
}
