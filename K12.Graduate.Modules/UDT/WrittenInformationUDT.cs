using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using System.IO;

namespace K12.Graduation.Modules
{
    //學生之制式書面資料 如:學籍表
    [TableName("K12.Graduation.Modules.WrittenInformationUDT")]
    public class WrittenInformationUDT : ActiveRecord
    {
        /// <summary>
        /// 核心資料之UDT ID
        /// </summary>
        [Field(Field = "RefUDT_ID", Indexed = true)]
        public string RefUDT_ID { get; set; }

        /// <summary>
        /// 學生系統編號
        /// </summary>
        [Field(Field = "StudentID", Indexed = true)]
        public string StudentID { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Field(Field = "Name", Indexed = false)]
        public string Name { get; set; }

        /// <summary>
        /// 格式
        /// </summary>
        [Field(Field = "Format", Indexed = false)]
        public string Format { get; set; } //Word,Excel,JPG,TIFF,PDF

        /// <summary>
        /// 封存日期
        /// </summary>
        [Field(Field = "Date", Indexed = false)]
        public DateTime? Date { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        [Field(Field = "Content", Indexed = false)]
        public string Content { get; set; } 
    }

    /// <summary>
    /// 各種指定格式
    /// </summary>
    //public enum Format
    //{
    //    Word,Excel,JPG,TIFF,PDF
    //}
}
