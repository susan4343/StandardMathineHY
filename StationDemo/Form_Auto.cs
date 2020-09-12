using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using HalconDotNet;
//using HalconLib;
using CameraLib;
using CommonTools;
using UserCtrl;
using MotionIoLib;
using Communicate;
using System.Threading.Tasks;
using System.Diagnostics;
using BaseDll;
using UserData;
using log4net;
using System.IO;
using LightControler;
using System.Windows.Forms.DataVisualization.Charting;

using OtherDevice;
using AlgorithmNamespace;
using System.Drawing.Imaging;
using ModuleCapture;


namespace StationDemo
{
    public partial class Form_Auto : Form
    {
        #region 变量
        public delegate void ShowImageDelegate(Bitmap bitmap, string strSaveImgpath = "", bool showRoi = false, SFRValue sFR = null, RectInfo objectInfo = null, Rectangle[] rectangles = null, LightValue lightValue = null);
        public static ShowImageDelegate EvenShowImageDelegate;
        public delegate void ShowChartDelegate(List<SFRValue> SfrInfoArr, int tf, double zPos = 0, double dstep = 0.01, string SaveChartImagePath = "");
        public static ShowChartDelegate EvenShowChartDelegate;
        public delegate void ResetDelegate();
        public static ResetDelegate EvenReset;
        public delegate void StopDelegate();
        public static StopDelegate EvenStop;
        public delegate void ShowCTDelegate(int AB);
        public static ShowCTDelegate EvenShowCT;
        public delegate void GetSNdelegate(int AB);
        public static GetSNdelegate EvenGetSN;
        List<string> m_listFlag = new List<string>();
        List<string> m_listInt = new List<string>();
        List<string> m_listDouble = new List<string>();
        TabPage tabPage1 = new TabPage();
        TabPage tabPage2 = new TabPage();
        Chart ThroughFocusChart1 = new Chart();
        Chart ThroughFocusChart2 = new Chart();
        object objlock = new object();
        const int nCount = 2000;
        public static bool PlayA = true;
        public static bool PlayB = true;
        public static bool PlayC = true;
        static bool showRoi = false;
        public static Bitmap ImageSave = null;
        Form_ShowImage form3 = new Form_ShowImage();
        public static bool IsHome = true;
        #endregion

        #region 事件
        private void EvenAdd()
        {
            GlobalVariable.g_eventStationStateChanged += StationStateChangedHandler;
            ParamSetMgr.GetInstance().m_eventChangedBoolSysVal += Form_Auto_m_eventChangedBoolSysVal;
            ParamSetMgr.GetInstance().m_eventChangedDoubleSysVal += Form_Auto_m_eventChangedDoubleSysVal;
            ParamSetMgr.GetInstance().m_eventLoadProductFile += LoadProductFile;
            EvenReset += ResetAll;
            EvenStop += StopAll;
            EvenShowImageDelegate += UpdateParaImgChart;
            EvenShowChartDelegate += ShowThroughFocus;
            EvenGetSN += GetSN;
            EvenShowCT += ShowCT;
        }
        #endregion

        #region 控件
        public Form_Auto()
        {
            InitializeComponent();
        }
        private void Form_Auto_Load(object sender, EventArgs e)
        {
            #region 初始化变量
            IOMgr.GetInstace().WriteIoBit("启动按钮灯", false);
            ParamSetMgr.GetInstance().SetBoolParam("启用安全门", true);
            ParamSetMgr.GetInstance().SetBoolParam("启用安全光栅", true);
            ParamSetMgr.GetInstance().SetIntParam("等待启动信号", 60000);
            ParamSetMgr.GetInstance().SetBoolParam("AA工站初始化完成", false);
            ParamSetMgr.GetInstance().SetBoolParam("点胶工站初始化完成", false);
            #endregion

            #region 变量定义
            UserTest.ProductCount = new ProductInfo();
            int num = 0;
            List<string> SN = new List<string>();
            string currentProductFile = ParamSetMgr.GetInstance().CurrentWorkDir + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + (".xml");
            string mfPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\MF.xml";
            string ctPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\ProductInfo.xml";
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}NST_ActiveAlignment.ini";
            string AlgorithType = ParamSetMgr.GetInstance().GetStringParam("算法类型");
            string DeviceType = ParamSetMgr.GetInstance().GetStringParam("工位点亮类型");
            string CalibType = ParamSetMgr.GetInstance().GetStringParam("校准相机点亮类型");
            string playPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\Play\\play.ini";
            #endregion

