using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FISCA.DSAUtil;

namespace K12.Graduation.Modules
{
    public static class Discipline
    {
        /// <summary>
        /// 依據各別的DataTable建立UDT資料
        /// (By 獎懲資料)
        /// </summary>
        public static void Discipline_ExtendMethod(this GetUDTXml xml, string Element)
        {
            foreach (DataRow dr in xml.dataTable.Rows)
            {
                //先判斷是否為Dic內存之資料
                if (xml.XmlUDTDic.ContainsKey("" + dr[8]))
                {
                    string student = "" + dr[8];
                    DSXmlHelper XmlDoc = xml.XmlUDTDic[student]._XmlHelper;

                    #region Element

                    XmlDoc.AddElement(Element);
                    XmlDoc.SetAttribute(Element, "ID", "" + dr[0]);

                    XmlDoc.AddElement(Element, "RefStudentID");
                    XmlDoc.SetText(Element + "/RefStudentID", "" + dr[8]);

                    XmlDoc.AddElement(Element, "SchoolYear");
                    XmlDoc.SetText(Element + "/SchoolYear", "" + dr[1]);

                    XmlDoc.AddElement(Element, "Semester");
                    XmlDoc.SetText(Element + "/Semester", "" + dr[2]);

                    XmlDoc.AddElement(Element, "OccurDate");
                    XmlDoc.SetText(Element + "/OccurDate", UDT_S.ChangeTime("" + dr[4]));

                    XmlDoc.AddElement(Element, "RegisterDate");
                    XmlDoc.SetText(Element + "/RegisterDate", UDT_S.ChangeTime("" + dr[12]));

                    //0懲戒,1獎勵,2留察
                    XmlDoc.AddElement(Element, "MeritFlag");
                    XmlDoc.SetText(Element + "/MeritFlag", "" + dr[10]);

                    XmlDoc.AddElement(Element, "Reason");
                    XmlDoc.SetText(Element + "/Reason", "" + dr[6]);
                    //XML
                    XmlDoc.AddElement(Element, "Detail");
                    XmlDoc.AddXmlString(Element + "/Detail", "" + dr[7]);

                    #endregion

                    #region DisciplineRecord - 原生Xml結構

                    //<Discipline ID="315122">
                    //    <RefStudentID>57097</RefStudentID>
                    //    <Name>王小彥</Name>
                    //    <StudentNumber>J00114</StudentNumber>
                    //    <SeatNo>1</SeatNo>
                    //    <ClassName>普一甲</ClassName>
                    //    <Gender>女</Gender>
                    //    <SchoolYear>100</SchoolYear>
                    //    <Semester>1</Semester>
                    //    <OccurDate>2011/07/09</OccurDate>
                    //    <GradeYear />
                    //    <Reason>群育:自動參加花燈創意比賽，熱心公務。</Reason>
                    //    <Type>1</Type>
                    //    <MeritFlag>1</MeritFlag>
                    //    <RegisterDate>2011/08/11</RegisterDate>
                    //    <Detail>
                    //        <Discipline>
                    //            <Merit A="0" B="0" C="1" />
                    //            <Demerit A="0" B="1" C="0" ClearDate="" ClearReason="" Cleared="" />
                    //        </Discipline>
                    //    </Detail>
                    //    </Discipline>

                    #endregion
                }
            }
        }
    }
}
