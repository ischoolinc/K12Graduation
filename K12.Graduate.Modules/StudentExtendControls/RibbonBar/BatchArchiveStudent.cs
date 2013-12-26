using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;
using FISCA.UDT;
using FISCA.Data;
using System.Xml;
using FISCA.DSAUtil;
using FISCA.LogAgent;

namespace K12.Graduation.Modules
{
    public partial class BatchArchiveStudent : BaseForm
    {
        //1.重覆封存防堵(更新狀態
        //2.畢業即離校狀態 / 有畢業異動
        //3.

        /// <summary>
        /// UDT工作物件
        /// </summary>
        AccessHelper _AccessHelper = new AccessHelper();

        /// <summary>
        /// 背景模式
        /// </summary>
        BackgroundWorker BGW = new BackgroundWorker();

        Dictionary<string, int> MessageDic = new Dictionary<string, int>();

        string _ArchiveNote = "";

        UpdateRecordHelp _GetUpdateRecord { get; set; }
        PhotoRecordHelp _GetPhotoRecord { get; set; }
        AddressHelp _GetAddressRecord { get; set; }
        PhoneHelp _GetPhoneRecord { get; set; }
        TagHelp _GetTagHelp { get; set; }
        /// <summary>
        /// FISCA Query物件
        /// </summary>
        QueryHelper _queryhelper = new QueryHelper();

        public BatchArchiveStudent()
        {
            InitializeComponent();
        }

        private void BatchArchiveStudent_Load(object sender, EventArgs e)
        {
            //註冊背景事件
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
            BGW.ProgressChanged += new ProgressChangedEventHandler(BGW_ProgressChanged);
            BGW.WorkerReportsProgress = true;

            //無論如何,先建立此Table..不然會爆阿...
            List<string> list1 = new List<string>() { "0" };
            List<GraduateUDT> list2 = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("StudentID", list1));

            string TableName = Tn._GraduateUDT;
            DataTable dt = _queryhelper.Select("select ArchiveNote from " + TableName.ToLower() + " group by ArchiveNote ORDER by ArchiveNote");
            List<string> UDTArchiveNote = new List<string>();
            foreach (DataRow each in dt.Rows)
            {
                UDTArchiveNote.Add("" + each[0]);
            }

            UDTArchiveNote.Sort();

            comboBoxEx1.Items.AddRange(UDTArchiveNote.ToArray());

            //if (UDTkey.Count != 0)
            //{
            //    string sqlStr2 = string.Format("select ArchiveNote from {0} WHERE " + UDT_S.PopOneCondition("UID", UDTkey) + " group by ArchiveNote ORDER by ArchiveNote", TableName.ToLower());
            //    dt = _queryhelper.Select(sqlStr2);
            //    if (dt.Rows.Count > 0)
            //    {
            //        List<string> NoteList = new List<string>();
            //        foreach (DataRow each in dt.Rows)
            //        {
            //            comboBoxEx1.Items.Add("" + each[0]);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 背景模式進度表
        /// </summary>
        void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage(e.UserState.ToString(), e.ProgressPercentage);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
            {
                MsgBox.Show("請選擇學生");
                return;
            }

            if (string.IsNullOrEmpty(comboBoxEx1.Text))
            {
                MsgBox.Show("請輸入索引分類");
                return;
            }


            if (!BGW.IsBusy)
            {
                btnStart.Enabled = false;
                BGW.RunWorkerAsync(K12.Presentation.NLDPanels.Student.SelectedSource);
                _ArchiveNote = comboBoxEx1.Text.Trim();
            }
            else
            {
                MsgBox.Show("背景模式,忙碌中...");
            }
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            BGW.ReportProgress(0, "取得學生資訊");

            MessageDic.Clear();
            //取得選擇學生ID