            #region 初始化参数配置
            EvenAdd();
            //保存加载参数配置
            ParamSetMgr.GetInstance().SaveParam(currentProductFile);
            //读取良率信息显示
            if (!ProductInfoFile.ReadCT(ctPath))
            {
                MessageBox.Show("序列化CT参数失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            ShowCT(0);
            //检测加载MF配置
            if (File.Exists(mfPath))
            {
                MFInfoFile.Read(mfPath);//检查
            }
            NoSN.Checked = ParamSetMgr.GetInstance().GetBoolParam("是否允许空SN");
            //清楚图片中空文件夹
            Task.Run(() => { ImageHelper.DeleteDir(PathHelper.ImagePathDelete); });
            //默认用户登陆
            // sys.g_User = sys.g_listUser.Find(t => t._userName == "admin");
            #endregion

            #region 初始化界面加载
            label_CurrentFile.Text = "当前产品:" + ParamSetMgr.GetInstance().CurrentProductFile;
            this.txt_Batch.Text = $"{ParamSetMgr.GetInstance().GetStringParam("批次")}";
            //Log界面加载
            tabControl_Log.Controls.Clear();
            UserTest.RunLog = new LogHelper();
            TabPage tabPage = new TabPage();
            tabPage.Text = "Log";
            UserTest.RunLog.Size = new Size(tabControl_Log.Size.Width - 20, tabControl_Log.Size.Height - 20);
            tabPage.Controls.Add((Control)UserTest.RunLog);
            tabControl_Log.Controls.Add(tabPage);
            foreach (var tem in StationMgr.GetInstance().GetAllStationName())
            {
                ListLog listLog = new ListLog();
                listLog.HorizontalScrollbar = true;
                listLog.Size = new Size(tabControl1.Size.Width - 20, tabControl1.Size.Height - 20);
                listLog.ItemHeight = 40;
                TabPage tabStaion = new TabPage();
                tabStaion.Name = tem;
                tabStaion.Text = tem;
                tabStaion.Controls.Add((Control)listLog);
                tabControl_Log.TabPages.Add(tabStaion);
                StationMgr.GetInstance().GetStation(tem).SetShowListBox(listLog);
                StationMgr.GetInstance().GetStation(tem).m_eventListBoxShow += ShowStationMsg;
                listLog.Dock = DockStyle.Fill;
                StationMgr.GetInstance().GetStation(tem).Err(tem + "加载成功");
            }
            UserTest.RunLog.Write($"加载软件OK", LogType.Info, PathHelper.LogPathManual);
            tabControl_Log.SelectedIndex = 0;
            MachineStateEmg.Name = "急停";
            MachineStateStop.Name = "停止";
            MachineStateAuto.Name = "自动";
            MachineStatePause.Name = "暂停";
            //添加 ------- 标志--------///
            userPanel_Flag.Visible = false;
            UserConfig.AddFlag(this);
            if (m_listFlag.Count > 0)
                userPanel_Flag.Visible = true;
            userPanel_Flag.Update();


            #endregion

            #region 初始化算法、硬件


            //初始化算法参数
            AlgorithmMgr.Instance.AddAlgorithm(0, AlgorithType);
            AlgorithmMgr.Instance.AddAlgorithm(1, AlgorithType);
            AlgorithmMgr.Instance.LoadConfig(path);
            //初始化度信盒
            if (AlgorithType == "Collimator")
            {
                IOMgr.GetInstace().WriteIoBit($"平行光管光源", true);
            }
            ModuleMgr.Instance.AddModule(DeviceType, 0);
            ModuleMgr.Instance.AddModule(CalibType, 1);
            string playDllPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\Play\\CliDll.dll";

            ModuleMgr.Instance.LoadDll(playDllPath);
            ModuleMgr.Instance.Enumerate(0, ref num, SN);
            ModuleMgr.Instance.Enumerate(1, ref num, SN);

            int SetSNA = ParamSetMgr.GetInstance().GetIntParam("A工位SetSN_ID");
            int SetSNB = ParamSetMgr.GetInstance().GetIntParam("B工位SetSN_ID");
            ModuleMgr.Instance.SetSN(0, SetSNA);
            ModuleMgr.Instance.SetSN(1, SetSNB);
            if (!ModuleMgr.Instance.Init(0, playPath))
            {
                MessageBox.Show("加载A工位点亮文件失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (!ModuleMgr.Instance.Init(1, playPath))
            {
                MessageBox.Show("加载B工位点亮文件失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            ////添加 添加相机
            if (!ParamSetMgr.GetInstance().GetBoolParam("屏蔽上相机"))
            {
                visionControl1.InitWindow();
                UserConfig.BandStationWithCtrl(this);
                List<CameraBase> listCam = new List<CameraBase>();
                CameraMgr.GetInstance().EnumDevices(new BaslerCamera("sd"), out listCam);
                if (listCam != null)
                {
                    foreach (var key in listCam)
                    {
                        CameraMgr.GetInstance().AddCamera(key.Name, key);
                        if (key.Name == "Top")
                            key.BindWindow(visionControl1);
                        Thread.Sleep(30);
                        key.Open();
                        key.SetExposureTime(20000);
                        //key.SetAcquisitionMode();
                        key.RegisterCallBack(0);
                        Thread.Sleep(30);
                        key.StartGrab();
                    }
                    foreach (var key in listCam)
                    {
                        key.SetAcquisitionMode();
                    }
                }
                HOperatorSet.SetDraw(visionControl1.GetHalconWindow(), "margin");
            }
            //初始化程控电源
            string err = "";
            //IOMgr.GetInstace().WriteIoBit($"A模组上电", true);
            //IOMgr.GetInstace().WriteIoBit($"B模组上电", true);
            if (!OtherDevices.ckPower.Init(ref err))
            {
                MessageBox.Show(err, "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            //OtherDevices.ckPower.SetAllCKPowerOn();
            //double valueVoltage = ParamSetMgr.GetInstance().GetDoubleParam("程控电源电压");
            //OtherDevices.ckPower.SetVoltage(1, valueVoltage);
            //OtherDevices.ckPower.SetVoltage(2, valueVoltage);
            //double valueCurrent = ParamSetMgr.GetInstance().GetDoubleParam("程控电源电流");
            //OtherDevices.ckPower.SetCurrent(1, valueCurrent);
            //OtherDevices.ckPower.SetCurrent(2, valueCurrent);
            Thread.Sleep(5000);
            //检测其他硬件状态
            UserConfig.InitHardWare();
            Task task = new Task(() =>
            {
                UserConfig.InitHandWareWithUI();
            });
            task.Start();
            UserConfig.InitProductNum();

            GlobalVariable.g_eventStationStateChanged += (StationState stationstate) =>
            {

                if (stationstate == StationState.StationStateRun)
                {
                    GlobelManualResetEvent.ContinueShowA.Reset();
                    GlobelManualResetEvent.ContinueShowB.Reset();
                    GlobelManualResetEvent.ContinueShowC.Reset();
                }

            };
            UserConfig.InitSysFun();
            #endregion
        }
        private void Form_Auto_m_eventChangedDoubleSysVal(string key, double val)
        {

            //if (InvokeRequired)
            //{
            //    this.BeginInvoke(new Action(() => Form_Auto_m_eventChangedDoubleSysVal(key, val)));
            //}
            //else
            //{
            //    int index = m_listDouble.FindIndex(t => t == key);
            //    if (index != -1)
            //    {
            //        double dval = 0;
            //        if (index < dataGridView_Sum.Rows.Count)
            //        {
            //            dataGridView_Sum.Rows[index].Cells[1].Value = val.ToString();
            //        }
            //    }
            //}
        }
        private void Form_Auto_m_eventChangedBoolSysVal(string key, bool val)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Form_Auto_m_eventChangedBoolSysVal(key, val)));
            }
            else
            {
                int index = m_listFlag.FindIndex(t => t == key);
                if (index != -1)
                {
                    if (index < m_listFlag.Count)
                    {
                        userPanel_Flag.SetLebalState(key, val);
                        if (val)
                        {

                            // dataGridView_Flag.Rows[index].Cells[1].Value = "ON";
                            //  dataGridView_Flag.Rows[index].Cells[1].Style.BackColor = Color.LightGreen;
                        }
                        else
                        {
                            //  dataGridView_Flag.Rows[index].Cells[1].Value = "OFF";
                            //  dataGridView_Flag.Rows[index].Cells[1].Style.BackColor = Color.LightBlue;

                        }

                    }
                }

            }

        }
        private void CloseMainForm(object sender, FormClosedEventArgs e)
        {
            GlobelManualResetEvent.ContinueShowB.Set();
            GlobelManualResetEvent.ContinueShowA.Set();
            GlobelManualResetEvent.AutoPlay.Set();
        }
        private void BtnReset_Click(object sender, EventArgs e)
        {


        }
        private void BtnClearAllProduct_Click(object sender, EventArgs e)
        {
            UserTest.RunLog.Write($"点击【清料】", LogType.Info, PathHelper.LogPathManual);
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许清料！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            StationDisp dospT = (StationDisp)StationMgr.GetInstance().GetStation("点胶站");
            StationAA stationAAT = (StationAA)StationMgr.GetInstance().GetStation("AA站");
            StationTable stationTableTT = (StationTable)StationMgr.GetInstance().GetStation("转盘站");
            int SocketNumOfUnloadLoad = TableData.GetInstance().GetSocketNum(1, 0.5) - 1;
            string Soket = SocketNumOfUnloadLoad == 0 ? "A" : "B";
            if (!IOMgr.GetInstace().ReadIoInBit("急停"))
            {
                MessageBox.Show("设备急停", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (!IOMgr.GetInstace().ReadIoInBit("气源检测"))
            {
                MessageBox.Show("气源检测异常", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            //if (!IOMgr.GetInstace().ReadIoInBit($"{Soket}治具盖上检测"))
            if (!SysFunConfig.LodUnloadPatten.IsSafeWhenURun(Soket))
            {
                MessageBox.Show($"{Soket}治具盖上没有盖上", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisX) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisY) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisZ) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("点胶站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisX) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisY) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisZ) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("AA站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (MotionMgr.GetInstace().GetHomeFinishFlag(stationTableTT.AxisU) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("转盘站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            stationAAT.GoAAReadySafe(true);//AA z轴到安全位置
            dospT.GoSanpReadySafe(true);   //点胶 z轴到安全位置

            stationTableTT.Urun($"{Soket}工位AA位", true);
            SocketNumOfUnloadLoad = TableData.GetInstance().GetSocketNum(1, 0.5) - 1;//获取当前位置的夹具号
            //刷新数据
            if (!UserTest.CTTestAB[SocketNumOfUnloadLoad].Star && UserTest.CTTestAB[SocketNumOfUnloadLoad].End && !UserTest.CTTestAB[SocketNumOfUnloadLoad].Show)
            {
                UserTest.TestResultAB[SocketNumOfUnloadLoad].SocketerNumber = Soket;
                UserTest.TestResultAB[SocketNumOfUnloadLoad].EndTime = DateTime.Now;
                UserTest.TestResultAB[SocketNumOfUnloadLoad].TestTime = (UserTest.TestResultAB[SocketNumOfUnloadLoad].EndTime - UserTest.TestResultAB[SocketNumOfUnloadLoad].StarTime).TotalSeconds;
                string errCsv = CSVHelper.Instance.SaveToCSVPath(PathHelper.TestResultCsvPath, UserTest.TestResultAB[SocketNumOfUnloadLoad]);
                IOMgr.GetInstace().WriteIoBit("OK指示绿灯", false);
                IOMgr.GetInstace().WriteIoBit("NG指示红灯", false);
                SocketState state = SocketMgr.GetInstance().socketArr[SocketNumOfUnloadLoad].socketState;
                string lightColor = state == SocketState.HaveOK ? "OK指示绿灯" : "NG指示红灯";
                IOMgr.GetInstace().WriteIoBit(lightColor, true);
                if (SocketNumOfUnloadLoad == 0)
                {
                    if (state == SocketState.HaveOK)
                        UserTest.ProductCount.OKA++;
                    else
                    {
                        UserTest.ProductCount.NGA++;
                    }
                }
                else
                {
                    if (state == SocketState.HaveOK)
                        UserTest.ProductCount.OKB++;
                    else
                    {
                        UserTest.ProductCount.NGB++;
                    }
                }
                UserTest.CTTestAB[SocketNumOfUnloadLoad].Star = false;
                UserTest.CTTestAB[SocketNumOfUnloadLoad].End = false;
                UserTest.CTTestAB[SocketNumOfUnloadLoad].Show = true;
                Form_Auto.EvenShowCT(SocketNumOfUnloadLoad + 1);
            }


        }
        private void ClearCT_Click(object sender, EventArgs e)
        {
            string ctPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\ProductInfo.xml";
            UserTest.ProductCount = new ProductInfo();
            ProductInfoFile.SaveCT(ctPath);
            Thread.Sleep(100);
            ShowCT(0);
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void BtnClose_Click(object sender, EventArgs e)
        {
            IOMgr.GetInstace().m_deltgateSystemSingl("复位", true);
            AlarmMgr.GetIntance().StopAlarmBeet();
        }
        private void MenuItem_PlayA_Click(object sender, EventArgs e)
        {
            RunA();
        }
        private void MenuItem_PlayB_Click(object sender, EventArgs e)
        {
            RunB();
        }
        private void MenuItem_Stop_Click(object sender, EventArgs e)
        {
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许手动点亮！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (!form3.IsDisposed)
            {
                form3.Dispose();
            }
            if (!PlayA)
            {
                ModuleMgr.Instance.Stop(0);
            }
            if (!PlayB)
            {
                ModuleMgr.Instance.Stop(1);
            }
            PlayB = true;
            PlayA = true;
            PlayC = true;
            Thread.Sleep(300);
            Form_Auto.EvenShowImageDelegate(null);
        }
        private void MenuItem_SaveImageResult_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "结果图片另存";
                saveFileDialog.Filter = "Color BMP Files (*.bmp)|*.bmp|Jpeg Files (*.jpg)|*.jpg|Color TIF Files (*.tif)|*.tif";
                saveFileDialog.FilterIndex = 1;//设置默认文件类型显示顺序 
                saveFileDialog.RestoreDirectory = true; //点了保存按钮进入if (picBox1.Image != null)
                saveFileDialog.FileName = "Image";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp;
                    if (PictureShow.Image != null)
                    {
                        bmp = (Bitmap)PictureShow.Image.Clone();
                        //保存到磁盘文件
                        bmp.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                        bmp.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存结果图片失败，{ex}", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        private void MenuItem_SaveImageBMP_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "原图另存";
                saveFileDialog.Filter = "Color BMP Files (*.bmp)|*.bmp|Jpeg Files (*.jpg)|*.jpg|Color TIF Files (*.tif)|*.tif";
                saveFileDialog.FilterIndex = 1;//设置默认文件类型显示顺序 
                saveFileDialog.RestoreDirectory = true; //点了保存按钮进入if (picBox1.Image != null)
                saveFileDialog.FileName = "Image";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp;
                    if (ImageSave != null)
                    {
                        bmp = (Bitmap)ImageSave.Clone();
                        //保存到磁盘文件
                        bmp.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                        bmp.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存原图失败，{ex}", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        private void PictureShow_DoubleClick(object sender, EventArgs e)
        {

            if (form3.IsDisposed)
            {
                form3 = new Form_ShowImage();
            }
            form3.Dock = DockStyle.Fill;
            form3.TopMost = true;
            form3.Show();


        }
        private void MenuItem_ShowRoi_Click(object sender, EventArgs e)
        {
            if (showRoi)
                showRoi = false;
            else
                showRoi = true;
        }
        #endregion

        #region 方法
        public void UpdateParaImgChart(Bitmap img, string strSaveImgpath = "", bool showRoi = false, SFRValue SFR_Info = null, RectInfo rectInfo = null, Rectangle[] rectangles = null, LightValue lightValue = null)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => { UpdateParaImgChart(img, strSaveImgpath, showRoi, SFR_Info, rectInfo, rectangles, lightValue); }));
            }
            else
            {


                try
                {
                    if (img == null)
                    {
                        Bitmap NullImage = new Bitmap(PictureShow.Image.Size.Width, PictureShow.Image.Size.Width, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        PictureShow.Image = (Image)NullImage.Clone();
                        return;
                    }
                    double countCenter = 4; double countCorner1 = 4; double countCorner2 = 4; double countCross = 4;
                    //bool Center = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterTop");
                    //Center |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterRight");
                    //Center |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterLeft");
                    //Center |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterBottom");
                    //bool Corner1 = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Top");
                    //Corner1 |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Right");
                    //Corner1 |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Left");
                    //Corner1 |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Bottom");
                    //bool Corner2 = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Top");
                    //Corner2 |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Right");
                    //Corner2 |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Left");
                    //Corner2 |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Bottom");
                    //bool Cross = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossTop");
                    //Cross |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossRight");
                    //Cross |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossLeft");
                    //Cross |= ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossBottom");
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterTop"))
                        countCenter--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterRight"))
                        countCenter--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterLeft"))
                        countCenter--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterBottom"))
                        countCenter--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Top"))
                        countCorner1--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Right"))
                        countCorner1--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Left"))
                        countCorner1--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Bottom"))
                        countCorner1--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Top"))
                        countCorner2--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Right"))
                        countCorner2--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Left"))
                        countCorner2--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Bottom"))
                        countCorner2--;

                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossTop"))
                        countCross--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossRight"))
                        countCross--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossLeft"))
                        countCross--;
                    if (!ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossBottom"))
                        countCross--;
                    int bigCenterX = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCenterROIW");
                    int bigCenterY = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCenterROIH");
                    int bigCorner1X = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCorner1ROIW");
                    int bigCorner1Y = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCorner1ROIH");
                    double xfieldCorner1 = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCorner1XField");
                    double yfieldCorner1 = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCorner1YField");
                    int bigCorner2X = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCorner2ROIW");
                    int bigCorner2Y = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCorner2ROIH");
                    double xfieldCorner2 = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCorner2XField");
                    double yfieldCorner2 = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCorner2YField");
                    int bigCrossX = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCrossROIW");
                    int bigCrossY = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCrossROIH");
                    double xfieldCross = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCrossXField");
                    double yfieldCross = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCrossYField");
                    // rectInfo 排序U L D R
                    Bitmap bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(img, new Point(0, 0));
                        // 画十字线
                        Pen pen = new Pen(Color.Blue, 3);
                        if (showRoi)
                        {
                            g.DrawLine(pen, 0, bmp.Height / 2, bmp.Width, bmp.Height / 2);
                            g.DrawLine(pen, bmp.Width / 2, 0, bmp.Width / 2, bmp.Height);
                        }

                        //画大Roi
                        //中心SFR_Info.block[0] 4边1 SFR_Info.block[1-4] 4边2 SFR_Info.block[5-8] corss SFR_Info.block[9-12]
                        if (lightValue == null)
                        {
                            lightValue = new LightValue();
                            double[] value = new double[] { -1, -1, -1, -1, -1 };
                            lightValue.blockValue = value;
                        }
                        else
                        {
                            if (lightValue.blockValue.Length < 5)
                            {
                                lightValue = new LightValue();
                                double[] value = new double[] { -1, -1, -1, -1, -1 };
                                lightValue.blockValue = value;
                            }
                        }
                        // for (int i=0;i< SFR_Info.block.Length;i++)
                        if (SFR_Info != null && rectInfo.Points != null)
                        {
                            if (countCenter > 0)
                            {
                                DrawRect(showRoi, bigCenterX, bigCenterY, g, pen, rectInfo.Points[0], SFR_Info.block[0], countCenter, lightValue.blockValue[0]);
                                DrawSet(showRoi, bigCenterX, bigCenterY, g, 0, 0, img.Width, img.Height);

                            }
                            if (countCorner1 > 0)
                            {
                                for (int i = 1; i < 5; i++)
                                {
                                    DrawRect(showRoi, bigCorner1X, bigCorner1Y, g, pen, rectInfo.Points[i], SFR_Info.block[i], countCorner1, lightValue.blockValue[i]);
                                }
                                DrawSet(showRoi, bigCorner1X, bigCorner1Y, g, xfieldCorner1, yfieldCorner1, img.Width, img.Height);
                            }
                            if (countCorner2 > 0)
                            {
                                for (int i = 5; i < 9; i++)
                                {
                                    DrawRect(showRoi, bigCorner2X, bigCorner2Y, g, pen, rectInfo.Points[i], SFR_Info.block[i], countCorner2);
                                }
                                DrawSet(showRoi, bigCorner2X, bigCorner2Y, g, xfieldCorner2, yfieldCorner2, img.Width, img.Height);
                            }
                            if (countCross > 0)
                            {
                                for (int i = 9; i < 13; i++)
                                {
                                    DrawRect(showRoi, bigCrossX, bigCrossY, g, pen, rectInfo.Points[i], SFR_Info.block[i], countCross);
                                }
                                DrawSet(showRoi, bigCrossX, bigCrossY, g, xfieldCross, yfieldCross, img.Width, img.Height);
                            }

                        }
                        if (showRoi)
                        {
                            //画设定的roi
                            //if (rectangles != null)
                            //{
                            //    Pen penA = new Pen(Color.Yellow, 6);
                            //    foreach (var a in rectangles)
                            //    {
                            //        g.DrawRectangle(penA, a);
                            //    }
                            //}
                            //else
                            //{
                            //    G.DrawRectangle(pen, new Rectangle { X = (int)img.Width/2, Y = (int)img.Height/2, Width = Lx, Height = Ly });
                            //}

                        }

                    }
                    PictureShow.Image = (Image)bmp.Clone();
                    Task.Run(() =>
                    {
                        if (strSaveImgpath != null && strSaveImgpath != "")
                            bmp.Save(strSaveImgpath, ImageFormat.Png);
                        strSaveImgpath = "";
                        if (!form3.IsDisposed)
                        {
                            form3.PictureShow.Image = (Image)bmp.Clone();
                        }
                    });


                }
                catch (Exception ex)
                {

                }
            }
        }
        public void ShowThroughFocus(List<SFRValue> SfrInfoArr, int tf, double zPos = 0, double dstep = 0.01, string SaveChartImagePath = "")
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ShowThroughFocus(SfrInfoArr, tf, zPos, dstep, SaveChartImagePath)));
            }
            else
            {
                if (SfrInfoArr == null)
                    return;


                int lenArr = SfrInfoArr.Count;

                Series[] series = new Series[5];
                string[] arrSeriesName = new string[] { "中间", "左上", "右上", "左下", "右下" };

                List<double> xVal = new List<double>();// { SfrInfoArr[0].dZ, SfrInfoArr[2].dZ, SfrInfoArr[4].dZ, SfrInfoArr[6].dZ, SfrInfoArr[8].dZ };

                double dStep = 0;
                double Zpos = 0;

                //indexsel = 2;
                //string strStepNum = $"第{indexsel}次Throughfocus步数";
                //string strExtent = $"第{indexsel}次Throughfocus范围";
                dStep = dstep;
                Zpos = Convert.ToDouble(zPos.ToString("0.000"));
                ChartArea chart = new ChartArea();
                if (tf == 1)
                {
                    tabControl1.SelectedIndex = 0;
                    chartData1.ChartAreas.Clear();
                    chartData1.Series.Clear();
                    chartData1.ChartAreas.Add(chart);
                    chartData1.ChartAreas[0].AxisX.Title = "Z Height";
                    chartData1.ChartAreas[0].AxisY.Title = "SFR";
                    chartData1.ChartAreas[0].AxisY.Interval = 20;
                    //  chartData1.ChartAreas[0].AxisY.Maximum = 140;
                    chartData1.ChartAreas[0].AxisX.Interval = dStep;

                }
                else
                {
                    tabControl1.SelectedIndex = 1;
                    chartData2.ChartAreas.Clear();
                    chartData2.Series.Clear();
                    chartData2.ChartAreas.Add(chart);
                    chartData2.ChartAreas[0].AxisX.Title = "Z Height";
                    chartData2.ChartAreas[0].AxisY.Title = "SFR";
                    chartData2.ChartAreas[0].AxisY.Interval = 20;
                    // chartData2.ChartAreas[0].AxisY.Maximum = 140;
                    chartData2.ChartAreas[0].AxisX.Interval = dStep;
                }




                List<double> yVal = new List<double>();// { SfrInfoArr[0].block, SfrInfoArr[2].dZ, SfrInfoArr[4].dZ, SfrInfoArr[6].dZ, SfrInfoArr[8].dZ };
                xVal.Clear();
                yVal.Clear();
                Color[] clor = new Color[] { Color.Red, Color.Blue, Color.Yellow, Color.Green, Color.Black };
                //五条曲线
                for (int i = 0; i < lenArr; i++)
                {
                    xVal.Add(Math.Round(Zpos + dStep * i, 3));
                    if (xVal.Count < lenArr)
                    {
                        continue;
                    }
                    for (int index = 0; index < series.Length; index++)
                    {
                        series[index] = new Series();
                        series[index].ChartType = SeriesChartType.Spline;
                        series[index].BorderWidth = 3;
                        series[index].Color = clor[index];
                        series[index].LegendText = arrSeriesName[index];
                        series[index].LegendToolTip = arrSeriesName[index];// 鼠标放到系列上出现的文字 

                        yVal.Clear();
                        //每条曲线的Y值
                        for (int j = 0; j < lenArr; j++)
                        {

                            yVal.Add(SfrInfoArr[j].block[1 * index].dValue);

                        }
                        series[index].Points.Clear();
                        series[index].Points.DataBindXY(xVal, yVal);
                        if (tf == 1)
                        {
                            chartData1.Series.Add(series[index]);
                        }
                        else
                        {
                            chartData2.Series.Add(series[index]);

                        }

                    }
                }
                if (SaveChartImagePath != "")
                {
                    if (tf == 1)
                    {
                        chartData1.SaveImage(SaveChartImagePath, ChartImageFormat.Bmp);
                    }
                    else
                    {
                        chartData2.SaveImage(SaveChartImagePath, ChartImageFormat.Bmp);
                    }
                }

                Legend mLegend = new Legend();
                mLegend.Docking = Docking.Right;
                //  chartData1 = chartData2;
            }
        }
        public async void ResetAll()
        {
            await Task.Run(() =>
            {
                IsHome = false;
                try
                {
                    if (GlobalVariable.g_StationState != StationState.StationStateStop)
                    {
                        MessageBox.Show("当前程序在运行，不允许手动复位！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }

                    //if (!IOMgr.GetInstace().ReadIoInBit("A治具盖上检测"))
                    if (!SysFunConfig.LodUnloadPatten.IsSafeWhenURun("A"))
                    {
                        MessageBox.Show("检查A治具盖上检测失败，不允许复位！", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    // if (!IOMgr.GetInstace().ReadIoInBit("B治具盖上检测"))
                    if (!SysFunConfig.LodUnloadPatten.IsSafeWhenURun("B"))
                    {
                        MessageBox.Show("检查B治具盖上检测失败，不允许复位！", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }

                    AlarmMgr.GetIntance().StopAlarmBeet();
                    StationMgr.GetInstance().Stop();
                    StationDisp dospT = (StationDisp)StationMgr.GetInstance().GetStation("点胶站");
                    StationAA stationAAT = (StationAA)StationMgr.GetInstance().GetStation("AA站");
                    StationTable stationTableTT = (StationTable)StationMgr.GetInstance().GetStation("转盘站");
                    if (ParamSetMgr.GetInstance().GetBoolParam("是否侧向UV"))
                        IOMgr.GetInstace().WriteIoBit("侧向UV气缸", false);

                    Task[] ResetTasks = new Task[2];
                    bool[] result = new bool[] { true, true };
                    ResetTasks[0] = Task.Run(() =>
                    {
                        result[0] = !dospT.GoSanpHome(true);
                        if (result[0])
                        {
                            MessageBox.Show("点胶回零失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            return;
                        }
                    });
                    ResetTasks[1] = Task.Run(() =>
                    {

                        Thread.Sleep(500);
                        if (ParamSetMgr.GetInstance().GetBoolParam("是否侧向UV") &&
                          (!IOMgr.GetInstace().ReadIoInBit("左侧UV原位") || !IOMgr.GetInstace().ReadIoInBit("左侧UV原位")))
                        {
                            if (GlobalVariable.g_StationState != StationState.StationStateRun)
                            {
                                MessageBox.Show("转盘复位前，左右UV是不在在原位， AA回零失败", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            return;
                        }
                        result[1] = !stationAAT.GoAAHome(true);
                        if (result[1])
                        {
                            MessageBox.Show("AA回零失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            return;
                        }
                    });
                    if (!Task.WaitAll(ResetTasks, 120000))
                    {
                        MessageBox.Show("AA和点胶超时", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    if (result[0] || result[1])
                    {
                        MessageBox.Show("AA和点胶回零失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;

                    }
                    if (!stationTableTT.GoTableHome(true))
                    {
                        MessageBox.Show("点胶回零失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    MessageBox.Show("所有轴回零完成", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (ParamSetMgr.GetInstance().GetBoolParam("是否选择程控电源"))
                    {
                        OtherDevices.ckPower.SetAllCKPowerOn();
                        double valueVoltage = ParamSetMgr.GetInstance().GetDoubleParam("程控电源电压");
                        OtherDevices.ckPower.SetVoltage(1, valueVoltage);
                        OtherDevices.ckPower.SetVoltage(2, valueVoltage);
                        double valueCurrent = ParamSetMgr.GetInstance().GetDoubleParam("程控电源电流");
                        OtherDevices.ckPower.SetCurrent(1, valueCurrent);
                        OtherDevices.ckPower.SetCurrent(2, valueCurrent);
                        IOMgr.GetInstace().WriteIoBit($"A模组上电", true);
                        IOMgr.GetInstace().WriteIoBit($"B模组上电", true);
                    }
                    IOMgr.GetInstace().WriteIoBit($"12V开启", ParamSetMgr.GetInstance().GetBoolParam("是否开启非程控12V"));

                }
                catch
                {

                }
                IsHome = true;
            });
            //   IOMgr.GetInstace().m_deltgateSystemSingl("复位", true);
        }
        public void StopAll()
        {
            StationDisp dospT = (StationDisp)StationMgr.GetInstance().GetStation("点胶站");
            StationAA stationAAT = (StationAA)StationMgr.GetInstance().GetStation("AA站");
            StationTable stationTableTT = (StationTable)StationMgr.GetInstance().GetStation("转盘站");
            if (dospT.AxisX != -1)
                MotionMgr.GetInstace().StopEmg(dospT.AxisX);
            if (dospT.AxisY != -1)
                MotionMgr.GetInstace().StopEmg(dospT.AxisY);
            if (dospT.AxisZ != -1)
                MotionMgr.GetInstace().StopEmg(dospT.AxisZ);
            if (dospT.AxisTx != -1)
                MotionMgr.GetInstace().StopEmg(dospT.AxisTx);
            if (dospT.AxisTy != -1)
                MotionMgr.GetInstace().StopEmg(dospT.AxisTy);
            if (dospT.AxisU != -1)
                MotionMgr.GetInstace().StopEmg(dospT.AxisU);
            if (stationAAT.AxisX != -1)
                MotionMgr.GetInstace().StopEmg(stationAAT.AxisX);
            if (stationAAT.AxisY != -1)
                MotionMgr.GetInstace().StopEmg(stationAAT.AxisY);
            if (stationAAT.AxisZ != -1)
                MotionMgr.GetInstace().StopEmg(stationAAT.AxisZ);
            if (stationAAT.AxisTx != -1)
                MotionMgr.GetInstace().StopEmg(stationAAT.AxisTx);
            if (stationAAT.AxisTy != -1)
                MotionMgr.GetInstace().StopEmg(stationAAT.AxisTy);
            if (stationAAT.AxisU != -1)
                MotionMgr.GetInstace().StopEmg(stationAAT.AxisU);
            if (stationTableTT.AxisX != -1)
                MotionMgr.GetInstace().StopEmg(stationTableTT.AxisX);
            if (stationTableTT.AxisY != -1)
                MotionMgr.GetInstace().StopEmg(stationTableTT.AxisY);
            if (stationTableTT.AxisZ != -1)
                MotionMgr.GetInstace().StopEmg(stationTableTT.AxisZ);
            if (stationTableTT.AxisTx != -1)
                MotionMgr.GetInstace().StopEmg(stationTableTT.AxisTx);
            if (stationTableTT.AxisTy != -1)
                MotionMgr.GetInstace().StopEmg(stationTableTT.AxisTy);
            if (stationTableTT.AxisU != -1)
                MotionMgr.GetInstace().StopEmg(stationTableTT.AxisU);

        }
        public void ShowCT(int AB)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => { ShowCT(AB); }));
            }
            else
            {
                string station = "A";
                if (AB > 0)
                    if (UserTest.TestResultAB[AB - 1].Result)
                    {
                        station = AB == 2 ? "B" : "A";
                        Result.Text = $"{station} PASS";
                        Result.BackColor = Color.Green;
                    }
                    else
                    {
                        station = AB == 2 ? "B" : "A";
                        if (UserTest.TestResultAB[AB - 1].FailStep == "")
                            Result.Text = $"{station}失败原因：其他";
                        else
                            Result.Text = $"{station}失败原因:{UserTest.TestResultAB[AB - 1].FailStep}";
                        Result.BackColor = Color.Red;
                    }
                int delA = 0;
                int delB = 0;
                if (UserTest.CTTestAB[0].Star && !UserTest.CTTestAB[0].End && !UserTest.CTTestAB[0].Show)//如果A工位开始，还未结束
                {
                    delA = 1;
                }
                if (UserTest.CTTestAB[1].Star && !UserTest.CTTestAB[1].End && !UserTest.CTTestAB[1].Show)//如果B工位开始，还未结束
                {
                    delB = 1;
                }
                //数据检查
                if (UserTest.ProductCount.CompeteA == 0)
                {
                    UserTest.ProductCount.OKA = 0;
                    UserTest.ProductCount.NGA = 0;
                    UserTest.ProductCount.PencentA = 0;
                    UserTest.ProductCount.OCFailA = 0;
                    UserTest.ProductCount.SFRFailA = 0;
                    UserTest.ProductCount.OtherFailA = 0;
                    UserTest.ProductCount.TiltFailA = 0;
                    UserTest.ProductCount.PlayFailA = 0;
                    delA = 0;
                }
                if (UserTest.ProductCount.CompeteB == 0)
                {
                    UserTest.ProductCount.OKB = 0;
                    UserTest.ProductCount.NGB = 0;
                    UserTest.ProductCount.PencentB = 0;
                    UserTest.ProductCount.OCFailB = 0;
                    UserTest.ProductCount.SFRFailB = 0;
                    UserTest.ProductCount.OtherFailB = 0;
                    UserTest.ProductCount.TiltFailB = 0;
                    UserTest.ProductCount.PlayFailB = 0;
                    delB = 0;
                }
                if (UserTest.ProductCount.CompeteA != 0)
                {
                    if (UserTest.ProductCount.CompeteA - delA == 0)
                        UserTest.ProductCount.PencentB = 0;

                    else
                        UserTest.ProductCount.PencentA = ((UserTest.ProductCount.OKA / ((UserTest.ProductCount.CompeteA - delA) * 1.0)) * 100);
                }
                if (UserTest.ProductCount.CompeteB != 0)
                {
                    if (UserTest.ProductCount.CompeteB - delB == 0)
                        UserTest.ProductCount.PencentB = 0;
                    else
                        UserTest.ProductCount.PencentB = ((UserTest.ProductCount.OKB / ((UserTest.ProductCount.CompeteB - delB) * 1.0)) * 100);
                }
                if (UserTest.ProductCount.CompeteA + UserTest.ProductCount.CompeteB != 0)
                {
                    double t = UserTest.ProductCount.CompeteA + UserTest.ProductCount.CompeteB - delB - delA;
                    if (t == 0)
                        UserTest.ProductCount.PencentCTAll = 0;
                    else
                        UserTest.ProductCount.PencentCTAll = (((UserTest.ProductCount.OKB + UserTest.ProductCount.OKA) / (t * 1.0)) * 100);

                }
                #region 显示UPH

                float height = pictureBox1.Height;
                float width = pictureBox1.Width;
                int RowsCount = 3; int ColCount = 6;
                int RowsDel = 5; int ColDel = 5;
                Bitmap bmp = new Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    SolidBrush drawSmallBrush = new SolidBrush(Color.Black);
                    Pen pen = new Pen(Color.Black, 1);
                    for (int i = 1; i <= ColCount; i++)
                    {
                        g.DrawLine(pen, (width * i) / ColCount, 0, (width * i) / ColCount, height);
                        for (int j = 1; j < RowsCount; j++)
                        {
                            if ((i == ColCount && j != 1) || (i == ColCount - 1 && j != 1))
                            {
                                continue;
                            }
                            g.DrawLine(pen, (width * (i - 1)) / ColCount, (height * j) / RowsCount, (width * i) / ColCount, (height * j) / RowsCount);
                        }
                    }
                    g.DrawString("工位", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 0 / ColCount + ColDel, height * 0 / RowsCount + RowsDel);
                    g.DrawString("投入", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 1 / ColCount + ColDel, height * 0 / RowsCount + RowsDel);
                    g.DrawString("产出", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 2 / ColCount + ColDel, height * 0 / RowsCount + RowsDel);
                    g.DrawString("良率", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 3 / ColCount + ColDel, height * 0 / RowsCount + RowsDel);
                    g.DrawString("总良率", new Font("Arial", 9, FontStyle.Regular), drawSmallBrush, width * 4 / ColCount + ColDel, height * 0 / RowsCount + RowsDel);
                    g.DrawString("UPH", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 5 / ColCount + ColDel, height * 0 / RowsCount + RowsDel);
                    g.DrawString($"{"A"}", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 0 / ColCount + ColDel, height * 1 / RowsCount + RowsDel);
                    g.DrawString($"{UserTest.ProductCount.CompeteA}", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 1 / ColCount + ColDel, height * 1 / RowsCount + RowsDel);
                    g.DrawString($"{UserTest.ProductCount.OKA}", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 2 / ColCount + ColDel, height * 1 / RowsCount + RowsDel);
                    g.DrawString($"{UserTest.ProductCount.PencentA.ToString("0.00")}%", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 3 / ColCount + ColDel, height * 1 / RowsCount + RowsDel);
                    g.DrawString($"{"B"}", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 0 / ColCount + ColDel, height * 2 / RowsCount + RowsDel);
                    g.DrawString($"{ UserTest.ProductCount.CompeteB}", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 1 / ColCount + ColDel, height * 2 / RowsCount + RowsDel);
                    g.DrawString($"{ UserTest.ProductCount.OKB}", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 2 / ColCount + ColDel, height * 2 / RowsCount + RowsDel);
                    g.DrawString($"{UserTest.ProductCount.PencentB.ToString("0.00")}%", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 3 / ColCount + ColDel, height * 2 / RowsCount + RowsDel);
                    g.DrawString($"{UserTest.ProductCount.PencentCTAll.ToString("0.00")}%", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 4 / ColCount + ColDel, (float)(height * 1.5 / RowsCount + RowsDel));
                    g.DrawString($"{UserTest.ProductCount.UPH.ToString("0.00")}", new Font("Arial", 10, FontStyle.Regular), drawSmallBrush, width * 5 / ColCount + ColDel, (float)(height * 1.5 / RowsCount + RowsDel));

                }
                pictureBox1.Image = bmp;
                #endregion

                #region 显示不良项目 表格
                if (dataGridViewNG.RowCount <= 0)
                {
                    dataGridViewNG.Rows.Add(6);
                }

                //UserTest.ProductCount.OtherFailA = UserTest.ProductCount.CompeteA - delA - UserTest.ProductCount.OKA - UserTest.ProductCount.NGA;
                //UserTest.ProductCount.OtherFailB = UserTest.ProductCount.CompeteB - delB - UserTest.ProductCount.OKB - UserTest.ProductCount.NGB;
                UserTest.ProductCount.OtherFailA = 0;
                UserTest.ProductCount.OtherFailB = 0;
                dataGridViewNG[0, 0].Value = "取图";
                dataGridViewNG[0, 1].Value = "Tilt";
                dataGridViewNG[0, 2].Value = "OC";
                dataGridViewNG[0, 3].Value = "SFR";
                dataGridViewNG[0, 4].Value = "其他";
                dataGridViewNG[0, 5].Value = "总计";
                dataGridViewNG[1, 0].Value = UserTest.ProductCount.PlayFailA;
                dataGridViewNG[1, 1].Value = UserTest.ProductCount.TiltFailA;
                dataGridViewNG[1, 2].Value = UserTest.ProductCount.OCFailA;
                dataGridViewNG[1, 3].Value = UserTest.ProductCount.SFRFailA;
                dataGridViewNG[1, 4].Value = UserTest.ProductCount.OtherFailA;
                dataGridViewNG[1, 5].Value = UserTest.ProductCount.CompeteA - delA - UserTest.ProductCount.OKA;
                dataGridViewNG[2, 0].Value = UserTest.ProductCount.PlayFailB;
                dataGridViewNG[2, 1].Value = UserTest.ProductCount.TiltFailB;
                dataGridViewNG[2, 2].Value = UserTest.ProductCount.OCFailB;
                dataGridViewNG[2, 3].Value = UserTest.ProductCount.SFRFailB;
                dataGridViewNG[2, 4].Value = UserTest.ProductCount.OtherFailB;
                dataGridViewNG[2, 5].Value = UserTest.ProductCount.CompeteB - delB - UserTest.ProductCount.OKB;
                dataGridViewNG[3, 0].Value = UserTest.ProductCount.PlayFailA + UserTest.ProductCount.PlayFailB;
                dataGridViewNG[3, 1].Value = UserTest.ProductCount.TiltFailA + UserTest.ProductCount.TiltFailB;
                dataGridViewNG[3, 2].Value = UserTest.ProductCount.OCFailA + UserTest.ProductCount.OCFailB;
                dataGridViewNG[3, 3].Value = UserTest.ProductCount.SFRFailA + UserTest.ProductCount.SFRFailB;
                dataGridViewNG[3, 4].Value = UserTest.ProductCount.OtherFailA + UserTest.ProductCount.OtherFailB;
                dataGridViewNG[3, 5].Value = UserTest.ProductCount.CompeteA - delA - UserTest.ProductCount.OKA + UserTest.ProductCount.CompeteB - delB - UserTest.ProductCount.OKB;

                #endregion

                #region 显示不良项目 饼图
                // Chart_NG.Series.Clear();
                Chart_NG.Legends.Clear();
                // Chart_NG.Series.Add(new Series("Data"));
                //   Chart_NG.Legends.Add(new Legend("Stores"));
                if (Chart_NG.Titles.Count == 0)
                    Chart_NG.Titles.Add(new Title("Titles"));
                Chart_NG.Series[0].ChartType = SeriesChartType.Pie;
                Chart_NG.Series[0].Points.Clear();
                List<string> x = new List<string>() { "取图", "Tilt", "OC", "SFR", "其他" };
                List<double> y = new List<double>() { UserTest.ProductCount.PlayFailA + UserTest.ProductCount.PlayFailB, UserTest.ProductCount.TiltFailA + UserTest.ProductCount.TiltFailB, UserTest.ProductCount.OCFailA + UserTest.ProductCount.OCFailB, UserTest.ProductCount.SFRFailA + UserTest.ProductCount.SFRFailB, UserTest.ProductCount.OtherFailA + UserTest.ProductCount.OtherFailB };
                Chart_NG.Series[0]["PieLabelStyle"] = "Outside";
                Chart_NG.Series[0]["PieLineColor"] = "Black";
                //  Chart_NG.Series[0].LegendText = "#VALX:#VALY";
                // Chart_NG.Series[0].Legend = "Stores";
                Chart_NG.Series[0].Label = "#VALX:#PERCENT";
                Chart_NG.Series[0].IsXValueIndexed = false;
                Chart_NG.Series[0].IsValueShownAsLabel = false;
                Chart_NG.Series[0].ToolTip = "#VAL{D}个";
                Chart_NG.Series[0].Points.DataBindXY(x, y);
                Chart_NG.Titles[0].Text = $"总NG数:{UserTest.ProductCount.CompeteA - delA - UserTest.ProductCount.OKA + UserTest.ProductCount.CompeteB - delB - UserTest.ProductCount.OKB}";
                Chart_NG.ChartAreas[0].AxisX.IsMarginVisible = false;
                Chart_NG.ChartAreas[0].Area3DStyle.Enable3D = true;
                // Chart_NG.ChartAreas[0].Area3DStyle.Rotation = 15;
                //  Chart_NG.ChartAreas[0].Area3DStyle.Inclination = 45;
                Chart_NG.ChartAreas[0].Area3DStyle.LightStyle = LightStyle.Realistic;
                #endregion
                txt_SN.Focus();
                string ctPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\ProductInfo.xml";
                ProductInfoFile.SaveCT(ctPath);

            }

        }
        public void GetSN(int index)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => { GetSN(index); }));
            }
            else
            {

                string value = "";
                UserTest.TestResultAB[index] = new TestResult();
                string a = txt_SN.Text.Trim();
                int c = a.Length;
                if (txt_SN.Text.Trim().Length == 0)
                {
                    if (NoSN.Checked)
                    {
                        value = $"Time_{DateTime.Now.ToString("HHmmssfff")}";
                    }
                    else
                    {
                        value = "NOSN";
                    }
                }
                else
                {
                    value = txt_SN.Text.Trim();
                }
                txt_SN.Text = "";

                UserTest.TestResultAB[index].StarTime = DateTime.Now;
                UserTest.TestResultAB[index].SerialNumber = value;
                UserTest.ProductCheckResultAB[index].SerialNumber = value;//获取SN
                UserTest.Model = ParamSetMgr.GetInstance().CurrentProductFile;//获取机型
                UserTest.Batch = txt_Batch.Text.Trim();//获取批次
                UserTest.TestResultAB[index].AAModel = ParamSetMgr.GetInstance().GetStringParam("AA模式选择");
            }

        }
        public async static void RunB()
        {
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许手动点亮！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            UserTest.ContiuneWhlie = false;
            GlobelManualResetEvent.ContinueShowB.Set();
            Thread.Sleep(200);
            PlayB = false;
            PlayA = true;
            PlayC = true;
            int SetSN = ParamSetMgr.GetInstance().GetIntParam("B工位SetSN_ID");
            // string PlayIni = ParamSetMgr.GetInstance().GetStringParam("点亮文件名称");
            string playPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\Play\\play.ini";
            ModuleMgr.Instance.SetSN(1, SetSN);
            //if (!ModuleMgr.Instance.Init(DeviceID, playPath))
            //{
            //    MessageBox.Show("加载B工位点亮文件失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}

            ModuleMgr.Instance.Stop(1);
            Thread.Sleep(200);

            if (!ModuleMgr.Instance.Play(1))
            {
                MessageBox.Show("点亮失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (ParamSetMgr.GetInstance().GetBoolParam("是否关闭曝光") && ParamSetMgr.GetInstance().GetStringParam("工位点亮类型").Contains("DT"))
            {
                int SlaveID = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("SlaveID"), 16);
                int addr = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("关闭曝光地址"), 16);
                int value = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("曝光值"), 16);
                ModuleMgr.Instance.WriteI2C(1, Convert.ToByte(SlaveID), 0, addr, value);
            }

            UserTest.ContiuneWhlie = true;
            Task task_ParaB_Grab = Task.Run(() =>
            {
                while (UserTest.ContiuneWhlie)
                {
                    if (GlobalVariable.g_StationState == StationState.StationStateRun)
                    {
                        ModuleMgr.Instance.Stop(1);
                        PlayB = true;
                        break;
                    }
                    if (PlayB)
                    {
                        Form_Auto.EvenShowImageDelegate(null);
                        break;
                    }
                    Bitmap bt = null;
                    SFRValue sFRValue = new SFRValue();
                    RectInfo rectInfo = new RectInfo();
                    LightValue lightValue = new LightValue();
                    Rectangle[] rectangles = new Rectangle[13];
                    DateTime a = DateTime.Now;
                    try
                    {

                        ModuleMgr.Instance.CaptureToBmpRGB(1, 1, ref bt);

                    }
                    catch
                    {
                        PlayB = true;
                        Form_Auto.EvenShowImageDelegate(null);
                        break;
                    }
                    try
                    {
                        ImageSave = (Bitmap)bt.Clone();
                        DateTime b = DateTime.Now;
                        double x1 = (b - a).TotalMilliseconds;
                        AlgorithmMgr.Instance.GetSFRValue((Bitmap)bt.Clone(), ref sFRValue, ref rectInfo, rectangles, ref lightValue, true);
                        DateTime c = DateTime.Now;
                        double x2 = (c - b).TotalMilliseconds;
                        double x3 = (c - a).TotalMilliseconds;

                        Form_Auto.EvenShowImageDelegate((Bitmap)bt, "", showRoi, sFRValue, rectInfo, rectangles, lightValue);

                    }
                    catch
                    {
                        Form_Auto.EvenShowImageDelegate((Bitmap)bt, "");
                    }
                    Thread.Sleep(150);
                    GlobelManualResetEvent.ContinueShowB.WaitOne();
                }
            });
            await task_ParaB_Grab;
            PlayB = true;
        }
        public async static void RunA()
        {
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许手动点亮！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            UserTest.ContiuneWhlie = false;
            GlobelManualResetEvent.ContinueShowA.Set();
            Thread.Sleep(200);
            PlayA = false;
            PlayB = true;
            PlayC = true;
            int SetSN = ParamSetMgr.GetInstance().GetIntParam("A工位SetSN_ID");
            // string PlayIni = ParamSetMgr.GetInstance().GetStringParam("点亮文件名称");
            string playPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\Play\\play.ini";

            if (!File.Exists(playPath))
            {
                MessageBox.Show($"点亮档不存在。", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            ModuleMgr.Instance.SetSN(0, SetSN);
            //if (!ModuleMgr.Instance.Init(DeviceID, playPath))
            //{
            //    MessageBox.Show("加载A工位点亮文件失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}


            ModuleMgr.Instance.Stop(0);
            Thread.Sleep(200);

            if (!ModuleMgr.Instance.Play(0))
            {
                MessageBox.Show("点亮失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (ParamSetMgr.GetInstance().GetBoolParam("是否关闭曝光") && ParamSetMgr.GetInstance().GetStringParam("工位点亮类型").Contains("DT"))
            {
                int SlaveID = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("SlaveID"), 16);
                int addr = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("关闭曝光地址"), 16);
                int value = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("曝光值"), 16);
                ModuleMgr.Instance.WriteI2C(0, Convert.ToByte(SlaveID), 0, addr, value);
            }
            UserTest.ContiuneWhlie = true;
            Task task_ParaA_Grab = Task.Run(() =>
            {
                while (UserTest.ContiuneWhlie)
                {
                    if (GlobalVariable.g_StationState == StationState.StationStateRun)
                    {
                        ModuleMgr.Instance.Stop(0);
                        PlayA = true;
                        break;
                    }
                    if (PlayA)
                    {
                        Form_Auto.EvenShowImageDelegate(null);
                        break;
                    }
                    Bitmap bt = null;
                    SFRValue sFRValue = new SFRValue();
                    RectInfo rectInfo = new RectInfo();
                    Rectangle[] rectangles = new Rectangle[13];
                    DateTime a = DateTime.Now;
                    try
                    {

                        ModuleMgr.Instance.CaptureToBmpRGB(0, 1, ref bt);

                    }
                    catch
                    {
                        PlayA = true;
                        Form_Auto.EvenShowImageDelegate(null);
                        break;
                    }
                    try
                    {
                        ImageSave = (Bitmap)bt.Clone();
                        DateTime b = DateTime.Now;
                        double x1 = (b - a).TotalMilliseconds;
                        LightValue lightValue = new LightValue();
                        AlgorithmMgr.Instance.GetSFRValue((Bitmap)bt.Clone(), ref sFRValue, ref rectInfo, rectangles, ref lightValue, true);
                        DateTime c = DateTime.Now;
                        double x2 = (c - b).TotalMilliseconds;
                        double x3 = (c - a).TotalMilliseconds;

                        Form_Auto.EvenShowImageDelegate((Bitmap)bt, "", showRoi, sFRValue, rectInfo, rectangles, lightValue);

                    }
                    catch
                    {
                        Form_Auto.EvenShowImageDelegate((Bitmap)bt, "");
                    }
                    Thread.Sleep(150);
                    GlobelManualResetEvent.ContinueShowA.WaitOne();
                }
            });
            await task_ParaA_Grab;
            PlayA = true;
        }

        public async static void RunCalib()
        {
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许手动点亮！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            UserTest.ContiuneWhlie = false;
            GlobelManualResetEvent.ContinueShowC.Set();
            Thread.Sleep(200);
            PlayC = false;
            PlayA = true;
            PlayB = true;
            List<string> strSns = new List<string>();
            int nCams = 0;
             ModuleMgr.Instance.Enumerate(0, ref nCams, strSns, 1);
            int SetSN_ID = ParamSetMgr.GetInstance().GetIntParam("校准相机SetSN_ID");

            ModuleMgr.Instance.Stop(0, 1);
            ModuleMgr.Instance.SetSN(0, SetSN_ID, 1);
            ModuleMgr.Instance.Init(0, "", 1);
            bool bRtn = ModuleMgr.Instance.Play(0, 1);
            Thread.Sleep(200);

            if (!bRtn)
            {
                MessageBox.Show("点亮失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            UserTest.ContiuneWhlie = true;
            Task task_ParaC_Grab = Task.Run(() =>
            {
                while (UserTest.ContiuneWhlie)
                {
                    if (GlobalVariable.g_StationState == StationState.StationStateRun)
                    {
                        ModuleMgr.Instance.Stop(0, 1);
                        PlayC = true;
                        break;
                    }
                    if (PlayC)
                    {
                        Form_Auto.EvenShowImageDelegate(null);
                        break;
                    }
                    Bitmap bt = null;
                    SFRValue sFRValue = new SFRValue();
                    RectInfo rectInfo = new RectInfo();
                    Rectangle[] rectangles = new Rectangle[13];
                    DateTime a = DateTime.Now;
                    try
                    {

                        ModuleMgr.Instance.CaptureToBmpRGB(0, 1, ref bt, 1);

                    }
                    catch
                    {
                        PlayC = true;
                        Form_Auto.EvenShowImageDelegate(null);
                        break;
                    }
                    try
                    {
                        ImageSave = (Bitmap)bt.Clone();
                        DateTime b = DateTime.Now;
                        double x1 = (b - a).TotalMilliseconds;
                        LightValue lightValue = new LightValue();
                        AlgorithmMgr.Instance.GetSFRValue((Bitmap)bt.Clone(), ref sFRValue, ref rectInfo, rectangles, ref lightValue, true);
                        DateTime c = DateTime.Now;
                        double x2 = (c - b).TotalMilliseconds;
                        double x3 = (c - a).TotalMilliseconds;

                        Form_Auto.EvenShowImageDelegate((Bitmap)bt, "", showRoi, sFRValue, rectInfo, rectangles, lightValue);

                    }
                    catch
                    {
                        Form_Auto.EvenShowImageDelegate((Bitmap)bt, "");
                    }
                    Thread.Sleep(150);
                    GlobelManualResetEvent.ContinueShowC.WaitOne();
                }
            });
            await task_ParaC_Grab;
            PlayC = true;
        }
        private static void DrawRect(bool showRoi, int bigX, int bigY, Graphics G, Pen pen, Point[] points, BlockValue blockValue, double count, double lightValue = -1)
        {
            SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Red);
            SolidBrush drawLightBrush = new SolidBrush(System.Drawing.Color.Green);
            SolidBrush drawSmallBrush = new SolidBrush(System.Drawing.Color.Blue);
            int samllX = ParamSetMgr.GetInstance().GetIntParam("[SFR] nSFRROI_Width");
            int samllY = ParamSetMgr.GetInstance().GetIntParam("[SFR] nSFRROI_Height");
            int x = (int)(points[0].X - bigX / 2);
            int y = (int)(points[0].Y - bigY / 2);
            int Lx = (int)bigX;
            int Ly = (int)bigY;
            if (showRoi)
            {
                if (points[0].X != -1 && points[0].Y != -1)
                {
                    G.DrawRectangle(pen, new Rectangle { X = x, Y = y, Width = Lx, Height = Ly });
                    G.DrawRectangle(pen, new Rectangle { X = x, Y = y, Width = 1, Height = 1 });
                    if (lightValue >= 0)
                    {
                        G.DrawString(Math.Round(lightValue, 1).ToString(), new Font("Arial", bigX / 6, FontStyle.Regular), drawLightBrush, new Point(points[0].X, points[0].Y - bigY / 4));
                    }
                }

            }
            G.DrawRectangle(pen, new Rectangle { X = points[0].X - 4, Y = points[0].Y - 4, Width = 8, Height = 8 });

            if (points.Length == 1)
            {
                int x1 = (int)(points[0].X - samllX / 2);
                int y1 = (int)(points[0].Y - samllY / 2);
                Lx = (int)samllX;
                Ly = (int)samllY;
                if (points[0].X != -1 && points[0].Y != -1)
                {
                    // if (blockValue.aryValue[0] == -1 || blockValue.aryValue[0] == 0)
                    // {
                    G.DrawRectangle(new Pen(Color.Red, 3), new Rectangle { X = x1, Y = y1, Width = Lx, Height = Ly });
                    // }
                    G.DrawString(Math.Round(blockValue.aryValue[0], 1).ToString(), new Font("Arial", bigX / 6, FontStyle.Regular), drawSmallBrush, new Point(points[0].X - 20, points[0].Y - bigY / 2 - 20));

                }
                // if (blockValue.aryValue[1] > 0 && blockValue.aryValue[2] > 0 && blockValue.aryValue[3] > 0 && blockValue.aryValue[4] > 0)
                double a = 0;
                a = blockValue.aryValue[1] > 0 ? blockValue.aryValue[1] : 0;

                if (count != 0)
                    G.DrawString(Math.Round(a, 1).ToString(), new Font("Arial", bigX / 3, FontStyle.Regular), drawBrush, new Point(points[0].X, points[0].Y));

            }
            else
            {
                for (int i = 1; i < points.Length; i++)
                {
                    int x1 = (int)(points[i].X - samllX / 2);
                    int y1 = (int)(points[i].Y - samllY / 2);
                    Lx = (int)samllX;
                    Ly = (int)samllY;
                    if (points[i].X != -1 && points[i].Y != -1)
                    {
                        if (blockValue.aryValue[i] == -1 || blockValue.aryValue[i] == 0)
                            continue;
                        G.DrawRectangle(pen, new Rectangle { X = x1, Y = y1, Width = Lx, Height = Ly });

                        if (i == 1)
                            G.DrawString(Math.Round(blockValue.aryValue[i], 1).ToString(), new Font("Arial", bigX / 6, FontStyle.Regular), drawSmallBrush, new Point(points[0].X - 20, points[0].Y - bigY / 2 - 20));
                        if (i == 2)
                            G.DrawString(Math.Round(blockValue.aryValue[i], 1).ToString(), new Font("Arial", bigX / 6, FontStyle.Regular), drawSmallBrush, new Point(points[0].X - bigX / 2 - 20, points[0].Y - 20));
                        if (i == 3)
                            G.DrawString(Math.Round(blockValue.aryValue[i], 1).ToString(), new Font("Arial", bigX / 6, FontStyle.Regular), drawSmallBrush, new Point(points[0].X - 20, points[0].Y + bigY / 2 - 20));
                        if (i == 4)
                            G.DrawString(Math.Round(blockValue.aryValue[i], 1).ToString(), new Font("Arial", bigX / 6, FontStyle.Regular), drawSmallBrush, new Point(points[0].X - 20 + bigX / 2, points[0].Y - 20));

                    }
                    // if (blockValue.aryValue[1] > 0 && blockValue.aryValue[2] > 0 && blockValue.aryValue[3] > 0 && blockValue.aryValue[4] > 0)
                    double a = 0;
                    a += blockValue.aryValue[1] > 0 ? blockValue.aryValue[1] : 0;
                    a += blockValue.aryValue[2] > 0 ? blockValue.aryValue[2] : 0;
                    a += blockValue.aryValue[3] > 0 ? blockValue.aryValue[3] : 0;
                    a += blockValue.aryValue[4] > 0 ? blockValue.aryValue[4] : 0;
                    if (count != 0)
                        G.DrawString(Math.Round(a / count, 1).ToString(), new Font("Arial", bigX / 3, FontStyle.Regular), drawBrush, new Point(points[0].X, points[0].Y));

                }



            }
        }
        private static void DrawSet(bool showRoi, int bigX, int bigY, Graphics G, double xfield, double yfield, double imgWidth, double imgHeight)
        {
            Pen penA = new Pen(Color.Yellow, 6);
            if (showRoi)
            {
                if (xfield == 0)
                {
                    G.DrawRectangle(penA, new Rectangle { X = (int)(imgWidth / 2 - 0 - bigX / 2), Y = (int)(imgHeight / 2 + 0 - bigY / 2), Width = bigX, Height = bigY });
                }
                else
                {
                    G.DrawRectangle(penA, new Rectangle { X = (int)(imgWidth / 2 - xfield * (imgWidth / 2) - bigX / 2), Y = (int)(imgHeight / 2 + yfield * (imgHeight / 2) - bigY / 2), Width = bigX, Height = bigY });
                    G.DrawRectangle(penA, new Rectangle { X = (int)(imgWidth / 2 - xfield * (imgWidth / 2) - bigX / 2), Y = (int)(imgHeight / 2 - yfield * (imgHeight / 2) - bigY / 2), Width = bigX, Height = bigY });
                    G.DrawRectangle(penA, new Rectangle { X = (int)(imgWidth / 2 + xfield * (imgWidth / 2) - bigX / 2), Y = (int)(imgHeight / 2 - yfield * (imgHeight / 2) - bigY / 2), Width = bigX, Height = bigY });
                    G.DrawRectangle(penA, new Rectangle { X = (int)(imgWidth / 2 + xfield * (imgWidth / 2) - bigX / 2), Y = (int)(imgHeight / 2 + yfield * (imgHeight / 2) - bigY / 2), Width = bigX, Height = bigY });
                }
            }

        }
        public void AddFlag(string strFlagName, bool bInitState)
        {
            m_listFlag.Add(strFlagName);
            userPanel_Flag.AddFlag(strFlagName);
            userPanel_Flag.SetLebalState(strFlagName, bInitState);


        }
        void LoadProductFile(string strFile)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => LoadProductFile(strFile)));
            }
            else
            {
                label_CurrentFile.Text = "当前产品:" + strFile;
                //读取工站位置坐标
                ConfigToolMgr.GetInstance().UpdatePointFilePath();
                Dictionary<string, PointInfo> dicPonit = new Dictionary<string, PointInfo>();
                foreach (var tem in StationMgr.GetInstance().GetAllStationName())
                {
                    ConfigToolMgr.GetInstance().ReadPoint(tem, out dicPonit);
                    StationMgr.GetInstance().GetStation(tem).SetStationPointDic(dicPonit);

                }
                ConfigToolMgr.GetInstance().UpdataMoveparamconfigPath();

                ConfigToolMgr.GetInstance().ReadMoveParamConfig();
                ConfigToolMgr.GetInstance().ReadHomeParamConfig();


                ParamSetMgr.GetInstance().m_eventLoadProductFileUpadata?.Invoke();
                //VisionMgr.GetInstance().PrItemChangedEvent
                GC.Collect();
            }

        }
        void StationStateChangedHandler(StationState currState)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => StationStateChangedHandler(currState)));
            }
            else
            {

                switch (currState)
                {
                    case StationState.StationStatePause:
                        MachineStateEmg.State = false;
                        MachineStateStop.State = false;
                        MachineStateAuto.State = false;
                        MachineStatePause.State = true;
                        break;
                    case StationState.StationStateRun:
                        MachineStateEmg.State = false;
                        MachineStateStop.State = false;
                        MachineStateAuto.State = true;
                        MachineStatePause.State = false;
                        break;
                    case StationState.StationStateStop:
                        MachineStateEmg.State = false;
                        MachineStateStop.State = true;
                        MachineStateAuto.State = false;
                        MachineStatePause.State = false;
                        break;
                    case StationState.StationStateEmg:
                        MachineStateEmg.State = true;
                        MachineStateStop.State = false;
                        MachineStateAuto.State = false;
                        MachineStatePause.State = false;
                        break;
                }
            }
        }
        void ShowStationMsg(ListLog listlog, string msg)
        {
            if (listlog == null)
                return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowStationMsg(listlog, msg)));
            }
            else
            {

                listlog.AddMsg(msg);
                //listlog?.Items.Add(msg);
                //listlog.SelectedIndex = listlog.Items.Count - 1;
                //while (listlog.Items.Count > nCount)
                //    listlog.Items.RemoveAt(0);


            }

        }
        #endregion

