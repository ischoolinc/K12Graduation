using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using System.IO;

namespace K12.Graduation.Modules
{
    //學生之基本資料內容
    //Tn.cs內容也要跟著改
    [TableName("K12.Graduation.Modules.GraduateUDT")]
    public class GraduateUDT : ActiveRecord
    {
        //Key值
        /// <summary>
        /// 學號
        /// </summary>
        [Field(Field = "StudentNumber", Indexed = true)]
        public string StudentNumber { get; set; }

        /// <summary>
        /// 身分證號
        /// </summary>
        [Field(Field = "IDNumber", Indexed = true)]
        public string IDNumber { get; set; }

        /// <summary>
        /// 封存分類
        /// </summary>
        [Field(Field = "ArchiveNote", Indexed = true)]
        public string ArchiveNote { get; set; }

        #region 其他額外資訊

        /// <summary>
        /// 學生類別
        /// </summary>
        [Field(Field = "Tag", Indexed = true)]
        public string Tag { get; set; }
        //<TagList>
        //<Tag Prefix="特殊身份" Name="障礙子女">特殊身份:障礙子女</Tag>
        //<Tag Prefix="減免" Name="障礙課輔">減免:障礙課輔</Tag>
        //<Tag Prefix="減免" Name="障礙子女書籍">減免:障礙子女書籍</Tag>
        //</TagList>

        /// <summary>
        /// 備註
        /// </summary>
        [Field(Field = "Remarks", Indexed = false)]
        public string Remarks { get; set; }

        /// <summary>
        /// 畢業班級
        /// </summary>
        [Field(Field = "ClassName", Indexed = false)]
        public string ClassName { get; set; }

        /// <summary>
        /// 畢業學年度
        /// 如未輸入,儲存資料為"0"
        /// </summary>
        [Field(Field = "GraduateSchoolYear", Indexed = false)]
        public int? GraduateSchoolYear { get; set; }

        #endregion

        #region 基本資料

        /// <summary>
        /// 學生系統編號
        /// </summary>
        [Field(Field = "StudentID", Indexed = true)]
        public string StudentID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Field(Field = "Name", Indexed = false)]
        public string Name { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        [Field(Field = "SeatNo", Indexed = false)]
        public int? SeatNo { get; set; }

        /// <summary>
        /// 性別
        /// </summary>
        [Field(Field = "Gender", Indexed = false)]
        public string Gender { get; set; }

        /// <summary>
        /// 國籍
        /// </summary>
        [Field(Field = "Nationality", Indexed = false)]
        public string Nationality { get; set; }

        /// <summary>
        /// 出生地
        /// </summary>
        [Field(Field = "BirthPlace", Indexed = false)]
        public string BirthPlace { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [Field(Field = "Birthday", Indexed = false)]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 英文姓名
        /// </summary>
        [Field(Field = "EnglishName", Indexed = false)]
        public string EnglishName { get; set; }

        #endregion

        #region 聯絡資訊

        /// <summary>
        /// 戶籍電話
        /// </summary>
        [Field(Field = "Permanent", Indexed = false)]
        public string Permanent { get; set; }

        /// <summary>
        /// 聯絡電話
        /// </summary>
        [Field(Field = "Contact", Indexed = false)]
        public string Contact { get; set; }

        /// <summary>
        /// 手機
        /// </summary>
        [Field(Field = "Cell", Indexed = false)]
        public string Cell { get; set; }

        /// <summary>
        /// 其它電話1
        /// </summary>
        [Field(Field = "Phone1", Indexed = false)]
        public string Phone1 { get; set; }

        /// <summary>
        /// 其它電話2
        /// </summary>
        [Field(Field = "Phone2", Indexed = false)]
        public string Phone2 { get; set; }

        /// <summary>
        /// 其它電話3
        /// </summary>
        [Field(Field = "Phone3", Indexed = false)]
        public string Phone3 { get; set; }

        /// <summary>
        /// 戶籍地址
        /// </summary>
        [Field(Field = "PermanentAddress", Indexed = false)]
        public string PermanentAddress { get; set; }

        /// <summary>
        /// 戶籍地址郵遞區號
        /// </summary>
        [Field(Field = "PermanentZipCode", Indexed = false)]
        public string PermanentZipCode { get; set; }

        /// <summary>
        /// 聯絡地址
        /// </summary>
        [Field(Field = "MailingAddress", Indexed = false)]
        public string MailingAddress { get; set; }

        /// <summary>
        /// 聯絡地址郵遞區號
        /// </summary>
        [Field(Field = "MailingZipCode", Indexed = false)]
        public string MailingZipCode { get; set; }

        /// <summary>
        /// 其它地址
        /// </summary>
        [Field(Field = "OtherAddresses", Indexed = false)]
        public string OtherAddresses { get; set; }

        /// <summary>
        /// 其它地址郵遞區號
        /// </summary>
        [Field(Field = "OtherZipCode", Indexed = false)]
        public string OtherZipCode { get; set; }

        ///// <summary>
        ///// 其它地址2
        ///// </summary>
        //[Field(Field = "OtherAddresses2", Indexed = false)]
        //public string OtherAddresses2 { get; set; }

        ///// <summary>
        ///// 其它地址3
        ///// </summary>
        //[Field(Field = "OtherAddresses3", Indexed = false)]
        //public string OtherAddresses3 { get; set; } 

        #endregion

    }
}
