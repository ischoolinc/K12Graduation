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
    public partial class ArchiveNoteView : NavView
    {

        private AccessHelper _AccessHelper = new AccessHelper();

        //依班及為單位
        private Dictionary<string, List<GraduateUDT>> TestDic1 = new Dictionary<string, List<GraduateUDT>>();

        //依分類為單位
        private Dictionary<string, NoteObj> CategoryDic = new Dictionary<string, NoteObj>();

        //以學生為單位
        private Dictionary<string, GraduateUDT> TestDic2 = new Dictionary<string, GraduateUDT>();

        string NoArchive = "未分類";

        public ArchiveNoteView()
        {
            InitializeComponent();

            NavText = "依索引分類檢視";

            SourceChanged += new EventHandler(NoteView_SourceChanged);
        }

        void NoteView_SourceChanged(object sender, EventArgs e)
        {
            TestDic1.Clear();
            TestDic2.Clear();
            CategoryDic.Clear();

            List<GraduateUDT> TestList = new List<GraduateUDT>();
            if (Source.Count() != 0)
            {
                List<string> list = new List<string>();
                foreach(string each in Source)
                    list.Add(each);
                //取得資料
                TestList = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("UID", list));
            }

            //排序
            TestList.Sort(SortClassName);

            foreach (GraduateUDT obj in TestList)
            {
                #region 建立依分類記錄
                if (!string.IsNullOrEmpty(obj.ArchiveNote))
                {
                    if (!CategoryDic.ContainsKey(obj.ArchiveNote)) //有畢業年度
                    {
                        CategoryDic.Add(obj.ArchiveNote, new NoteObj(obj.ArchiveNote));
                    }

                    if (!CategoryDic[obj.ArchiveNote]._ClassNameList.ContainsKey(obj.ClassName)) //是否為已存在班級
                    {
                        CategoryDic[obj.ArchiveNote]._ClassNameList.Add(obj.ClassName, new List<GraduateUDT>());
                    }


                    CategoryDic[obj.ArchiveNote]._ClassNameList[obj.ClassName].Add(obj);
                }
                else
                {
                    if (!CategoryDic.ContainsKey(NoArchive)) //有畢業年度
                    {
                        CategoryDic.Add(NoArchive, new NoteObj(obj.ArchiveNote));
                    }

                    if (!CategoryDic[NoArchive]._ClassNameList.ContainsKey(obj.ClassName)) //是否為已存在班級
                    {
                        CategoryDic[NoArchive]._ClassNameList.Add(obj.ClassName, new List<GraduateUDT>());
                    }


                    CategoryDic[NoArchive]._ClassNameList[obj.ClassName].Add(obj);
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

            CategoryDic = SortCategoryDic(CategoryDic);

            advTree1.Nodes.Clear();

            #region 增加Node

            //第一層Node
            DevComponents.AdvTree.Node Node1 = new DevComponents.AdvTree.Node();
            Node1.Text = "依索引分類(" + TestDic2.Count() + ")";
            Node1.Tag = "All";
            advTree1.Nodes.Add(Node1); //加入

            //分類Node
            foreach (string each1 in CategoryDic.Keys)
            {
                //Count該學年度有多少學生
                int ClassStudentCount = 0;
                foreach (string each2 in CategoryDic[each1]._ClassNameList.Keys)
                {
                    ClassStudentCount += CategoryDic[each1]._ClassNameList[each2].Count;
                }

                if (each1 != NoArchive) //如果是無分類者
                {
                    //增加分類Node
                    DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                    Node2.Text = each1 + "(" + ClassStudentCount + ")";
                    Node2.Tag = each1;
                    Node1.Nodes.Add(Node2);

                    //班級名稱Node
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
                //Count該學年度有多少學生
                int ClassStudentCount = 0;
                foreach (string each2 in CategoryDic[NoArchive]._ClassNameList.Keys)
                {
                    ClassStudentCount += CategoryDic[NoArchive]._ClassNameList[each2].Count;
                }

                //增加無分類Node
                DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                Node2.Text = NoArchive + "(" + ClassStudentCount + ")";
                Node2.Tag = NoArchive;
                Node1.Nodes.Add(Node2);

                //班級名稱Node
                foreach (string each3 in CategoryDic[NoArchive]._ClassNameList.Keys)
                {
                    DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                    Node3.Text = each3 + "(" + CategoryDic[NoArchive]._ClassNameList[each3].Count() + ")";
                    Node3.Tag = each3;
                    Node2.Nodes.Add(Node3);
                }
            }
            #endregion

            List<string> _Source = new List<string>();
            foreach (GraduateUDT each in TestList)
            {
                if (!_Source.Contains(each.UID))
                {
                    _Source.Add(each.UID);
                }
            }
            SetListPaneSource(_Source, false, false);
        }

        private Dictionary<string, NoteObj> SortCategoryDic(Dictionary<string, NoteObj> CategoryDic)
        {
            Dictionary<string, NoteObj> dic = new Dictionary<string, NoteObj>();
            List<string> stringList = new List<string>();
            foreach (string each in CategoryDic.Keys)
            {
                stringList.Add(each);
            }
            stringList.Sort();
            foreach (string each in stringList)
            {
                dic.Add(each, CategoryDic[each]);
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

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            //判斷是否有按Control,Shift
            bool SelectedAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            bool AddToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            //傳入ID

            if (!CategoryDic.ContainsKey("" + e.Node.Tag)) //如果不是未分類(是所有學生 or 班級名稱Node)
            {
                #region 當使用者是選取標頭(所有學生)
                if ("" + e.Node.Tag == "All")
                {
                    SetListPaneSource(TestDic2.Keys, SelectedAll, AddToTemp);
                }
                else if (TestDic1.ContainsKey("" + e.Node.Tag)) //當使用者是選取班級名稱
                {
                    if (CategoryDic.ContainsKey("" + e.Node.Parent.Tag)) //取得上層Node是否為分類Node
                    {
                        string StringX = (string)e.Node.Parent.Tag;
                        if (CategoryDic.ContainsKey(StringX))
                        {
                            if (CategoryDic[StringX]._ClassNameList.ContainsKey("" + e.Node.Tag))
                            {
                                List<string> list = new List<string>();
                                foreach (GraduateUDT each in CategoryDic[StringX]._ClassNameList["" + e.Node.Tag])
                                {
                                    if (!list.Contains(each.UID))
                                    {
                                        list.Add(each.UID);
                                    }
                                }
                                //填入
                                SetListPaneSource(list, SelectedAll, AddToTemp);
                            }
                        }
                    }
                }
                else //未選取
                {

                }
                #endregion
            }
            else
            {
                #region 如果是數字(是學年度Node)
                //取得該學年所有學生清單
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
