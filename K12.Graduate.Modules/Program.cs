using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using FISCA.Presentation;
using FISCA.Permission;
using K12.Data;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using FISCA.LogAgent;
using System.ComponentModel;
using K12.Data.Configuration;

namespace K12.Graduation.Modules
{
    public class Program
    {
        [MainMethod("K12.Graduation.Modules")]
        static public void Main()
        {

            #region 處理UDT Table沒有的問題

            ConfigData cd = K12.Data.School.Configuration["畢業生檔案檢索UDT載入設定"];
            bool checkClubUDT = false;

            string name = "畢業生UDT是否已載入_20200821";
            //如果尚無設定值,預設為
            if (string.IsNullOrEmpty(cd[name]))
            {
                cd[name] = "false";
            }

            //檢查是否為布林
            bool.TryParse(cd[name], out checkClubUDT);

            if (!checkClubUDT)
            {
                AccessHelper _accessHelper = new AccessHelper();
                _accessHelper.Select<GraduateUDT>("UID = '00000'");
                _accessHelper.Select<PhotoDataUDT>("UID = '00000'");
                _accessHelper.Select<AllXMLDataUDT>("UID = '00000'");
                _accessHelper.Select<WrittenInformationUDT>("UID = '00000'");
                _accessHelper.Select<WriteCounselingUDT>("UID = '00000'");
                cd[name] = "true";
                cd.Save();
            }

            #endregion

            //增加一個頁籤
            MotherForm.AddPanel(GraduationAdmin.Instance);

            //設定一個學生資訊在上面
            GraduationAdmin.Instance.SetDescriptionPaneBulider(new DescriptionPaneBulider<StudentDescription>());

            //增加一個ListView
            GraduationAdmin.Instance.AddView(new ArchiveNoteView()); //依封存分類檢視
            GraduationAdmin.Instance.AddView(new TagView()); //依學生類別檢視
            GraduationAdmin.Instance.AddView(new GraduationYearView()); //依畢業年度檢視

            #region 資料項目

            //基本資料
            GraduationAdmin.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<GraduateDetailItem>());
            //書面資料
            GraduationAdmin.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<WrittenInfomationItem>());
            //書面資料(輔導)
            GraduationAdmin.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<WriteCounselingItem>());
            //連絡資料
            GraduationAdmin.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<InformationItem>());
            //系統封存資料(XML)
            //GraduationAdmin.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<StorageProjectsItem>());
            //備註
            GraduationAdmin.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<RemarksItem>());

            #endregion

            #region 功能按鈕

            RibbonBarItem StudentItem = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "畢業生"];
            StudentItem["建立檔案索引"].Image = Properties.Resources.atm_lock_128;
            StudentItem["建立檔案索引"].Size = RibbonBarButton.MenuButtonSize.Large;
            StudentItem["建立檔案索引"].Enable = false;
            StudentItem["建立檔案索引"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0)
                {
                    BatchArchiveStudent BAS = new BatchArchiveStudent();
                    BAS.ShowDialog();
                }
                else
                    MsgBox.Show("請選擇學生。");
            };

            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                StudentItem["建立檔案索引"].Enable = Permissions.畢業封存權限 && K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0;
            };

            //StudentItem["永久移除學生資料"].Click += delegate
            //{

            //};
            List<string> GraduationList = new List<string>();

            RibbonBarItem SystemLog = GraduationAdmin.Instance.RibbonBarItems["系統"];
            SystemLog["取得索引資料"].Size = RibbonBarButton.MenuButtonSize.Large;
            SystemLog["取得索引資料"].Image = Properties.Resources.layers_zoom_64;
            SystemLog["取得索引資料"].Enable = Permissions.取得封存資料權限;
            SystemLog["取得索引資料"].Click += delegate
            {
                GetGraduationFrom g = new GetGraduationFrom();
                g.ShowDialog();
                GraduationList = g.UDTkey;
            };

            RibbonBarItem Report = GraduationAdmin.Instance.RibbonBarItems["資料統計"];
            Report["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            Report["匯出"].Image = Properties.Resources.匯出;
            Report["匯出"]["匯出畢業生基本資料"].Enable = Permissions.匯出畢業生基本資料權限;
            Report["匯出"]["匯出畢業生基本資料"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new ExportGraduation();
                ExportStudentV2 wizard = new ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            Report["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            Report["匯入"].Image = Properties.Resources.匯入;
            Report["匯入"]["匯入畢業生基本資料(新增)"].Enable = Permissions.匯入畢業生基本資料權限;
            Report["匯入"]["匯入畢業生基本資料(新增)"].Click += delegate
            {
                ImportGraduation wizard = new ImportGraduation();
                wizard.Execute();

                foreach (string each in wizard.InsertListID)
                {
                    if (!GraduationList.Contains(each))
                    {
                        GraduationList.Add(each);
                    }
                }

                GraduationAdmin.Instance.BGW.RunWorkerAsync(GraduationList);
            };

            RibbonBarItem DownLoads = GraduationAdmin.Instance.RibbonBarItems["書面管理"];
            DownLoads["上傳書面"].Size = RibbonBarButton.MenuButtonSize.Large;
            DownLoads["上傳書面"].Image = Properties.Resources.cabinet_up_128;
            DownLoads["上傳書面"].Enable = Permissions.上傳書面資料權限;
            DownLoads["上傳書面"].Click += delegate
            {
                UploadWrittenInformation infDlg = new UploadWrittenInformation();
                infDlg.ShowDialog();
            };

            DownLoads["下載書面"].Size = RibbonBarButton.MenuButtonSize.Large;
            DownLoads["下載書面"].Image = Properties.Resources.cabinet_down_128;
            DownLoads["下載書面"].Enable = Permissions.下載書面資料權限;
            DownLoads["下載書面"].Click += delegate
            {
                if (GraduationAdmin.Instance.SelectedSource.Count > 0)
                {
                    DownLoadWritlenInformation DownLoad = new DownLoadWritlenInformation();
                    DownLoad.ShowDialog();
                }
                else
                {
                    MsgBox.Show("請選擇學生。");
                }

            };

            #endregion

            //2020/8/21 - 輔導書面
            RibbonBarItem DownLoadCounseling = GraduationAdmin.Instance.RibbonBarItems["書面管理(輔導)"];
            DownLoadCounseling["上傳輔導書面"].Size = RibbonBarButton.MenuButtonSize.Large;
            DownLoadCounseling["上傳輔導書面"].Image = Properties.Resources.new_cabinet_up_128;
            DownLoadCounseling["上傳輔導書面"].Enable = Permissions.上傳書面資料輔導權限;
            DownLoadCounseling["上傳輔導書面"].Click += delegate
            {
                UploadCounseling infDlg = new UploadCounseling();
                infDlg.ShowDialog();
            };

            DownLoadCounseling["下載輔導書面"].Size = RibbonBarButton.MenuButtonSize.Large;
            DownLoadCounseling["下載輔導書面"].Image = Properties.Resources.new_cabinet_down_128;
            DownLoadCounseling["下載輔導書面"].Enable = Permissions.下載書面資料輔導權限;
            DownLoadCounseling["下載輔導書面"].Click += delegate
            {
                if (GraduationAdmin.Instance.SelectedSource.Count > 0)
                {
                    DownLoadCounseling DownLoad = new DownLoadCounseling();
                    DownLoad.ShowDialog();
                }
                else
                {
                    MsgBox.Show("請選擇學生。");
                }

            };

            RibbonBarItem tools = GraduationAdmin.Instance.RibbonBarItems["工具"];
            tools["文件分割"].Image = Properties.Resources.windows_64;
            tools["文件分割"].Size = RibbonBarButton.MenuButtonSize.Large;
            //DownLoads["文件分割"]["Word文件"].Enable = Permissions.下載書面資料權限;
            tools["文件分割"]["Word文件"].Click += delegate
            {
                WordCut word = new WordCut();
                word.ShowDialog();
            };

            #region 右鍵

            GraduationAdmin.Instance.ListPaneContexMenu["更新畫面資料"].Enable = Permissions.更新畫面資料權限;
            GraduationAdmin.Instance.ListPaneContexMenu["更新畫面資料"].Click += delegate
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("開始取得學生索引資料!!");
                GraduationAdmin.Instance.BGW.RunWorkerAsync(GraduationList);
            };

            GraduationAdmin.Instance.ListPaneContexMenu["刪除選擇資料"].Enable = Permissions.刪除選擇資料權限;
            GraduationAdmin.Instance.ListPaneContexMenu["刪除選擇資料"].Click += delegate
            {
                #region 刪除選擇資料

                DialogResult dr = MsgBox.Show("您確定要刪除這些學生資料?\n將會一併將其它關聯資料刪除。", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.No)
                    return;

                if (GraduationAdmin.Instance.SelectedSource.Count != 0)
                {
                    BackgroundWorker BGW = new BackgroundWorker();
                    BGW.WorkerReportsProgress = true;
                    BGW.DoWork += delegate
                    {
                        #region DoWork
                        AccessHelper _AccessHelper = new AccessHelper();
                        BGW.ReportProgress(0, "刪除作業,取得學生索引...");
                        List<GraduateUDT> list1 = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("UID", GraduationAdmin.Instance.SelectedSource));

                        BGW.ReportProgress(10, "刪除作業,取得XML索引資料...");
                        List<AllXMLDataUDT> list2 = _AccessHelper.Select<AllXMLDataUDT>(UDT_S.PopOneCondition("RefUDT_ID", GraduationAdmin.Instance.SelectedSource));

                        BGW.ReportProgress(25, "刪除作業,取得學生照片索引資料...");
                        List<PhotoDataUDT> list3 = _AccessHelper.Select<PhotoDataUDT>(UDT_S.PopOneCondition("RefUDT_ID", GraduationAdmin.Instance.SelectedSource));

                        BGW.ReportProgress(34, "刪除作業,取得書面索引資料...");
                        List<WrittenInformationUDT> list4 = _AccessHelper.Select<WrittenInformationUDT>(UDT_S.PopOneCondition("RefUDT_ID", GraduationAdmin.Instance.SelectedSource));

                        BGW.ReportProgress(42, "系統歷程記錄準備.");
                        DeleteAllData1 dad = new DeleteAllData1(list1);

                        BGW.ReportProgress(47, "系統歷程記錄準備..");
                        dad.setAllXml(list2);

                        BGW.ReportProgress(53, "系統歷程記錄準備...");
                        dad.setPhoto(list3);

                        BGW.ReportProgress(57, "系統歷程記錄準備.....");
                        dad.setWritten(list4);

                        BGW.ReportProgress(62, "刪除學生索引基本資料...");
                        _AccessHelper.DeletedValues(list1.ToArray());

                        BGW.ReportProgress(75, "刪除XML索引資料...");
                        _AccessHelper.DeletedValues(list2.ToArray());

                        BGW.ReportProgress(79, "刪除學生照片索引資料...");
                        _AccessHelper.DeletedValues(list3.ToArray());

                        BGW.ReportProgress(84, "刪除書面資料...");
                        _AccessHelper.DeletedValues(list4.ToArray());

                        BGW.ReportProgress(95, "刪除作業,系統歷程記錄中...");
                        ApplicationLog.Log("刪除學生索引資料", "刪除", dad.GetString());

                        BGW.ReportProgress(100, "已刪除學生索引資料。");
                        #endregion
                    };

                    BGW.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
                    {
                        FISCA.Presentation.MotherForm.SetStatusBarMessage(e.UserState.ToString(), e.ProgressPercentage);
                    };

                    BGW.RunWorkerCompleted += delegate
                    {
                        #region RunWorkerCompleted
                        MsgBox.Show("已刪除「" + GraduationAdmin.Instance.SelectedSource.Count + "」筆學生索引資料");
                        FISCA.Presentation.MotherForm.SetStatusBarMessage("已刪除學生索引資料。");
                        foreach (string each in GraduationAdmin.Instance.SelectedSource)
                        {
                            if (GraduationList.Contains(each))
                            {
                                GraduationList.Remove(each);
                            }
                        }
                        GraduationAdmin.Instance.BGW.RunWorkerAsync(GraduationList);
                        #endregion
                    };

                    BGW.RunWorkerAsync();

                }
                else
                {
                    MsgBox.Show("未刪除任何資料。");
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("未刪除任何資料。");
                }

                #endregion
            };

            #endregion

            #region 權限設定
            Catalog detail;

            detail = RoleAclSource.Instance["畢業生檔案檢索"]["資料項目"];
            detail.Add(new DetailItemFeature(Permissions.基本資料, "基本資料"));
            detail.Add(new DetailItemFeature(Permissions.聯絡資訊, "聯絡資訊"));
            detail.Add(new DetailItemFeature(Permissions.書面檔案, "書面檔案"));
            detail.Add(new DetailItemFeature(Permissions.書面檔案輔導, "書面檔案(輔導)"));
            //detail.Add(new DetailItemFeature(Permissions.系統封存, "系統封存"));
            detail.Add(new DetailItemFeature(Permissions.備註, "備註"));

            detail = RoleAclSource.Instance["畢業生檔案檢索"]["功能按鈕"];
            //detail.Add(new ReportFeature(Permissions.畢業封存, "建立畢業生檔案索引"));
            detail.Add(new ReportFeature(Permissions.取得封存資料, "取得索引資料"));
            detail.Add(new ReportFeature(Permissions.匯出畢業生基本資料, "匯出畢業生基本資料"));
            detail.Add(new ReportFeature(Permissions.匯入畢業生基本資料, "匯入畢業生基本資料(新增)"));
            detail.Add(new ReportFeature(Permissions.上傳書面資料, "上傳書面"));
            detail.Add(new ReportFeature(Permissions.下載書面資料, "下載書面"));
            detail.Add(new ReportFeature(Permissions.上傳書面資料輔導, "上傳輔導書面"));
            detail.Add(new ReportFeature(Permissions.下載書面資料輔導, "下載輔導書面"));

            detail = RoleAclSource.Instance["畢業生檔案檢索"]["右鍵功能"];
            detail.Add(new ReportFeature(Permissions.更新畫面資料, "更新畫面資料"));
            detail.Add(new ReportFeature(Permissions.刪除選擇資料, "刪除選擇資料"));

            detail = RoleAclSource.Instance["學生"]["功能按鈕"];
            detail.Add(new ReportFeature(Permissions.畢業封存, "建立檔案索引(畢業生)"));
            #endregion
        }
    }
}
