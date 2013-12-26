using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Graduation.Modules
{
    class UpdateRecordHelp
    {
        /// <summary>
        /// (學生系統編號/畢業學年度)
        /// </summary>
        public Dictionary<string, int> UpdateRecordBySchoolYear { get; set; }

        /// <summary>
        /// 未有畢業異動記錄
        /// </summary>
        public List<string> NotUpdateRecordList { get; set; }

        public UpdateRecordHelp()
        {
        }

        /// <summary>
        /// 傳入學生ID,取得並處理畢業異動(註解)
        /// </summary>
        public UpdateRecordHelp(List<string> StudentIDList)
        {
            UpdateRecordBySchoolYear = new Dictionary<string, int>();
            NotUpdateRecordList = new List<string>();

            List<K12.Data.UpdateRecordRecord> UpdateRecordList = K12.Data.UpdateRecord.SelectByStudentIDs(StudentIDList);

            Dictionary<string, UpdateRecordRecord> UpdateRecordDic = new Dictionary<string, UpdateRecordRecord>();
            foreach (UpdateRecordRecord each in UpdateRecordList)
            {
                if (!UpdateRecordDic.ContainsKey(each.StudentID))
                {
                    UpdateRecordDic.Add(each.StudentID, null);
                }

                if (each.UpdateCode == "2" || each.UpdateCode == "501") //當此筆為 畢業 異動記錄
                {
                    UpdateRecordDic[each.StudentID] = each;
                }
            }

            foreach (string each in UpdateRecordDic.Keys)
            {
                if (UpdateRecordDic[each] != null)
                {
                    if (!UpdateRecordBySchoolYear.ContainsKey(each)) //是否已有資料
                    {
                        if (UpdateRecordDic[each].SchoolYear.HasValue)
                        {
                            UpdateRecordBySchoolYear.Add(UpdateRecordDic[each].StudentID, UpdateRecordDic[each].SchoolYear.Value);
                        }
                        else
                        {
                            //有畢業異動,卻又無畢業學年度資料
                            //目前系統,是必要輸入之資訊
                        }
                    }
                    else
                    {
                        //重複2筆,畢業異動者
                    }
                }
                else
                {
                    NotUpdateRecordList.Add(each);
                }
            }
        }

        /// <summary>
        /// 傳入學生ID,取得畢業學年度
        /// </summary>
        public int GetSY(string ID)
        {     
            if (UpdateRecordBySchoolYear.ContainsKey(ID)) //有畢業學年度者
                return UpdateRecordBySchoolYear[ID]; //畢業學年度
            else
                return 0; //無畢業學年度者,預設為0年
        }
    }
}