            List<StudentRecord> StudentList = Student.SelectByIDs((List<string>)e.Argument);
            List<StudentRecord> NewStudentList = new List<StudentRecord>();
            List<StudentRecord> LowStudentList = new List<StudentRecord>();
            BGW.ReportProgress(6, "取得異動記錄");

            //取得異動記錄
            _GetUpdateRecord = new UpdateRecordHelp((List<string>)e.Argument);
            BGW.ReportProgress(13, "取得學生照片");

            //取得學生新生/畢業照片
            _GetPhotoRecord = new PhotoRecordHelp((List<string>)e.Argument);
            BGW.ReportProgress(17, "取得地址資料");

            //取得地址資料
            _GetAddressRecord = new AddressHelp((List<string>)e.Argument);
            BGW.ReportProgress(21, "取得電話資料");

            //取得電話資料
            _GetPhoneRecord = new PhoneHelp((List<string>)e.Argument);
            BGW.ReportProgress(24, "取得類別標記");

            //取得學生類別資料
            _GetTagHelp = new TagHelp((List<string>)e.Argument);
            BGW.ReportProgress(31, "比對索引資料");

            //取得學生ID的已封存資料(用於更新)
            Dictionary<string, GraduateUDT> TestDic = GetUDTObj();

            //判斷分類學生資料,是未封存過之學生 OR 已封存之學生

            StudentList = SortStudent.sort(StudentList);
            foreach (StudentRecord student in StudentList)
            {
                if (!TestDic.ContainsKey(student.ID))
                    NewStudentList.Add(student); //未封存過之學生
                else
                    LowStudentList.Add(student); //已封存之學生
            }
            //log
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("已進行學生電子檔案索引建立。");
            sb.AppendLine("索引分類「" + _ArchiveNote + "」");
            #region 新增

            //新增之UDT資料 - 需要先進行儲存,才會知道該UDT之UID    
            //封存書面資料
            //封存缺曠 / 獎懲 / 異動 / 成績 - XML資料

            BGW.ReportProgress(40, "開始新增索引");

            if (NewStudentList.Count != 0)
            {
                //組合要新增的UDT資料
                List<GraduateUDT> Uid_UDTList = InsertStudent(NewStudentList);
                BGW.ReportProgress(42, "開始新增索引");

                MessageDic.Add("新增索引", Uid_UDTList.Count); //訊息

                //InsertUDT資料 - 並且取得已新增的UDT ID
                List<string> UDT_id = _AccessHelper.InsertValues(Uid_UDTList.ToArray());
                BGW.ReportProgress(43, "開始新增索引");
                //重新取得有UID之資料
                string test1 = UDT_S.PopOneCondition("UID", UDT_id);
                Uid_UDTList = _AccessHelper.Select<GraduateUDT>(test1);
                BGW.ReportProgress(49, "開始新增索引");
                //準備Insert照片UDT資料
                List<PhotoDataUDT> InsertPhoto = InsertStudentPhoto(Uid_UDTList);
                _AccessHelper.InsertValues(InsertPhoto.ToArray());
                BGW.ReportProgress(54, "開始新增索引");
                SetXmlData(Uid_UDTList);
                BGW.ReportProgress(59, "開始新增索引");
                #region 新增封存Log

                sb.AppendLine("新增索引清單：");
                BGW.ReportProgress(67, "開始新增索引");
                foreach (StudentRecord student in NewStudentList)
                {

                    sb.Append("班級「" + (student.Class != null ? student.Class.Name : "") + "」");
                    sb.Append("座號「" + (student.SeatNo.HasValue ? student.SeatNo.Value.ToString() : "") + "」");
                    sb.AppendLine("姓名「" + student.Name + "」");
                }
                BGW.ReportProgress(70, "開始新增索引");
                #endregion

            }
            #endregion

            #region 更新

