namespace StationDemo
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel_window = new System.Windows.Forms.Panel();
            this.imageListStationState = new System.Windows.Forms.ImageList(this.components);
            this.button_Sys = new System.Windows.Forms.Button();
            this.button_UserSMgr = new System.Windows.Forms.Button();
            this.button_Param = new System.Windows.Forms.Button();
            this.button_AalarmReport = new System.Windows.Forms.Button();
            this.button_LoadInOut = new System.Windows.Forms.Button();
            this.button_vision = new System.Windows.Forms.Button();
            this.button_Home = new System.Windows.Forms.Button();
            this.button_Set = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.button_start = new System.Windows.Forms.Button();
            this.button_Reset = new System.Windows.Forms.Button();
            this.Ver = new System.Windows.Forms.Label();
            this.RunTime = new System.Windows.Forms.Label();
            this.NewTime = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // panel_window
            // 
            this.panel_window.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_window.Location = new System.Drawing.Point(1, 100);
            this.panel_window.Name = "panel_window";
            this.panel_window.Size = new System.Drawing.Size(1556, 457);
            this.panel_window.TabIndex = 0;
            // 
            // imageListStationState
            // 
            this.imageListStationState.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStationState.ImageStream")));
            this.imageListStationState.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListStationState.Images.SetKeyName(0, "Start.jpg");
            this.imageListStationState.Images.SetKeyName(1, "pause.jpg");
            this.imageListStationState.Images.SetKeyName(2, "resume.jpg");
            // 
            // button_Sys
            // 
            this.button_Sys.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Sys.BackgroundImage")));
            this.button_Sys.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Sys.Location = new System.Drawing.Point(1035, 2);
            this.button_Sys.Name = "button_Sys";
            this.button_Sys.Size = new System.Drawing.Size(104, 84);
            this.button_Sys.TabIndex = 11;
            this.button_Sys.Text = "系统";
            this.button_Sys.UseVisualStyleBackColor = true;
            this.button_Sys.Visible = false;
            // 
            // button_UserSMgr
            // 
            this.button_UserSMgr.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_UserSMgr.BackgroundImage")));
            this.button_UserSMgr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_UserSMgr.Location = new System.Drawing.Point(887, 2);
            this.button_UserSMgr.Name = "button_UserSMgr";
            this.button_UserSMgr.Size = new System.Drawing.Size(104, 84);
            this.button_UserSMgr.TabIndex = 10;
            this.button_UserSMgr.Text = "用户管理";
            this.button_UserSMgr.UseVisualStyleBackColor = true;
            this.button_UserSMgr.Click += new System.EventHandler(this.button_UserSet_Click);
            // 
            // button_Param
            // 
            this.button_Param.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Param.BackgroundImage")));
            this.button_Param.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Param.Location = new System.Drawing.Point(667, 2);
            this.button_Param.Name = "button_Param";
            this.button_Param.Size = new System.Drawing.Size(104, 84);
            this.button_Param.TabIndex = 9;
            this.button_Param.Text = "参数";
            this.button_Param.UseVisualStyleBackColor = true;
            this.button_Param.Click += new System.EventHandler(this.button_Param_Click);
            // 
            // button_AalarmReport
            // 
            this.button_AalarmReport.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_AalarmReport.BackgroundImage")));
            this.button_AalarmReport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_AalarmReport.Location = new System.Drawing.Point(1145, 2);
            this.button_AalarmReport.Name = "button_AalarmReport";
            this.button_AalarmReport.Size = new System.Drawing.Size(104, 84);
            this.button_AalarmReport.TabIndex = 8;
            this.button_AalarmReport.Text = "报警";
            this.button_AalarmReport.UseVisualStyleBackColor = true;
            this.button_AalarmReport.Visible = false;
            this.button_AalarmReport.Click += new System.EventHandler(this.button_AalarmReport_Click);
            // 
            // button_LoadInOut
            // 
            this.button_LoadInOut.BackColor = System.Drawing.SystemColors.Control;
            this.button_LoadInOut.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_LoadInOut.BackgroundImage")));
            this.button_LoadInOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_LoadInOut.Location = new System.Drawing.Point(8, 2);
            this.button_LoadInOut.Margin = new System.Windows.Forms.Padding(2);
            this.button_LoadInOut.Name = "button_LoadInOut";
            this.button_LoadInOut.Size = new System.Drawing.Size(104, 84);
            this.button_LoadInOut.TabIndex = 7;
            this.button_LoadInOut.Text = "登陆";
            this.button_LoadInOut.UseVisualStyleBackColor = false;
            this.button_LoadInOut.Click += new System.EventHandler(this.button_LoadInOut_Click);
            // 
            // button_vision
            // 
            this.button_vision.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_vision.BackgroundImage")));
            this.button_vision.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_vision.Location = new System.Drawing.Point(777, 2);
            this.button_vision.Name = "button_vision";
            this.button_vision.Size = new System.Drawing.Size(104, 84);
            this.button_vision.TabIndex = 6;
            this.button_vision.Text = "图像设置";
            this.button_vision.UseVisualStyleBackColor = true;
            this.button_vision.Click += new System.EventHandler(this.button_vision_Click);
            // 
            // button_Home
            // 
            this.button_Home.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Home.BackgroundImage")));
            this.button_Home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Home.Location = new System.Drawing.Point(447, 2);
            this.button_Home.Name = "button_Home";
            this.button_Home.Size = new System.Drawing.Size(104, 84);
            this.button_Home.TabIndex = 5;
            this.button_Home.Text = "主窗口";
            this.button_Home.UseVisualStyleBackColor = true;
            this.button_Home.Click += new System.EventHandler(this.button_Home_Click);
            // 
            // button_Set
            // 
            this.button_Set.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Set.BackgroundImage")));
            this.button_Set.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Set.Location = new System.Drawing.Point(557, 2);
            this.button_Set.Name = "button_Set";
            this.button_Set.Size = new System.Drawing.Size(104, 84);
            this.button_Set.TabIndex = 4;
            this.button_Set.Text = "设置";
            this.button_Set.UseVisualStyleBackColor = true;
            this.button_Set.Click += new System.EventHandler(this.button_set_Click);
            // 
            // button_stop
            // 
            this.button_stop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_stop.BackgroundImage")));
            this.button_stop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_stop.Location = new System.Drawing.Point(337, 2);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(104, 84);
            this.button_stop.TabIndex = 3;
            this.button_stop.Text = "停止";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // button_start
            // 
            this.button_start.BackgroundImage = global::StationDemo.Properties.Resources.Start;
            this.button_start.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_start.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_start.Location = new System.Drawing.Point(227, 2);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(104, 84);
            this.button_start.TabIndex = 1;
            this.button_start.Text = "启动";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_Reset
            // 
            this.button_Reset.BackColor = System.Drawing.Color.Yellow;
            this.button_Reset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Reset.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Reset.Location = new System.Drawing.Point(117, 2);
            this.button_Reset.Name = "button_Reset";
            this.button_Reset.Size = new System.Drawing.Size(104, 84);
            this.button_Reset.TabIndex = 12;
            this.button_Reset.Text = "复位";
            this.button_Reset.UseVisualStyleBackColor = false;
            this.button_Reset.Click += new System.EventHandler(this.button_Reset_Click);
            // 
            // Ver
            // 
            this.Ver.AutoSize = true;
            this.Ver.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Ver.Location = new System.Drawing.Point(997, 2);
            this.Ver.Name = "Ver";
            this.Ver.Size = new System.Drawing.Size(115, 21);
            this.Ver.TabIndex = 25;
            this.Ver.Text = "软件版本：";
            // 
            // RunTime
            // 
            this.RunTime.AutoSize = true;
            this.RunTime.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.RunTime.Location = new System.Drawing.Point(997, 32);
            this.RunTime.Name = "RunTime";
            this.RunTime.Size = new System.Drawing.Size(115, 21);
            this.RunTime.TabIndex = 26;
            this.RunTime.Text = "运行时间：";
            // 
            // NewTime
            // 
            this.NewTime.AutoSize = true;
            this.NewTime.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.NewTime.Location = new System.Drawing.Point(997, 64);
            this.NewTime.Name = "NewTime";
            this.NewTime.Size = new System.Drawing.Size(115, 21);
            this.NewTime.TabIndex = 27;
            this.NewTime.Text = "当前时间：";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 569);
            this.Controls.Add(this.NewTime);
            this.Controls.Add(this.RunTime);
            this.Controls.Add(this.Ver);
            this.Controls.Add(this.button_Reset);
            this.Controls.Add(this.button_Sys);
            this.Controls.Add(this.button_UserSMgr);
            this.Controls.Add(this.button_Param);
            this.Controls.Add(this.button_AalarmReport);
            this.Controls.Add(this.button_LoadInOut);
            this.Controls.Add(this.button_vision);
            this.Controls.Add(this.button_Home);
            this.Controls.Add(this.button_Set);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.panel_window);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "HWAutoFrame1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClosingMainFrom);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CloseFrom);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel_window;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.Button button_Set;
        private System.Windows.Forms.Button button_Home;
        private System.Windows.Forms.Button button_vision;
        private System.Windows.Forms.Button button_LoadInOut;
        private System.Windows.Forms.Button button_AalarmReport;
        private System.Windows.Forms.Button button_Param;
        private System.Windows.Forms.Button button_UserSMgr;
        private System.Windows.Forms.Button button_Sys;
        private System.Windows.Forms.ImageList imageListStationState;
        private System.Windows.Forms.Button button_Reset;
        private System.Windows.Forms.Label Ver;
        private System.Windows.Forms.Label RunTime;
        private System.Windows.Forms.Label NewTime;
        private System.Windows.Forms.Timer timer1;
    }
}

