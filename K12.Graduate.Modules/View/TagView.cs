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

        string NoTag = "未分類";

        public TagView()
        {
            InitializeComponent();

            NavText = "依學生類別檢視";

            SourceChanged += new EventHandler(TagView_SourceChanged);
        }

        void TagView_SourceChanged(object sender, EventArgs e)
        {
            TagDic.Clear();
            TestDic1.Clear();

            List<GraduateUDT> TestList = new List<GraduateUDT>();
            if (Source.Count() != 0)
            {
                List<string> list = new List<string>();
                foreach (string each in Source)
                {
                    if (list.Contains(each))
                        continue;
                    list.Add(each);
                }
                //取得資料
                TestList = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("UID", list));
            }
            //排序
            TestList.Sort(SortClassName);

            foreach (GraduateUDT obj in TestList)
            {
                #region 依學生類別
                if (!string.IsNullOrEmpty(obj.Tag))
                {
                    XmlElement xmlelement = DSXmlHelper.LoadXml(obj.Tag);
                    foreach (XmlElement xmlE in xmlelement.SelectNodes("Tag"))
                    {
                        string Prefix = xmlE.GetAttribute("Prefix");
                        string Name = xmlE.GetAttribute("Name");
                        //第一層
                        if (!TagDic.ContainsKey(Prefix))
                            TagDic.Add(Prefix, new Dictionary<string, List<string>>());

                        //第二層
                        if (!TagDic[Prefix].ContainsKey(Name))
                            TagDic[Prefix].Add(Name, new List<string>());

                        TagDic[Prefix][Name].Add(obj.UID);
                    }
                }
                else
                {                    
                    if (!TagDic.ContainsKey(NoTag))
                        TagDic.Add(NoTag, new Dictionary<string, List<string>>());

                    if (!TagDic[NoTag].ContainsKey(NoTag))
                        TagDic[NoTag].Add(NoTag, new List<string>());

                    TagDic[NoTag][NoTag].Add(obj.UID);
                } 
                #endregion

                #region 建立所有學生記錄
                if (!TestDic1.ContainsKey(obj.UID))
                {
                    TestDic1.Add(obj.UID, obj);
                }
                #endregion
            }

            advTree1.Nodes.Clear();

            DevComponents.AdvTree.Node Node1 = new DevComponents.AdvTree.Node();
            Node1.Text = "依學生類別(" + TestDic1.Count() + ")";
            Node1.Tag = "All";
            advTree1.Nodes.Add(Node1); //加入

            foreach (string each in TagDic.Keys) //前置詞
            {
                if (each == NoTag)
                    continue;

                if (!string.IsNullOrEmpty(each))
                {
                    List<string> list = new List<string>();
                    foreach (string each2 in TagDic[each].Keys) //標籤名稱
                    {
                        foreach (string each3 in TagDic[each][each2])
                        {
                            if (list.Contains(each3))
                                continue;

                            list.Add(each3);
                        }
                    }

                    DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                    Node2.Text = each + "(" + list.Count + ")";
                    Node2.Tag = each;
                    Node1.Nodes.Add(Node2); //加入

                    foreach (string each2 in TagDic[each].Keys) //標籤名稱
                    {
                        TagViewObj obj = new TagViewObj(each, each2);

                        DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                        Node3.Text = each2 + "(" + TagDic[each][each2].Count + ")";
                        Node3.Tag = obj;
                        Node2.Nodes.Add(Node3); //加入   

                    }
                }
                else //如果是未分群組
                {
                    List<string> list = new List<string>();
                    foreach (string each2 in TagDic[each].Keys) //標籤名稱
                    {
                        foreach (string each3 in TagDic[each][each2])
                        {
                            if (list.Contains(each3))
                                continue;

                            list.Add(each3);
                        }
                    }

                    //DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                    //Node2.Text = each + "(" + list.Count + ")";
                    //Node2.Tag = each;
                    //Node1.Nodes.Add(Node2); //加入

                    foreach (string each2 in TagDic[each].Keys) //標籤名稱
                    {
                        TagViewObj obj = new TagViewObj(each, each2);

                        DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                        Node3.Text = each2 + "(" + TagDic[each][each2].Count + ")";
                        Node3.Tag = obj;
                        Node1.Nodes.Add(Node3); //加入   
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
                    Node1.Nodes.Add(Node4); //加入 
                }
            }
            

            //List<string> _Source = TestList.Select(x => x.UID).ToList();
            //SetListPaneSource(_Source, false, false);
        }

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            //判斷是否有按Control,Shift
            bool SelectedAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            bool AddToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

            if (e.Node.Tag is string)
            {
                string tag = "" + e.Node.Tag;
                if (TagDic.ContainsKey(tag)) //所有學生 or 班級名稱Node)
                {
                    List<string> list = new List<string>();
                    foreach (string each1 in TagDic[tag].Keys)
                    {
                        foreach(string each2 in TagDic[tag][each1])
                        {
                            if (list.Contains(each2))
                                continue;

                            list.Add(each2);
                        }
                    }
                    SetListPaneSource(list, SelectedAll, AddToTemp);

                }
                else if (tag == NoTag) //未分類
                {
                    List<string> list = new List<string>();
                    list = TagDic[NoTag][NoTag];
                    SetListPaneSource(list, SelectedAll, AddToTemp);
                    //星期一解決
                }
                else if (tag == "ALL")
                {
                    SetListPaneSource(TestDic1.Keys, SelectedAll, AddToTemp);
                }
            }
            else if (e.Node.Tag is TagViewObj)
            { //如果是特殊TagViewObj物件
                List<string> list = new List<string>();
                TagViewObj obj = (TagViewObj)e.Node.Tag;
                foreach (string each1 in TagDic.Keys)
                {
                    if (each1 != obj._Prefix)
                        continue;

                    foreach (string each2 in TagDic[each1].Keys)
                    {
                        if (each2 != obj._Name)
                            continue;
                 
                        list = TagDic[each1][each2];
                        SetListPaneSource(list, SelectedAll, AddToTemp);
                        
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
