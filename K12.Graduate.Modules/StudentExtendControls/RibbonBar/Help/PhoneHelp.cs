using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Graduation.Modules
{
    class PhoneHelp
    {
        /// <summary>
        /// (學生系統編號/地址資料)
        /// </summary>
        public Dictionary<string, PhoneRecord> PhoneDic { get; set; }

        public PhoneHelp()
        {
        }
        /// <summary>
        /// 傳入學生ID,取得並處理畢業異動(註解)
        /// </summary>
        public PhoneHelp(List<string> StudentIDList)
        {
            PhoneDic = new Dictionary<string, PhoneRecord>();
            //取得學生電話資料
            List<PhoneRecord> StudentPhoneList = Phone.SelectByStudentIDs(StudentIDList);
            foreach (PhoneRecord each in StudentPhoneList)
            {
                if (!PhoneDic.ContainsKey(each.RefStudentID))
                {
                    PhoneDic.Add(each.RefStudentID, each);
                }
            }
        }


        /// <summary>
        /// 傳入學生ID,取得電話資料
        /// </summary>
        public GraduateUDT GetSY(GraduateUDT udt)
        {
            if (PhoneDic.ContainsKey(udt.StudentID))
            {
                udt.Permanent = PhoneDic[udt.StudentID].Permanent.Trim();
                udt.Contact = PhoneDic[udt.StudentID].Contact.Trim();
                udt.Cell = PhoneDic[udt.StudentID].Cell.Trim();
                udt.Phone1 = PhoneDic[udt.StudentID].Phone1.Trim();
                udt.Phone2 = PhoneDic[udt.StudentID].Phone2.Trim();
                udt.Phone3 = PhoneDic[udt.StudentID].Phone3.Trim();         
            }
            return udt;

        }
    }
}
