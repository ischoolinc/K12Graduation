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
    public static class SemsMoralScore
    {
        /// <summary>
        /// 依據各別的DataTable建立UDT資料
        /// (By 德行表現成績)
        /// </summary>
        public static void SemsMoralScore_ExtendMethod(this GetUDTXml xml,string Element)
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

                    XmlDoc.AddElement(Element, "Diff");
                    XmlDoc.SetText(Element + "/Diff", "" + dr[4]);

                    XmlDoc.AddElement(Element, "Comment");
                    XmlDoc.SetText(Element + "/Comment", "" + dr[5]);

                    XmlDoc.AddElement(Element, "Other_Diff");
                    XmlDoc.AddXmlString(Element + "/Other_Diff", "" + dr[6]);

                    XmlDoc.AddElement(Element, "Text_Score");
                    XmlDoc.AddXmlString(Element + "/Text_Score", "" + dr[7]);

                    XmlDoc.AddElement(Element, "Initial_Summary");
                    XmlDoc.AddXmlString(Element + "/Initial_Summary", "" + dr[8]);

                    XmlDoc.AddElement(Element, "Summary");
                    XmlDoc.AddXmlString(Element + "/Summary", "" + dr[9]);
                    #endregion

                    #region 原生結構


                    #endregion
                }
            }
        }
    }
}
