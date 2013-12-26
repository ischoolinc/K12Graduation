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
    public class ArchiveHelper
    {
        /// <summary>
        /// FISCA Query物件
        /// </summary>
        QueryHelper _queryhelper = new QueryHelper();

        /// <summary>
        /// 學生清單
        /// </summary>
        private List<GraduateUDT> Students;

        private Dictionary<string, GraduateUDT> StudentDics;

        private string DBTableName { get; set; }

        private string XmlTableName { get; set; }

        private List<FieldInfo> Fields { get; set; }

        public ArchiveHelper(List<GraduateUDT> students)
        {
            Students = students;

            StudentDics = new Dictionary<string, GraduateUDT>();
            Fields = new List<FieldInfo>();

            foreach (GraduateUDT each in Students)
            {
                if (!StudentDics.ContainsKey(each.StudentID))
                    StudentDics.Add(each.StudentID, each);
            }
        }

        public List<AllXMLDataUDT> GenerateXml(string tableName, string fields, string forgeKey, string desc)
        {
            ParseTableName(tableName);
            ParseFieldName(fields);

            string forgeKeyName = "";

            foreach (FieldInfo field in Fields)
            {
                if (field.DBName == forgeKey)
                {
                    forgeKeyName = field.XmlName;
                    break;
                }
            }

            if (string.IsNullOrEmpty(forgeKeyName))
            {
                throw new ArgumentException(string.Format("{0} 不存在於Fields描述中。", forgeKey));
            }

            string sqlStr1 = "Select " + GetSelectFields() + " From " + DBTableName + " where " + UDT_S.PopOneCondition(forgeKey, StudentDics.Keys.ToList());

            DataTable dt = _queryhelper.Select(sqlStr1);

            //XmlHelper收集器
            Dictionary<string, List<DSXmlHelper>> HelperDic = new Dictionary<string, List<DSXmlHelper>>();

            foreach (DataRow row in dt.Rows)
            {
                string StudentID = "" + row[forgeKeyName];
                if (!HelperDic.ContainsKey(StudentID))
                    HelperDic.Add(StudentID, new List<DSXmlHelper>());

                //XmlTableName = RootElement ?
                DSXmlHelper h = new DSXmlHelper();                
                h.AddElement(XmlTableName); //建立該筆資料的Element

                foreach (FieldInfo field in Fields)
                {
                    string RowValue = "" + row[field.XmlName];

                    //if (string.IsNullOrEmpty(RowValue))
                    //    continue;

                    if (field.IsAttribute)
                        h.SetAttribute(XmlTableName, field.XmlName, RowValue);
                    else
                        h.AddElement(XmlTableName, field.XmlName, RowValue, true);
                }

                HelperDic[StudentID].Add(h);
            }

            List<AllXMLDataUDT> aUDTList = new List<AllXMLDataUDT>();
            foreach (string each1 in HelperDic.Keys)
            {
                AllXMLDataUDT aUDT = new AllXMLDataUDT();
                aUDT.Name = desc;
                aUDT.StudentID = each1;
                aUDT.RefUDT_ID = StudentDics[each1].UID;
                DSXmlHelper dh = new DSXmlHelper(XmlTableName + "List");
                foreach (DSXmlHelper each2 in HelperDic[each1])
                {
                    dh.AddXmlString(".", each2.BaseElement.InnerXml);
                }
                aUDT.Content = dh.BaseElement.OuterXml;

                aUDTList.Add(aUDT);
            }

            return aUDTList;
        }

        private string GetSelectFields()
        {
            List<string> fields = new List<string>();
            foreach (FieldInfo field in Fields)
            {
                fields.Add(field.ToString());
            }

            return string.Join(",", fields.ToArray());
        }

        //id as @ID,name as Name,student_number as StudentNumber
        private void ParseFieldName(string fields)
        {
            Fields.Clear();
            string[] fieldDescs = fields.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string fieldDesc in fieldDescs)
                Fields.Add(new FieldInfo(fieldDesc));
        }

        private void ParseTableName(string tableName)
        {
            string[] parts = tableName.Split(new string[] { " as " }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                DBTableName = parts[0].Trim();
                XmlTableName = parts[1].Trim();
            }
            else
            {
                DBTableName = parts[0].Trim();
                XmlTableName = parts[0].Trim();
            }
        }

        // student_number as @StudentNumber
        class FieldInfo
        {
            //private string fieldDesc;

            public FieldInfo(string fieldDesc)
            {
                string[] parts = fieldDesc.Split(new string[] { " as " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    DBName = parts[0].Trim();
                    XmlName = parts[1].Trim();
                }
                else
                {
                    DBName = parts[0].Trim();
                    XmlName = parts[0].Trim();
                }

                if (XmlName.StartsWith("@"))
                {
                    IsAttribute = true;
                    XmlName = XmlName.Remove(0, 1); //@Student -> Student
                }
                else
                    IsAttribute = false;
            }

            public string DBName { get; private set; }

            public string XmlName { get; private set; }

            public bool IsAttribute { get; private set; }

            public override string ToString()
            {
                //name as "@Name"
                return string.Format("{0} as \"{1}\"", DBName, XmlName);
            }
        }
    }
}
