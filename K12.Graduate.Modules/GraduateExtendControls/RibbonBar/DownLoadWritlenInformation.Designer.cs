namespace K12.Graduation.Modules
{
    partial class DownLoadWritlenInformation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDownLoad = new DevComponents.DotNetBar.ButtonX();
            this.txtHelp1 = new DevComponents.DotNetBar.LabelX();
            this.cbDownLoadAll = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.comboBoxEx1 = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // btnDownLoad
            // 
            this.btnDownLoad.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDownLoad.AutoSize = true;
            this.btnDownLoad.BackColor = System.Drawing.Color.Transparent;
            this.btnDownLoad.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnDownLoad.Location = new System.Drawing.Point(186, 122);
            this.btnDownLoad.Name = "btnDownLoad";
            this.btnDownLoad.Size = new System.Drawing.Size(75, 25);
            this.btnDownLoad.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDownLoad.TabIndex = 0;
            this.btnDownLoad.Text = "下載";
            this.btnDownLoad.Click += new System.EventHandler(this.btnDownLoad_Click);
            // 
            // txtHelp1
            // 
            this.txtHelp1.AutoSize = true;
            this.txtHelp1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.txtHelp1.BackgroundStyle.Class = "";
            this.txtHelp1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtHelp1.Location = new System.Drawing.Point(12, 16);
            this.txtHelp1.Name = "txtHelp1";
            this.txtHelp1.Size = new System.Drawing.Size(167, 21);
            this.txtHelp1.TabIndex = 6;
            this.txtHelp1.Text = "請輸入欲下載之書面名稱：\r\n";
            // 
            // cbDownLoadAll
            // 
            this.cbDownLoadAll.AutoSize = true;
            this.cbDownLoadAll.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.cbDownLoadAll.BackgroundStyle.Class = "";
            this.cbDownLoadAll.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbDownLoadAll.Location = new System.Drawing.Point(12, 122);
            this.cbDownLoadAll.Name = "cbDownLoadAll";
            this.cbDownLoadAll.Size = new System.Drawing.Size(107, 21);
            this.cbDownLoadAll.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbDownLoadAll.TabIndex = 7;
            this.cbDownLoadAll.Text = "下載所有書面";
            this.cbDownLoadAll.CheckedChanged += new System.EventHandler(this.cbDownLoadAll_CheckedChanged);
            // 
            // comboBoxEx1
            // 
            this.comboBoxEx1.DisplayMember = "Text";
            this.comboBoxEx1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxEx1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEx1.FormattingEnabled = true;
            this.comboBoxEx1.ItemHeight = 19;
            this.comboBoxEx1.Location = new System.Drawing.Point(36, 66);
            this.comboBoxEx1.Name = "comboBoxEx1";
            this.comboBoxEx1.Size = new System.Drawing.Size(282, 25);
            this.comboBoxEx1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.comboBoxEx1.TabIndex = 8;
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.AutoSize = true;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(267, 122);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 25);
            this.btnExit.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnExit.TabIndex = 10;
            this.btnExit.Text = "取消";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // DownLoadWritlenInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 157);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.comboBoxEx1);
            this.Controls.Add(this.cbDownLoadAll);
            this.Controls.Add(this.txtHelp1);
            this.Controls.Add(this.btnDownLoad);
            this.Name = "DownLoadWritlenInformation";
            this.Text = "下載書面";
            this.Load += new System.EventHandler(this.DownLoadWritlenInFormation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnDownLoad;
        private DevComponents.DotNetBar.LabelX txtHelp1;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbDownLoadAll;
        private DevComponents.DotNetBar.Controls.ComboBoxEx comboBoxEx1;
        private DevComponents.DotNetBar.ButtonX btnExit;
    }
}