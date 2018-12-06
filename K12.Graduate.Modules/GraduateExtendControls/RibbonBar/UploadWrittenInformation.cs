using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using System.IO;
using FISCA.UDT;
using Aspose.Words;
using Aspose.Cells;
using FISCA.Data;
using FISCA.LogAgent;

namespace K12.Graduation.Modules
{
    public partial class UploadWrittenInformation : BaseForm
    {
        //Path //處理路徑
        //File //處理檔案
        //Directory //處理目錄

        /// <summary>
        /// 背景模式
        /// </summary>
        BackgroundWorker BGW = new BackgroundWorker();

        /// <summary>
        /// UDT工作物件
        /// </summary>
        AccessHelper _AccessHelper = new AccessHelper();

        bool ByIDNumber = false;
        bool ByStudentNumber = false;
        string WritleName = "";
        string _Note = "";

        /// <summary>
        /// FISCA Query物件
        /// </summary>
        QueryHelper _queryhelper = new QueryHelper();

        public UploadWrittenInformation()
        {
            InitializeComponent();
        }

        private void ImportWrittenInformation_Load(object sender, EventArgs e)
        {
            //註冊背景事件
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
            BGW.ProgressChanged += new ProgressChangedEventHandler(BGW_ProgressChanged);
            BGW.WorkerReportsProgress = true;

            //我必需group by出目前的分類名稱有哪些
            //取得所有學生GraduateUDT的UID資料
            string TableName = Tn._GraduateUDT;
            DataTable dt = _queryhelper.Select("select UID from " + TableName.ToLower());
            List<string> UDTkey = new List<string>();
            foreach (DataRow each in dt.Rows)
            {
                UDTkey.Add("" + each[0]);
            }

            string sqlStr2 = string.Format("select ArchiveNote from {0} WHERE " + UDT_S.PopOneCondition("UID", UDTkey) + " group by ArchiveNote ORDER by ArchiveNote", TableName.ToLower());
            dt = _queryhelper.Select(sqlStr2);
            if (dt.Rows.Count > 0)
            {
                List<string> NoteList = new List<string>();
                foreach (DataRow each in dt.Rows)
                {
                    comboBoxEx1.Items.Add("" + each[0]);
                }
                comboBoxEx1.SelectedIndex = 0;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbWritleName.Text))
            {
                MsgBox.Show("請設定書面名稱!!");
                return;
            }

            if (string.IsNullOrEmpty(comboBoxEx1.Text))
            {
                MsgBox.Show("必須選擇所要上傳的[學生索引分類]!!");
                return;
            }

            if (!BGW.IsBusy)
            {
                #region 開始背景模式
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "請選擇匯入資料夾";
                fbd.ShowNewFolderButton = false;

                if (fbd.ShowDialog() == DialogResult.Cancel) return;

                if (radioButton1.Checked)
                {
                    ByIDNumber = true;
                }
                else if (radioButton2.Checked)
                {
                    ByStudentNumber = true;
                }

                WritleName = tbWritleName.Text;
                _Note = comboBoxEx1.Text;

                btnSelect.Enabled = false; //關閉按鈕
                BGW.RunWorkerAsync(fbd.SelectedPath); //開始背景模式 
                #endregion
            }
            else
            {
                MsgBox.Show("背景模式,忙碌中...");
            }
        }

