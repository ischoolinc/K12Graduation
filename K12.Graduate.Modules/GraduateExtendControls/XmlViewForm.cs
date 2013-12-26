using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using System.Xml;
using FISCA.DSAUtil;

namespace K12.Graduation.Modules
{
    public partial class XmlViewForm : BaseForm
    {
        public XmlViewForm()
        {
            InitializeComponent();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void PopXml(string xml)
        {
            textBoxX1.Text = DSXmlHelper.Format(xml);

        }
    }
}
