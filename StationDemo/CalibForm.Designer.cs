namespace StationDemo
{
    partial class CalibForm
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
            this.PictureShow = new System.Windows.Forms.PictureBox();
            this.BtnSnap = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PictureShow)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureShow
            // 
            this.PictureShow.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.PictureShow.Location = new System.Drawing.Point(-21, -19);
            this.PictureShow.Margin = new System.Windows.Forms.Padding(2);
            this.PictureShow.Name = "PictureShow";
            this.PictureShow.Size = new System.Drawing.Size(440, 330);
            this.PictureShow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureShow.TabIndex = 42;
            this.PictureShow.TabStop = false;
            // 
            // BtnSnap
            // 
            this.BtnSnap.Location = new System.Drawing.Point(12, 329);
            this.BtnSnap.Name = "BtnSnap";
            this.BtnSnap.Size = new System.Drawing.Size(102, 50);
            this.BtnSnap.TabIndex = 43;
            this.BtnSnap.Text = "取图";
            this.BtnSnap.UseVisualStyleBackColor = true;
            this.BtnSnap.Click += new System.EventHandler(this.BtnSnap_Click);
            // 
            // CalibForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.BtnSnap);
            this.Controls.Add(this.PictureShow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CalibForm";
            this.Text = "CalibForm";
            this.Load += new System.EventHandler(this.CalibForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureShow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureShow;
        private System.Windows.Forms.Button BtnSnap;
    }
}