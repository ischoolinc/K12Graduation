using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.UDT;
using System.Xml;
using FISCA.DSAUtil;

namespace K12.Graduation.Modules
{
    public partial class StudentDescription : FISCA.Presentation.DescriptionPane
    {
        //UDT物件
        private AccessHelper _AccessHelper = new AccessHelper();

        BackgroundWorker BGW = new BackgroundWorker();

        //背景忙碌
        private bool BkWBool = false;

        public StudentDescription()
        {
            InitializeComponent();

            this.PrimaryKeyChanged += new EventHandler(StudentDescription_PrimaryKeyChanged);
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            List<GraduateUDT> list = _AccessHelper.Select<GraduateUDT>(string.Format("UID='{0}'", this.PrimaryKey));
            e.Result = list;
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (BkWBool) //如果有其他的更新事件
            {
                BkWBool = false;
                BGW.RunWorkerAsync();
                return;
            }

            List<GraduateUDT> list = (List<GraduateUDT>)e.Result;
            if (list.Count == 1)
            {
                GraduateUDT GraduateOBJ = list[0];
                StringBuilder sb = new StringBuilder();
                sb.Append("索引分類：" + GraduateOBJ.ArchiveNote);
                sb.Append("　班級：" + GraduateOBJ.ClassName);
                sb.Append("　座號：" + GraduateOBJ.SeatNo);
                sb.Append("　姓名：" + GraduateOBJ.Name);
                sb.Append("　學號：" + GraduateOBJ.StudentNumber);

                StringBuilder sb2 = new StringBuilder();
                sb2.Append("學生類別：");
                if (!string.IsNullOrEmpty(GraduateOBJ.Tag))
                {
                    XmlElement xmlelement = DSXmlHelper.LoadXml(GraduateOBJ.Tag);
                    foreach (XmlElement xmlE in xmlelement.SelectNodes("Tag"))
                    {

                        string Prefix = xmlE.GetAttribute("Prefix");
                        string Name = xmlE.GetAttribute("Name");
                        if (!string.IsNullOrEmpty(Prefix))
                        {
                            sb2.Append("(" + Prefix + ":" + Name + ")");
                        }
                        else
                        {
                            sb2.Append("(" + Name + ")");
                        }
                    }
                }
                else
                {
                    sb2.Append("無學生類別資訊");
                }
                labelX1.Text = sb.ToString();
                labelX2.Text = sb2.ToString();
            }
        }

        void StudentDescription_PrimaryKeyChanged(object sender, EventArgs e)
        {
            if (BGW.IsBusy)
            {
                BkWBool = true;
            }
            else
            {
                BGW.RunWorkerAsync();
            }
        }
    }
}
