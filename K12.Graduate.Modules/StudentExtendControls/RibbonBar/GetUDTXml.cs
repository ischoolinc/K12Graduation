using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Data;
using FISCA.UDT;
using System.Data;
using FISCA.DSAUtil;

namespace K12.Graduation.Modules
{
    public class GetUDTXml
    {      /// <summary>
        /// UDT工作物件
        /// </summary>
        AccessHelper _AccessHelper = new AccessHelper();

        /// <summary>
        /// FISCA Query物件
        /// </summary>
        QueryHelper _queryhelper = new QueryHelper();

        /// <summary>
        /// Xml分裝物件
        /// </summary>
        public Dictionary<string, XmlUdtHelp> XmlUDTDic = new Dictionary<string, XmlUdtHelp>();

        public DataTable dataTable { get; set; }

        public GetUDTXml(List<GraduateUDT> InsertArchiveList,string TableName,string RootName)
        {
            //建立學生ID 清單
            List<string> list = new List<string>();
            foreach (GraduateUDT each in InsertArchiveList)
            {
                if (!list.Contains(each.StudentID))
                    list.Add(each.StudentID);
            }

            //QueryHelper
            QueryHelper _queryhelper = new QueryHelper();
            string sqlStr1 = "Select * From " + TableName + " where " + UDT_S.PopOneCondition("ref_student_id", list);
            
            //取得所有學生ID的 TableName 資料
            dataTable = _queryhelper.Select(sqlStr1);

            //建立資料盒子
            foreach (GraduateUDT each in InsertArchiveList)
            {
                if (!XmlUDTDic.ContainsKey(each.StudentID))
                {
                    XmlUdtHelp H = new XmlUdtHelp(each, RootName); //Mini盒子
                    XmlUDTDic.Add(each.StudentID, H);
                }
            }
        }

        public List<AllXMLDataUDT> GetUDTDataList(string SetUDTName)
        {
            List<AllXMLDataUDT> AllXmlList = new List<AllXMLDataUDT>();

            foreach (string each in XmlUDTDic.Keys)
            {
                AllXmlList.Add(XmlUDTDic[each].GetUDTData(SetUDTName));
            }
            return AllXmlList;
        }
    }
}
