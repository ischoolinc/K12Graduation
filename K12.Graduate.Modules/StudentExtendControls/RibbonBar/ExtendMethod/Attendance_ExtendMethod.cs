using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FISCA.DSAUtil;

namespace K12.Graduation.Modules
{
    public static class Attendance
    {
        /// <summary>
        /// 依據各別的DataTable建立UDT資料
        /// (By 缺曠資料)
        /// </summary>
        public static void Attendance_ExtendMethod(this GetUDTXml xml, string Element)
        {
            foreach (DataRow dr in xml.dataTable.Rows)
            {
                //先判斷是否為Dic內存之資料
                if (xml.XmlUDTDic.ContainsKey("" + dr[1]))
                {
                    string student = "" + dr[1];
                    DSXmlHelper XmlDoc = xml.XmlUDTDic[student]._XmlHelper;

                    #region Element

                    XmlDoc.AddElement(Element);
                    XmlDoc.SetAttribute(Element, "ID", "" + dr[0]);

                    XmlDoc.AddElement(Element, "RefStudentID");
                    XmlDoc.SetText(Element + "/RefStudentID", "" + dr[1]);

                    XmlDoc.AddElement(Element, "SchoolYear");
                    XmlDoc.SetText(Element + "/SchoolYear", "" + dr[2]);

                    XmlDoc.AddElement(Element, "Semester");
                    XmlDoc.SetText(Element + "/Semester", "" + dr[3]);

                    XmlDoc.AddElement(Element, "OccurDate");
                    XmlDoc.SetText(Element + "/OccurDate", UDT_S.ChangeTime("" + dr[4]));
                    //XML
                    XmlDoc.AddElement(Element, "Detail");
                    XmlDoc.AddXmlString(Element + "/Detail", "" + dr[5]);

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
    }
}
