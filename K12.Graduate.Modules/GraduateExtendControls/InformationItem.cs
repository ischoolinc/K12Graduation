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

namespace K12.Graduation.Modules
{
    [FISCA.Permission.FeatureCode("K12.Graduation.Modules.InformationItem", "聯絡資訊")]
    public partial class InformationItem : DetailContentBase
    {
        //權限
        internal static FISCA.Permission.FeatureAce UserPermission;
        //背景模式
        private BackgroundWorker BGW = new BackgroundWorker();

        //背景忙碌
        private bool BkWBool = false;

        private GraduateUDT GraduateOBJ;
        private PhotoDataUDT photoOBJ;

        //UDT物件
        private AccessHelper _AccessHelper = new AccessHelper();

        public InformationItem()
        {
            InitializeComponent();

            Group = "聯絡資訊";

            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

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

            this.Loading = false;
        }

        private void BindData()
        {
            if (GraduateOBJ == null)
                return;

            comboBoxEx1.Items.Clear();

            textBoxX1.Text = GraduateOBJ.Contact;
            textBoxX2.Text = GraduateOBJ.Permanent;
            textBoxX3.Text = GraduateOBJ.Cell;
            if (!string.IsNullOrEmpty(GraduateOBJ.Phone1))
                comboBoxEx1.Items.Add(GraduateOBJ.Phone1);
            if (!string.IsNullOrEmpty(GraduateOBJ.Phone2))
                comboBoxEx1.Items.Add(GraduateOBJ.Phone2);
            if (!string.IsNullOrEmpty(GraduateOBJ.Phone3))
                comboBoxEx1.Items.Add(GraduateOBJ.Phone3);

            textBoxX8.Text = GraduateOBJ.PermanentAddress;
            textBoxX5.Text = GraduateOBJ.PermanentZipCode;

            textBoxX7.Text = GraduateOBJ.MailingAddress;
            textBoxX6.Text = GraduateOBJ.MailingZipCode;

            textBoxX4.Text = GraduateOBJ.OtherAddresses;
            textBoxX9.Text = GraduateOBJ.OtherZipCode;


            //if (!string.IsNullOrEmpty(GraduateOBJ.OtherAddresses1))
            //    comboBoxEx2.Items.Add(GraduateOBJ.OtherAddresses1);
            //if (!string.IsNullOrEmpty(GraduateOBJ.OtherAddresses2))
            //    comboBoxEx2.Items.Add(GraduateOBJ.OtherAddresses2);
            //if (!string.IsNullOrEmpty(GraduateOBJ.OtherAddresses3))
            //    comboBoxEx2.Items.Add(GraduateOBJ.OtherAddresses3);

            if (comboBoxEx1.Items.Count != 0)
                comboBoxEx1.SelectedIndex = 0;

            //if (comboBoxEx2.Items.Count != 0)
            //    comboBoxEx2.SelectedIndex = 0;

            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        /// <summary>
        /// KEY值切換時(PrimaryKey更新)
        /// </summary>
        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            Changed();
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
    }
}
