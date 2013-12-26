using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.UDT;
using FISCA.Presentation.Controls;
using FISCA.LogAgent;

namespace K12.Graduation.Modules
{
    [FISCA.Permission.FeatureCode("K12.Graduation.Modules.RemarksItem", "備註")]
    public partial class RemarksItem : DetailContentBase
    {
        //權限
        internal static FISCA.Permission.FeatureAce UserPermission;
        //背景模式
        private BackgroundWorker BGW = new BackgroundWorker();
        //背景忙碌
        private bool BkWBool = false;
        //UDT物件
        private AccessHelper _AccessHelper = new AccessHelper();
        //主物件
        private GraduateUDT GraduateOBJ;

        private Campus.Windows.ChangeListener DataListener { get; set; }

        public RemarksItem()
        {
            InitializeComponent();
            
            Group = "備註";

            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            DataListener = new Campus.Windows.ChangeListener();
            DataListener = new Campus.Windows.ChangeListener();
            DataListener.Add(new Campus.Windows.TextBoxSource(textBoxX1));
            DataListener.StatusChanged += new EventHandler<Campus.Windows.ChangeEventArgs>(DataListener_StatusChanged);

            UserPermission = FISCA.Permission.UserAcl.Current[FISCA.Permission.FeatureCodeAttribute.GetCode(GetType())];
            this.Enabled = UserPermission.Editable;

            GraduationEvents.GraduationChanged += new EventHandler(GraduationEvents_GraduationChanged);
        }

        void GraduationEvents_GraduationChanged(object sender, EventArgs e)
        {
            Changed();
        }

        /// <summary>
        /// 背景模式
        /// </summary>
        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得資料
            List<GraduateUDT> list = _AccessHelper.Select<GraduateUDT>(string.Format("UID='{0}'", this.PrimaryKey));
            if (list.Count == 1)
            {
                GraduateOBJ = list[0];
            }
            else if (list.Count > 1) //UDT資料多餘
            {
                MsgBox.Show("UDT資料有誤!!");
            }
            else //無UDT資料
            {

            }
        }


        /// <summary>
        /// 背景模式完成
        /// </summary>
        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (BkWBool) //如果有其他的更新事件
            {
                BkWBool = false;
                BGW.RunWorkerAsync();
                return;
            }

            BindData();

            DataListener.Reset();
            DataListener.ResumeListen();
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            this.Loading = false;
        }

        private void BindData()
        {
            if (GraduateOBJ == null)
                return;

            textBoxX1.Text = GraduateOBJ.Remarks.Replace("\n","\r\n");

        }

        /// <summary>
        /// KEY值切換時
        /// </summary>
        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            Changed();
        }

        /// <summary>
        /// 按下儲存時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSaveButtonClick(EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("學生電子檔案索引系統「備註已修改」");
            sb.AppendLine("姓名「" + GraduateOBJ.Name + "」");
            sb.AppendLine("學號「" + GraduateOBJ.StudentNumber + "」");
            sb.AppendLine("索引分類「" + GraduateOBJ.ArchiveNote + "」");
            sb.AppendLine("====修改前====");
            if (!string.IsNullOrEmpty(GraduateOBJ.Remarks))
                sb.AppendLine(GraduateOBJ.Remarks);
            else
                sb.AppendLine("(無內容)");

            sb.AppendLine("====修改後====");
            if (!string.IsNullOrEmpty(textBoxX1.Text))
                sb.AppendLine(textBoxX1.Text);
            else
                sb.AppendLine("(無內容)");

            GraduateOBJ.Remarks = textBoxX1.Text;
            List<GraduateUDT> list = new List<GraduateUDT>();
            list.Add(GraduateOBJ);
            _AccessHelper.UpdateValues(list.ToArray());

            SaveButtonVisible = false;
            CancelButtonVisible = false;
            MsgBox.Show("儲存成功!");
            ApplicationLog.Log("畢業生檔案檢索.備註", "修改", sb.ToString());
            //this.Loading = true;

            BGW.RunWorkerAsync();
        }

        /// <summary>
        /// 取消儲存時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCancelButtonClick(EventArgs e)
        {
            this.SaveButtonVisible = false;
            this.CancelButtonVisible = false;

            DataListener.SuspendListen(); //終止變更判斷
            BGW.RunWorkerAsync(); //背景作業,取得並重新填入原資料
        }

        void DataListener_StatusChanged(object sender, Campus.Windows.ChangeEventArgs e)
        {
            this.SaveButtonVisible = (e.Status == Campus.Windows.ValueStatus.Dirty);
            this.CancelButtonVisible = (e.Status == Campus.Windows.ValueStatus.Dirty);
        }

        void Changed()
        {
            #region 更新時
            if (this.PrimaryKey != "")
            {
                this.Loading = true;

                if (BGW.IsBusy)
                {
                    BkWBool = true;
                }
                else
                {
                    BGW.RunWorkerAsync();
                }
            }
            #endregion
        }
    }
}
