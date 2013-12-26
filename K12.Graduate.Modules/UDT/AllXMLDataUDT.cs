using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using System.Xml;

namespace K12.Graduation.Modules
{
    //儲存學生的各項記錄完整XML
    //缺曠記錄
    //獎懲記錄
    //異動記錄
    //學期歷程記錄
    //成績記錄
    [TableName("K12.Graduation.Modules.AllXMLDataUDT")]
    public class AllXMLDataUDT : ActiveRecord
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
        /// 內容
        /// </summary>
        [Field(Field = "Content", Indexed = false)]
        public string Content { get; set; }
    }
}
