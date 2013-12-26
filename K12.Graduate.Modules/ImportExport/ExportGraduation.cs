using System.Collections.Generic;
using K12.Data;
using SmartSchool.API.PlugIn;
using System;
using FISCA.UDT;
using System.Text;
using FISCA.LogAgent;

namespace K12.Graduation.Modules
{
    class ExportGraduation : SmartSchool.API.PlugIn.Export.Exporter
    {
        //建構子
        public ExportGraduation()
        {
            this.Image = null;
            this.Text = "匯出畢業生基本資料";
        }

        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            wizard.ExportableFields.AddRange("學生系統編號", "學號", "身分證號", "索引分類", "畢業學年度",
                "畢業班級", "座號", "姓名", "性別", "國籍", "出生地", "生日", "英文姓名",
                "戶籍電話", "聯絡電話", "手機", "其它電話1", "其它電話2", "其它電話3",
                "戶籍地址郵遞區號", "戶籍地址", "聯絡地址郵遞區號", "聯絡地址", "其它地址郵遞區號", "其它地址", "備註");

            wizard.ExportPackage += delegate(object sender, SmartSchool.API.PlugIn.Export.ExportPackageEventArgs e)
            {
                #region 收集資料(DicMerit)
                Dictionary<string, GraduateUDT> DicGraduate = new Dictionary<string, GraduateUDT>();

                AccessHelper _AccessHelper = new AccessHelper();
                List<GraduateUDT> GraduateList = _AccessHelper.Select<GraduateUDT>(UDT_S.PopOneCondition("UID", e.List));//取得UDT清單

                //ListDiscipline.Sort(SortDate);

                foreach (GraduateUDT Record in GraduateList)
                {
                    if (!DicGraduate.ContainsKey(Record.UID))
                    {
                        DicGraduate.Add(Record.UID, Record);
                    }
                }
                #endregion
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("匯出學生索引基本資料：");

                foreach (string each in DicGraduate.Keys)
                {
                    GraduateUDT udt = DicGraduate[each];

                    sb.Append("班級「" + udt.ClassName + "」");
                    sb.Append("座號「" + (udt.SeatNo.HasValue ? udt.SeatNo.Value.ToString() : "") + "」");
                    sb.Append("學號「" + udt.StudentNumber + "」");
                    sb.AppendLine("姓名「" + udt.Name + "」");


                    RowData row = new RowData();
                    row.ID = each;
                    foreach (string field in e.ExportFields)
                    {
                        if (wizard.ExportableFields.Contains(field))
                        {
                            switch (field)
                            {
                                case "學生系統編號": row.Add(field, "" + udt.StudentID); break;
                                case "學號": row.Add(field, "" + udt.StudentNumber); break;
                                case "身分證號": row.Add(field, "" + udt.IDNumber); break;
                                case "索引分類": row.Add(field, "" + udt.ArchiveNote); break;
                                case "畢業學年度": row.Add(field, udt.GraduateSchoolYear.HasValue ? udt.GraduateSchoolYear.Value.ToString() : ""); break;

                                case "畢業班級": row.Add(field, "" + udt.ClassName); break;
                                case "座號": row.Add(field, udt.SeatNo.HasValue ? udt.SeatNo.Value.ToString() : ""); break;
                                case "姓名": row.Add(field, "" + udt.Name); break;
                                case "性別": row.Add(field, "" + udt.Gender); break;
                                case "國籍": row.Add(field, "" + udt.Nationality); break;

                                case "出生地": row.Add(field, "" + udt.BirthPlace); break;
                                case "生日": row.Add(field, udt.Birthday.HasValue ? udt.Birthday.Value.ToShortDateString() : ""); break;
                                case "英文姓名": row.Add(field, "" + udt.EnglishName); break;
                                case "戶籍電話": row.Add(field, "" + udt.Permanent); break;
                                case "聯絡電話": row.Add(field, "" + udt.Contact); break;

                                case "手機": row.Add(field, "" + udt.Cell); break;
                                case "其它電話1": row.Add(field, "" + udt.Phone1); break;
                                case "其它電話2": row.Add(field, "" + udt.Phone2); break;
                                case "其它電話3": row.Add(field, "" + udt.Phone3); break;
                                case "戶籍地址郵遞區號": row.Add(field, "" + udt.PermanentZipCode); break;

                                case "戶籍地址": row.Add(field, "" + udt.PermanentAddress); break;
                                case "聯絡地址郵遞區號": row.Add(field, "" + udt.MailingZipCode); break;
                                case "聯絡地址": row.Add(field, "" + udt.MailingAddress); break;
                                case "其它地址郵遞區號": row.Add(field, "" + udt.OtherZipCode); break;
                                case "其它地址": row.Add(field, "" + udt.OtherAddresses); break;
                                //case "其它地址2": row.Add(field, "" + udt.OtherAddresses2); break;
                                //case "其它地址3": row.Add(field, "" + udt.OtherAddresses3); break;
                                case "備註": row.Add(field, "" + udt.Remarks); break;
                            }
                        }
                    }
                    e.Items.Add(row);
                }
                ApplicationLog.Log("畢業生檔案檢索.匯出索引基本資料", "匯出", sb.ToString());
            };
        }
    }
}
