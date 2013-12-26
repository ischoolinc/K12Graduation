using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.LogAgent;

namespace K12.Graduation.Modules
{

    public class DeleteAllData1
    {
        public Dictionary<string, DeleteAllData2> dic { get; set; }

        public DeleteAllData1(List<GraduateUDT> list1)
        {
            dic = new Dictionary<string, DeleteAllData2>();
            foreach (GraduateUDT each in list1)
            {
                if (!dic.ContainsKey(each.UID))
                {
                    dic.Add(each.UID, new DeleteAllData2(each));
                }
            }
        }

        public void setAllXml(List<AllXMLDataUDT> udt)
        {
            foreach (AllXMLDataUDT each in udt)
            {
                if (dic.ContainsKey(each.RefUDT_ID))
                {
                    dic[each.RefUDT_ID]._AllXMLDataUDT.Add(each);
                }
            }
        }

        public void setPhoto(List<PhotoDataUDT> udt)
        {
            foreach (PhotoDataUDT each in udt)
            {
                if (dic.ContainsKey(each.RefUDT_ID))
                {
                    dic[each.RefUDT_ID]._PhotoDataUDT.Add(each);
                }
            }
        }

        public void setWritten(List<WrittenInformationUDT> udt)
        {
            foreach (WrittenInformationUDT each in udt)
            {
                if (dic.ContainsKey(each.RefUDT_ID))
                {
                    dic[each.RefUDT_ID]._WrittenInformationUDT.Add(each);
                }
            }
        }

        /// <summary>
        /// 取得字串
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DeleteAllData2 each in dic.Values)
            {
                sb.Append("班級「" + each._UDT.ClassName + "」");
                sb.Append("座號「" + (each._UDT.SeatNo.HasValue ? each._UDT.SeatNo.Value.ToString() : "") + "」");
                sb.Append("學號「" + each._UDT.StudentNumber + "」");
                sb.AppendLine("姓名「" + each._UDT.Name + "」");
                sb.AppendLine("已刪除「基本」資料");

                foreach (AllXMLDataUDT each2 in each._AllXMLDataUDT)
                {
                    sb.AppendLine("已刪除XML資料「" + each2.Name + "」");
                }
                foreach (PhotoDataUDT each3 in each._PhotoDataUDT)
                {
                    if (!string.IsNullOrEmpty(each3.FreshmanPhoto))
                    {
                        sb.AppendLine("已刪除「入學」照片資料");
                    }
                    if (!string.IsNullOrEmpty(each3.GraduatePhoto))
                    {
                        sb.AppendLine("已刪除「畢業」照片資料");
                    }
                }
                foreach (WrittenInformationUDT each4 in each._WrittenInformationUDT)
                {
                    sb.AppendLine("已刪除書面資料「" + each4.Name + "」");
                }
                sb.AppendLine("");
            }
            return sb.ToString();
        }
    }
}