            if (LowStudentList.Count != 0)
            {

                //取得要更新的UDT資料
                List<GraduateUDT> UpdateArchiveList = UpdateStudent(LowStudentList, TestDic);
                BGW.ReportProgress(74, "開始更新索引");
                _AccessHelper.UpdateValues(UpdateArchiveList.ToArray());
                MessageDic.Add("更新索引", UpdateArchiveList.Count);
                BGW.ReportProgress(80, "開始更新索引");
                //重新建立新照片
                List<PhotoDataUDT> UpdatePhoto = UpdateStudentPhoto(UpdateArchiveList);
                _AccessHelper.UpdateValues(UpdatePhoto.ToArray());
                BGW.ReportProgress(87, "開始更新索引");
                //取得選擇學生的所有AllXMLDataUDT資料後刪除
                List<string> list = new List<string>();
                BGW.ReportProgress(91, "開始更新索引");
                foreach (GraduateUDT each in UpdateArchiveList)
                {
                    list.Add(each.UID);
                }

                //XML註解
                //List<AllXMLDataUDT> records = _AccessHelper.Select<AllXMLDataUDT>(UDT_S.PopOneCondition("RefUDT_ID", list));
                //_AccessHelper.DeletedValues(records.ToArray());
                ////重新進行封存資料作業
                //SetXmlData(UpdateArchiveList);

                #region 更新封存Log
                sb.AppendLine("更新索引清單：");
                foreach (StudentRecord student in LowStudentList)
                {
                    sb.Append("班級「" + (student.Class != null ? student.Class.Name : "") + "」");
                    sb.Append("座號「" + (student.SeatNo.HasValue ? student.SeatNo.Value.ToString() : "") + "」");
                    sb.AppendLine("姓名「" + student.Name + "」");
                }
                BGW.ReportProgress(96, "開始更新索引");
                #endregion
            }

            #endregion
            BGW.ReportProgress(100, "索引建立完成");
            ApplicationLog.Log("畢業生檔案檢索", "索引建立", sb.ToString());
        }

        //XML註解
        private void SetXmlData(List<GraduateUDT> Uid_UDTList)
        {
            //ArchiveHelper ah = new ArchiveHelper(Uid_UDTList);

            //List<AllXMLDataUDT> records;

            ////基本資料
            //records = ah.GenerateXml("student as Student", GetText.StudentInfo(), "id", "基本資料");
            //_AccessHelper.InsertValues(records.ToArray());
            ////缺曠
            //records = ah.GenerateXml("attendance as Attendance", GetText.Attendance(), "ref_student_id", "缺曠資料");
            //_AccessHelper.InsertValues(records.ToArray());
            ////獎懲
            //records = ah.GenerateXml("discipline as Discipline", GetText.Discipline(), "ref_student_id", "獎懲資料");
            //_AccessHelper.InsertValues(records.ToArray());
            ////異動
            //records = ah.GenerateXml("update_record as UpdateRecord", GetText.UpdataRecord(), "ref_student_id", "異動資料");
            //_AccessHelper.InsertValues(records.ToArray());
            ////德行表現
            //records = ah.GenerateXml("sems_moral_score as SemsMoralScore", GetText.SemsMoralScore(), "ref_student_id", "學期德行成積");
            //_AccessHelper.InsertValues(records.ToArray());
            ////修課記錄
            //records = ah.GenerateXml("sc_attend as SC_Attend", GetText.SCAttend(), "ref_student_id", "學生修課記錄");
            //_AccessHelper.InsertValues(records.ToArray());
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnStart.Enabled = true;
            if (e.Error == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("索引已建立完成!!");
                if (MessageDic.ContainsKey("新增索引"))
                    sb.AppendLine("新增索引學生：" + MessageDic["新增索引"] + "筆");
                if (MessageDic.ContainsKey("更新索引"))
                    sb.AppendLine("更新索引學生：" + MessageDic["更新索引"] + "筆");
                MsgBox.Show(sb.ToString());
                FISCA.Presentation.MotherForm.SetStatusBarMessage("索引建立完成");

                this.Close(); //關閉
            }
            else
            {
                MsgBox.Show("索引建立作業發生錯誤!!\n" + e.Error.Message);
                FISCA.Presentation.MotherForm.SetStatusBarMessage("索引建立作業發生錯誤!!");
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
            }

        }

