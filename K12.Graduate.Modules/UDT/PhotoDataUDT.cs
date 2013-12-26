using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace K12.Graduation.Modules
{
    [TableName("K12.Graduation.Modules.PhotoDataUDT")]
    public class PhotoDataUDT : ActiveRecord
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
        /// 學生入學照片
        /// </summary>
        [Field(Field = "FreshmanPhoto", Indexed = false)]
        public string FreshmanPhoto { get; set; }

        /// <summary>
        /// 學生畢業照片
        /// </summary>
        [Field(Field = "GraduatePhoto", Indexed = false)]
        public string GraduatePhoto { get; set; }
    }
}