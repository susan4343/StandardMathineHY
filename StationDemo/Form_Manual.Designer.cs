namespace StationDemo
{
    partial class Form_Manual
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.visionControl1 = new UserCtrl.VisionControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.单次采集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.连续采集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存原图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBox_SelCamera = new System.Windows.Forms.ComboBox();
            this.OpenCKPower = new System.Windows.Forms.Button();
            this.CloseCKPower = new System.Windows.Forms.Button();
            this.ChannelChoose = new System.Windows.Forms.ComboBox();
            this.RunOneStep = new System.Windows.Forms.Button();
            this.GetCurrent = new System.Windows.Forms.Button();
            this.ManualStepAA = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioBtnAve = new System.Windows.Forms.RadioButton();
            this.radioBtnPreak = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxB = new System.Windows.Forms.CheckBox();
            this.checkBoxA = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.MFTest = new System.Windows.Forms.Button();
            this.TableRunManual = new System.Windows.Forms.Button();
            this.radioBtnA = new System.Windows.Forms.RadioButton();
            this.radioBtnB = new System.Windows.Forms.RadioButton();
            this.TestCompete = new System.Windows.Forms.Button();
            this.AAGroupBox = new System.Windows.Forms.GroupBox();
            this.btnStep_7 = new System.Windows.Forms.Button();
            this.btnStep_4 = new System.Windows.Forms.Button();
            this.btnStep_6 = new System.Windows.Forms.Button();
            this.btnStep_5 = new System.Windows.Forms.Button();
            this.btnStep_3 = new System.Windows.Forms.Button();
            this.btnStep_2 = new System.Windows.Forms.Button();
            this.btnStep_1 = new System.Windows.Forms.Button();
            this.btnStep_0 = new System.Windows.Forms.Button();
            this.GoDisp = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.chartData1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chartData2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Open_UV = new System.Windows.Forms.Button();
            this.ShowResult = new System.Windows.Forms.TextBox();
            this.ManualStepDisp = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.RunDispStep = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnDisp_7 = new System.Windows.Forms.Button();
            this.btnDisp_6 = new System.Windows.Forms.Button();
            this.btnDisp_0 = new System.Windows.Forms.Button();
            this.btnDisp_5 = new System.Windows.Forms.Button();
            this.btnDisp_1 = new System.Windows.Forms.Button();
            this.btnDisp_2 = new System.Windows.Forms.Button();
            this.btnDisp_4 = new System.Windows.Forms.Button();
            this.btnDisp_3 = new System.Windows.Forms.Button();
            this.Group = new System.Windows.Forms.GroupBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.btnEnumAllCam = new System.Windows.Forms.Button();
            this.cbxAllUSB = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.visionControl1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.ManualStepAA.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.AAGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.ManualStepDisp.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.Group.SuspendLayout();
            this.SuspendLayout();
            // 
            // visionControl1
            // 
            this.visionControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.visionControl1.ImgHight = 512;
            this.visionControl1.ImgWidth = 612;
            this.visionControl1.Location = new System.Drawing.Point(-14, 431);
            this.visionControl1.Name = "visionControl1";
            this.visionControl1.Size = new System.Drawing.Size(559, 287);
            this.visionControl1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.visionControl1.TabIndex = 151;
            this.visionControl1.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.单次采集ToolStripMenuItem,
            this.连续采集ToolStripMenuItem,
            this.保存原图ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 70);
            // 
            // 单次采集ToolStripMenuItem
            // 
            this.单次采集ToolStripMenuItem.Name = "单次采集ToolStripMenuItem";
            this.单次采集ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.单次采集ToolStripMenuItem.Text = "单次采集";
            this.单次采集ToolStripMenuItem.Click += new System.EventHandler(this.单次采集ToolStripMenuItem_Click);
            // 
            // 连续采集ToolStripMenuItem
            // 
            this.连续采集ToolStripMenuItem.Name = "连续采集ToolStripMenuItem";
            this.连续采集ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.连续采集ToolStripMenuItem.Text = "连续采集";
            this.连续采集ToolStripMenuItem.Click += new System.EventHandler(this.连续采集ToolStripMenuItem_Click);
            // 
            // 保存原图ToolStripMenuItem
            // 
            this.保存原图ToolStripMenuItem.Name = "保存原图ToolStripMenuItem";
            this.保存原图ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.保存原图ToolStripMenuItem.Text = "保存原图";
            this.保存原图ToolStripMenuItem.Click += new System.EventHandler(this.保存原图ToolStripMenuItem_Click);
            // 
            // comboBox_SelCamera
            // 
            this.comboBox_SelCamera.FormattingEnabled = true;
            this.comboBox_SelCamera.Location = new System.Drawing.Point(359, 93);
            this.comboBox_SelCamera.Name = "comboBox_SelCamera";
            this.comboBox_SelCamera.Size = new System.Drawing.Size(106, 20);
            this.comboBox_SelCamera.TabIndex = 154;
            this.comboBox_SelCamera.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelCamera_SelectedIndexChanged);
            // 
            // OpenCKPower
            // 
            this.OpenCKPower.Location = new System.Drawing.Point(88, 29);
            this.OpenCKPower.Name = "OpenCKPower";
            this.OpenCKPower.Size = new System.Drawing.Size(78, 50);
            this.OpenCKPower.TabIndex = 183;
            this.OpenCKPower.Text = "打开电源";
            this.OpenCKPower.UseVisualStyleBackColor = true;
            this.OpenCKPower.Click += new System.EventHandler(this.OpenCKPower_Click);
            // 
            // CloseCKPower
            // 
            this.CloseCKPower.Location = new System.Drawing.Point(88, 85);
            this.CloseCKPower.Name = "CloseCKPower";
            this.CloseCKPower.Size = new System.Drawing.Size(78, 50);
            this.CloseCKPower.TabIndex = 186;
            this.CloseCKPower.Text = "关闭电源";
            this.CloseCKPower.UseVisualStyleBackColor = true;
            this.CloseCKPower.Click += new System.EventHandler(this.CloseCKPower_Click);
            // 
            // ChannelChoose
            // 
            this.ChannelChoose.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ChannelChoose.FormattingEnabled = true;
            this.ChannelChoose.ItemHeight = 20;
            this.ChannelChoose.Items.AddRange(new object[] {
            "CH1",
            "CH2"});
            this.ChannelChoose.Location = new System.Drawing.Point(194, 38);
            this.ChannelChoose.Name = "ChannelChoose";
            this.ChannelChoose.Size = new System.Drawing.Size(81, 28);
            this.ChannelChoose.TabIndex = 187;
            this.ChannelChoose.Text = "CH1";
            // 
            // RunOneStep
            // 
            this.RunOneStep.Location = new System.Drawing.Point(6, 83);
            this.RunOneStep.Name = "RunOneStep";
            this.RunOneStep.Size = new System.Drawing.Size(85, 40);
            this.RunOneStep.TabIndex = 193;
            this.RunOneStep.Text = "单步执行";
            this.RunOneStep.UseVisualStyleBackColor = true;
            this.RunOneStep.Click += new System.EventHandler(this.RunAAStep_Click);
            // 
            // GetCurrent
            // 
            this.GetCurrent.Location = new System.Drawing.Point(194, 85);
            this.GetCurrent.Name = "GetCurrent";
            this.GetCurrent.Size = new System.Drawing.Size(93, 50);
            this.GetCurrent.TabIndex = 197;
            this.GetCurrent.Text = "读取电压电流";
            this.GetCurrent.UseVisualStyleBackColor = true;
            this.GetCurrent.Click += new System.EventHandler(this.GetCurrent_Click);
            // 
            // ManualStepAA
            // 
            this.ManualStepAA.Controls.Add(this.groupBox5);
            this.ManualStepAA.Controls.Add(this.groupBox4);
            this.ManualStepAA.Controls.Add(this.groupBox3);
            this.ManualStepAA.Controls.Add(this.AAGroupBox);
            this.ManualStepAA.Location = new System.Drawing.Point(556, 12);
            this.ManualStepAA.Name = "ManualStepAA";
            this.ManualStepAA.Size = new System.Drawing.Size(542, 394);
            this.ManualStepAA.TabIndex = 205;
            this.ManualStepAA.TabStop = false;
            this.ManualStepAA.Text = "手动流程调试";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioBtnAve);
            this.groupBox5.Controls.Add(this.radioBtnPreak);
            this.groupBox5.Location = new System.Drawing.Point(242, 220);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(235, 89);
            this.groupBox5.TabIndex = 233;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "算法模式";
            // 
            // radioBtnAve
            // 
            this.radioBtnAve.AutoSize = true;
            this.radioBtnAve.BackColor = System.Drawing.SystemColors.Control;
            this.radioBtnAve.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioBtnAve.Location = new System.Drawing.Point(9, 54);
            this.radioBtnAve.Name = "radioBtnAve";
            this.radioBtnAve.Size = new System.Drawing.Size(160, 28);
            this.radioBtnAve.TabIndex = 233;
            this.radioBtnAve.Text = "Average模式";
            this.radioBtnAve.UseVisualStyleBackColor = false;
            this.radioBtnAve.CheckedChanged += new System.EventHandler(this.CheckModule_CheckedChanged);
            // 
            // radioBtnPreak
            // 
            this.radioBtnPreak.AutoSize = true;
            this.radioBtnPreak.BackColor = System.Drawing.SystemColors.Control;
            this.radioBtnPreak.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioBtnPreak.Location = new System.Drawing.Point(9, 20);
            this.radioBtnPreak.Name = "radioBtnPreak";
            this.radioBtnPreak.Size = new System.Drawing.Size(136, 28);
            this.radioBtnPreak.TabIndex = 232;
            this.radioBtnPreak.Text = "Peark模式";
            this.radioBtnPreak.UseVisualStyleBackColor = false;
            this.radioBtnPreak.CheckedChanged += new System.EventHandler(this.CheckModule_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxB);
            this.groupBox4.Controls.Add(this.checkBoxA);
            this.groupBox4.Location = new System.Drawing.Point(242, 315);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(235, 63);
            this.groupBox4.TabIndex = 232;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "自动生产工位选择";
            // 
            // checkBoxB
            // 
            this.checkBoxB.AutoSize = true;
            this.checkBoxB.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBoxB.Location = new System.Drawing.Point(122, 28);
            this.checkBoxB.Name = "checkBoxB";
            this.checkBoxB.Size = new System.Drawing.Size(89, 28);
            this.checkBoxB.TabIndex = 1;
            this.checkBoxB.Text = "B工位";
            this.checkBoxB.UseVisualStyleBackColor = true;
            this.checkBoxB.CheckedChanged += new System.EventHandler(this.checkBoxB_CheckedChanged);
            // 
            // checkBoxA
            // 
            this.checkBoxA.AutoSize = true;
            this.checkBoxA.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBoxA.Location = new System.Drawing.Point(14, 28);
            this.checkBoxA.Name = "checkBoxA";
            this.checkBoxA.Size = new System.Drawing.Size(89, 28);
            this.checkBoxA.TabIndex = 0;
            this.checkBoxA.Text = "A工位";
            this.checkBoxA.UseVisualStyleBackColor = true;
            this.checkBoxA.CheckedChanged += new System.EventHandler(this.checkBoxA_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.MFTest);
            this.groupBox3.Controls.Add(this.TableRunManual);
            this.groupBox3.Controls.Add(this.radioBtnA);
            this.groupBox3.Controls.Add(this.radioBtnB);
            this.groupBox3.Controls.Add(this.TestCompete);
            this.groupBox3.Controls.Add(this.RunOneStep);
            this.groupBox3.Location = new System.Drawing.Point(236, 29);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(288, 185);
            this.groupBox3.TabIndex = 231;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "单步调试工位选择";
            // 
            // MFTest
            // 
            this.MFTest.Location = new System.Drawing.Point(6, 139);
            this.MFTest.Name = "MFTest";
            this.MFTest.Size = new System.Drawing.Size(85, 40);
            this.MFTest.TabIndex = 232;
            this.MFTest.Text = "MF标定";
            this.MFTest.UseVisualStyleBackColor = true;
            this.MFTest.Click += new System.EventHandler(this.MFTest_Click);
            // 
            // TableRunManual
            // 
            this.TableRunManual.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TableRunManual.Location = new System.Drawing.Point(128, 83);
            this.TableRunManual.Name = "TableRunManual";
            this.TableRunManual.Size = new System.Drawing.Size(85, 40);
            this.TableRunManual.TabIndex = 231;
            this.TableRunManual.Text = "转盘到上料位";
            this.TableRunManual.UseVisualStyleBackColor = true;
            this.TableRunManual.Click += new System.EventHandler(this.TableRunManual_Click);
            // 
            // radioBtnA
            // 
            this.radioBtnA.AutoSize = true;
            this.radioBtnA.BackColor = System.Drawing.SystemColors.Control;
            this.radioBtnA.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioBtnA.Location = new System.Drawing.Point(15, 36);
            this.radioBtnA.Name = "radioBtnA";
            this.radioBtnA.Size = new System.Drawing.Size(88, 28);
            this.radioBtnA.TabIndex = 230;
            this.radioBtnA.Text = "A工位";
            this.radioBtnA.UseVisualStyleBackColor = false;
            this.radioBtnA.CheckedChanged += new System.EventHandler(this.CheckAB);
            // 
            // radioBtnB
            // 
            this.radioBtnB.AutoSize = true;
            this.radioBtnB.BackColor = System.Drawing.SystemColors.Control;
            this.radioBtnB.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioBtnB.Location = new System.Drawing.Point(123, 36);
            this.radioBtnB.Name = "radioBtnB";
            this.radioBtnB.Size = new System.Drawing.Size(88, 28);
            this.radioBtnB.TabIndex = 229;
            this.radioBtnB.Text = "B工位";
            this.radioBtnB.UseVisualStyleBackColor = false;
            this.radioBtnB.CheckedChanged += new System.EventHandler(this.CheckAB);
            // 
            // TestCompete
            // 
            this.TestCompete.Location = new System.Drawing.Point(128, 139);
            this.TestCompete.Name = "TestCompete";
            this.TestCompete.Size = new System.Drawing.Size(85, 40);
            this.TestCompete.TabIndex = 225;
            this.TestCompete.Text = "成品检测";
            this.TestCompete.UseVisualStyleBackColor = true;
            this.TestCompete.Click += new System.EventHandler(this.TestCompete_Click);
            // 
            // AAGroupBox
            // 
            this.AAGroupBox.Controls.Add(this.btnStep_7);
            this.AAGroupBox.Controls.Add(this.btnStep_4);
            this.AAGroupBox.Controls.Add(this.btnStep_6);
            this.AAGroupBox.Controls.Add(this.btnStep_5);
            this.AAGroupBox.Controls.Add(this.btnStep_3);
            this.AAGroupBox.Controls.Add(this.btnStep_2);
            this.AAGroupBox.Controls.Add(this.btnStep_1);
            this.AAGroupBox.Controls.Add(this.btnStep_0);
            this.AAGroupBox.Location = new System.Drawing.Point(6, 29);
            this.AAGroupBox.Name = "AAGroupBox";
            this.AAGroupBox.Size = new System.Drawing.Size(224, 426);
            this.AAGroupBox.TabIndex = 228;
            this.AAGroupBox.TabStop = false;
            this.AAGroupBox.Text = "AA流程";
            // 
            // btnStep_7
            // 
            this.btnStep_7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep_7.BackColor = System.Drawing.Color.Cyan;
            this.btnStep_7.FlatAppearance.BorderSize = 0;
            this.btnStep_7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnStep_7.Location = new System.Drawing.Point(6, 300);
            this.btnStep_7.Margin = new System.Windows.Forms.Padding(0);
            this.btnStep_7.Name = "btnStep_7";
            this.btnStep_7.Size = new System.Drawing.Size(214, 40);
            this.btnStep_7.TabIndex = 235;
            this.btnStep_7.Text = "结束退出";
            this.btnStep_7.UseVisualStyleBackColor = false;
            this.btnStep_7.Click += new System.EventHandler(this.ChangeTypeAA);
            // 
            // btnStep_4
            // 
            this.btnStep_4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep_4.BackColor = System.Drawing.Color.Cyan;
            this.btnStep_4.FlatAppearance.BorderSize = 0;
            this.btnStep_4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnStep_4.Location = new System.Drawing.Point(6, 180);
            this.btnStep_4.Margin = new System.Windows.Forms.Padding(0);
            this.btnStep_4.Name = "btnStep_4";
            this.btnStep_4.Size = new System.Drawing.Size(214, 40);
            this.btnStep_4.TabIndex = 234;
            this.btnStep_4.Text = "TiltMove";
            this.btnStep_4.UseVisualStyleBackColor = false;
            this.btnStep_4.Click += new System.EventHandler(this.ChangeTypeAA);
            // 
            // btnStep_6
            // 
            this.btnStep_6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep_6.BackColor = System.Drawing.Color.Cyan;
            this.btnStep_6.FlatAppearance.BorderSize = 0;
            this.btnStep_6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnStep_6.Location = new System.Drawing.Point(6, 260);
            this.btnStep_6.Margin = new System.Windows.Forms.Padding(0);
            this.btnStep_6.Name = "btnStep_6";
            this.btnStep_6.Size = new System.Drawing.Size(214, 40);
            this.btnStep_6.TabIndex = 233;
            this.btnStep_6.Text = "UV";
            this.btnStep_6.UseVisualStyleBackColor = false;
            this.btnStep_6.Click += new System.EventHandler(this.ChangeTypeAA);
            // 
            // btnStep_5
            // 
            this.btnStep_5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep_5.BackColor = System.Drawing.Color.Cyan;
            this.btnStep_5.FlatAppearance.BorderSize = 0;
            this.btnStep_5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnStep_5.Location = new System.Drawing.Point(6, 220);
            this.btnStep_5.Margin = new System.Windows.Forms.Padding(0);
            this.btnStep_5.Name = "btnStep_5";
            this.btnStep_5.Size = new System.Drawing.Size(214, 40);
            this.btnStep_5.TabIndex = 232;
            this.btnStep_5.Text = "Check";
            this.btnStep_5.UseVisualStyleBackColor = false;
            this.btnStep_5.Click += new System.EventHandler(this.ChangeTypeAA);
            // 
            // btnStep_3
            // 
            this.btnStep_3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep_3.BackColor = System.Drawing.Color.Cyan;
            this.btnStep_3.FlatAppearance.BorderSize = 0;
            this.btnStep_3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnStep_3.Location = new System.Drawing.Point(6, 140);
            this.btnStep_3.Margin = new System.Windows.Forms.Padding(0);
            this.btnStep_3.Name = "btnStep_3";
            this.btnStep_3.Size = new System.Drawing.Size(214, 40);
            this.btnStep_3.TabIndex = 231;
            this.btnStep_3.Text = "ThroughFocus";
            this.btnStep_3.UseVisualStyleBackColor = false;
            this.btnStep_3.Click += new System.EventHandler(this.ChangeTypeAA);
            // 
            // btnStep_2
            // 
            this.btnStep_2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep_2.BackColor = System.Drawing.Color.Cyan;
            this.btnStep_2.FlatAppearance.BorderSize = 0;
            this.btnStep_2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnStep_2.Location = new System.Drawing.Point(6, 100);
            this.btnStep_2.Margin = new System.Windows.Forms.Padding(0);
            this.btnStep_2.Name = "btnStep_2";
            this.btnStep_2.Size = new System.Drawing.Size(214, 40);
            this.btnStep_2.TabIndex = 230;
            this.btnStep_2.Text = "对心";
            this.btnStep_2.UseVisualStyleBackColor = false;
            this.btnStep_2.Click += new System.EventHandler(this.ChangeTypeAA);
            // 
            // btnStep_1
            // 
            this.btnStep_1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep_1.BackColor = System.Drawing.Color.Cyan;
            this.btnStep_1.FlatAppearance.BorderSize = 0;
            this.btnStep_1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnStep_1.Location = new System.Drawing.Point(6, 60);
            this.btnStep_1.Margin = new System.Windows.Forms.Padding(0);
            this.btnStep_1.Name = "btnStep_1";
            this.btnStep_1.Size = new System.Drawing.Size(214, 40);
            this.btnStep_1.TabIndex = 229;
            this.btnStep_1.Text = "夹取Lens到AA位";
            this.btnStep_1.UseVisualStyleBackColor = false;
            this.btnStep_1.Click += new System.EventHandler(this.ChangeTypeAA);
            // 
            // btnStep_0
            // 
            this.btnStep_0.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep_0.BackColor = System.Drawing.Color.Cyan;
            this.btnStep_0.FlatAppearance.BorderColor = System.Drawing.Color.Cyan;
            this.btnStep_0.FlatAppearance.BorderSize = 0;
            this.btnStep_0.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnStep_0.Location = new System.Drawing.Point(6, 20);
            this.btnStep_0.Margin = new System.Windows.Forms.Padding(0);
            this.btnStep_0.Name = "btnStep_0";
            this.btnStep_0.Size = new System.Drawing.Size(214, 40);
            this.btnStep_0.TabIndex = 227;
            this.btnStep_0.Text = "到上料位(初始化)";
            this.btnStep_0.UseVisualStyleBackColor = false;
            this.btnStep_0.Click += new System.EventHandler(this.ChangeTypeAA);
            // 
            // GoDisp
            // 
            this.GoDisp.Location = new System.Drawing.Point(372, 177);
            this.GoDisp.Name = "GoDisp";
            this.GoDisp.Size = new System.Drawing.Size(75, 50);
            this.GoDisp.TabIndex = 224;
            this.GoDisp.Text = "点胶测试";
            this.GoDisp.UseVisualStyleBackColor = true;
            this.GoDisp.Click += new System.EventHandler(this.GoDisp_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(545, 768);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(0, 12);
            this.label15.TabIndex = 217;
            // 
            // chartData1
            // 
            chartArea1.Name = "ChartArea1";
            this.chartData1.ChartAreas.Add(chartArea1);
            this.chartData1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartData1.Legends.Add(legend1);
            this.chartData1.Location = new System.Drawing.Point(3, 3);
            this.chartData1.Name = "chartData1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartData1.Series.Add(series1);
            this.chartData1.Size = new System.Drawing.Size(519, 368);
            this.chartData1.TabIndex = 221;
            this.chartData1.Text = "chart1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(533, 400);
            this.tabControl1.TabIndex = 222;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chartData1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(525, 374);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chartData2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(525, 374);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chartData2
            // 
            chartArea2.Name = "ChartArea1";
            this.chartData2.ChartAreas.Add(chartArea2);
            this.chartData2.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.chartData2.Legends.Add(legend2);
            this.chartData2.Location = new System.Drawing.Point(3, 3);
            this.chartData2.Name = "chartData2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartData2.Series.Add(series2);
            this.chartData2.Size = new System.Drawing.Size(519, 368);
            this.chartData2.TabIndex = 222;
            this.chartData2.Text = "chart2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Open_UV);
            this.groupBox1.Controls.Add(this.ShowResult);
            this.groupBox1.Controls.Add(this.OpenCKPower);
            this.groupBox1.Controls.Add(this.CloseCKPower);
            this.groupBox1.Controls.Add(this.GetCurrent);
            this.groupBox1.Controls.Add(this.ChannelChoose);
            this.groupBox1.Location = new System.Drawing.Point(12, 739);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(522, 146);
            this.groupBox1.TabIndex = 223;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "其他操作";
            // 
            // Open_UV
            // 
            this.Open_UV.Location = new System.Drawing.Point(390, 29);
            this.Open_UV.Name = "Open_UV";
            this.Open_UV.Size = new System.Drawing.Size(78, 50);
            this.Open_UV.TabIndex = 199;
            this.Open_UV.Text = "打开UV";
            this.Open_UV.UseVisualStyleBackColor = true;
            this.Open_UV.Click += new System.EventHandler(this.Open_UV_Click);
            // 
            // ShowResult
            // 
            this.ShowResult.Location = new System.Drawing.Point(304, 101);
            this.ShowResult.Name = "ShowResult";
            this.ShowResult.Size = new System.Drawing.Size(175, 21);
            this.ShowResult.TabIndex = 198;
            // 
            // ManualStepDisp
            // 
            this.ManualStepDisp.Controls.Add(this.button1);
            this.ManualStepDisp.Controls.Add(this.label1);
            this.ManualStepDisp.Controls.Add(this.RunDispStep);
            this.ManualStepDisp.Controls.Add(this.groupBox6);
            this.ManualStepDisp.Controls.Add(this.comboBox_SelCamera);
            this.ManualStepDisp.Controls.Add(this.GoDisp);
            this.ManualStepDisp.Location = new System.Drawing.Point(556, 412);
            this.ManualStepDisp.Name = "ManualStepDisp";
            this.ManualStepDisp.Size = new System.Drawing.Size(542, 393);
            this.ManualStepDisp.TabIndex = 224;
            this.ManualStepDisp.TabStop = false;
            this.ManualStepDisp.Text = "点胶调试";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(256, 177);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 50);
            this.button1.TabIndex = 228;
            this.button1.Text = "校准测试";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(370, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 227;
            this.label1.Text = "相机选择";
            // 
            // RunDispStep
            // 
            this.RunDispStep.Location = new System.Drawing.Point(256, 56);
            this.RunDispStep.Name = "RunDispStep";
            this.RunDispStep.Size = new System.Drawing.Size(75, 50);
            this.RunDispStep.TabIndex = 226;
            this.RunDispStep.Text = "单步执行";
            this.RunDispStep.UseVisualStyleBackColor = true;
            this.RunDispStep.Click += new System.EventHandler(this.RunDispStep_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnDisp_7);
            this.groupBox6.Controls.Add(this.btnDisp_6);
            this.groupBox6.Controls.Add(this.btnDisp_0);
            this.groupBox6.Controls.Add(this.btnDisp_5);
            this.groupBox6.Controls.Add(this.btnDisp_1);
            this.groupBox6.Controls.Add(this.btnDisp_2);
            this.groupBox6.Controls.Add(this.btnDisp_4);
            this.groupBox6.Controls.Add(this.btnDisp_3);
            this.groupBox6.Location = new System.Drawing.Point(16, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(214, 381);
            this.groupBox6.TabIndex = 225;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "针头校正流程";
            // 
            // btnDisp_7
            // 
            this.btnDisp_7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisp_7.BackColor = System.Drawing.Color.Cyan;
            this.btnDisp_7.FlatAppearance.BorderSize = 0;
            this.btnDisp_7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnDisp_7.Location = new System.Drawing.Point(0, 300);
            this.btnDisp_7.Margin = new System.Windows.Forms.Padding(0);
            this.btnDisp_7.Name = "btnDisp_7";
            this.btnDisp_7.Size = new System.Drawing.Size(214, 40);
            this.btnDisp_7.TabIndex = 241;
            this.btnDisp_7.Text = "结束退出";
            this.btnDisp_7.UseVisualStyleBackColor = false;
            this.btnDisp_7.Click += new System.EventHandler(this.ChangeTypeDisp);
            // 
            // btnDisp_6
            // 
            this.btnDisp_6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisp_6.BackColor = System.Drawing.Color.Cyan;
            this.btnDisp_6.FlatAppearance.BorderSize = 0;
            this.btnDisp_6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnDisp_6.Location = new System.Drawing.Point(0, 260);
            this.btnDisp_6.Margin = new System.Windows.Forms.Padding(0);
            this.btnDisp_6.Name = "btnDisp_6";
            this.btnDisp_6.Size = new System.Drawing.Size(214, 40);
            this.btnDisp_6.TabIndex = 243;
            this.btnDisp_6.Text = "9点标定";
            this.btnDisp_6.UseVisualStyleBackColor = false;
            this.btnDisp_6.Click += new System.EventHandler(this.ChangeTypeDisp);
            // 
            // btnDisp_0
            // 
            this.btnDisp_0.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisp_0.BackColor = System.Drawing.Color.Cyan;
            this.btnDisp_0.FlatAppearance.BorderSize = 0;
            this.btnDisp_0.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnDisp_0.Location = new System.Drawing.Point(0, 20);
            this.btnDisp_0.Margin = new System.Windows.Forms.Padding(0);
            this.btnDisp_0.Name = "btnDisp_0";
            this.btnDisp_0.Size = new System.Drawing.Size(214, 40);
            this.btnDisp_0.TabIndex = 236;
            this.btnDisp_0.Text = "转盘到上料位置";
            this.btnDisp_0.UseVisualStyleBackColor = false;
            this.btnDisp_0.Click += new System.EventHandler(this.ChangeTypeDisp);
            // 
            // btnDisp_5
            // 
            this.btnDisp_5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisp_5.BackColor = System.Drawing.Color.Cyan;
            this.btnDisp_5.FlatAppearance.BorderSize = 0;
            this.btnDisp_5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnDisp_5.Location = new System.Drawing.Point(0, 220);
            this.btnDisp_5.Margin = new System.Windows.Forms.Padding(0);
            this.btnDisp_5.Name = "btnDisp_5";
            this.btnDisp_5.Size = new System.Drawing.Size(214, 40);
            this.btnDisp_5.TabIndex = 242;
            this.btnDisp_5.Text = "相机到产品拍照位";
            this.btnDisp_5.UseVisualStyleBackColor = false;
            this.btnDisp_5.Click += new System.EventHandler(this.ChangeTypeDisp);
            // 
            // btnDisp_1
            // 
            this.btnDisp_1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisp_1.BackColor = System.Drawing.Color.Cyan;
            this.btnDisp_1.FlatAppearance.BorderSize = 0;
            this.btnDisp_1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnDisp_1.Location = new System.Drawing.Point(0, 60);
            this.btnDisp_1.Margin = new System.Windows.Forms.Padding(0);
            this.btnDisp_1.Name = "btnDisp_1";
            this.btnDisp_1.Size = new System.Drawing.Size(214, 40);
            this.btnDisp_1.TabIndex = 237;
            this.btnDisp_1.Text = "针头到点胶校正位置";
            this.btnDisp_1.UseVisualStyleBackColor = false;
            this.btnDisp_1.Click += new System.EventHandler(this.ChangeTypeDisp);
            // 
            // btnDisp_2
            // 
            this.btnDisp_2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisp_2.BackColor = System.Drawing.Color.Cyan;
            this.btnDisp_2.FlatAppearance.BorderSize = 0;
            this.btnDisp_2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnDisp_2.Location = new System.Drawing.Point(0, 100);
            this.btnDisp_2.Margin = new System.Windows.Forms.Padding(0);
            this.btnDisp_2.Name = "btnDisp_2";
            this.btnDisp_2.Size = new System.Drawing.Size(214, 40);
            this.btnDisp_2.TabIndex = 238;
            this.btnDisp_2.Text = "点胶";
            this.btnDisp_2.UseVisualStyleBackColor = false;
            this.btnDisp_2.Click += new System.EventHandler(this.ChangeTypeDisp);
            // 
            // btnDisp_4
            // 
            this.btnDisp_4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisp_4.BackColor = System.Drawing.Color.Cyan;
            this.btnDisp_4.FlatAppearance.BorderSize = 0;
            this.btnDisp_4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnDisp_4.Location = new System.Drawing.Point(0, 180);
            this.btnDisp_4.Margin = new System.Windows.Forms.Padding(0);
            this.btnDisp_4.Name = "btnDisp_4";
            this.btnDisp_4.Size = new System.Drawing.Size(214, 40);
            this.btnDisp_4.TabIndex = 240;
            this.btnDisp_4.Text = "计算Offset";
            this.btnDisp_4.UseVisualStyleBackColor = false;
            this.btnDisp_4.Click += new System.EventHandler(this.ChangeTypeDisp);
            // 
            // btnDisp_3
            // 
            this.btnDisp_3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisp_3.BackColor = System.Drawing.Color.Cyan;
            this.btnDisp_3.FlatAppearance.BorderSize = 0;
            this.btnDisp_3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.btnDisp_3.Location = new System.Drawing.Point(0, 140);
            this.btnDisp_3.Margin = new System.Windows.Forms.Padding(0);
            this.btnDisp_3.Name = "btnDisp_3";
            this.btnDisp_3.Size = new System.Drawing.Size(214, 40);
            this.btnDisp_3.TabIndex = 239;
            this.btnDisp_3.Text = "CCD到校正位置";
            this.btnDisp_3.UseVisualStyleBackColor = false;
            this.btnDisp_3.Click += new System.EventHandler(this.ChangeTypeDisp);
            // 
            // Group
            // 
            this.Group.Controls.Add(this.buttonTest);
            this.Group.Controls.Add(this.btnEnumAllCam);
            this.Group.Controls.Add(this.cbxAllUSB);
            this.Group.Location = new System.Drawing.Point(562, 811);
            this.Group.Name = "Group";
            this.Group.Size = new System.Drawing.Size(536, 100);
            this.Group.TabIndex = 225;
            this.Group.TabStop = false;
            this.Group.Text = "校准调试";
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(478, 17);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(42, 23);
            this.buttonTest.TabIndex = 2;
            this.buttonTest.Text = "测试";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // btnEnumAllCam
            // 
            this.btnEnumAllCam.Location = new System.Drawing.Point(411, 17);
            this.btnEnumAllCam.Name = "btnEnumAllCam";
            this.btnEnumAllCam.Size = new System.Drawing.Size(61, 23);
            this.btnEnumAllCam.TabIndex = 1;
            this.btnEnumAllCam.Text = "枚举USB";
            this.btnEnumAllCam.UseVisualStyleBackColor = true;
            this.btnEnumAllCam.Click += new System.EventHandler(this.btnEnumAllCam_Click);
            // 
            // cbxAllUSB
            // 
            this.cbxAllUSB.FormattingEnabled = true;
            this.cbxAllUSB.Location = new System.Drawing.Point(10, 20);
            this.cbxAllUSB.Name = "cbxAllUSB";
            this.cbxAllUSB.Size = new System.Drawing.Size(395, 20);
            this.cbxAllUSB.TabIndex = 0;
            // 
            // Form_Manual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 897);
            this.Controls.Add(this.Group);
            this.Controls.Add(this.ManualStepDisp);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.ManualStepAA);
            this.Controls.Add(this.visionControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_Manual";
            this.Text = "Form_Manual";
            this.Load += new System.EventHandler(this.Form_Manual_Load);
            this.VisibleChanged += new System.EventHandler(this.OnVisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.visionControl1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ManualStepAA.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.AAGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartData1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartData2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ManualStepDisp.ResumeLayout(false);
            this.ManualStepDisp.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.Group.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private UserCtrl.VisionControl visionControl1;
        private System.Windows.Forms.ComboBox comboBox_SelCamera;
        private System.Windows.Forms.Button OpenCKPower;
        private System.Windows.Forms.Button CloseCKPower;
        private System.Windows.Forms.ComboBox ChannelChoose;
        private System.Windows.Forms.Button RunOneStep;
        private System.Windows.Forms.Button GetCurrent;
        private System.Windows.Forms.GroupBox ManualStepAA;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button GoDisp;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartData1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartData2;
        private System.Windows.Forms.Button TestCompete;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox AAGroupBox;
        private System.Windows.Forms.Button btnStep_3;
        private System.Windows.Forms.Button btnStep_2;
        private System.Windows.Forms.Button btnStep_1;
        private System.Windows.Forms.Button btnStep_0;
        private System.Windows.Forms.Button btnStep_7;
        private System.Windows.Forms.Button btnStep_4;
        private System.Windows.Forms.Button btnStep_6;
        private System.Windows.Forms.Button btnStep_5;
        private System.Windows.Forms.GroupBox ManualStepDisp;
        private System.Windows.Forms.RadioButton radioBtnA;
        private System.Windows.Forms.RadioButton radioBtnB;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBoxB;
        private System.Windows.Forms.CheckBox checkBoxA;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button TableRunManual;
        private System.Windows.Forms.RadioButton radioBtnAve;
        private System.Windows.Forms.RadioButton radioBtnPreak;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnDisp_4;
        private System.Windows.Forms.Button btnDisp_3;
        private System.Windows.Forms.Button btnDisp_2;
        private System.Windows.Forms.Button btnDisp_1;
        private System.Windows.Forms.Button btnDisp_0;
        private System.Windows.Forms.Button btnDisp_7;
        private System.Windows.Forms.Button RunDispStep;
        private System.Windows.Forms.Button MFTest;
        private System.Windows.Forms.TextBox ShowResult;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 单次采集ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 连续采集ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存原图ToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnDisp_6;
        private System.Windows.Forms.Button btnDisp_5;
        private System.Windows.Forms.Button Open_UV;
        private System.Windows.Forms.GroupBox Group;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.Button btnEnumAllCam;
        private System.Windows.Forms.ComboBox cbxAllUSB;
    }
}