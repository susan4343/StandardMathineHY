namespace StationDemo
{
    partial class Form_ShowImage
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
            ((System.ComponentModel.ISupportInitialize)(this.PictureShow)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureShow
            // 
            this.PictureShow.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.PictureShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PictureShow.Location = new System.Drawing.Point(0, 0);
            this.PictureShow.Name = "PictureShow";
            this.PictureShow.Size = new System.Drawing.Size(575, 414);
            this.PictureShow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureShow.TabIndex = 0;
            this.PictureShow.TabStop = false;
            this.PictureShow.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // Form_ShowImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 414);
            this.Controls.Add(this.PictureShow);
            this.Name = "Form_ShowImage";
            this.Text = "Form_ShowImage";
            ((System.ComponentModel.ISupportInitialize)(this.PictureShow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox PictureShow;
    }
}