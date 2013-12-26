using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Graduation.Modules
{
    class TagViewObj
    {
        public string _Prefix { get; set; }
        public string _Name { get; set; }

        public TagViewObj(string Prefix, string Name)
        {
            _Prefix = Prefix;
            _Name = Name;
        }
    }
}
