using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Graduation.Modules
{
    class Permissions
    {
        public static string 基本資料 { get { return "K12.Graduation.Modules.GraduateDetailItem"; } }
        public static string 聯絡資訊 { get { return "K12.Graduation.Modules.InformationItem"; } }
        public static string 書面檔案 { get { return "K12.Graduation.Modules.WrittenInfomationItem"; } }

        //2020/8/21 - 輔導專用
        public static string 書面檔案輔導 { get { return "K12.Graduation.Modules.WriteCounselingItem"; } }

        public static string 系統封存 { get { return "K12.Graduation.Modules.StorageProjectsItem"; } }    
        public static string 備註 { get { return "K12.Graduation.Modules.RemarksItem"; } }

        public static string 畢業封存 { get { return "K12.Graduation.Modules.BatchArchiveStudent"; } }
        public static string 取得封存資料 { get { return "K12.Graduation.Modules.GraduationEvents"; } }
        public static string 匯出畢業生基本資料 { get { return "K12.Graduation.Modules.ExportGraduation"; } }
        public static string 匯入畢業生基本資料 { get { return "K12.Graduation.Modules.ImportGraduation"; } }

        public static string 上傳書面資料 { get { return "K12.Graduation.Modules.UploadWrittenInformation"; } }
        public static string 下載書面資料 { get { return "K12.Graduation.Modules.DownLoadWritlenInformation"; } }

        public static string 上傳書面資料輔導 { get { return "K12.Graduation.Modules.UploadCounseling"; } }
        public static string 下載書面資料輔導 { get { return "K12.Graduation.Modules.DownLoadCounseling"; } }

        public static string 更新畫面資料 { get { return "K12.Graduation.Modules.Raise"; } }
        public static string 刪除選擇資料 { get { return "K12.Graduation.Modules.Delete"; } }

        public static bool 更新畫面資料權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[更新畫面資料].Executable;
            }
        }
        public static bool 刪除選擇資料權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[刪除選擇資料].Executable;
            }
        }
        public static bool 基本資料權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[基本資料].Executable;
            }
        }

        public static bool 聯絡資訊權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[聯絡資訊].Executable;
            }
        }

        public static bool 備註權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[備註].Executable;
            }
        }

        public static bool 系統封存權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[系統封存].Executable;
            }
        }

        public static bool 書面檔案權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[書面檔案].Executable;
            }
        }

        public static bool 畢業封存權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[畢業封存].Executable;
            }
        }

        public static bool 取得封存資料權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[取得封存資料].Executable;
            }
        }

        public static bool 匯出畢業生基本資料權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[匯出畢業生基本資料].Executable;
            }
        }

        public static bool 匯入畢業生基本資料權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[匯入畢業生基本資料].Executable;
            }
        }

        public static bool 上傳書面資料權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[上傳書面資料].Executable;
            }
        }

        public static bool 下載書面資料權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[下載書面資料].Executable;
            }
        }

        public static bool 上傳書面資料輔導權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[上傳書面資料輔導].Executable;
            }
        }

        public static bool 下載書面資料輔導權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[下載書面資料輔導].Executable;
            }
        }

    }
}
