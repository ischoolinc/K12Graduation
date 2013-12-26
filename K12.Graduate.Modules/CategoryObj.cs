using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Graduation.Modules
{
    class NoteObj
    {
        public string _Note { get; set; }
        public Dictionary<string, List<GraduateUDT>> _ClassNameList { get; set; }

        public NoteObj(string Note)
        {
            _Note = Note;
            _ClassNameList = new Dictionary<string, List<GraduateUDT>>();
        }
    }
}
