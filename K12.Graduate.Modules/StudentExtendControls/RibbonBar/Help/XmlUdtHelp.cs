using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;

namespace K12.Graduation.Modules
{
    public class XmlUdtHelp
    {
        public DSXmlHelper _XmlHelper { get; set; }
        public GraduateUDT _GraUDT { get; set; }

        public XmlUdtHelp(GraduateUDT GraUDT,string RootElement)
        {
            _GraUDT = GraUDT;
            _XmlHelper = new DSXmlHelper(RootElement);
        }

        public AllXMLDataUDT GetUDTData(string UDTDataName)
        {
            AllXMLDataUDT udt = new AllXMLDataUDT();
            udt.Name = UDTDataName;
            udt.RefUDT_ID = _GraUDT.UID;
            udt.StudentID = _GraUDT.StudentID;
            if (_XmlHelper != null)
            {
                udt.Content = _XmlHelper.BaseElement.OuterXml; //為何xml是null
            }
            return udt;
        }
    }
}
