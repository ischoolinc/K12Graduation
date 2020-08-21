using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using System.IO;
using FISCA.LogAgent;
using FISCA.Data;

namespace K12.Graduation.Modules
{
    //本資料項目用以儲存使用者設計之書面資料
    //如學籍表...
    [FISCA.Permission.FeatureCode("K12.Graduation.Modules.WriteCounselingItem", "書面檔案(輔導)")]
    public partial class WriteCounselingItem : DetailContentBase
    {
        //權限
        internal static FISCA.Permission.FeatureAce UserPermission;
        //背景模式
        private BackgroundWorker BGW = new BackgroundWorker();

        //UDT操作物件
        private AccessHelper _AccessHelper = new AccessHelper();

        //批次上傳
        BackgroundWorker linkBGW = new BackgroundWorker();

        //學生UDT
        GraduateUDT _StudentUdt;

        List<string> ReNameCheck;

        //背景忙碌
        private bool BkWBool = false;

        public WriteCounselingItem()
        {
            InitializeComponent();

            Group = "書面檔案(輔導)";

            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            UserPermission = FISCA.Permission.UserAcl.Current[FISCA.Permission.FeatureCodeAttribute.GetCode(GetType())];
            this.Enabled = UserPermission.Editable;

            linkBGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(linkBGW_RunWorkerCompleted);
            linkBGW.DoWork += new DoWorkEventHandler(linkBGW_DoWork);
            linkBGW.WorkerReportsProgress = true;
            linkBGW.ProgressChanged += new ProgressChangedEventHandler(linkBGW_ProgressChanged);
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得PrimaryKey的UDT資料
            List<GraduateUDT> GraduateUDTList = _AccessHelper.Select<GraduateUDT>(string.Format("UID='{0}'", this.PrimaryKey));
            if (GraduateUDTList.Count == 1)
            {
                _StudentUdt = GraduateUDTList[0];
                //再得此UDT的相依UDT資料
                List<WriteCounselingUDT> listUDT = _AccessHelper.Select<WriteCounselingUDT>(string.Format("RefUDT_ID='{0}'", GraduateUDTList[0].UID));
                e.Result = listUDT;
            }
            else if (GraduateUDTList.Count > 1) //UDT資料多餘
            {
                MsgBox.Show("UDT資料有誤!!");
            }
            else //無UDT資料
            {

            }
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (BkWBool) //如果有其他的更新事件
            {
                BkWBool = false;
                BGW.RunWorkerAsync();
                return;
            }

            if (e.Error == null)
            {
                List<WriteCounselingUDT> listUDT = (List<WriteCounselingUDT>)e.Result;
                if (listUDT != null)
                {
                    if (listUDT.Count != 0) //如果不為0
                    {
                        BindData(listUDT);
                    }
                }
            }

            this.Loading = false;
        }

        private void BindData(List<WriteCounselingUDT> listUDT)
        {
            foreach (WriteCounselingUDT eachUDT in listUDT)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Cells[0].Value = eachUDT.Date.HasValue ? eachUDT.Date.Value.ToShortDateString() : "";
                row.Cells[0].Tag = eachUDT.UID; //此筆記錄UID,用以刪除使用
                row.Cells[1].Value = eachUDT.Name;
                row.Cells[2].Value = eachUDT.Format;
                row.Cells[3].Value = "Download";
                row.Tag = eachUDT.Content;
                dataGridViewX1.Rows.Add(row);
            }
        }

