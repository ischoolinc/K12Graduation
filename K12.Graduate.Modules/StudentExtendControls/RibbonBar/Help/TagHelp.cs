using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;
using FISCA.DSAUtil;

namespace K12.Graduation.Modules
{
    class TagHelp
    {
        Dictionary<string, List<StudentTagRecord>> TagDic { get; set; }

        public TagHelp()
        {
        }

        public TagHelp(List<string> StudentIDList)
        {
            TagDic = new Dictionary<string, List<StudentTagRecord>>();
            List<StudentTagRecord> StudentTagList = StudentTag.SelectByStudentIDs(StudentIDList);
            foreach (StudentTagRecord each in StudentTagList)
            {
                if (!TagDic.ContainsKey(each.RefStudentID))
                {
                    TagDic.Add(each.RefStudentID, new List<StudentTagRecord>());
                }
                TagDic[each.RefStudentID].Add(each);
            }
        }

        /// <summary>
        /// 取得學生類別字串
        /// </summary>
        public string GetSY(GraduateUDT udt)
        {
            //如果此學生有類別資訊
            if (TagDic.ContainsKey(udt.StudentID))
            {
                DSXmlHelper helper = new DSXmlHelper("TagList");

                foreach (StudentTagRecord each in TagDic[udt.StudentID])
                {
                    //each.FullName; //完整名稱
                    //each.Prefix; //前置詞
                    //each.Name; //標籤名稱
                    //<Tag FullName="" Prefix="" Name="">

                    helper.AddElement("Tag");
                    helper.SetAttribute("Tag", "Prefix", each.Prefix); //前置詞
                    helper.SetAttribute("Tag", "Name", each.Name); //前置詞
                    helper.AddText("Tag", each.FullName); //前置詞

                }

                return helper.BaseElement.OuterXml;
            }

            return "";
        }
    }
}
