using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Graduation.Modules
{
    class PhotoRecordHelp
    {
        public Dictionary<string, string> FreshmanPhoto { get; set; }
        public Dictionary<string, string> GraduatePhoto { get; set; }

        public PhotoRecordHelp()
        {
        }

        public PhotoRecordHelp(List<string> StudentList)
        {
            FreshmanPhoto = K12.Data.Photo.SelectFreshmanPhoto(StudentList);
            GraduatePhoto = K12.Data.Photo.SelectGraduatePhoto(StudentList);
        }

        /// <summary>
        /// 取得入學照片字串
        /// </summary>
        public string GetFH(string ID)
        {
            if (FreshmanPhoto.ContainsKey(ID))
                return FreshmanPhoto[ID];
            else
                return "";
        }

        /// <summary>
        /// 取得畢業照片字串
        /// </summary>
        public string GetGD(string ID)
        {
            if (GraduatePhoto.ContainsKey(ID))
                return GraduatePhoto[ID];
            else
                return "";
        }
    }
}
