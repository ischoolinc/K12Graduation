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
    class GetDisciplineUDTXml
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

        public GetDisciplineUDTXml(List<GraduateUDT> InsertArchiveList)
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
            string sqlStr1 = "Select * From discipline where " + UDT_S.PopOneCondition("ref_student_id", list);

            //取得所有學生ID的獎懲資料
            DataTable dt1 = _queryhelper.Select(sqlStr1);

            foreach (GraduateUDT each in InsertArchiveList)
            {
                if (!XmlUDTDic.ContainsKey(each.StudentID))
                {
                    XmlUdtHelp H = new XmlUdtHelp(each, "DisciplineList"); //Mini盒子
                    XmlUDTDic.Add(each.StudentID, H);
                }
            }

            foreach (DataRow dr in dt1.Rows)
            {
                //先判斷是否為Dic內存之資料
                if (XmlUDTDic.ContainsKey("" + dr[8]))
                {
                    string student = "" + dr[8];
                    DSXmlHelper XmlDoc = XmlUDTDic[student]._XmlHelper;

                    #region Element

                    XmlDoc.AddElement("Discipline");
                    XmlDoc.SetAttribute("Discipline", "ID", "" + dr[0]);

                    XmlDoc.AddElement("Discipline", "RefStudentID");
                    XmlDoc.SetText("Discipline/RefStudentID", "" + dr[8]);

                    XmlDoc.AddElement("Discipline", "SchoolYear");
                    XmlDoc.SetText("Discipline/SchoolYear", "" + dr[1]);

                    XmlDoc.AddElement("Discipline", "Semester");
                    XmlDoc.SetText("Discipline/Semester", "" + dr[2]);

                    XmlDoc.AddElement("Discipline", "OccurDate");
                    XmlDoc.SetText("Discipline/OccurDate", UDT_S.ChangeTime("" + dr[4]));

                    XmlDoc.AddElement("Discipline", "RegisterDate");
                    XmlDoc.SetText("Discipline/RegisterDate", UDT_S.ChangeTime("" + dr[12]));

                    //0懲戒,1獎勵,2留察
                    XmlDoc.AddElement("Discipline", "MeritFlag");
                    XmlDoc.SetText("Discipline/MeritFlag", "" + dr[10]);

                    XmlDoc.AddElement("Discipline", "Reason");
                    XmlDoc.SetText("Discipline/Reason", "" + dr[6]);
                    //XML
                    XmlDoc.AddElement("Discipline", "Detail");
                    XmlDoc.AddXmlString("Discipline/Detail", "" + dr[7]); 

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

        public List<AllXMLDataUDT> GetDisciplineList()
        {
            List<AllXMLDataUDT> AllXmlList = new List<AllXMLDataUDT>();

            foreach (string each in XmlUDTDic.Keys)
            {
                AllXmlList.Add(XmlUDTDic[each].GetUDTData("獎懲資料"));
            }
            return AllXmlList;
        }

    }
}
