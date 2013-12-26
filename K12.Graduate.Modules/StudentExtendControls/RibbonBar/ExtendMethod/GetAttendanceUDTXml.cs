using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FISCA.UDT;
using FISCA.Data;
using FISCA.DSAUtil;

namespace K12.Graduation.Modules
{
    class GetAttendanceUDTXml
    {
        /// <summary>
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

        public GetAttendanceUDTXml(List<GraduateUDT> InsertArchiveList)
        {
            //建立學生ID 清單
            List<string> list = new List<string>();
            foreach (GraduateUDT each in InsertArchiveList)
            {
                if(!list.Contains(each.StudentID))
                    list.Add(each.StudentID);
            }

            //QueryHelper
            QueryHelper _queryhelper = new QueryHelper();
            string sqlStr1 = "Select * From attendance where " + UDT_S.PopOneCondition("ref_student_id", list);
            //取得所有學生ID的缺曠資料
            DataTable dt1 = _queryhelper.Select(sqlStr1);

            foreach (GraduateUDT each in InsertArchiveList)
            {
                if (!XmlUDTDic.ContainsKey(each.StudentID))
                {
                    XmlUdtHelp H = new XmlUdtHelp(each, "AttendanceList"); //Mini盒子
                    XmlUDTDic.Add(each.StudentID, H);
                }
            }

            foreach (DataRow dr in dt1.Rows)
            {
                //先判斷是否為Dic內存之資料
                if (XmlUDTDic.ContainsKey("" + dr[1]))
                {
                    string student = "" + dr[1];
                    DSXmlHelper XmlDoc = XmlUDTDic[student]._XmlHelper;

                    #region Element

                    XmlDoc.AddElement("Attendance");
                    XmlDoc.SetAttribute("Attendance", "ID", "" + dr[0]);

                    XmlDoc.AddElement("Attendance", "RefStudentID");
                    XmlDoc.SetText("Attendance/RefStudentID", "" + dr[1]);

                    XmlDoc.AddElement("Attendance", "SchoolYear");
                    XmlDoc.SetText("Attendance/SchoolYear", "" + dr[2]);

                    XmlDoc.AddElement("Attendance", "Semester");
                    XmlDoc.SetText("Attendance/Semester", "" + dr[3]);

                    XmlDoc.AddElement("Attendance", "OccurDate");
                    XmlDoc.SetText("Attendance/OccurDate", UDT_S.ChangeTime("" + dr[4]));
                    //XML
                    XmlDoc.AddElement("Attendance", "Detail");
                    XmlDoc.AddXmlString("Attendance/Detail", "" + dr[5]); 

                    #endregion

                    #region AttendanceRecord - 原生結構

                    //<Attendance ID="351287">
                    //    <RefStudentID>53972</RefStudentID>
                    //    <SchoolYear>99</SchoolYear>
                    //    <Semester>2</Semester>
                    //    <OccurDate>2011-01-20 00:00:00</OccurDate>
                    //    <Detail>
                    //        <Attendance>
                    //            <Period AbsenceType="公假">一</Period>
                    //            <Period AbsenceType="公假">二</Period>
                    //            <Period AbsenceType="公假">三</Period>
                    //            <Period AbsenceType="公假">四</Period>
                    //            <Period AbsenceType="公假">午休</Period>
                    //            <Period AbsenceType="公假">五</Period>
                    //        </Attendance>
                    //    </Detail>
                    //</Attendance> 

                    #endregion
                }
            }
        }

        public List<AllXMLDataUDT> GetAttendanceList()
        {
            List<AllXMLDataUDT> AllXmlList = new List<AllXMLDataUDT>();

            foreach (string each in XmlUDTDic.Keys)
            {
                AllXmlList.Add(XmlUDTDic[each].GetUDTData("缺曠資料"));
            }
            return AllXmlList;
        }
    }
}