        /// <summary>
        /// 取得目前已封存之學生資料(ID)
        /// </summary>
        private Dictionary<string, GraduateUDT> GetUDTObj()
        {

            List<GraduateUDT> TestList = _AccessHelper.Select<GraduateUDT>();
            Dictionary<string, GraduateUDT> TestDic = new Dictionary<string, GraduateUDT>();
            foreach (GraduateUDT obj in TestList)
            {
                //依系統編號+封存分類 不可重覆
                if (!TestDic.ContainsKey(obj.StudentID))
                {
                    TestDic.Add(obj.StudentID, obj);
                }
            }
            return TestDic;
        }

        /// <summary>
        /// 新增封存學生
        /// </summary>
        private List<GraduateUDT> InsertStudent(List<StudentRecord> StudentList1)
        {
            List<GraduateUDT> ArchiveList = new List<GraduateUDT>();

            foreach (StudentRecord student in StudentList1)
            {
                //封存基本資料
                GraduateUDT obj = new GraduateUDT(); //建立新的UDT
                obj = SetValue(student, obj);
                ArchiveList.Add(obj);
            }

            return ArchiveList;
        }

        /// <summary>
        /// 更新封存學生
        /// </summary>
        private List<GraduateUDT> UpdateStudent(List<StudentRecord> StudentList2, Dictionary<string, GraduateUDT> TestDic)
        {
            List<GraduateUDT> ArchiveList = new List<GraduateUDT>();
            foreach (StudentRecord student in StudentList2)
            {
                if (TestDic.ContainsKey(student.ID))
                {
                    GraduateUDT obj = TestDic[student.ID]; //取得原有UDT
                    obj = SetValue(student, obj);
                    ArchiveList.Add(obj);
                }
            }

            return ArchiveList;
        }

        /// <summary>
        /// 設定UDT內容值
        /// </summary>
        private GraduateUDT SetValue(StudentRecord student, GraduateUDT obj)
        {
            if (obj.StudentID == null) //如果是空的才予以更新
            {
                obj.StudentID = student.ID; //系統編號
            }

            obj.Name = student.Name; //姓名
            if (student.SeatNo.HasValue)
                obj.SeatNo = student.SeatNo.Value; //座號

            obj.StudentNumber = student.StudentNumber.Trim(); //學號
            obj.Nationality = student.Nationality.Trim(); //國籍
            obj.BirthPlace = student.BirthPlace.Trim(); //出生地
            obj.Birthday = student.Birthday; //生日
            obj.EnglishName = student.EnglishName.Trim(); //英文姓名

            obj.ClassName = student.Class != null ? student.Class.Name : ""; //班級
            obj.Gender = student.Gender; //性別
            obj.IDNumber = student.IDNumber; //身分證號
            if (_GetUpdateRecord.GetSY(student.ID) != 0) //如果是0就沒有學年度
                obj.GraduateSchoolYear = _GetUpdateRecord.GetSY(student.ID); //畢業學年度
            obj.ArchiveNote = _ArchiveNote.Trim(); //封存分類
            //封存電話資料
            obj = _GetPhoneRecord.GetSY(obj);
            //封存地址資料
            obj = _GetAddressRecord.GetSY(obj);

            obj.Tag = _GetTagHelp.GetSY(obj);
            return obj;
        }

