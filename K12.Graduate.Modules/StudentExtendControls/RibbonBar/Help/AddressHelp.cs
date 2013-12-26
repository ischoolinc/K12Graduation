using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Graduation.Modules
{
    class AddressHelp
    {
        /// <summary>
        /// (學生系統編號/地址資料)
        /// </summary>
        public Dictionary<string, AddressRecord> AddressDic { get; set; }

        public AddressHelp()
        {
        }

        /// <summary>
        /// 傳入學生ID,取得並處理畢業異動(註解)
        /// </summary>
        public AddressHelp(List<string> StudentIDList)
        {
            AddressDic = new Dictionary<string, AddressRecord>();
            //取得選擇學生的地址資料
            List<AddressRecord> StudentAddressList = Address.SelectByStudentIDs(StudentIDList);
            foreach (AddressRecord each in StudentAddressList)
            {
                if (!AddressDic.ContainsKey(each.RefStudentID))
                {
                    AddressDic.Add(each.RefStudentID, each);
                }
            }
        }

        /// <summary>
        /// 傳入學生ID,取得地址資料
        /// </summary>
        public GraduateUDT GetSY(GraduateUDT udt)
        {
            if (AddressDic.ContainsKey(udt.StudentID))
            {
                AddressRecord ar = AddressDic[udt.StudentID];
                if (ar != null)
                {
                    udt.PermanentZipCode = ar.PermanentZipCode;
                    udt.PermanentAddress = ar.PermanentCounty + ar.PermanentTown + ar.PermanentDistrict + ar.PermanentArea + ar.PermanentDetail;

                    udt.MailingZipCode = ar.MailingZipCode;
                    udt.MailingAddress = ar.MailingCounty + ar.MailingTown + ar.MailingDistrict + ar.MailingArea + ar.MailingDetail;

                    udt.OtherZipCode = ar.Address1ZipCode;
                    udt.OtherAddresses = ar.Address1County + ar.Address1Town + ar.Address1District + ar.Address1Area + ar.Address1Detail;
                }
                //udt.OtherAddresses2 = AddressDic[udt.StudentID].Address2Address.Trim();
                //udt.OtherAddresses3 = AddressDic[udt.StudentID].Address3Address.Trim();
            }

            return udt;

        }
    }
}
