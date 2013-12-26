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
using System.IO;

namespace K12.Graduation.Modules
{
    [FISCA.Permission.FeatureCode("K12.Graduation.Modules.GraduateDetailItem", "基本資料")]
    public partial class GraduateDetailItem : DetailContentBase
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

        public GraduateDetailItem()
        {
            InitializeComponent();

            Group = "基本資料";

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
            //取得資料
            List<GraduateUDT> list = _AccessHelper.Select<GraduateUDT>(string.Format("UID='{0}'", this.PrimaryKey));
            if (list.Count == 1)
            {
                GraduateOBJ = list[0];
                //取得本UDT資料之照片UDT資料
                List<PhotoDataUDT> list2 = _AccessHelper.Select<PhotoDataUDT>(string.Format("RefUDT_ID='{0}'", GraduateOBJ.UID));
                if (list2.Count == 1)
                    photoOBJ = list2[0];
                else
                    photoOBJ = null;
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

        /// <summary>
        /// 更新畫面資料
        /// </summary>
        private void BindData()
        {
            if (GraduateOBJ == null)
                return;

            textBoxX5.Text = GraduateOBJ.ArchiveNote;
            textBoxX4.Text = GraduateOBJ.StudentNumber;
            txtName.Text = GraduateOBJ.Name;
            txtSSN.Text = GraduateOBJ.IDNumber;
            txtGender.Text = GraduateOBJ.Gender;
            txtNationality.Text = GraduateOBJ.Nationality;

            txtBirthDate.Text = GraduateOBJ.Birthday.HasValue ? GraduateOBJ.Birthday.Value.ToShortDateString() : "";
            txtBirthPlace.Text = GraduateOBJ.BirthPlace;
            txtEngName.Text = GraduateOBJ.EnglishName;

            //畢業學年/班級/座號
            textBoxX1.Text = GraduateOBJ.GraduateSchoolYear.HasValue ? GraduateOBJ.GraduateSchoolYear.Value.ToString() : "";
            textBoxX2.Text = GraduateOBJ.ClassName;
            textBoxX3.Text = GraduateOBJ.SeatNo.HasValue ? GraduateOBJ.SeatNo.Value.ToString() : "";

            if (photoOBJ != null)
            {
                if (!string.IsNullOrEmpty(photoOBJ.FreshmanPhoto)) //入學照片
                {
                    pic1.Image = PhotoConvert.ConvertFromBase64Encoding(photoOBJ.FreshmanPhoto, pic1.Width, pic1.Height);
                    pic1.Tag = photoOBJ.FreshmanPhoto;
                }
                else
                {
                    pic1.Image = pic1.InitialImage;
                    pic1.Tag = null;
                }

                if (!string.IsNullOrEmpty(photoOBJ.GraduatePhoto)) //畢業照片
                {
                    pic2.Image = PhotoConvert.ConvertFromBase64Encoding(photoOBJ.GraduatePhoto, pic2.Width, pic2.Height);
                    pic2.Tag = photoOBJ.GraduatePhoto;
                }
                else
                {
                    pic2.Image = pic2.InitialImage;
                    pic2.Tag = null;
                }
            }
            else
            {
                pic1.Image = pic1.InitialImage;
                pic2.Image = pic2.InitialImage;
                pic1.Tag = null;
                pic2.Tag = null;
            }

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

        private void pic1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (pic1.Tag != null)
            {
                string tag = "" + pic1.Tag;
                ViewJPG view = new ViewJPG(tag);
                view.ShowDialog();
            }
        }

        private void pic2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (pic2.Tag != null)
            {
                string tag = "" + pic2.Tag;
                ViewJPG view = new ViewJPG(tag);
                view.ShowDialog();
            }
        }
    }
}
