using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Graduation.Modules
{
    class CommentLogRobot
    {
        private bool Mode = false; //模式為true,就是新增資料

        private string _OldString { get; set; }

        private string _NewString { get; set; }

        private GraduateUDT _UDT { get; set; } //_UDT為null就是新增資料

        private GraduateUDT new_UDT { get; set; }

        public CommentLogRobot(GraduateUDT UDT)
        {
            if (UDT != null)
            {
                #region 備份修改前狀態
                _UDT = new GraduateUDT();
                _UDT.ArchiveNote = UDT.ArchiveNote;
                _UDT.Birthday = UDT.Birthday;
                _UDT.BirthPlace = UDT.BirthPlace;
                _UDT.Cell = UDT.Cell;
                _UDT.ClassName = UDT.ClassName;

                _UDT.Contact = UDT.Contact;
                _UDT.EnglishName = UDT.EnglishName;
                _UDT.Gender = UDT.Gender;
                _UDT.GraduateSchoolYear = UDT.GraduateSchoolYear;
                _UDT.IDNumber = UDT.IDNumber;

                _UDT.MailingAddress = UDT.MailingAddress;
                _UDT.Name = UDT.Name;
                _UDT.Nationality = UDT.Nationality;
                _UDT.OtherAddresses = UDT.OtherAddresses;
                _UDT.Permanent = UDT.Permanent;

                _UDT.PermanentAddress = UDT.PermanentAddress;
                _UDT.Phone1 = UDT.Phone1;
                _UDT.Phone2 = UDT.Phone2;
                _UDT.Phone3 = UDT.Phone3;
                _UDT.Remarks = UDT.Remarks;

                _UDT.SeatNo = UDT.SeatNo;
                _UDT.StudentID = UDT.StudentID;
                _UDT.StudentNumber = UDT.StudentNumber;
                _UDT.Tag = UDT.Tag; 
                #endregion
            }
        }

        /// <summary>
        /// 新增代表UDT是null
        /// </summary>
        public void Set(GraduateUDT UDT)
        {
            new_UDT = UDT;
        }

        /// <summary>
        /// 是否為新增資料
        /// </summary>
        /// <returns></returns>
        public bool check()
        {
            if (_UDT == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 取得Log字串
        /// </summary>
        public string LogToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_UDT == null)
            {
                #region 新增

                if (CheckEpt(new_UDT.ArchiveNote))
                    sb.AppendLine("索引分類「" + new_UDT.ArchiveNote + "」");
                if (CheckEpt(new_UDT.ClassName))
                    sb.Append("班級「" + new_UDT.ClassName + "」");

                string seat = new_UDT.SeatNo.HasValue ? new_UDT.SeatNo.Value.ToString() : "";
                if (CheckEpt(seat))
                    sb.Append("座號「" + seat + "」");
                if (CheckEpt(new_UDT.Name))
                    sb.AppendLine("姓名「" + new_UDT.Name + "」");

                if (CheckEpt(new_UDT.StudentNumber))
                    sb.AppendLine("學號「" + new_UDT.StudentNumber + "」");

                string bir = new_UDT.Birthday.HasValue ? new_UDT.Birthday.Value.ToShortDateString() : "";

                if (CheckEpt(bir))
                    sb.AppendLine("生日「" + bir + "」");
                if (CheckEpt(new_UDT.BirthPlace))
                    sb.AppendLine("出生地「" + new_UDT.BirthPlace + "」");
                if (CheckEpt(new_UDT.Cell))
                    sb.AppendLine("手機「" + new_UDT.Cell + "」");
                if (CheckEpt(new_UDT.ClassName))
                    sb.AppendLine("班級「" + new_UDT.ClassName + "」");
                if (CheckEpt(new_UDT.EnglishName))
                    sb.AppendLine("英文姓名「" + new_UDT.EnglishName + "」");

                sb.AppendLine("姓別「" + new_UDT.Gender + "」");
                string gra = new_UDT.GraduateSchoolYear.HasValue ? new_UDT.GraduateSchoolYear.Value.ToString() : "";
                if (CheckEpt(gra))
                    sb.AppendLine("畢業學年度「" + gra + "」");
                if (CheckEpt(new_UDT.IDNumber))
                    sb.AppendLine("身分證號「" + new_UDT.IDNumber + "」");
                if (CheckEpt(new_UDT.Nationality))
                    sb.AppendLine("國籍「" + new_UDT.Nationality + "」");

                if (CheckEpt(new_UDT.PermanentAddress))
                    sb.AppendLine("戶籍地址「" + new_UDT.PermanentAddress + "」");
                if (CheckEpt(new_UDT.MailingAddress))
                    sb.AppendLine("聯絡地址「" + new_UDT.MailingAddress + "」");
                if (CheckEpt(new_UDT.OtherAddresses))
                    sb.AppendLine("其它地址「" + new_UDT.OtherAddresses + "」");

                if (CheckEpt(new_UDT.Permanent))
                    sb.AppendLine("戶籍電話「" + new_UDT.Permanent + "」");
                if (CheckEpt(new_UDT.Contact))
                    sb.AppendLine("聯絡電話「" + new_UDT.Contact + "」");
                if (CheckEpt(new_UDT.Phone1))
                    sb.AppendLine("其它電話1「" + new_UDT.Phone1 + "」");
                if (CheckEpt(new_UDT.Phone2))
                    sb.AppendLine("其它電話2「" + new_UDT.Phone2 + "」");
                if (CheckEpt(new_UDT.Phone3))
                    sb.AppendLine("其它電話3「" + new_UDT.Phone3 + "」");

                if (CheckEpt(new_UDT.Remarks))
                sb.AppendLine("備註「" + new_UDT.Remarks + "」");

                sb.AppendLine("");
                #endregion
                return sb.ToString();
            }
            else
            {
                #region 修改
                //封存分類+學號 - 不可任意修改
                sb.AppendLine("學號「" + _UDT.StudentNumber + "」學生「" + new_UDT.Name + "」已更新學生索引資料");

                if (CheckEmn(_UDT.ClassName, new_UDT.ClassName))
                    sb.AppendLine("班級由「" + _UDT.ClassName + "」修改為「" + new_UDT.ClassName + "」");

                string seat1 = _UDT.SeatNo.HasValue ? _UDT.SeatNo.Value.ToString() : "";
                string seat2 = new_UDT.SeatNo.HasValue ? new_UDT.SeatNo.Value.ToString() : "";
                if (CheckEmn(seat1, seat2))
                    sb.AppendLine("座號由「" + seat1 + "」修改為「" + seat2 + "」");

                if (CheckEmn(_UDT.Name, new_UDT.Name))
                    sb.AppendLine("姓名由「" + _UDT.Name + "」修改為「" + new_UDT.Name + "」");

                string bir1 = _UDT.Birthday.HasValue ? _UDT.Birthday.Value.ToShortDateString() : "";
                string bir2 = new_UDT.Birthday.HasValue ? new_UDT.Birthday.Value.ToShortDateString() : "";
                if (CheckEmn(bir1, bir2))
                    sb.AppendLine("生日由「" + bir1 + "」修改為「" + bir2 + "」");
                if (CheckEmn(_UDT.BirthPlace, new_UDT.BirthPlace))
                    sb.AppendLine("出生地由「" + _UDT.BirthPlace + "」修改為「" + new_UDT.BirthPlace + "」");

                if (CheckEmn(_UDT.Gender, new_UDT.Gender))
                    sb.AppendLine("姓別由「" + _UDT.Gender + "」修改為「" + new_UDT.Gender + "」");

                string gra1 = _UDT.GraduateSchoolYear.HasValue ? _UDT.GraduateSchoolYear.Value.ToString() : "";
                string gra2 = new_UDT.GraduateSchoolYear.HasValue ? new_UDT.GraduateSchoolYear.Value.ToString() : "";
                if (CheckEmn(gra1, gra2))
                    sb.AppendLine("畢業學年度由「" + gra1 + "」修改為「" + gra2 + "」");
                if (CheckEmn(_UDT.IDNumber, new_UDT.IDNumber))
                    sb.AppendLine("身分證號由「" + _UDT.IDNumber + "」修改為「" + new_UDT.IDNumber + "」");
                if (CheckEmn(_UDT.EnglishName, new_UDT.EnglishName))
                    sb.AppendLine("英文姓名由「" + _UDT.EnglishName + "」修改為「" + new_UDT.EnglishName + "」");
                if (CheckEmn(_UDT.Nationality, new_UDT.Nationality))
                    sb.AppendLine("國籍由「" + _UDT.Nationality + "」修改為「" + new_UDT.Nationality + "」");

                if (CheckEmn(_UDT.Permanent, new_UDT.Permanent))
                    sb.AppendLine("戶籍電話由「" + _UDT.Permanent + "」修改為「" + new_UDT.Permanent + "」");
                if (CheckEmn(_UDT.Contact, new_UDT.Contact))
                    sb.AppendLine("聯絡電話由「" + _UDT.Contact + "」修改為「" + new_UDT.Contact + "」");
                if (CheckEmn(_UDT.Cell, new_UDT.Cell))
                    sb.AppendLine("手機由「" + _UDT.Cell + "」修改為「" + new_UDT.Cell + "」");
                if (CheckEmn(_UDT.Phone1, new_UDT.Phone1))
                    sb.AppendLine("其它電話1由「" + _UDT.Phone1 + "」修改為「" + new_UDT.Phone1 + "」");
                if (CheckEmn(_UDT.Phone2, new_UDT.Phone2))
                    sb.AppendLine("其它電話2由「" + _UDT.Phone2 + "」修改為「" + new_UDT.Phone2 + "」");
                if (CheckEmn(_UDT.Phone3, new_UDT.Phone3))
                    sb.AppendLine("其它電話3由「" + _UDT.Phone3 + "」修改為「" + new_UDT.Phone3 + "」");

                if (CheckEmn(_UDT.PermanentAddress, new_UDT.PermanentAddress))
                    sb.AppendLine("戶籍地址由「" + _UDT.PermanentAddress + "」修改為「" + new_UDT.PermanentAddress + "」");
                if (CheckEmn(_UDT.MailingAddress, new_UDT.MailingAddress))
                    sb.AppendLine("聯絡地址由「" + _UDT.MailingAddress + "」修改為「" + new_UDT.MailingAddress + "」");
                if (CheckEmn(_UDT.OtherAddresses, new_UDT.OtherAddresses))
                    sb.AppendLine("其它地址由「" + _UDT.OtherAddresses + "」修改為「" + new_UDT.OtherAddresses + "」");

                if (CheckEmn(_UDT.Remarks, new_UDT.Remarks))
                    sb.AppendLine("備註由「" + _UDT.Remarks + "」備註為「" + new_UDT.Remarks + "」");
                sb.AppendLine("");
                #endregion
                return sb.ToString();
            }
        }

        /// <summary>
        /// 檢查字串是否不相等
        /// </summary>
        private bool CheckEmn(string a, string b)
        {
            if (a != b)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 檢查是否"不"為空值
        /// </summary>
        private bool CheckEpt(string a)
        {
            if (!string.IsNullOrEmpty(a))
                return true;
            else
                return false;
        }
    }
}
