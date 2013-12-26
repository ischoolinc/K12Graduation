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
using System.Xml;
using FISCA.DSAUtil;
using Aspose.Cells;

namespace K12.Graduation.Modules
{
    //本資料項目用以封存使用者之XML資料
    //如缺曠 / 獎勵 / 懲戒 / 異動 / 成績
    [FISCA.Permission.FeatureCode("K12.Graduation.Modules.StorageProjectsItem", "系統封存")]
    public partial class StorageProjectsItem : DetailContentBase
    {
        //權限
        internal static FISCA.Permission.FeatureAce UserPermission;
        //背景模式
        private BackgroundWorker BGW = new BackgroundWorker();

        //UDT操作物件
        private AccessHelper _AccessHelper = new AccessHelper();


        //學生UDT
        GraduateUDT _StudentUdt;

        //背景忙碌
        private bool BkWBool = false;

        public StorageProjectsItem()
        {
            InitializeComponent();

            Group = "系統封存";

            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            UserPermission = FISCA.Permission.UserAcl.Current[FISCA.Permission.FeatureCodeAttribute.GetCode(GetType())];
            this.Enabled = UserPermission.Editable;
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得PrimaryKey的UDT資料
            List<GraduateUDT> GraduateUDTList = _AccessHelper.Select<GraduateUDT>(string.Format("UID='{0}'", this.PrimaryKey));
            if (GraduateUDTList.Count == 1)
            {
                _StudentUdt = GraduateUDTList[0];
                //再得此UDT的相依UDT資料
                List<AllXMLDataUDT> listUDT = _AccessHelper.Select<AllXMLDataUDT>(string.Format("RefUDT_ID='{0}'", GraduateUDTList[0].UID));
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
                List<AllXMLDataUDT> listUDT = (List<AllXMLDataUDT>)e.Result;
                if (listUDT != null)
                {
                    if (listUDT.Count != 0)
                    {
                        BindData(listUDT);
                    }
                }
            }

            this.Loading = false;
        }

        private void BindData(List<AllXMLDataUDT> listUDT)
        {
            foreach (AllXMLDataUDT eachUDT in listUDT)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Cells[0].Value = eachUDT.Name;
                if (eachUDT.Content != null)              
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

        //匯出功能
        private void dataGridViewX1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //DataGridViewRow row = dataGridViewX1.Rows[e.RowIndex];
            ////匯出Xml規格資料
            //if (e.ColumnIndex == Column3.Index) //Column3是下載為Excel檔案
            //{
            //    //雙擊Column2是開啟即時檢視視窗
            //    if (!string.IsNullOrEmpty("" + row.Tag)) //如果Xml資料不是空的
            //    {
            //        Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
            //        Worksheet ws = workbook.Worksheets[0];
            //        XmlElement xml = DSXmlHelper.LoadXml("" + row.Tag);

            //        //建立標頭
            //        int TitleColumnIndex = 0;
            //        foreach (XmlElement each in xml.SelectNodes("*"))
            //        {
            //            ws.Cells[0, TitleColumnIndex].PutValue("ID"); //標題
            //            TitleColumnIndex++;
            //            foreach (XmlElement each2 in each.SelectNodes("*"))
            //            {
            //                ws.Cells[0, TitleColumnIndex].PutValue(each2.Name);
            //                TitleColumnIndex++;
            //            }
            //            break;
            //        }

            //        int RowIndex = 1;
            //        int ColumnIndex = 0;
            //        foreach (XmlElement each in xml.SelectNodes("*"))
            //        {
            //            ws.Cells[RowIndex, ColumnIndex].PutValue(each.GetAttribute("ID")); //內容
            //            ColumnIndex++;
            //            foreach (XmlElement each2 in each.SelectNodes("*"))
            //            {
            //                ws.Cells[RowIndex, ColumnIndex].PutValue(each2.InnerXml);
            //                ColumnIndex++;

            //                //if (each2.SelectNodes("*").Count == 0)
            //                //{
            //                //    ws.Cells[RowIndex, ColumnIndex].PutValue(each2.InnerXml);
            //                //    ColumnIndex++;
            //                //}
            //                //else //進入迴圈,以處理下階層的資料
            //                //{
            //                //}
            //            }
            //            ColumnIndex = 0;
            //            RowIndex++;
            //        }
            //        SaveFileDialog SF = new SaveFileDialog();
            //        SF.Title = "請選擇儲存的位置..";
            //        SF.FileName = row.Cells[Column1.Index].Value + ".xls";
            //        SF.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            //        SF.ShowDialog();
            //        workbook.Save(SF.FileName);
            //    }
            //}
        }

        //開啟功能
        private void dataGridViewX1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridViewX1.Rows[e.RowIndex];

            if (e.ColumnIndex == Column1.Index)
            {
                if (!string.IsNullOrEmpty("" + row.Tag)) //如果Xml資料不是空的
                {
                    //怎麼下載阿...
                    string xml = "" + row.Tag;
                    XmlViewForm xvf = new XmlViewForm();
                    xvf.PopXml(xml);
                    xvf.ShowDialog();
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
    }
}
