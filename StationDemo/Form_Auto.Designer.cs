namespace StationDemo
{
    partial class Form_Auto
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Auto));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tabControl_SelCammer = new System.Windows.Forms.TabControl();
            this.tabControl_Log = new System.Windows.Forms.TabControl();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label_CurrentFile = new System.Windows.Forms.Label();
            this.userPanel_Flag = new UserCtrl.UserPanel();
            this.visionControl1 = new UserCtrl.VisionControl();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_SN = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Throufocus1 = new System.Windows.Forms.TabPage();
            this.chartData1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Throufocus2 = new System.Windows.Forms.TabPage();
            this.chartData2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ClearCT = new System.Windows.Forms.Button();
            this.Chart_NG = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.NoSN = new System.Windows.Forms.CheckBox();
            this.PictureShow = new System.Windows.Forms.PictureBox();
            this.CheckModule = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItem_PlayA = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_PlayB = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_SaveImageResult = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_SaveImageBMP = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_ShowRoi = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnClearAllProduct = new AutoFrameUI.RoundButton();
            this.BtnClose = new System.Windows.Forms.Button();
            this.MachineStatePause = new UserCtrl.UserLabel();
            this.MachineStateStop = new UserCtrl.UserLabel();
            this.MachineStateAuto = new UserCtrl.UserLabel();
            this.MachineStateEmg = new UserCtrl.UserLabel();
            this.dataGridViewNG = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ProductBatch = new System.Windows.Forms.CheckBox();
            this.txt_Batch = new System.Windows.Forms.TextBox();
            this.Result = new System.Windows.Forms.Label();
            this.AACalib = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.visionControl1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.Throufocus1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData1)).BeginInit();
            this.Throufocus2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Chart_NG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureShow)).BeginInit();
            this.CheckModule.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl_SelCammer
            // 
            this.tabControl_SelCammer.Location = new System.Drawing.Point(2, 12);
            this.tabControl_SelCammer.Name = "tabControl_SelCammer";
            this.tabControl_SelCammer.SelectedIndex = 0;
            this.tabControl_SelCammer.Size = new System.Drawing.Size(9, 16);
            this.tabControl_SelCammer.TabIndex = 0;
            // 
            // tabControl_Log
            // 
            this.tabControl_Log.Location = new System.Drawing.Point(11, 359);
            this.tabControl_Log.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl_Log.Name = "tabControl_Log";
            this.tabControl_Log.SelectedIndex = 0;
            this.tabControl_Log.Size = new System.Drawing.Size(403, 342);
            this.tabControl_Log.TabIndex = 1;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "light_gray.png");
            this.imageList1.Images.SetKeyName(1, "light_green.png");
            this.imageList1.Images.SetKeyName(2, "light_red.png");
            // 
            // label_CurrentFile
            // 
            this.label_CurrentFile.AutoSize = true;
            this.label_CurrentFile.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_CurrentFile.Location = new System.Drawing.Point(947, 13);
            this.label_CurrentFile.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_CurrentFile.Name = "label_CurrentFile";
            this.label_CurrentFile.Size = new System.Drawing.Size(93, 16);
            this.label_CurrentFile.TabIndex = 6;
            this.label_CurrentFile.Text = "当前产品：";
            // 
            // userPanel_Flag
            // 
            this.userPanel_Flag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.userPanel_Flag.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.userPanel_Flag.Location = new System.Drawing.Point(8, 707);
            this.userPanel_Flag.m_nNumPerPage = 10;
            this.userPanel_Flag.m_nNumPerRow = 5;
            this.userPanel_Flag.m_page = 0;
            this.userPanel_Flag.m_splitHigh = 30;
            this.userPanel_Flag.m_splitWidth = 160;
            this.userPanel_Flag.Margin = new System.Windows.Forms.Padding(4);
            this.userPanel_Flag.Name = "userPanel_Flag";
            this.userPanel_Flag.Size = new System.Drawing.Size(861, 99);
            this.userPanel_Flag.TabIndex = 17;
            // 
            // visionControl1
            // 
            this.visionControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.visionControl1.ImgHight = 1024;
            this.visionControl1.ImgWidth = 1280;
            this.visionControl1.Location = new System.Drawing.Point(8, 11);
            this.visionControl1.Margin = new System.Windows.Forms.Padding(2);
            this.visionControl1.Name = "visionControl1";
            this.visionControl1.Size = new System.Drawing.Size(403, 332);
            this.visionControl1.TabIndex = 12;
            this.visionControl1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(912, 689);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 26;
            this.label1.Text = "条码：";
            // 
            // txt_SN
            // 
            this.txt_SN.Location = new System.Drawing.Point(959, 685);
            this.txt_SN.Name = "txt_SN";
            this.txt_SN.Size = new System.Drawing.Size(133, 21);
            this.txt_SN.TabIndex = 29;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Throufocus1);
            this.tabControl1.Controls.Add(this.Throufocus2);
            this.tabControl1.Location = new System.Drawing.Point(433, 359);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(440, 342);
            this.tabControl1.TabIndex = 38;
            // 
            // Throufocus1
            // 
            this.Throufocus1.Controls.Add(this.chartData1);
            this.Throufocus1.Location = new System.Drawing.Point(4, 22);
            this.Throufocus1.Margin = new System.Windows.Forms.Padding(2);
            this.Throufocus1.Name = "Throufocus1";
            this.Throufocus1.Size = new System.Drawing.Size(432, 316);
            this.Throufocus1.TabIndex = 0;
            this.Throufocus1.Text = "Throufocus1";
            this.Throufocus1.UseVisualStyleBackColor = true;
            // 
            // chartData1
            // 
            chartArea4.Name = "ChartArea1";
            this.chartData1.ChartAreas.Add(chartArea4);
            this.chartData1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend4.Name = "Legend1";
            this.chartData1.Legends.Add(legend4);
            this.chartData1.Location = new System.Drawing.Point(0, 0);
            this.chartData1.Margin = new System.Windows.Forms.Padding(2);
            this.chartData1.Name = "chartData1";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartData1.Series.Add(series4);
            this.chartData1.Size = new System.Drawing.Size(432, 316);
            this.chartData1.TabIndex = 0;
            this.chartData1.Text = "chart1";
            // 
            // Throufocus2
            // 
            this.Throufocus2.Controls.Add(this.chartData2);
            this.Throufocus2.Location = new System.Drawing.Point(4, 22);
            this.Throufocus2.Margin = new System.Windows.Forms.Padding(2);
            this.Throufocus2.Name = "Throufocus2";
            this.Throufocus2.Size = new System.Drawing.Size(432, 316);
            this.Throufocus2.TabIndex = 1;
            this.Throufocus2.Text = "Throufocus2";
            this.Throufocus2.UseVisualStyleBackColor = true;
            // 
            // chartData2
            // 
            chartArea5.Name = "ChartArea1";
            this.chartData2.ChartAreas.Add(chartArea5);
            this.chartData2.Dock = System.Windows.Forms.DockStyle.Fill;
            legend5.Name = "Legend1";
            this.chartData2.Legends.Add(legend5);
            this.chartData2.Location = new System.Drawing.Point(0, 0);
            this.chartData2.Margin = new System.Windows.Forms.Padding(2);
            this.chartData2.Name = "chartData2";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.chartData2.Series.Add(series5);
            this.chartData2.Size = new System.Drawing.Size(432, 316);
            this.chartData2.TabIndex = 1;
            this.chartData2.Text = "chart2";
            // 
            // ClearCT
            // 
            this.ClearCT.Location = new System.Drawing.Point(904, 766);
            this.ClearCT.Name = "ClearCT";
            this.ClearCT.Size = new System.Drawing.Size(83, 35);
            this.ClearCT.TabIndex = 39;
            this.ClearCT.Text = "清空数据";
            this.ClearCT.UseVisualStyleBackColor = true;
            this.ClearCT.Click += new System.EventHandler(this.ClearCT_Click);
            // 
            // Chart_NG
            // 
            chartArea6.Name = "ChartArea1";
            this.Chart_NG.ChartAreas.Add(chartArea6);
            legend6.Enabled = false;
            legend6.Name = "Legend1";
            this.Chart_NG.Legends.Add(legend6);
            this.Chart_NG.Location = new System.Drawing.Point(915, 381);
            this.Chart_NG.Margin = new System.Windows.Forms.Padding(2);
            this.Chart_NG.Name = "Chart_NG";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series6.IsValueShownAsLabel = true;
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.Chart_NG.Series.Add(series6);
            this.Chart_NG.Size = new System.Drawing.Size(309, 299);
            this.Chart_NG.TabIndex = 2;
            this.Chart_NG.Text = "chart3";
            // 
            // NoSN
            // 
            this.NoSN.AutoSize = true;
            this.NoSN.Checked = true;
            this.NoSN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.NoSN.Location = new System.Drawing.Point(1098, 689);
            this.NoSN.Margin = new System.Windows.Forms.Padding(2);
            this.NoSN.Name = "NoSN";
            this.NoSN.Size = new System.Drawing.Size(72, 16);
            this.NoSN.TabIndex = 40;
            this.NoSN.Text = "允许空SN";
            this.NoSN.UseVisualStyleBackColor = true;
            // 
            // PictureShow
            // 
            this.PictureShow.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.PictureShow.ContextMenuStrip = this.CheckModule;
            this.PictureShow.Location = new System.Drawing.Point(433, 13);
            this.PictureShow.Margin = new System.Windows.Forms.Padding(2);
            this.PictureShow.Name = "PictureShow";
            this.PictureShow.Size = new System.Drawing.Size(440, 330);
            this.PictureShow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureShow.TabIndex = 41;
            this.PictureShow.TabStop = false;
            this.PictureShow.DoubleClick += new System.EventHandler(this.PictureShow_DoubleClick);
            // 
            // CheckModule
            // 
            this.CheckModule.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_PlayA,
            this.MenuItem_PlayB,
            this.MenuItem_Stop,
            this.MenuItem_SaveImageResult,
            this.MenuItem_SaveImageBMP,
            this.MenuItem_ShowRoi,
            this.AACalib});
            this.CheckModule.Name = "CheckModule";
            this.CheckModule.Size = new System.Drawing.Size(181, 180);
            // 
            // MenuItem_PlayA
            // 
            this.MenuItem_PlayA.Name = "MenuItem_PlayA";
            this.MenuItem_PlayA.Size = new System.Drawing.Size(180, 22);
            this.MenuItem_PlayA.Text = "A工位点亮";
            this.MenuItem_PlayA.Click += new System.EventHandler(this.MenuItem_PlayA_Click);
            // 
            // MenuItem_PlayB
            // 
            this.MenuItem_PlayB.Name = "MenuItem_PlayB";
            this.MenuItem_PlayB.Size = new System.Drawing.Size(180, 22);
            this.MenuItem_PlayB.Text = "B工位点亮";
            this.MenuItem_PlayB.Click += new System.EventHandler(this.MenuItem_PlayB_Click);
            // 
            // MenuItem_Stop
            // 
            this.MenuItem_Stop.Name = "MenuItem_Stop";
            this.MenuItem_Stop.Size = new System.Drawing.Size(180, 22);
            this.MenuItem_Stop.Text = "关闭取图";
            this.MenuItem_Stop.Click += new System.EventHandler(this.MenuItem_Stop_Click);
            // 
            // MenuItem_SaveImageResult
            // 
            this.MenuItem_SaveImageResult.Name = "MenuItem_SaveImageResult";
            this.MenuItem_SaveImageResult.Size = new System.Drawing.Size(180, 22);
            this.MenuItem_SaveImageResult.Text = "保存图片";
            this.MenuItem_SaveImageResult.Click += new System.EventHandler(this.MenuItem_SaveImageResult_Click);
            // 
            // MenuItem_SaveImageBMP
            // 
            this.MenuItem_SaveImageBMP.Name = "MenuItem_SaveImageBMP";
            this.MenuItem_SaveImageBMP.Size = new System.Drawing.Size(180, 22);
            this.MenuItem_SaveImageBMP.Text = "保存原图";
            this.MenuItem_SaveImageBMP.Click += new System.EventHandler(this.MenuItem_SaveImageBMP_Click);
            // 
            // MenuItem_ShowRoi
            // 
            this.MenuItem_ShowRoi.Name = "MenuItem_ShowRoi";
            this.MenuItem_ShowRoi.Size = new System.Drawing.Size(180, 22);
            this.MenuItem_ShowRoi.Text = "显示ROI";
            this.MenuItem_ShowRoi.Click += new System.EventHandler(this.MenuItem_ShowRoi_Click);
            // 
            // BtnClearAllProduct
            // 
            this.BtnClearAllProduct.BackColor = System.Drawing.Color.Transparent;
            this.BtnClearAllProduct.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.BtnClearAllProduct.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.BtnClearAllProduct.FlatAppearance.BorderSize = 0;
            this.BtnClearAllProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnClearAllProduct.ImageHeight = 80;
            this.BtnClearAllProduct.ImageWidth = 80;
            this.BtnClearAllProduct.Location = new System.Drawing.Point(1171, 766);
            this.BtnClearAllProduct.Margin = new System.Windows.Forms.Padding(2);
            this.BtnClearAllProduct.Name = "BtnClearAllProduct";
            this.BtnClearAllProduct.Radius = 24;
            this.BtnClearAllProduct.Size = new System.Drawing.Size(83, 35);
            this.BtnClearAllProduct.SpliteButtonWidth = 18;
            this.BtnClearAllProduct.TabIndex = 42;
            this.BtnClearAllProduct.Text = "清料";
            this.BtnClearAllProduct.UseVisualStyleBackColor = false;
            this.BtnClearAllProduct.Click += new System.EventHandler(this.BtnClearAllProduct_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(1038, 766);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(83, 35);
            this.BtnClose.TabIndex = 44;
            this.BtnClose.Text = "关闭蜂鸣器";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // MachineStatePause
            // 
            this.MachineStatePause.Location = new System.Drawing.Point(1184, 734);
            this.MachineStatePause.Margin = new System.Windows.Forms.Padding(1);
            this.MachineStatePause.Name = "MachineStatePause";
            this.MachineStatePause.Size = new System.Drawing.Size(73, 28);
            this.MachineStatePause.State = false;
            this.MachineStatePause.TabIndex = 48;
            // 
            // MachineStateStop
            // 
            this.MachineStateStop.Location = new System.Drawing.Point(994, 734);
            this.MachineStateStop.Margin = new System.Windows.Forms.Padding(1);
            this.MachineStateStop.Name = "MachineStateStop";
            this.MachineStateStop.Size = new System.Drawing.Size(73, 28);
            this.MachineStateStop.State = false;
            this.MachineStateStop.TabIndex = 47;
            // 
            // MachineStateAuto
            // 
            this.MachineStateAuto.Location = new System.Drawing.Point(1090, 734);
            this.MachineStateAuto.Margin = new System.Windows.Forms.Padding(1);
            this.MachineStateAuto.Name = "MachineStateAuto";
            this.MachineStateAuto.Size = new System.Drawing.Size(73, 28);
            this.MachineStateAuto.State = false;
            this.MachineStateAuto.TabIndex = 46;
            // 
            // MachineStateEmg
            // 
            this.MachineStateEmg.Location = new System.Drawing.Point(904, 734);
            this.MachineStateEmg.Margin = new System.Windows.Forms.Padding(1);
            this.MachineStateEmg.Name = "MachineStateEmg";
            this.MachineStateEmg.Size = new System.Drawing.Size(73, 28);
            this.MachineStateEmg.State = false;
            this.MachineStateEmg.TabIndex = 45;
            // 
            // dataGridViewNG
            // 
            this.dataGridViewNG.AllowUserToAddRows = false;
            this.dataGridViewNG.AllowUserToDeleteRows = false;
            this.dataGridViewNG.AllowUserToResizeColumns = false;
            this.dataGridViewNG.AllowUserToResizeRows = false;
            this.dataGridViewNG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewNG.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.dataGridViewNG.Location = new System.Drawing.Point(919, 183);
            this.dataGridViewNG.Name = "dataGridViewNG";
            this.dataGridViewNG.ReadOnly = true;
            this.dataGridViewNG.RowHeadersVisible = false;
            this.dataGridViewNG.RowTemplate.Height = 23;
            this.dataGridViewNG.Size = new System.Drawing.Size(305, 160);
            this.dataGridViewNG.TabIndex = 51;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "不良项目";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 95;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "A";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 65;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "B";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn3.Width = 65;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "总数";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn4.Width = 75;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(919, 74);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(305, 103);
            this.pictureBox1.TabIndex = 53;
            this.pictureBox1.TabStop = false;
            // 
            // ProductBatch
            // 
            this.ProductBatch.AutoSize = true;
            this.ProductBatch.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ProductBatch.Location = new System.Drawing.Point(950, 33);
            this.ProductBatch.Name = "ProductBatch";
            this.ProductBatch.Size = new System.Drawing.Size(98, 23);
            this.ProductBatch.TabIndex = 54;
            this.ProductBatch.Text = "产品批号：";
            this.ProductBatch.UseVisualStyleBackColor = true;
            this.ProductBatch.CheckedChanged += new System.EventHandler(this.ProductBatch_CheckedChanged);
            // 
            // txt_Batch
            // 
            this.txt_Batch.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_Batch.Location = new System.Drawing.Point(1054, 33);
            this.txt_Batch.Name = "txt_Batch";
            this.txt_Batch.ReadOnly = true;
            this.txt_Batch.Size = new System.Drawing.Size(100, 23);
            this.txt_Batch.TabIndex = 55;
            this.txt_Batch.Text = "批次1";
            // 
            // Result
            // 
            this.Result.AutoSize = true;
            this.Result.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Result.Location = new System.Drawing.Point(607, 359);
            this.Result.Name = "Result";
            this.Result.Size = new System.Drawing.Size(66, 19);
            this.Result.TabIndex = 56;
            this.Result.Text = "初始化";
            // 
            // AACalib
            // 
            this.AACalib.Name = "AACalib";
            this.AACalib.Size = new System.Drawing.Size(180, 22);
            this.AACalib.Text = "AA校准";
            this.AACalib.Click += new System.EventHandler(this.AACalib_Click);
            // 
            // Form_Auto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1292, 808);
            this.Controls.Add(this.Result);
            this.Controls.Add(this.txt_Batch);
            this.Controls.Add(this.ProductBatch);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.dataGridViewNG);
            this.Controls.Add(this.MachineStatePause);
            this.Controls.Add(this.MachineStateStop);
            this.Controls.Add(this.MachineStateAuto);
            this.Controls.Add(this.MachineStateEmg);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnClearAllProduct);
            this.Controls.Add(this.PictureShow);
            this.Controls.Add(this.NoSN);
            this.Controls.Add(this.Chart_NG);
            this.Controls.Add(this.ClearCT);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.txt_SN);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.userPanel_Flag);
            this.Controls.Add(this.visionControl1);
            this.Controls.Add(this.label_CurrentFile);
            this.Controls.Add(this.tabControl_Log);
            this.Controls.Add(this.tabControl_SelCammer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_Auto";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CloseMainForm);
            this.Load += new System.EventHandler(this.Form_Auto_Load);
            ((System.ComponentModel.ISupportInitialize)(this.visionControl1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.Throufocus1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartData1)).EndInit();
            this.Throufocus2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartData2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Chart_NG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureShow)).EndInit();
            this.CheckModule.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl_SelCammer;
        private System.Windows.Forms.TabControl tabControl_Log;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label_CurrentFile;
        private UserCtrl.UserPanel userPanel_Flag;
        public UserCtrl.VisionControl visionControl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_SN;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Throufocus1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartData1;
        private System.Windows.Forms.TabPage Throufocus2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartData2;
        private System.Windows.Forms.Button ClearCT;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart_NG;
        private System.Windows.Forms.CheckBox NoSN;
        private System.Windows.Forms.PictureBox PictureShow;
        private AutoFrameUI.RoundButton BtnClearAllProduct;
        private System.Windows.Forms.ContextMenuStrip CheckModule;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_SaveImageResult;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_SaveImageBMP;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_PlayA;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_PlayB;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Stop;
        private System.Windows.Forms.Button BtnClose;
        private UserCtrl.UserLabel MachineStatePause;
        private UserCtrl.UserLabel MachineStateStop;
        private UserCtrl.UserLabel MachineStateAuto;
        private UserCtrl.UserLabel MachineStateEmg;
        private System.Windows.Forms.DataGridView dataGridViewNG;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_ShowRoi;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox ProductBatch;
        private System.Windows.Forms.TextBox txt_Batch;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Label Result;
        private System.Windows.Forms.ToolStripMenuItem AACalib;
    }
}