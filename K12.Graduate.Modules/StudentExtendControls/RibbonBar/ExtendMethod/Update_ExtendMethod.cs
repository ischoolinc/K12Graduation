using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FISCA.DSAUtil;

namespace K12.Graduation.Modules
{
    public static class Update
    {
        /// <summary>
        /// 依據各別的DataTable建立UDT資料
        /// (By 異動資料)
        /// </summary>
        public static void Update_ExtendMethod(this GetUDTXml xml, string Element)
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

                    XmlDoc.AddElement(Element, "S_Name");
                    XmlDoc.SetText(Element + "/S_Name", "" + dr[4]);

                    XmlDoc.AddElement(Element, "S_StudentNumber");
                    XmlDoc.SetText(Element + "/S_StudentNumber", "" + dr[5]);

                    XmlDoc.AddElement(Element, "S_Gender");
                    XmlDoc.SetText(Element + "/S_Gender", "" + dr[6]);

                    XmlDoc.AddElement(Element, "S_IDNumber");
                    XmlDoc.SetText(Element + "/S_IDNumber", "" + dr[7]);

                    XmlDoc.AddElement(Element, "S_Birthdate");
                    XmlDoc.SetText(Element + "/S_Birthdate", UDT_S.ChangeTime("" + dr[8]));

                    XmlDoc.AddElement(Element, "S_GradeYear");
                    XmlDoc.SetText(Element + "/S_GradeYear", "" + dr[9]);

                    XmlDoc.AddElement(Element, "S_Dept");
                    XmlDoc.SetText(Element + "/S_Dept", "" + dr[10]);

                    XmlDoc.AddElement(Element, "Update_Date");
                    XmlDoc.SetText(Element + "/Update_Date", UDT_S.ChangeTime("" + dr[11]));

                    XmlDoc.AddElement(Element, "Update_Code");
                    XmlDoc.SetText(Element + "/Update_Code", "" + dr[12]);

                    XmlDoc.AddElement(Element, "Update_Type");
                    XmlDoc.SetText(Element + "/Update_Type", "" + dr[13]);

                    XmlDoc.AddElement(Element, "Update_Reason");
                    XmlDoc.SetText(Element + "/Update_Reason", "" + dr[14]);

                    XmlDoc.AddElement(Element, "Update_Desc");
                    XmlDoc.SetText(Element + "/Update_Desc", "" + dr[15]);

                    XmlDoc.AddElement(Element, "AD_Date");
                    XmlDoc.SetText(Element + "/AD_Date", "" + dr[16]);

                    XmlDoc.AddElement(Element, "AD_Numbar");
                    XmlDoc.SetText(Element + "/AD_Numbar", "" + dr[17]);

                    XmlDoc.AddElement(Element, "Last_AD_Date");
                    XmlDoc.SetText(Element + "/Last_AD_Date", UDT_S.ChangeTime("" + dr[18]));

                    XmlDoc.AddElement(Element, "Last_AD_Numbar");
                    XmlDoc.SetText(Element + "/Last_AD_Numbar", "" + dr[19]);

                    XmlDoc.AddElement(Element, "Comment");
                    XmlDoc.SetText(Element + "/Comment", "" + dr[20]);
                    //XML
                    XmlDoc.AddElement(Element, "ConText_Info");
                    XmlDoc.AddXmlString(Element + "/ConText_Info", "" + dr[21]);

                    XmlDoc.AddElement(Element, "Last_Update_Date");
                    XmlDoc.SetText(Element + "/Last_Update_Date", UDT_S.ChangeTime("" + dr[22]));

                    #endregion

                    #region  UpdateRecord - 原生結構

                    //<UpdateRecord ID="100540" RefStudentID="54139">
                    //    <SchoolYear>99</SchoolYear>
                    //    <Semester>2</Semester>
                    //    <Name>呂小鍾</Name>
                    //    <StudentNumber>712043</StudentNumber>
                    //    <Gender>男</Gender>
                    //    <IDNumber>J12254139</IDNumber>
                    //    <Birthdate>1993/08/15</Birthdate>
                    //    <GradeYear>3</GradeYear>
                    //    <Department>汽車科</Department>
                    //    <UpdateDate>2011/06/23</UpdateDate>
                    //    <UpdateCode>001</UpdateCode>
                    //    <UpdateType />
                    //    <UpdateReason />
                    //    <UpdateDescription>持國民中學畢業證明書者(含國中補校)</UpdateDescription>
                    //    <ADDate>1753/01/01</ADDate>
                    //    <ADNumber>澔字第1111111號</ADNumber>
                    //    <LastADDate />
                    //    <LastADNumber />
                    //    <Comment />
                    //    <ContextInfo>
                    //        <ContextInfo>
                    //            <GraduateComment>
                    //            </GraduateComment>
                    //            <ClassType> 3</ClassType>
                    //            <SpecialStatus>
                    //            </SpecialStatus>
                    //            <GraduateSchool> 市立光華國中</GraduateSchool>
                    //            <GraduateSchoolYear> 99</GraduateSchoolYear>
                    //            <GraduateSchoolCode> 183503</GraduateSchoolCode>
                    //            <GraduateSchoolLocationCode> 18</GraduateSchoolLocationCode>
                    //        </ContextInfo>
                    //    </ContextInfo>
                    //</UpdateRecord>

                    #endregion
                }
            }
        }
    }
}
