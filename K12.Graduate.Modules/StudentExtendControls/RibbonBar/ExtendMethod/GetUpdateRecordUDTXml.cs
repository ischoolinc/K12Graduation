using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using FISCA.Data;
using System.Data;
using FISCA.DSAUtil;

namespace K12.Graduation.Modules
{
    class GetUpdateRecordUDTXml
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
        Dictionary<string, XmlUdtHelp> XmlUDTDic = new Dictionary<string, XmlUdtHelp>();

        public GetUpdateRecordUDTXml(List<GraduateUDT> InsertArchiveList)
        {
            List<string> list = new List<string>();
            foreach (GraduateUDT each in InsertArchiveList)
            {
                if (!list.Contains(each.StudentID))
                    list.Add(each.StudentID);
            }

            //QueryHelper
            QueryHelper _queryhelper = new QueryHelper();
            string sqlStr1 = "Select * From update_record where " + UDT_S.PopOneCondition("ref_student_id", list);

            //取得所有學生ID的異動資料
            DataTable dt1 = _queryhelper.Select(sqlStr1);

            foreach (GraduateUDT each in InsertArchiveList)
            {
                if (!XmlUDTDic.ContainsKey(each.StudentID))
                {
                    XmlUdtHelp H = new XmlUdtHelp(each, "UpdateRecordList"); //Mini盒子
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

                    XmlDoc.AddElement("UpdateRecord");
                    XmlDoc.SetAttribute("UpdateRecord", "ID", "" + dr[0]);

                    XmlDoc.AddElement("UpdateRecord", "RefStudentID");
                    XmlDoc.SetText("UpdateRecord/RefStudentID", "" + dr[1]);

                    XmlDoc.AddElement("UpdateRecord", "SchoolYear");
                    XmlDoc.SetText("UpdateRecord/SchoolYear", "" + dr[2]);

                    XmlDoc.AddElement("UpdateRecord", "Semester");
                    XmlDoc.SetText("UpdateRecord/Semester", "" + dr[3]);

                    XmlDoc.AddElement("UpdateRecord", "S_Name");
                    XmlDoc.SetText("UpdateRecord/S_Name", "" + dr[4]);

                    XmlDoc.AddElement("UpdateRecord", "S_StudentNumber");
                    XmlDoc.SetText("UpdateRecord/S_StudentNumber", "" + dr[5]);

                    XmlDoc.AddElement("UpdateRecord", "S_Gender");
                    XmlDoc.SetText("UpdateRecord/S_Gender", "" + dr[6]);

                    XmlDoc.AddElement("UpdateRecord", "S_IDNumber");
                    XmlDoc.SetText("UpdateRecord/S_IDNumber", "" + dr[7]);

                    XmlDoc.AddElement("UpdateRecord", "S_Birthdate");
                    XmlDoc.SetText("UpdateRecord/S_Birthdate", UDT_S.ChangeTime("" + dr[8]));

                    XmlDoc.AddElement("UpdateRecord", "S_GradeYear");
                    XmlDoc.SetText("UpdateRecord/S_GradeYear", "" + dr[9]);

                    XmlDoc.AddElement("UpdateRecord", "S_Dept");
                    XmlDoc.SetText("UpdateRecord/S_Dept", "" + dr[10]);

                    XmlDoc.AddElement("UpdateRecord", "Update_Date");
                    XmlDoc.SetText("UpdateRecord/Update_Date", UDT_S.ChangeTime("" + dr[11]));

                    XmlDoc.AddElement("UpdateRecord", "Update_Code");
                    XmlDoc.SetText("UpdateRecord/Update_Code", "" + dr[12]);

                    XmlDoc.AddElement("UpdateRecord", "Update_Type");
                    XmlDoc.SetText("UpdateRecord/Update_Type", "" + dr[13]);

                    XmlDoc.AddElement("UpdateRecord", "Update_Reason");
                    XmlDoc.SetText("UpdateRecord/Update_Reason", "" + dr[14]);

                    XmlDoc.AddElement("UpdateRecord", "Update_Desc");
                    XmlDoc.SetText("UpdateRecord/Update_Desc", "" + dr[15]);

                    XmlDoc.AddElement("UpdateRecord", "AD_Date");
                    XmlDoc.SetText("UpdateRecord/AD_Date", "" + dr[16]);

                    XmlDoc.AddElement("UpdateRecord", "AD_Numbar");
                    XmlDoc.SetText("UpdateRecord/AD_Numbar", "" + dr[17]);

                    XmlDoc.AddElement("UpdateRecord", "Last_AD_Date");
                    XmlDoc.SetText("UpdateRecord/Last_AD_Date", UDT_S.ChangeTime("" + dr[18]));

                    XmlDoc.AddElement("UpdateRecord", "Last_AD_Numbar");
                    XmlDoc.SetText("UpdateRecord/Last_AD_Numbar", "" + dr[19]);

                    XmlDoc.AddElement("UpdateRecord", "Comment");
                    XmlDoc.SetText("UpdateRecord/Comment", "" + dr[20]);
                    //XML
                    XmlDoc.AddElement("UpdateRecord", "ConText_Info");
                    XmlDoc.AddXmlString("UpdateRecord/ConText_Info", "" + dr[21]);

                    XmlDoc.AddElement("UpdateRecord", "Last_Update_Date");
                    XmlDoc.SetText("UpdateRecord/Last_Update_Date", UDT_S.ChangeTime("" + dr[22])); 

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

        public List<AllXMLDataUDT> GetUpdateRecordList()
        {
            List<AllXMLDataUDT> AllXmlList = new List<AllXMLDataUDT>();

            foreach (string each in XmlUDTDic.Keys)
            {
                AllXmlList.Add(XmlUDTDic[each].GetUDTData("異動資料"));
            }
            return AllXmlList;
        }
    }
}