        //進度顯示
        void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage(e.UserState.ToString(), e.ProgressPercentage);
            }
            else
                FISCA.Presentation.MotherForm.SetStatusBarMessage("", e.ProgressPercentage);
        }

        //開始背景模式
        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            BGW.ReportProgress(1, "收集資料夾內容資料...");
            //檔名 / 路徑
            Dictionary<string, string> FileDic = new Dictionary<string, string>();

            //篩選資料夾內容
            foreach (string each in Directory.GetFiles("" + e.Argument))
            {
                if (CheckExtension(each))
                {
                    string Without = Path.GetFileNameWithoutExtension(each);
                    if (Without.Contains('_')) //如果包含底線,則進行清除動作
                        Without = Without.Remove(Without.IndexOf('_'));

                    if (!FileDic.ContainsKey(Without))
                    {
                        FileDic.Add(Without, each); //檔名與位置
                    }

                }
            }
            BGW.ReportProgress(5, "收集資料夾內容資料...");
            //目前已整理出檔案內容

            if (ByIDNumber) //依學號上傳
            {
                InsertByIDNumber(FileDic);
            }
            else if (ByStudentNumber) //依身分證字號上傳
            {

            }
        }

        /// <summary>
        /// 依學號匯入
        /// </summary>
        private void InsertByIDNumber(Dictionary<string, string> FileDic)
        {
            if (FileDic.Count == 0)
                return;

            //新增與更新內容
            List<WrittenInformationUDT> InsertUDTData = new List<WrittenInformationUDT>();
            List<WrittenInformationUDT> UpDataUDTData = new List<WrittenInformationUDT>();

            BGW.ReportProgress(10, "判斷是否為封存學生...");

            //依照檔名,取得目前系統內該學號,是否為現存封存學生
            //需新增條件 - 封存分類(11/11日)
            List<GraduateUDT> ExtantStudentUDT = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("StudentNumber", FileDic.Keys.ToList()) + " AND ArchiveNote='" + _Note + "'");
            BGW.ReportProgress(15, "是否已有封存書面資料...");
            Dictionary<string, WrittenInformationUDT> HaveWrittenDic = new Dictionary<string, WrittenInformationUDT>();
            //取得該學生,是否有封存"學籍表"書面資料
            List<string> list = new List<string>();
            foreach (GraduateUDT each in ExtantStudentUDT)
            {
                list.Add(each.UID);
            }
            BGW.ReportProgress(18, "是否已有封存書面資料...");
            if (ExtantStudentUDT.Count != 0)
            {
                List<WrittenInformationUDT> HaveWrittenList = _AccessHelper.Select<WrittenInformationUDT>(UDT_S.PopOneCondition("RefUDT_ID", list));
                BGW.ReportProgress(25, "判斷書面資料...");
                //RefUDT_ID / Record
                foreach (WrittenInformationUDT each in HaveWrittenList)
                {
                    if (each.Name != WritleName) //不是本表單則跳開
                        continue;

                    if (!HaveWrittenDic.ContainsKey(each.RefUDT_ID))
                    {
                        HaveWrittenDic.Add(each.RefUDT_ID, each);
                    }
                    else
                    {
                        MsgBox.Show("學生有重覆2張不同之" + WritleName + "!!");
                    }

                }
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("書面名稱「" + WritleName + "」");

            int countTime = 35;
            BGW.ReportProgress(countTime);

            double count1 = ExtantStudentUDT.Count / 55; //百分之1的度量
            double count = 0;

            foreach (GraduateUDT each in ExtantStudentUDT)
            {
                count++;
                if (count >= count1)
                {
                    count = 0; //歸0
                    countTime++; //步進1
                    BGW.ReportProgress(countTime, "建立學生書面資料...");
                }

                if (FileDic.ContainsKey(each.StudentNumber)) //匯入資料夾中,是否有該學號記錄                
                {
                    //沒有相同的書面資料
                    if (!HaveWrittenDic.ContainsKey(each.UID))
                    {
                        sb.Append("「新增」書面資料,");
                        sb.Append("班級「" + each.ClassName + "」");
                        sb.Append("座號「" + (each.SeatNo.HasValue ? each.SeatNo.Value.ToString() : "") + "」");
                        sb.AppendLine("姓名「" + each.Name + "」");

                        WrittenInformationUDT udt = new WrittenInformationUDT();
                        udt.RefUDT_ID = each.UID;
                        udt.StudentID = each.StudentID;
                        udt.Name = WritleName;
                        udt.Format = Path.GetExtension(FileDic[each.StudentNumber]).ToLower();
                        udt.Date = DateTime.Today; //封存日期為今天

                        //依照副檔名,取得檔案內容
                        string base64 = InsertFormat(FileDic[each.StudentNumber]);
                        if (base64 != "") //如果不是空值
                        {
                            udt.Content = base64;
                            InsertUDTData.Add(udt);
                        }
                    }
                    else //如果有相同檔名
                    {
                        sb.Append("「更新」書面資料,");
                        sb.Append("班級「" + each.ClassName + "」");
                        sb.Append("座號「" + (each.SeatNo.HasValue ? each.SeatNo.Value.ToString() : "") + "」");
                        sb.AppendLine("姓名「" + each.Name + "」");

                        WrittenInformationUDT udt = HaveWrittenDic[each.UID];
                        //書面之名稱
                        udt.Name = WritleName;
                        //格式可調整如:DOC -> XLS
                        udt.Format = Path.GetExtension(FileDic[each.StudentNumber]).ToLower();
                        //更新日期調整為今天
                        udt.Date = DateTime.Today;
                        //依照副檔名,取得檔案內容
                        string base64 = InsertFormat(FileDic[each.StudentNumber]);
                        if (base64 != "")
                        {
                            udt.Content = base64;
                            UpDataUDTData.Add(udt);
                        }
                    }
                }
            }

            if (InsertUDTData.Count != 0)
            {
                BGW.ReportProgress(80, "開始新增上傳書面資料...");
                _AccessHelper.InsertValues(InsertUDTData.ToArray());
            }
            if (UpDataUDTData.Count != 0)
            {
                BGW.ReportProgress(90, "開始更新上傳書面資料...");
                _AccessHelper.UpdateValues(UpDataUDTData.ToArray());
            }

            BGW.ReportProgress(100, "上傳完成...");
            ApplicationLog.Log("畢業生檔案檢索.書面資料", "上傳", sb.ToString());
        }

        //完成
        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            btnSelect.Enabled = true; //按鈕

            if (e.Error == null)
            {
                MsgBox.Show("檔案上傳完成!!");
                FISCA.Presentation.MotherForm.SetStatusBarMessage("上傳完成!!");
                radioButton();
            }
            else
            {
                MsgBox.Show("檔案上傳發生錯誤!!\n" + e.Error.Message);
                FISCA.Presentation.MotherForm.SetStatusBarMessage("檔案上傳發生錯誤!!");
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);

                radioButton();
            }
            this.Close();
        }

        public void radioButton()
        {
            ByIDNumber = false;
            ByStudentNumber = false;
        }

        private string InsertFormat(string FilePath)
        {
            switch (Path.GetExtension(FilePath).ToLower())
            {
                case ".docx":
                    return Create(FilePath);
                case ".xlsx":
                    return Create(FilePath);
                case ".doc":
                    return Create(FilePath);
                case ".xls":
                    return Create(FilePath);
                case ".jpg":
                    return Create(FilePath);
                case ".tiff":
                    return Create(FilePath);
                case ".pdf":
                    return Create(FilePath);
                default:
                    MsgBox.Show("未知的格式無法匯入!!\n(已知格式包含:docx,xlsx,doc,xls,jpg,tiff,pdf)");
                    return "";
            }
        }

        private string Create(string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open);
            byte[] tempBuffer = new byte[fs.Length];
            fs.Read(tempBuffer, 0, tempBuffer.Length);
            string base64 = Convert.ToBase64String(tempBuffer);
            fs.Close();

            return base64;
        }

        //List<string> FormatList = new List<string>() { ".doc", ".xls", ".jpg", ".tiff", ".pdf" };

        /// <summary>
        /// 是否為DOC,XLS,JPG,TIFF,PDF
        /// </summary>
        private bool CheckExtension(string FilePath)
        {
            switch (Path.GetExtension(FilePath).ToLower())
            {
                case ".docx":
                    return true;
                case ".xlsx":
                    return true;
                case ".doc":
                    return true;
                case ".xls":
                    return true;
                case ".jpg":
                    return true;
                case ".tiff":
                    return true;
                case ".pdf":
                    return true;
                default:
                    return false;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