        /// <summary>
        /// KEY值切換時
        /// </summary>
        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            #region PrimaryKey更新
            if (this.PrimaryKey != "")
            {
                this.Loading = true;

                if (BGW.IsBusy)
                {
                    BkWBool = true;
                }
                else
                {
                    dataGridViewX1.Rows.Clear();
                    BGW.RunWorkerAsync();
                }
            }
            #endregion
        }

        private void dataGridViewX1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            DataGridViewRow row = dataGridViewX1.Rows[e.RowIndex];
            //下載書面資料
            if (e.ColumnIndex == Column4.Index)
            {
                if ("" + row.Tag != "") //如果書面資料不是空的
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "另存新檔";
                    sfd.FileName = _StudentUdt.StudentNumber + "_" + row.Cells[1].Value + "(" + _StudentUdt.Name + ")";
                    sfd.Filter = string.Format("檔案 (*{0})|{0}|所有檔案 (*.*)|*.*", "" + row.Cells[2].Value);

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
                            byte[] tempBuffer = Convert.FromBase64String("" + dataGridViewX1.Rows[e.RowIndex].Tag);
                            fs.Write(tempBuffer, 0, tempBuffer.Length);
                            fs.Close();
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                        catch
                        {
                            FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "另存檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }



                }


            }
        }

        /// <summary>
        /// 按下儲存時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSaveButtonClick(EventArgs e)
        {
            //目前並不提供修改資料
        }

        /// <summary>
        /// 取消儲存時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCancelButtonClick(EventArgs e)
        {
            //目前並不提供修改資料
        }

        private void dataGridViewX1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult dr = MsgBox.Show("您確定於封存資料中,刪除選擇之書面記錄(輔導)?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.No)
            {
                MsgBox.Show("已中止刪除資料！");
                e.Cancel = true;
            }
            else
            {
                //說明:
                //CurrentRow的TAG內存之UID / 是WriteCounselingUDT的UID
                //欲取得GraduateUDT資料,需要先取得WriteCounselingUDT
                //後由WriteCounselingUDT的RefUDT_ID取得GraduateUDT資料

                List<string> uidList = new List<string>();
                foreach (DataGridViewRow each in dataGridViewX1.SelectedRows)
                {
                    string UID = "" + each.Cells[0].Tag;

                    if (!string.IsNullOrEmpty(UID))
                    {
                        uidList.Add(UID);
                    }
                }

                //取得所選的書面資料

                List<WriteCounselingUDT> listUDT2 = _AccessHelper.Select<WriteCounselingUDT>(UDT_S.PopOneCondition("UID", uidList));
                if (listUDT2.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("班級「" + _StudentUdt.ClassName + "」");
                    sb.Append("座號「" + (_StudentUdt.SeatNo.HasValue ? _StudentUdt.SeatNo.Value.ToString() : "") + "」");
                    sb.Append("姓名「" + _StudentUdt.Name + "」");
                    sb.AppendLine("學號「" + _StudentUdt.StudentNumber + "」");
                    foreach (WriteCounselingUDT each in listUDT2)
                    {
                        sb.AppendLine("已刪除書面(輔導)資料「" + each.Name + each.Format + "」");
                    }


                    _AccessHelper.DeletedValues(listUDT2.ToArray());

                    ApplicationLog.Log("畢業生檔案檢索.書面資料(輔導)", "刪除", sb.ToString());
                    MsgBox.Show("已刪除所選書面資料(輔導)");
                    e.Cancel = true;
                    OnPrimaryKeyChanged(null);
                }
                else
                {
                    MsgBox.Show("刪除書面資料發生錯誤！");
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// 單一書面檔案的上傳
        /// </summary>
        private void linkUpData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("書面上傳支援 (*.docx;*.xlsx;*.doc;*.xls;*.pdf;*.jpg;*.png;*.tiff)|*.docx;*.xlsx;*.doc;*.xls;*.pdf;*.jpg;*.png;*.tiff");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "選擇上傳檔案";
            ofd.Filter = sb.ToString();
            ofd.Multiselect = true;

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            linkBGW.RunWorkerAsync(ofd.FileNames); //傳入選擇之檔案清單
        }

        void linkBGW_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] ofd = (string[])e.Argument;

            List<string> ofdName = new List<string>();
            foreach (string each in ofd)
            {
                ofdName.Add(Path.GetFileName(each));
            }

            QueryHelper _queryhelper = new QueryHelper();
            AccessHelper _AccessHelper = new AccessHelper();

            string TableName = Tn._WriteCounselingUDT;
            DataTable dt = _queryhelper.Select("select name,format from " + TableName.ToLower() + " where " + string.Format("RefUDT_ID = '{0}'", this.PrimaryKey));

            ReNameCheck = new List<string>();
            foreach (DataRow each in dt.Rows)
            {
                string information = "" + each["name"] + each["format"];

                //上傳檔案必須確認
                //1.學生身上有哪些書面資料
                if (ofdName.Contains(information))
                {
                    //2.檔名來源是否重覆
                    ReNameCheck.Add(information);
                }
            }

            //如果檔名重覆
            if (ReNameCheck.Count != 0)
            {
                e.Cancel = true;
            }
            else
            {

                List<WriteCounselingUDT> InsertUDTData = new List<WriteCounselingUDT>();

                //Log
                StringBuilder Logsb = new StringBuilder();
                Logsb.AppendLine("批次上傳書面(輔導)資料：");
                string SeatNo = _StudentUdt.SeatNo.HasValue ? _StudentUdt.SeatNo.Value.ToString() : "";
                Logsb.AppendLine("班級「" + _StudentUdt.ClassName + "」座號「" + SeatNo + "」學號「" + _StudentUdt.StudentNumber + "」姓名「" + _StudentUdt.Name + "」");


                //3.使用者選擇之資料(去路徑&副檔名)即為檔名
                foreach (string each in ofd)
                {
                    WriteCounselingUDT udt = new WriteCounselingUDT();
                    udt.RefUDT_ID = this.PrimaryKey;
                    udt.StudentID = _StudentUdt.StudentID;
                    udt.Name = Path.GetFileNameWithoutExtension(each);
                    udt.Format = Path.GetExtension(each).ToLower();
                    udt.Date = DateTime.Today; //封存日期為今天

                    Logsb.Append("書面名稱(輔導)「" + udt.Name + udt.Format + "」");

                    FileStream fs = new FileStream(each, FileMode.Open);
                    byte[] tempBuffer = new byte[fs.Length];
                    fs.Read(tempBuffer, 0, tempBuffer.Length);
                    string base64 = Convert.ToBase64String(tempBuffer);

                    if (base64 != "") //如果不是空值
                    {
                        udt.Content = base64;
                        InsertUDTData.Add(udt);
                    }
                }



                if (InsertUDTData.Count != 0)
                {
                    _AccessHelper.InsertValues(InsertUDTData.ToArray());

                    ApplicationLog.Log("畢業生檔案檢索.個人書面資料(輔導)", "批次上傳", Logsb.ToString());
                }
            }
        }

        void linkBGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState);
        }

        void linkBGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Cancelled)
            {
                StringBuilder sb2 = new StringBuilder();
                sb2.AppendLine("檔案與伺服端檔名重覆：");
                foreach (string each in ReNameCheck)
                {
                    sb2.AppendLine(each);
                }
                MsgBox.Show(sb2.ToString());
                return;
            }


            if (e.Error != null)
            {
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
                MsgBox.Show("檔案上傳失敗\n" + e.Error.Message);
                return;
            }

            MsgBox.Show("書面檔案上傳完成");
            FISCA.Presentation.MotherForm.SetStatusBarMessage("書面檔案上傳完成");
            OnPrimaryKeyChanged(null);


        }
    }
}
