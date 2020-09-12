namespace StationDemo
{
    partial class Form_Alarm
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
            this.rightTab_Alarm = new AutoFrameUI.RightTab();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rightTab_Alarm.SuspendLayout();
            this.SuspendLayout();
            // 
            // rightTab_Alarm
            // 
            this.rightTab_Alarm.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.rightTab_Alarm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rightTab_Alarm.Controls.Add(this.tabPage1);
            this.rightTab_Alarm.Controls.Add(this.tabPage2);
            this.rightTab_Alarm.ItemSize = new System.Drawing.Size(25, 100);
            this.rightTab_Alarm.Location = new System.Drawing.Point(12, 3);
            this.rightTab_Alarm.Multiline = true;
            this.rightTab_Alarm.Name = "rightTab_Alarm";
            this.rightTab_Alarm.SelectedIndex = 0;
            this.rightTab_Alarm.Size = new System.Drawing.Size(1783, 506);
            this.rightTab_Alarm.TabColor = System.Drawing.SystemColors.Control;
            this.rightTab_Alarm.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1675, 498);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(138, 92);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Form_Alarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1807, 521);
            this.Controls.Add(this.rightTab_Alarm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_Alarm";
            this.Text = "Form_Alarm";
            this.rightTab_Alarm.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AutoFrameUI.RightTab rightTab_Alarm;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}