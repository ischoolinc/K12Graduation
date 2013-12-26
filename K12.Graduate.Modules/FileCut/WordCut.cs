using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Aspose.Words;
using System.IO;

namespace K12.Graduation.Modules
{
    public partial class WordCut : BaseForm
    {
        public WordCut()
        {
            InitializeComponent();
        }

        private void WordCut_Load(object sender, EventArgs e)
        {

        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "選擇自訂的缺曠通知單範本";
            ofd.Filter = "Word檔案 (*.doc)|*.doc";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //開啟檔案
                Document doc = new Document(ofd.FileName);
                //掃瞄每個頁面
                int count = 1;
                foreach (Section each in doc.Sections)
                {
                    //測試如何透過標籤來insert學號資訊

                    string GetText = each.GetText();
                    int sdfs = GetText.IndexOf("學號{");
                    if (sdfs != -1) //沒找到資料是-1
                    {
                        GetText = GetText.Remove(0, sdfs);
                        GetText = GetText.Remove(0, GetText.IndexOf('{') + 1);
                        GetText = GetText.Remove(GetText.IndexOf('}'));

                        if (!string.IsNullOrEmpty(GetText))
                        {
                            SaveFile(true, each, ofd.FileName, GetText, count);
                            count++;
                        }
                        else //未篩選出檔案名稱
                        {
                            SaveFile(false, each, ofd.FileName, "", count);
                            count++;
                        }
                    }
                    else //沒有 "學號{" 字樣
                    {
                        SaveFile(false, each, ofd.FileName, "", count);
                        count++;
                    }
                }
                MsgBox.Show("分割完成\n(共" + (count - 1) + "個檔案)");
            }
        }

        private void SaveFile(bool SaveMode, Section each, string FileName, string GetText, int count)
        {
            if (SaveMode)
            {
                Document idoc = new Document();
                idoc.Sections.Clear();
                Node n = idoc.ImportNode(each, true);
                idoc.Sections.Add(n);

                string gg = Path.GetDirectoryName(FileName);
                idoc.Save(gg + "\\" + GetText + ".doc");
                count++;
            }
            else
            {
                Document idoc = new Document();

                idoc.Sections.Clear();
                Node n = idoc.ImportNode(each, true);
                idoc.Sections.Add(n);

                string gg = Path.GetDirectoryName(FileName);
                idoc.Save(gg + "\\" + "0000" + count + ".doc");
            }
        }
    }
}
