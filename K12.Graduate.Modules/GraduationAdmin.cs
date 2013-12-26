using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using System.ComponentModel;
using K12.Data;
using FISCA.UDT;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace K12.Graduation.Modules
{
    public partial class GraduationAdmin : NLDPanel
    {
        public BackgroundWorker BGW = new BackgroundWorker();

        private AccessHelper _AccessHelper = new AccessHelper();

        //搜尋條件(姓名,學號,身分證號,畢業班級)
        private MenuButton SearchStudentName, SearchStudentNumber;
        private MenuButton SearchIDNumber, SearchClassName;
        private MenuButton SearchRemarks, SearchAddress;
        //(搜尋使用)
        private SearchEventArgs SearEvArgs = null;

        private Dictionary<string, GraduateUDT> TestDic = new Dictionary<string, GraduateUDT>();

        //private string FiltedSemester = "";

        ListPaneField Field1; //班級
        ListPaneField Field2; //座號 
        ListPaneField Field3; //姓名 
        ListPaneField Field4; //學號
        ListPaneField Field5; //姓別
        ListPaneField Field6; //身分證號
        ListPaneField Field7; //戶籍地址
        ListPaneField Field8; //聯絡地址
        ListPaneField Field9; //其他地址
        ListPaneField Field10; //備註

        private bool isbusy = false;

        public GraduationAdmin()
        {
            //畢業生檔案檢索
            Group = "畢業";

            GraduationEvents.GraduationChanged += new EventHandler(GraduationEvents_GraduationChanged);

            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            #region 班級
            Field1 = new ListPaneField("班級");
            Field1.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (TestDic.ContainsKey(e.Key))
                {
                    e.Value = TestDic[e.Key].ClassName;
                }
            };
            this.AddListPaneField(Field1);
            #endregion
            #region 座號
            Field2 = new ListPaneField("座號");
            //Field2.CompareValue += new EventHandler<CompareValueEventArgs>(Field2_CompareValue);
            Field2.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (TestDic.ContainsKey(e.Key))
                {
                    e.Value = TestDic[e.Key].SeatNo;
                }
            };
            this.AddListPaneField(Field2);
            #endregion
            #region 姓名
            Field3 = new ListPaneField("姓名");
            Field3.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (TestDic.ContainsKey(e.Key))
                {
                    e.Value = TestDic[e.Key].Name;
                }
            };
            this.AddListPaneField(Field3);
            #endregion
            #region 學號
            Field4 = new ListPaneField("學號");
            Field4.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (TestDic.ContainsKey(e.Key))
                {
                    e.Value = TestDic[e.Key].StudentNumber;
                }
            };
            this.AddListPaneField(Field4);
            #endregion
            #region 姓別
            Field5 = new ListPaneField("姓別");
            Field5.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (TestDic.ContainsKey(e.Key))
                {
                    e.Value = TestDic[e.Key].Gender;
                }
            };
            this.AddListPaneField(Field5);
            #endregion
            #region 身分證號
            Field6 = new ListPaneField("身分證號");
            Field6.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (TestDic.ContainsKey(e.Key))
                {
                    e.Value = TestDic[e.Key].IDNumber;
                }
            };
            this.AddListPaneField(Field6);
            #endregion
            #region 戶籍地址
            Field7 = new ListPaneField("戶籍地址");
            Field7.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (TestDic.ContainsKey(e.Key))
                {
                    e.Value = TestDic[e.Key].PermanentAddress;
                }
            };
            this.AddListPaneField(Field7);
            #endregion
            #region 聯絡地址
            Field8 = new ListPaneField("聯絡地址");
            Field8.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (TestDic.ContainsKey(e.Key))
                {
                    e.Value = TestDic[e.Key].MailingAddress;
                }
            };
            this.AddListPaneField(Field8);
            #endregion
            #region 其它地址
            Field9 = new ListPaneField("其它地址");
            Field9.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (TestDic.ContainsKey(e.Key))
                {
                    e.Value = TestDic[e.Key].OtherAddresses;
                }
            };
            this.AddListPaneField(Field9);
            #endregion
            #region 備註
            Field10 = new ListPaneField("備註");
            Field10.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (TestDic.ContainsKey(e.Key))
                {
                    e.Value = TestDic[e.Key].Remarks;
                }
            };
            this.AddListPaneField(Field10);
            #endregion

            #region 學年度篩選
            FilterMenu.Visible = false;
            //FilterMenu.SupposeHasChildern = true;
            //FilterMenu.PopupOpen += delegate(object sender, PopupOpenEventArgs e)
            //{
            //foreach (string item in SemesterCourse.Keys)
            //{
            //    MenuButton mb = e.VirtualButtons[item];
            //    mb.AutoCheckOnClick = true;
            //    mb.AutoCollapseOnClick = true;
            //    mb.Checked = (item == FiltedSemester);
            //    mb.Tag = item;
            //    mb.CheckedChanged += delegate(object sender1, EventArgs e1)
            //    {
            //        MenuButton mb1 = sender1 as MenuButton;
            //        SetFilterSource(mb1.Text);
            //        FiltedSemester = FilterMenu.Text = mb1.Text;
            //        mb1.Checked = true;
            //    };
            //}
            //};

            #endregion

            //BGW.RunWorkerAsync(); //取得各項資料課程

            //預設選擇上一學年度畢業班級
            //int defSchool = int.Parse(School.DefaultSchoolYear) - 1;
            //FiltedSemester = defSchool + "學年度畢業班級";
            //FilterMenu.Text = FiltedSemester;
            //SetFilterSource(FiltedSemester);

            #region 搜尋
            Campus.Configuration.ConfigData cd = Campus.Configuration.Config.User["AssociationSearchOptionPreference"];
            SearchStudentName = SearchConditionMenu["姓名"];
            SearchStudentName.AutoCheckOnClick = true;
            SearchStudentName.AutoCollapseOnClick = false;
            SearchStudentName.Checked = CheckBool(cd["SearchGraduateStudentName"]);
            SearchStudentName.Click += delegate
            {
                cd["SearchGraduateStudentName"] = SearchStudentName.Checked.ToString();
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as Campus.Configuration.ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchStudentNumber = SearchConditionMenu["學號"];
            SearchStudentNumber.AutoCheckOnClick = true;
            SearchStudentNumber.AutoCollapseOnClick = false;
            SearchStudentNumber.Checked = CheckBool(cd["SearchGraduateStudentNumber"]);
            SearchStudentNumber.Click += delegate
            {
                cd["SearchGraduateStudentNumber"] = SearchStudentNumber.Checked.ToString();
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as Campus.Configuration.ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchIDNumber = SearchConditionMenu["身分證號"];
            SearchIDNumber.AutoCheckOnClick = true;
            SearchIDNumber.AutoCollapseOnClick = false;
            SearchIDNumber.Checked = CheckBool(cd["SearchGraduateIDNumber"]);
            SearchIDNumber.Click += delegate
            {
                cd["SearchGraduateIDNumber"] = SearchIDNumber.Checked.ToString();
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as Campus.Configuration.ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchClassName = SearchConditionMenu["畢業班級"];
            SearchClassName.AutoCheckOnClick = true;
            SearchClassName.AutoCollapseOnClick = false;
            SearchClassName.Checked = CheckBool(cd["SearchGraduateClass"]);
            SearchClassName.Click += delegate
            {
                cd["SearchGraduateClass"] = SearchClassName.Checked.ToString();
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as Campus.Configuration.ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchAddress = SearchConditionMenu["地址"];
            SearchAddress.AutoCheckOnClick = true;
            SearchAddress.AutoCollapseOnClick = false;
            SearchAddress.Checked = CheckBool(cd["SearchGraduateAddress"]);
            SearchAddress.Click += delegate
            {
                cd["SearchGraduateAddress"] = SearchAddress.Checked.ToString();
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as Campus.Configuration.ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchRemarks = SearchConditionMenu["備註"];
            SearchRemarks.AutoCheckOnClick = true;
            SearchRemarks.AutoCollapseOnClick = false;
            SearchRemarks.Checked = CheckBool(cd["SearchGraduateRemarks"]);
            SearchRemarks.Click += delegate
            {
                cd["SearchGraduateRemarks"] = SearchRemarks.Checked.ToString();
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as Campus.Configuration.ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            #endregion

            this.Search += new EventHandler<SearchEventArgs>(GraduationAdmin_Search);

        }

        void Field2_CompareValue(object sender, CompareValueEventArgs e)
        {
            int x = 0;
            if (e.Value1 is int)
                x = int.Parse("" + e.Value1);

            int y = 0;
            if (e.Value2 is int)
                y = int.Parse("" + e.Value2);

            e.Result = x.CompareTo(y);
        }

        //資料更新事件
        void GraduationEvents_GraduationChanged(object sender, EventArgs e)
        {
            if (BGW.IsBusy)
            {
                isbusy = true;
            }
            else
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("開始取得畢業生檢索資料!!");
                BGW.RunWorkerAsync(TestDic.Keys);
            }
        }

        private bool CheckBool(string Sam)
        {
            bool check;
            if (!bool.TryParse(Sam, out check))
            {
                check = true;
            }
            return check;
        }

        void GraduationAdmin_Search(object sender, SearchEventArgs e)
        {
            SearEvArgs = e;
            Campus.Windows.BlockMessage.Display("資料搜尋中,請稍候....", new Campus.Windows.ProcessInvoker(ProcessSearch));
        }

        //開始找尋資料
        private void ProcessSearch(Campus.Windows.MessageArgs args)
        {
            refTestList(TestDic.Keys.ToList());

            List<string> results = new List<string>();
            Regex rx = new Regex(SearEvArgs.Condition, RegexOptions.IgnoreCase);
            #region 姓名
            if (SearchStudentName.Checked)
            {
                foreach (string each in TestDic.Keys)
                {
                    if (rx.Match(TestDic[each].Name).Success)
                    {
                        if (!results.Contains(each))
                            results.Add(each);
                    }
                }
            }
            #endregion

            #region 學號
            if (SearchStudentNumber.Checked)
            {
                foreach (string each in TestDic.Keys)
                {
                    if (rx.Match(TestDic[each].StudentNumber).Success)
                    {
                        if (!results.Contains(each))
                            results.Add(each);
                    }
                }
            }
            #endregion

            #region 身分證號
            if (SearchIDNumber.Checked)
            {
                foreach (string each in TestDic.Keys)
                {
                    if (rx.Match(TestDic[each].IDNumber).Success)
                    {
                        if (!results.Contains(each))
                            results.Add(each);
                    }
                }
            }
            #endregion

            #region 畢業班級
            if (SearchClassName.Checked)
            {
                foreach (string each in TestDic.Keys)
                {
                    if (rx.Match(TestDic[each].ClassName).Success)
                    {
                        if (!results.Contains(each))
                            results.Add(each);
                    }
                }
            }
            #endregion

            #region 地址
            if (SearchAddress.Checked)
            {
                foreach (string each in TestDic.Keys)
                {
                    if (rx.Match(TestDic[each].MailingAddress).Success)
                    {
                        if (!results.Contains(each))
                            results.Add(each);
                    }

                    if (rx.Match(TestDic[each].PermanentAddress).Success)
                    {
                        if (!results.Contains(each))
                            results.Add(each);
                    }

                    if (rx.Match(TestDic[each].OtherAddresses).Success)
                    {
                        if (!results.Contains(each))
                            results.Add(each);
                    }
                }
            }
            #endregion

            #region 備註
            if (SearchRemarks.Checked)
            {
                foreach (string each in TestDic.Keys)
                {
                    if (rx.Match(TestDic[each].Remarks).Success)
                    {
                        if (!results.Contains(each))
                            results.Add(each);
                    }
                }
            }
            #endregion

            SearEvArgs.Result.AddRange(results);
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            ChengFilter((List<string>)e.Argument);
        }

        private void ChengFilter(List<string> list)
        {
            if (list.Count > 0)
            {
                refTestList(list);
            }
            isbusy = false;
        }

        private void refTestList(List<string> list)
        {
            TestDic.Clear();

            List<GraduateUDT> TestList = _AccessHelper.Select<GraduateUDT>("uid in ('" + string.Join("','", list) + "')");
            //TestList.Sort(SortClassName);
            foreach (GraduateUDT obj in TestList)
            {
                if (!TestDic.ContainsKey(obj.UID))
                {
                    TestDic.Add(obj.UID, obj);
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

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isbusy)
            {
                BGW.RunWorkerAsync(TestDic.Keys);
            }
            else
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("取得畢業生檢索資料,完成!!");
                SetFilteredSource(TestDic.Keys.ToList());
            }

            Field4.Column.DataGridView.Sort(Field4.Column, ListSortDirection.Ascending);
        }

        private static GraduationAdmin _GraduateAdmin;

        public static GraduationAdmin Instance
        {
            get
            {
                if (_GraduateAdmin == null)
                {
                    _GraduateAdmin = new GraduationAdmin();
                }
                return _GraduateAdmin;
            }
        }
    }
}