        private void Batch_Click(object sender, EventArgs e)
        {
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许输入批次！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (txt_SN.Text.Trim() == "")
            {
                MessageBox.Show("请先在SN中输入批次", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

        }

        private void ProductBatch_CheckedChanged(object sender, EventArgs e)
        {
            if (ProductBatch.Checked)
            {
                txt_Batch.ReadOnly = false;
            }
            else
            {
                txt_Batch.ReadOnly = true;
                string ctPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\ProductInfo.xml";
                string currentProductFile = ParamSetMgr.GetInstance().CurrentWorkDir + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + (".xml");
                UserTest.ProductCount = new ProductInfo();
                ProductInfoFile.SaveCT(ctPath);
                Thread.Sleep(100);
                ShowCT(0);
                ParamSet paramSet = new ParamSet();
                ParamSetMgr.GetInstance().GetParam("批次", out paramSet);
                paramSet._strParamVal = txt_Batch.Text.Trim();
                ParamSetMgr.GetInstance().SetParam("批次", paramSet);
                ParamSetMgr.GetInstance().SaveParam(currentProductFile);
                UserTest.Batch = ParamSetMgr.GetInstance().GetStringParam("批次");
                this.txt_Batch.Text = $"{ParamSetMgr.GetInstance().GetStringParam("批次")}";
            }
        }

        private void AACalib_Click(object sender, EventArgs e)
        {
            RunCalib();
        }
    }
}
