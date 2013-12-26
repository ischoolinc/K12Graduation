namespace K12.Graduation.Modules
{
    partial class GetGraduationFrom
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
            this.btnGetData = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.lbHelp = new DevComponents.DotNetBar.LabelX();
            this.cbDataLiat = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cbGetAllData = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.SuspendLayout();
            // 
            // btnGetData
            // 
            this.btnGetData.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnGetData.AutoSize = true;
            this.btnGetData.BackColor = System.Drawing.Color.Transparent;
            this.btnGetData.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnGetData.Location = new System.Drawing.Point(133, 98);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(75, 25);
            this.btnGetData.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnGetData.TabIndex = 0;
            this.btnGetData.Text = "取得";
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.AutoSize = true;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(214, 98);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 25);
            this.btnClose.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "關閉";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lbHelp
            // 
            this.lbHelp.AutoSize = true;
            this.lbHelp.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbHelp.BackgroundStyle.Class = "";
            this.lbHelp.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbHelp.Location = new System.Drawing.Point(13, 14);
            this.lbHelp.Name = "lbHelp";
            this.lbHelp.Size = new System.Drawing.Size(114, 21);
            this.lbHelp.TabIndex = 2;
            this.lbHelp.Text = "請選擇索引類別：";
            // 
            // cbDataLiat
            // 
            this.cbDataLiat.DisplayMember = "Text";
            this.cbDataLiat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbDataLiat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataLiat.FormattingEnabled = true;
            this.cbDataLiat.ItemHeight = 19;
            this.cbDataLiat.Location = new System.Drawing.Point(13, 50);
            this.cbDataLiat.Name = "cbDataLiat";
            this.cbDataLiat.Size = new System.Drawing.Size(276, 25);
            this.cbDataLiat.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbDataLiat.TabIndex = 3;
            // 
            // cbGetAllData
            // 
            this.cbGetAllData.AutoSize = true;
            this.cbGetAllData.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.cbGetAllData.BackgroundStyle.Class = "";
            this.cbGetAllData.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbGetAllData.Location = new System.Drawing.Point(12, 102);
            this.cbGetAllData.Name = "cbGetAllData";
            this.cbGetAllData.Size = new System.Drawing.Size(80, 21);
            this.cbGetAllData.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbGetAllData.TabIndex = 4;
            this.cbGetAllData.Text = "取得全部";
            this.cbGetAllData.CheckedChanged += new System.EventHandler(this.cbGetAllData_CheckedChanged);
            // 
            // GetGraduationFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 135);
            this.Controls.Add(this.cbGetAllData);
            this.Controls.Add(this.cbDataLiat);
            this.Controls.Add(this.lbHelp);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnGetData);
            this.Name = "GetGraduationFrom";
            this.Text = "取得學生索引資料";
            this.Load += new System.EventHandler(this.GetGraduationFrom_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnGetData;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.LabelX lbHelp;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbDataLiat;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbGetAllData;
    }
}