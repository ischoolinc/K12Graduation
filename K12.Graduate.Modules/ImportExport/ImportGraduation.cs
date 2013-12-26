using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Import;
using System.Xml;
using K12.Data;
using FISCA.DSAUtil;
using K12.Data;
using Campus.DocumentValidator;
using FISCA.Presentation.Controls;
using FISCA.LogAgent;
using FISCA.UDT;

namespace K12.Graduation.Modules
{
    class ImportGraduation : ImportWizard
    {
        //設定檔
        private ImportOption mOption;
        //Log內容
        private StringBuilder mstrLog = new StringBuilder();

        AccessHelper _accessHelper = new AccessHelper();

        public List<string> InsertListID = new List<string>();

        //學生Record,與學號對應
        private Dictionary<string, GraduateUDT> GraduateByStudentNumber = new Dictionary<string, GraduateUDT>();

        /// <summary>
        /// 準備動作
        /// </summary>
        public override void Prepare(ImportOption Option)
        {
            mOption = Option;
        }


        /// <summary>
        /// 依學生學號資料
        /// 取得學生的畢業生封存資料
        /// </summary>
        private List<GraduateUDT> GetGraduateList(List<Campus.DocumentValidator.IRowStream> Rows)
        {
            //學號清單
            List<string> StudentNumberList = new List<string>();
            foreach (IRowStream Row in Rows)
            {
                string StudentNumber = Row.GetValue("學號");
                if (!StudentNumberList.Contains(StudentNumber))
                {
                    StudentNumberList.Add(StudentNumber);
                }
            }
            //依學號取得學生UDT資料
            List<GraduateUDT> importStudRecList = _accessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("StudentNumber", StudentNumberList));

            foreach (GraduateUDT each in importStudRecList)
            {
                //包含於匯入資料"學號清單之學生
                if (StudentNumberList.Contains(each.StudentNumber))
                {
                    //建立學號比對學生Record字典
                    if (!GraduateByStudentNumber.ContainsKey(each.StudentNumber))
                    {
                        GraduateByStudentNumber.Add(each.StudentNumber, each);
                    }
                }
            }
            return importStudRecList;
        }

        /// <summary>
        /// 由學號與學年度學期,取得UDT - Record
        /// 如果沒有已存在Record,則是Null
        /// </summary>
        public GraduateUDT GetGraduateRecord(List<GraduateUDT> GraduateList, string StudentNumber, string Note)
        {
            foreach (GraduateUDT each in GraduateList)
            {
                if (each.StudentNumber == StudentNumber && each.ArchiveNote == Note)
                {
                    return each;
                }
            }
            return null;
        }