        /// <summary>
        /// 新增照片封存資料
        /// </summary>
        private List<PhotoDataUDT> InsertStudentPhoto(List<GraduateUDT> GraUDTList)
        {
            List<PhotoDataUDT> ArchiveList = new List<PhotoDataUDT>();

            foreach (GraduateUDT udt in GraUDTList)
            {
                //封存入學/畢業照片
                PhotoDataUDT photo = new PhotoDataUDT();
                photo.StudentID = udt.StudentID;
                photo.RefUDT_ID = udt.UID;
                photo.FreshmanPhoto = SetPhoto(udt.StudentID, true); //入學照片
                photo.GraduatePhoto = SetPhoto(udt.StudentID, false); //入學照片
                ArchiveList.Add(photo);
            }
            return ArchiveList;
        }

        /// <summary>
        /// 更新照片封存資料
        /// </summary>
        private List<PhotoDataUDT> UpdateStudentPhoto(List<GraduateUDT> UpdateArchiveList)
        {
            List<PhotoDataUDT> ArchiveList = new List<PhotoDataUDT>();
            //取得照片資料(照片資料需取得後更新)

            List<string> list = new List<string>();
            foreach (GraduateUDT each in UpdateArchiveList)
            {
                list.Add(each.UID);
            }
            string test1 = UDT_S.PopOneCondition("RefUDT_ID", list);
            List<PhotoDataUDT> PhotoArchiveList = _AccessHelper.Select<PhotoDataUDT>(test1);
            Dictionary<string, PhotoDataUDT> photoDic = new Dictionary<string, PhotoDataUDT>();
            foreach (PhotoDataUDT each in PhotoArchiveList)
            {
                if (!photoDic.ContainsKey(each.StudentID))
                    photoDic.Add(each.StudentID, each);
                else
                    MsgBox.Show("一名學生有2筆照片資料!!\n" + each.StudentID);
            }

            foreach (GraduateUDT udt in UpdateArchiveList)
            {
                if (photoDic.ContainsKey(udt.StudentID))
                {
                    photoDic[udt.StudentID].FreshmanPhoto = SetPhoto(udt.StudentID, true); //入學照片
                    photoDic[udt.StudentID].GraduatePhoto = SetPhoto(udt.StudentID, false); //入學照片
                    ArchiveList.Add(photoDic[udt.StudentID]);
                }
                else //如果不存在,表示前次封存無照片
                {
                    //封存入學/畢業照片
                    PhotoDataUDT photo = new PhotoDataUDT();
                    photo.StudentID = udt.StudentID;
                    photo.RefUDT_ID = udt.UID;
                    photo.FreshmanPhoto = SetPhoto(udt.StudentID, true); //入學照片
                    photo.GraduatePhoto = SetPhoto(udt.StudentID, false); //入學照片
                    ArchiveList.Add(photo);
                }
            }

            return ArchiveList;
        }

        /// <summary>
        /// 傳入學生ID,取得學生照片資料
        /// 如果沒有照片,回傳空字串
        /// </summary>
        public string SetPhoto(string ID, bool Freshman)
        {
            if (Freshman)
                return _GetPhotoRecord.GetFH(ID);
            else
                return _GetPhotoRecord.GetGD(ID);
        }

        /// <summary>
        /// 離開
        /// </summary>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private void lbHelp1_Click(object sender, EventArgs e)
        //{
        //    List<GraduateUDT> TestList = _AccessHelper.Select<GraduateUDT>();
        //    _AccessHelper.DeletedValues(TestList.ToArray());

        //    List<PhotoDataUDT> TestList2 = _AccessHelper.Select<PhotoDataUDT>();
        //    _AccessHelper.DeletedValues(TestList2.ToArray());

        //    List<AllXMLDataUDT> TestList3 = _AccessHelper.Select<AllXMLDataUDT>();
        //    _AccessHelper.DeletedValues(TestList3.ToArray());

        //    List<WrittenInformationUDT> TestList4 = _AccessHelper.Select<WrittenInformationUDT>();
        //    _AccessHelper.DeletedValues(TestList4.ToArray());

        //    MsgBox.Show("已清除所有封存資料!!");
        //    GraduationEvents.RaiseAssnChanged();
        //}
    }
}
