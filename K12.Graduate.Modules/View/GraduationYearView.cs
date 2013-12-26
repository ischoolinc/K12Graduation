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
    public partial class GraduationYearView :NavView
    {
        private List<string> mPrimaryKeys = new List<string>();

        private AccessHelper _AccessHelper = new AccessHelper();

        //依學年為單位
        private Dictionary<int, SchoolYearObj> SchoolYearDic = new Dictionary<int, SchoolYearObj>();
        //依班及為單位
        private Dictionary<string, List<GraduateUDT>> TestDic1 = new Dictionary<string, List<GraduateUDT>>();
        //以學生為單位
        private Dictionary<string, GraduateUDT> TestDic2 = new Dictionary<string, GraduateUDT>();

        public GraduationYearView()
        {
            InitializeComponent();

            //選取的結點的完整路徑
            //mPrimaryKeys = new List<string>(Source);

            NavText = "依畢業年檢視";
 
            SourceChanged += new EventHandler(ExtracurricularActivitiesView_SourceChanged);
        }

        //private Dictionary<DevComponents.AdvTree.Node, List<string>> items = new Dictionary<DevComponents.AdvTree.Node, List<string>>();

        void ExtracurricularActivitiesView_SourceChanged(object sender, EventArgs e)
        {
            TestDic1.Clear();
            TestDic2.Clear();
            SchoolYearDic.Clear();

            List<GraduateUDT> TestList = new List<GraduateUDT>();
            if (Source.Count() != 0)
            {
                List<string> list = new List<string>();
                foreach (string each in Source)
                    list.Add(each);
                //取得資料
                TestList = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("UID", list));
            }

            //排序
            TestList.Sort(SortClassName);

            foreach (GraduateUDT obj in TestList)
            {
                #region 建立依畢業學年度記錄
                if (obj.GraduateSchoolYear.HasValue)
                {
                    if (!SchoolYearDic.ContainsKey(obj.GraduateSchoolYear.Value)) //有畢業年度
                    {
                        SchoolYearDic.Add(obj.GraduateSchoolYear.Value, new SchoolYearObj(obj.GraduateSchoolYear.Value));
                    }

                    if (!SchoolYearDic[obj.GraduateSchoolYear.Value]._ClassNameList.ContainsKey(obj.ClassName)) //是否為已存在班級
                    {
                        SchoolYearDic[obj.GraduateSchoolYear.Value]._ClassNameList.Add(obj.ClassName, new List<GraduateUDT>());
                    }

                    SchoolYearDic[obj.GraduateSchoolYear.Value]._ClassNameList[obj.ClassName].Add(obj);
                }
                else //沒有學年度,加入0學年度
                {
                    if (!SchoolYearDic.ContainsKey(0)) //有畢業年度
                    {
                        SchoolYearDic.Add(0, new SchoolYearObj(0));
                    }

                    if (!SchoolYearDic[0]._ClassNameList.ContainsKey(obj.ClassName)) //是否為已存在班級
                    {
                        SchoolYearDic[0]._ClassNameList.Add(obj.ClassName, new List<GraduateUDT>());
                    }

                    SchoolYearDic[0]._ClassNameList[obj.ClassName].Add(obj);
                }  
                
                #endregion

                #region 建立依班級學生記錄
                if (!TestDic1.ContainsKey(obj.ClassName))
                {
                    TestDic1.Add(obj.ClassName, new List<GraduateUDT>());
                }

                TestDic1[obj.ClassName].Add(obj);                

                #endregion

                #region 建立所有學生記錄
                if (!TestDic2.ContainsKey(obj.UID))
                {
                    TestDic2.Add(obj.UID, obj);
                } 
                #endregion

            }

            SchoolYearDic = SortSchoolYearDic(SchoolYearDic);

            advTree1.Nodes.Clear();
            #region 增加Node

            //第一層Node
            DevComponents.AdvTree.Node Node1 = new DevComponents.AdvTree.Node();
            Node1.Text = "依畢業年(" + TestDic2.Count() + ")";
            Node1.Tag = "All";           
            advTree1.Nodes.Add(Node1); //加入
            //advTree1.SelectedNode = Node1; //預設選擇

            //學年度Node
            foreach (int each1 in SchoolYearDic.Keys)
            {
                //Count該學年度有多少學生
                int ClassStudentCount = 0;
                foreach (string each2 in SchoolYearDic[each1]._ClassNameList.Keys)
                {
                    ClassStudentCount += SchoolYearDic[each1]._ClassNameList[each2].Count;
                }

                if (each1 != 0) //如果是無畢業年度者
                {
                    //增加學年度Node
                    DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                    Node2.Text = each1 + "學年度畢業生(" + ClassStudentCount + ")";
                    Node2.Tag = each1;
                    Node1.Nodes.Add(Node2);

                    //班級名稱Node
                    foreach (string each3 in SchoolYearDic[each1]._ClassNameList.Keys)
                    {
                        DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                        Node3.Text = each3 + "(" + SchoolYearDic[each1]._ClassNameList[each3].Count() + ")";
                        Node3.Tag = each3;
                        Node2.Nodes.Add(Node3);
                    }
                }
            }

            //未分學年度
            if (SchoolYearDic.ContainsKey(0))
            {
                //Count該學年度有多少學生
                int ClassStudentCount = 0;
                foreach (string each2 in SchoolYearDic[0]._ClassNameList.Keys)
                {
                    ClassStudentCount += SchoolYearDic[0]._ClassNameList[each2].Count;
                }

                //增加未分學年度Node
                DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                Node2.Text = "未分學年度(" + ClassStudentCount + ")";
                Node2.Tag = 0;
                Node1.Nodes.Add(Node2);

                //班級名稱Node
                foreach (string each3 in SchoolYearDic[0]._ClassNameList.Keys)
                {
                    DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                    Node3.Text = each3 + "(" + SchoolYearDic[0]._ClassNameList[each3].Count() + ")";
                    Node3.Tag = each3;
                    Node2.Nodes.Add(Node3);
                }
            }
            #endregion

            List<string> _Source = TestList.Select(x => x.UID).ToList();
            SetListPaneSource(_Source, false, false);
        }

        private Dictionary<int, SchoolYearObj> SortSchoolYearDic(Dictionary<int, SchoolYearObj> SchoolYearDic)
        {
            Dictionary<int, SchoolYearObj> dic = new Dictionary<int,SchoolYearObj>();
            List<int> intList = new List<int>();
            foreach(int each in SchoolYearDic.Keys)
            {
                intList.Add(each);
            }
            intList.Sort();
            foreach (int each in intList)
            {
                dic.Add(each, SchoolYearDic[each]);
            }
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

        //private DevComponents.AdvTree.Node SelectNode(List<string> selectPath, int level, DevComponents.AdvTree.NodeCollection nodeCollection)
        //{

        //}

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            //判斷是否有按Control,Shift
            bool SelectedAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            bool AddToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            //傳入ID

            if (e.Node.Tag is string) //如果是字串(是所有學生 or 班級名稱Node)
            {
                #region 當使用者是選取標頭(所有學生)
                if ("" + e.Node.Tag == "All")
                {
                    SetListPaneSource(TestDic2.Keys, SelectedAll, AddToTemp);
                }
                else if (TestDic1.ContainsKey("" + e.Node.Tag)) //當使用者是選取班級名稱
                {
                    if (e.Node.Parent.Tag is int) //取得上層Node是否為學年度Node
                    {
                        int CountX = (int)e.Node.Parent.Tag;
                        if (SchoolYearDic.ContainsKey(CountX))
                        {
                            if(SchoolYearDic[CountX]._ClassNameList.ContainsKey(""+e.Node.Tag))
                            {
                                SetListPaneSource(SchoolYearDic[CountX]._ClassNameList["" + e.Node.Tag].Select(x => x.UID), SelectedAll, AddToTemp);
                            }
                        }
                    }
                }
                else //未選取
                {

                } 
                #endregion
            }
            else if (e.Node.Tag is int)
            {
                #region 如果是數字(是學年度Node)
                //取得該學年所有學生清單
                List<string> ClassStudent = new List<string>();
                int CountX = (int)e.Node.Tag;
                if (SchoolYearDic.ContainsKey(CountX))
                {
                    foreach (string each in SchoolYearDic[CountX]._ClassNameList.Keys)
                    {
                        List<string> list = SchoolYearDic[CountX]._ClassNameList[each].Select(x => x.UID).ToList();
                        ClassStudent.AddRange(list);
                    }
                }
                SetListPaneSource(ClassStudent, SelectedAll, AddToTemp); 
                #endregion
            }
        }

    }
}
