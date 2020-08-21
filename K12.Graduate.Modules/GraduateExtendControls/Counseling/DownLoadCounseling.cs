using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using System.IO;
using FISCA.Data;
using FISCA.LogAgent;

namespace K12.Graduation.Modules
{
    public partial class DownLoadCounseling : BaseForm
    {
        /// <summary>
        /// 背景模式
        /// </summary>
        BackgroundWorker BGW = new BackgroundWorker();

        Timer T = new Timer();
        int T_int = 0;

        string FileName = "學籍表";

        /// <summary>
        /// FISCA Query物件
        /// </summary>
        QueryHelper _queryhelper = new QueryHelper();

        public DownLoadCounseling()
        {
            InitializeComponent();
        }

        private void DownLoadCounseling_Load(object sender, EventArgs e)
        {
            T.Interval = 1000; //每0.5秒
            T.Tick += new EventHandler(T_Tick);

            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
            BGW.ProgressChanged += new ProgressChangedEventHandler(BGW_ProgressChanged);
            BGW.WorkerReportsProgress = true;


            //書面資料Table
            string TableName = Tn._WriteCounselingUDT;
            string sqlStr2 = string.Format("select name,count(name) from {0} WHERE " + UDT_S.PopOneCondition("RefUDT_ID", GraduationAdmin.Instance.SelectedSource) + " group by name ", TableName.ToLower());
            DataTable dt = _queryhelper.Select(sqlStr2);
            foreach (DataRow each in dt.Rows)
            {
                //由於each[0]是檔名,因此需要把" (共"這個當條件字串移除,以比對檔名
                comboBoxEx1.Items.Add("" + each[0] + " (共" + each[1] + "張)");
            }
            if (comboBoxEx1.Items.Count > 0)
            {
                comboBoxEx1.SelectedIndex = 0;
            }
        }

        void T_Tick(object sender, EventArgs e)
        {
            T_int++;
            if (T_int % 2 != 0)
            {
                BGW.ReportProgress(T_int, "資料下載中．．");
            }
            else
            {
                BGW.ReportProgress(T_int, "資料下載中．．．．．");
            }
        }

        private void btnDownLoad_Click(object sender, EventArgs e)
        {
            if (!BGW.IsBusy)
            {
                if (cbDownLoadAll.Checked)
                    FileName = "*";
                else //由於comboBoxEx1.Text會當檔名,因此需要把" (共"這個當條件字串移除,以比對檔名
                    FileName = comboBoxEx1.Text.Remove(comboBoxEx1.Text.IndexOf(" (共"));

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "請選擇儲存學籍表位置";
                fbd.ShowNewFolderButton = true;

                if (fbd.ShowDialog() == DialogResult.Cancel) return;

                btnDownLoad.Enabled = false; //關閉按鈕
                T.Start();
                BGW.RunWorkerAsync(fbd.SelectedPath);
            }
            else
            {
                MsgBox.Show("背景模式,忙碌中...");
            }
        }

        void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage(e.UserState.ToString());
            }
            else
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("");
            }
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            string SelectPath = "" + e.Argument;
            AccessHelper _AccessHelper = new AccessHelper();
            //取得UDT - KEY
            List<string> KeyList = GraduationAdmin.Instance.SelectedSource;
            List<GraduateUDT> GrList = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("UID", KeyList)); //取得學生封存資料
            Dictionary<string, GraduateUDT> GrDic = new Dictionary<string, GraduateUDT>(); //建立索引
            foreach (GraduateUDT each in GrList)
            {
                if (!GrDic.ContainsKey(each.UID))
                {
                    GrDic.Add(each.UID, each);
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("下載學生書面資料");

            if (FileName == "*") //當FileName是*時,下載所有學生之書面資料
            {
                sb.AppendLine("下載操作：「所有書面資料」");
                List<WriteCounselingUDT> WrList = _AccessHelper.Select<WriteCounselingUDT>(UDT_S.PopOneCondition("RefUDT_ID", KeyList));

                foreach (WriteCounselingUDT each in WrList)
                {
                    if (!GrDic.ContainsKey(each.RefUDT_ID))
                        return;

                    sb.Append("班級「" + GrDic[each.RefUDT_ID].ClassName + "」");
                    sb.Append("座號「" + (GrDic[each.RefUDT_ID].SeatNo.HasValue ? GrDic[each.RefUDT_ID].SeatNo.Value.ToString() : "") + "」");
                    sb.Append("姓名「" + GrDic[each.RefUDT_ID].Name + "」");

                    //因為是所有書面,所以特別註記了單一書面的名稱
                    sb.AppendLine("書面名稱「" + each.Name + "」");
                    SaveValue(SelectPath, GrDic[each.RefUDT_ID], each);
                }
            }
            else
            {
                sb.AppendLine("下載操作：「指定書面名稱：" + FileName + "」");
                //需要下條件UID+FileName等於輸入內容,才回傳
                T.Start();
                List<WriteCounselingUDT> WrList = _AccessHelper.Select<WriteCounselingUDT>(UDT_S.PopTwoCondition("RefUDT_ID", FileName, KeyList));

                foreach (WriteCounselingUDT each in WrList)
                {
                    if (!GrDic.ContainsKey(each.RefUDT_ID))
                        return;

                    if (each.Name != FileName) //當名稱相符才下載所有資料
                        return;

                    sb.Append("班級「" + GrDic[each.RefUDT_ID].ClassName + "」");
                    sb.Append("座號「" + (GrDic[each.RefUDT_ID].SeatNo.HasValue ? GrDic[each.RefUDT_ID].SeatNo.Value.ToString() : "") + "」");
                    sb.AppendLine("姓名「" + GrDic[each.RefUDT_ID].Name + "」");
                    SaveValue(SelectPath, GrDic[each.RefUDT_ID], each);
                }
            }
            ApplicationLog.Log("畢業生檔案檢索.書面資料", "下載", sb.ToString());
            T.Stop();
            e.Result = SelectPath; //傳出選擇Pash
        }

        /// <summary>
        /// 儲存檔案
        /// </summary>
        private void SaveValue(string SelectPath, GraduateUDT value1, WriteCounselingUDT value2)
        {
            //讀取資料
            byte[] base64 = Convert.FromBase64String(value2.Content);

            //建立檔案
            StringBuilder sb = new StringBuilder();
            sb.Append(SelectPath + "\\");
            sb.Append(value1.StudentNumber + "_");
            sb.Append(value2.Name);
            sb.Append("(" + value1.Name + ")" + value2.Format);

            FileStream fs = new FileStream(sb.ToString(), FileMode.Create);
            //寫入檔案
            fs.Write(base64, 0, base64.Length);
            fs.Close();
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnDownLoad.Enabled = true; //按鈕
            T.Stop();

            if (e.Error == null)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("檔案下載完成!!");
                MsgBox.Show("檔案下載完成!!");
                System.Diagnostics.Process.Start("" + e.Result);
            }
            else
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("檔案下載發生錯誤!!");
                MsgBox.Show("檔案下載發生錯誤!!\n" + e.Error.Message);
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
            }

            this.Close();
        }

        private void cbDownLoadAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDownLoadAll.Checked)
                comboBoxEx1.Enabled = !cbDownLoadAll.Checked;
            else
                //未勾選
                comboBoxEx1.Enabled = !cbDownLoadAll.Checked;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
