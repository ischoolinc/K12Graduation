using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Graduation.Modules
{
    public class DeleteAllData2
    {
        public string GrUID { get; set; }
        public GraduateUDT _UDT { get; set; }
        public List<AllXMLDataUDT> _AllXMLDataUDT { get; set; }
        public List<PhotoDataUDT> _PhotoDataUDT { get; set; }
        public List<WrittenInformationUDT> _WrittenInformationUDT { get; set; }

        public DeleteAllData2(GraduateUDT UDT)
        {
            GrUID = UDT.UID;
            _UDT = UDT;
            _AllXMLDataUDT = new List<AllXMLDataUDT>();
            _PhotoDataUDT = new List<PhotoDataUDT>();
            _WrittenInformationUDT = new List<WrittenInformationUDT>();
        }
    }
}
