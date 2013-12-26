using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Data;
using FISCA.UDT;

namespace K12.Graduation.Modules
{
    public partial class GetGraduationFrom : BaseForm
    {
        /// <summary>
        /// FISCA Query物件
        /// </summary>
        QueryHelper _queryhelper = new QueryHelper();

        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

        public List<string> UDTkey = new List<string>();

        public GetGraduationFrom()
        {
            InitializeComponent();
        }

        private void GetGraduationFrom_Load(object sender, EventArgs e)
        {
            string TableName = Tn._GraduateUDT;
            DataTable dt = _queryhelper.Select("select ArchiveNote from " + TableName.ToLower() + " group by ArchiveNote ORDER by ArchiveNote");

            List<string> ArchiveNotekey = new List<string>();
            foreach (DataRow each in dt.Rows)
            {
                string ArchiveNote = "" + each[0];
                if (!ArchiveNotekey.Contains(ArchiveNote))
                {
                    ArchiveNotekey.Add(ArchiveNote);
                }
            }

            ArchiveNotekey.Sort();

            cbDataLiat.Items.AddRange(ArchiveNotekey.ToArray());

            if (cbDataLiat.Items.Count > 0)
            {
                cbDataLiat.SelectedIndex = 0;
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {

            if (!cbGetAllData.Checked)
            {
                if (cbDataLiat.SelectedIndex != -1)
                {
                    string name = cbDataLiat.SelectedItem.ToString();
                    string TableName = Tn._GraduateUDT;
                    DataTable dt = _queryhelper.Select("select uid from " + TableName.ToLower() + " where ArchiveNote='" + name + "'");
                    UDTkey.Clear();
                    foreach (DataRow each in dt.Rows)
                    {
                        string uid = "" + each[0];
                        UDTkey.Add(uid);
                    }

                    FISCA.Presentation.MotherForm.SetStatusBarMessage("開始取得學生索引資料!!");
                    GraduationAdmin.Instance.BGW.RunWorkerAsync(UDTkey);
                }
                else
                {
                    MsgBox.Show("未選擇,已中止資料取得!!");
                }
            }
            else
            {
                string TableName = Tn._GraduateUDT;
                DataTable dt = _queryhelper.Select("select uid from " + TableName.ToLower());
                UDTkey.Clear();
                foreach (DataRow each in dt.Rows)
                {
                    string uid = "" + each[0];
                    UDTkey.Add(uid);
                }

                FISCA.Presentation.MotherForm.SetStatusBarMessage("開始取得學生索引資料!!");
                GraduationAdmin.Instance.BGW.RunWorkerAsync(UDTkey);
            }



            this.Close();
        }

        private void cbGetAllData_CheckedChanged(object sender, EventArgs e)
        {
            cbDataLiat.Enabled = !cbGetAllData.Checked;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
