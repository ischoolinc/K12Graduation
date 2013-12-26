using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Graduation.Modules
{
    class SchoolYearObj
    {
        public int _SchoolYear { get; set; }
        public Dictionary<string, List<GraduateUDT>> _ClassNameList { get; set; }

        public SchoolYearObj(int SchoolYear)
        {
            _SchoolYear = SchoolYear;
            _ClassNameList = new Dictionary<string, List<GraduateUDT>>();
        }
    }
}