        /// <summary>
        /// 每1000筆資料,分批執行匯入
        /// Return是Log資訊
        /// </summary>
        public override string Import(List<Campus.DocumentValidator.IRowStream> Rows)
        {
            //log
            List<CommentLogRobot> LogList = new List<CommentLogRobot>();

            //取得匯入資料中,所有學生編號下的的日常生活表現資料
            List<GraduateUDT> GraduateList = GetGraduateList(Rows);

            List<GraduateUDT> InsertList = new List<GraduateUDT>();
            //List<GraduateUDT> UpdateList = new List<GraduateUDT>();

            //int NochangeCount = 0; //未處理資料記數
            if (mOption.Action == ImportAction.Insert)
            {
                #region Insert

                foreach (IRowStream Row in Rows)
                {
                    string StudentNumber = Row.GetValue("學號");
                    string ArchiveNote = Row.GetValue("索引分類");

                    GraduateUDT SSR = GetGraduateRecord(GraduateList, StudentNumber, ArchiveNote);

                    //Log機器人
                    CommentLogRobot log = new CommentLogRobot(SSR);

                    if (SSR == null) //新增
                    {
                        GraduateUDT newRecord = SetUDTValue(Row, new GraduateUDT());
                        newRecord.StudentNumber = StudentNumber;
                        newRecord.ArchiveNote = ArchiveNote;
                        InsertList.Add(newRecord);

                        //Log
                        log.Set(newRecord);

                        LogList.Add(log);
                    }
                    //else //已存在資料需要修改 or 覆蓋
                    //{
                    //    GraduateUDT newRecord = SetUDTValue(Row, SSR);

                    //    //Log
                    //    log.Set(newRecord);

                    //    UpdateList.Add(newRecord);

                    //}

                }

                #endregion
            }


            if (InsertList.Count > 0)
            {
                try
                {
                    InsertListID = _accessHelper.InsertValues(InsertList.ToArray());
                }
                catch (Exception ex)
                {
                    MsgBox.Show("於新增資料時發生錯誤!!\n" + ex.Message);
                }
            }
            //if (UpdateList.Count > 0)
            //{
            //    try
            //    {
            //        _accessHelper.UpdateValues(UpdateList.ToArray());
            //    }
            //    catch (Exception ex)
            //    {
            //        MsgBox.Show("於更新資料時發生錯誤!!\n" + ex.Message);
            //    }
            //}

            //批次記錄Log
            StringBuilder sbSummary = new StringBuilder();
            sbSummary.AppendLine("「新增」學生索引，筆數共「" + InsertList.Count() + "」筆");
            sbSummary.AppendLine("");
            //StringBuilder sbSummary2 = new StringBuilder();
            //sbSummary2.Append("「更新」畢業生封存：");
            //sbSummary2.AppendLine("更新筆數「" + UpdateList.Count() + "」");
            foreach (CommentLogRobot each in LogList)
            {
                if (each.check()) //新增
                {
                    sbSummary.AppendLine(each.LogToString());
                }
                //else //更新
                //{
                //    sbSummary2.AppendLine(each.LogToString());
                //}
            }
            //更新
            if (InsertList.Count > 0)
            {
                ApplicationLog.Log("畢業生檔案檢索.匯入索引基本資料", "匯入", sbSummary.ToString());
            }
            //新增
            //if (UpdateList.Count > 0)
            //{
            //    ApplicationLog.Log("畢業生模組.匯入畢業生封存", "匯入", sbSummary2.ToString());
            //}

            StringBuilder sbSummary3 = new StringBuilder();
            sbSummary3.AppendLine("新增筆數「" + InsertList.Count() + "」");
            //sbSummary3.AppendLine("更新筆數「" + UpdateList.Count() + "」");
            return sbSummary3.ToString();
        }

        private GraduateUDT SetUDTValue(IRowStream Row, GraduateUDT newRecord)
        {
            GraduateUDT Record = newRecord;
            //newRecord.StudentNumber = Row.GetValue("學號");
            newRecord.IDNumber = Row.GetValue("身分證號");
            //newRecord.ArchiveNote = Row.GetValue("封存分類");
            if (Row.GetValue("畢業學年度") != "")
                newRecord.GraduateSchoolYear = int.Parse(Row.GetValue("畢業學年度"));
            newRecord.ClassName = Row.GetValue("畢業班級");

            if (Row.GetValue("座號") != "")
                newRecord.SeatNo = int.Parse(Row.GetValue("座號"));
            newRecord.Name = Row.GetValue("姓名");
            newRecord.Gender = Row.GetValue("性別");
            newRecord.Nationality = Row.GetValue("國籍");
            newRecord.BirthPlace = Row.GetValue("出生地");

            if (Row.GetValue("生日") != "")
                newRecord.Birthday = DateTime.Parse(Row.GetValue("生日"));
            newRecord.EnglishName = Row.GetValue("英文姓名");
            newRecord.Permanent = Row.GetValue("戶籍電話");
            newRecord.Contact = Row.GetValue("聯絡電話");
            newRecord.Cell = Row.GetValue("手機");

            newRecord.Phone1 = Row.GetValue("其它電話1");
            newRecord.Phone2 = Row.GetValue("其它電話2");
            newRecord.Phone3 = Row.GetValue("其它電話3");
            newRecord.PermanentZipCode = Row.GetValue("戶籍地址郵遞區號");
            newRecord.PermanentAddress = Row.GetValue("戶籍地址");
            newRecord.MailingZipCode = Row.GetValue("聯絡地址郵遞區號");
            newRecord.MailingAddress = Row.GetValue("聯絡地址");

            newRecord.OtherZipCode = Row.GetValue("其它地址郵遞區號");
            newRecord.OtherAddresses = Row.GetValue("其它地址");
            newRecord.Remarks = Row.GetValue("備註");
            //無法匯狀態??

            return newRecord;
        }

        /// <summary>
        /// 取得驗證規則(動態建置XML內容)
        /// </summary>
        public override string GetValidateRule()
        {
            //動態建立XmlRule
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Properties.Resources.ImportGradution);
            return xmlDoc.InnerXml;
        }

        /// <summary>
        /// 設定匯入功能,所提供的匯入動作
        /// </summary>
        public override ImportAction GetSupportActions()
        {
            //新增(不可更新)
            return ImportAction.Insert;
        }
    }
}
