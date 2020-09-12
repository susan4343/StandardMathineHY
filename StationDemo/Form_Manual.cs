using BaseDll;
using EpsonRobot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UserData;

using HalconDotNet;
using CommonTools;
using UserCtrl;
using CameraLib;
using System.IO;
using System.Diagnostics;
using System.Threading;
using MotionIoLib;
using System.Threading.Tasks;
using LightControler;
using System.Reflection;
using OtherDevice;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Drawing.Imaging;
using System.Windows.Forms.DataVisualization.Charting;
using VisionProcess;
using ModuleCapture;

namespace StationDemo
{
    public partial class Form_Manual : Form
    {
        #region 变量\事件
        public delegate void ShowChartDelegate(List<SFRValue> SfrInfoArr, int tf, double zPos = 0, double dstep = 0.01, string SaveChartImagePath = "");
        public static ShowChartDelegate EvenShowChartDelegate;
        private delegate bool My(ref double x, ref double y);
        #endregion

        #region 控件
        public Form_Manual()
        {
            InitializeComponent();
            Form_Manual.EvenShowChartDelegate += ShowThroughFocus;
        }
        private void Form_Manual_Load(object sender, EventArgs e)
        {
            //初始化界面选择
            {
                //List<string> cameraArr = CameraMgr.GetInstance().GetCameraNameArr();
                //foreach (var a in cameraArr)
                //{
                //    comboBox_SelCamera.Items.Add(a);
                //}

                checkBoxA.Checked = !ParamSetMgr.GetInstance().GetBoolParam("屏蔽A工位");
                checkBoxB.Checked = !ParamSetMgr.GetInstance().GetBoolParam("屏蔽B工位");
                radioBtnPreak.Checked = ParamSetMgr.GetInstance().GetStringParam("AA模式选择") == "Peak";
                radioBtnAve.Checked = ParamSetMgr.GetInstance().GetStringParam("AA模式选择") == "Average";
                radioBtnA.Checked = true;
            }
            //初始化手动AA步骤
            BtnAAStepList.Add(new BtnStepAA { Index = 0, ButtonValue = btnStep_0, ColorValue = Color.Green, NextStep = 1, NextStepList = new List<int> { 0, 1, 7 } });
            BtnAAStepList.Add(new BtnStepAA { Index = 1, ButtonValue = btnStep_1, ColorValue = Color.Gray, NextStep = 2, NextStepList = new List<int> { 2, 7, } });
            BtnAAStepList.Add(new BtnStepAA { Index = 2, ButtonValue = btnStep_2, ColorValue = Color.Gray, NextStep = 3, NextStepList = new List<int> { 2, 3, 7, } });
            BtnAAStepList.Add(new BtnStepAA { Index = 3, ButtonValue = btnStep_3, ColorValue = Color.Gray, NextStep = 4, NextStepList = new List<int> { 2, 3, 4, 5, 7, } });
            BtnAAStepList.Add(new BtnStepAA { Index = 4, ButtonValue = btnStep_4, ColorValue = Color.Gray, NextStep = 5, NextStepList = new List<int> { 2, 3, 4, 5, 6, 7, } });
            BtnAAStepList.Add(new BtnStepAA { Index = 5, ButtonValue = btnStep_5, ColorValue = Color.Gray, NextStep = 6, NextStepList = new List<int> { 2, 3, 4, 5, 6, 7, } });
            BtnAAStepList.Add(new BtnStepAA { Index = 6, ButtonValue = btnStep_6, ColorValue = Color.Gray, NextStep = 7, NextStepList = new List<int> { 7 } });
            BtnAAStepList.Add(new BtnStepAA { Index = 7, ButtonValue = btnStep_7, ColorValue = Color.Gray, NextStep = 0, NextStepList = new List<int> { 0 } });
            EnableAA(new List<int> { 0 });
            SetNestSetpAA(0, e);
            //初始化手动点胶步骤
            BtnDispStepList.Add(new BtnStepAA { Index = 0, ButtonValue = btnDisp_0, ColorValue = Color.Green, NextStep = 1, NextStepList = new List<int> { 0, 1, 5, 7 } });
            BtnDispStepList.Add(new BtnStepAA { Index = 1, ButtonValue = btnDisp_1, ColorValue = Color.Gray, NextStep = 2, NextStepList = new List<int> { 1, 2, 7, } });
            BtnDispStepList.Add(new BtnStepAA { Index = 2, ButtonValue = btnDisp_2, ColorValue = Color.Gray, NextStep = 3, NextStepList = new List<int> { 2, 3, 7, } });
            BtnDispStepList.Add(new BtnStepAA { Index = 3, ButtonValue = btnDisp_3, ColorValue = Color.Gray, NextStep = 4, NextStepList = new List<int> { 3, 4, 7, } });
            BtnDispStepList.Add(new BtnStepAA { Index = 4, ButtonValue = btnDisp_4, ColorValue = Color.Gray, NextStep = 5, NextStepList = new List<int> { 4, 5, 7, } });
            BtnDispStepList.Add(new BtnStepAA { Index = 5, ButtonValue = btnDisp_5, ColorValue = Color.Gray, NextStep = 6, NextStepList = new List<int> { 5, 6, 7, } });
            BtnDispStepList.Add(new BtnStepAA { Index = 6, ButtonValue = btnDisp_6, ColorValue = Color.Gray, NextStep = 7, NextStepList = new List<int> { 6, 7 } });
            BtnDispStepList.Add(new BtnStepAA { Index = 7, ButtonValue = btnDisp_7, ColorValue = Color.Gray, NextStep = 0, NextStepList = new List<int> { 0 } });
            EnableDisp(new List<int> { 0 });
            SetNestSetpDisp(0, e);


            //枚举和listbox关联
            ////visionControl1.InitWindow();
            ////Thread.Sleep(10);
            ////List<string> camname = CameraMgr.GetInstance().GetCameraNameArr();
            ////foreach (var temp in camname)
            ////    comboBox_SelCamera.Items.Add(temp.ToString());
            ////if (camname != null && camname.Count > 0)
            ////{
            ////    comboBox_SelCamera.Visible = true;
            ////    comboBox_SelCamera.SelectedIndex = 0;
            ////}

            ////else
            ////{
            ////    visionControl1.Visible = false;
            ////}
            ////VisionMgr.GetInstance().PrItemChangedEvent += ChagedPrItem;
            ////ChagedPrItem("");
            //DataGridViewComboBoxColumn myCombo = new DataGridViewComboBoxColumn();
            //myCombo.DataSource = new GripperTeachingMode[] { GripperTeachingMode.ABS, GripperTeachingMode.TLSAdd, GripperTeachingMode.TLSSub };
            //myCombo.HeaderText = "ConfigMode";
            //myCombo.Name = "ConfigMode";
            //myCombo.ValueType = typeof(GripperTeachingPara);
            //myCombo.DataPropertyName = "ConfigMode";
            //dataGridView1.Columns.Add(myCombo);
            // this.dataGridView1.DataSource = jobInfo.ColumnConfigs;


            MFTest.Enabled = false;
        }
        private void OnVisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {

                IOMgr.GetInstace().m_eventIoOutputChanageByName += ChangedIoOutState;
            }
            else
            {

                IOMgr.GetInstace().m_eventIoOutputChanageByName -= ChangedIoOutState;
            }
        }
        private void comboBox_SelCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < comboBox_SelCamera.Items.Count; i++)
                CameraMgr.GetInstance().SetTriggerSoftMode(comboBox_SelCamera.Items[i].ToString());
            CameraMgr.GetInstance().BindWindow(comboBox_SelCamera.Text, visionControl1);
            CameraMgr.GetInstance().SetAcquisitionMode(comboBox_SelCamera.Text);
        }
        private void OpenCKPower_Click(object sender, EventArgs e)
        {
            OtherDevices.ckPower.SetAllCKPowerOn();
            int num = ChannelChoose.Text == "CH1" ? 1 : 2;
            if (ParamSetMgr.GetInstance().GetBoolParam("是否选择程控电源"))
            {
                OtherDevices.ckPower.SetAllCKPowerOn();

                double valueVoltage = ParamSetMgr.GetInstance().GetDoubleParam("程控电源电压");
                OtherDevices.ckPower.SetVoltage(num, valueVoltage);
                double valueCurrent = ParamSetMgr.GetInstance().GetDoubleParam("程控电源电流");
                OtherDevices.ckPower.SetCurrent(num, valueCurrent);
            }
            string stationAAName = num == 0 ? "A" : "B";
            IOMgr.GetInstace().WriteIoBit($"{stationAAName}模组上电", true);

            IOMgr.GetInstace().WriteIoBit($"12V开启", ParamSetMgr.GetInstance().GetBoolParam("是否开启非程控12V"));
        }
        private void CloseCKPower_Click(object sender, EventArgs e)
        {
            if (ParamSetMgr.GetInstance().GetBoolParam("是否选择程控电源"))
            {
                OtherDevices.ckPower.SetAllCKPowerOff();
            }
        }
        private void StepList_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void StepList_MouseMove(object sender, MouseEventArgs e)
        {
        }
        private void GetCurrent_Click(object sender, EventArgs e)
        {
            int num = ChannelChoose.Text == "CH1" ? 1 : 2;
            string r = "";
            double value = 0;
            if (ParamSetMgr.GetInstance().GetBoolParam("是否选择程控电源"))
            {
                if (!OtherDevices.ckPower.GetCurrent(num, ref value))
                    r = r + "获取电流失败";
                else
                    r = r + "电流:" + value.ToString();
                if (!OtherDevices.ckPower.GetVoltage(num, ref value))
                    r = r + "获取电压失败";
                else
                    r = r + "电压:" + value.ToString();
            }
            ShowResult.Text = r;
        }
        private void RunAllStep_Click(object sender, EventArgs e)
        {

        }
        private void btnGetState_Click(object sender, EventArgs e)
        {
            string Socketstate = "";
            for (int i = 0; i < SocketMgr.GetInstance().socketArr.Length; i++)
            {
                //SocketType nozzletyple = (SocketType)i;
                Socketstate += "socketArr[" + i.ToString() + "]:" + SocketMgr.GetInstance().socketArr[i].socketState.ToString() + " # " + " ";
                Socketstate += "\n";
                //for (int j = 0; j < 8; j++)
                //    Socketstate += (j + 1).ToString() + " 1:" + SocketMgr.GetInstance().socketArr[i].socketcells[j].Cellstate.ToString() + " 2:" + SocketMgr.GetInstance().socketArr[i].socketcells[j].Cellstate2.ToString() + " # " + "\n";

            }
            File.WriteAllText("C:\\SocketState.txt", Socketstate);
        }
        private void Open_UV_Click(object sender, EventArgs e)
        {
            int uvTime = 0;
            try
            {
                uvTime = Convert.ToInt32(ShowResult.Text);
            }
            catch
            {
                MessageBox.Show("打开UV灯失败！", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            IOMgr.GetInstace().WriteIoBit("UV固化", true);
            Thread.Sleep(uvTime);
            IOMgr.GetInstace().WriteIoBit("UV固化", false);
        }
        #endregion

        #region 方法
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
                    //chartData1.ChartAreas[0].AxisY.Maximum = 140;
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
                    xVal.Add(Zpos + dStep * i);
                    if (xVal.Count < lenArr)
                    {
                        continue;
                    }
                    for (int index = 0; index < series.Length; index++)
                    {
                        series[index] = new Series();
                        series[index].ChartType = SeriesChartType.Spline;
                        series[index].BorderWidth = 3;
                        series[index].BorderColor = clor[index];
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

            }
        }
        public void ChangedIoOutState(string IoName, bool bStateCurrent)
        {
            int nRow = 0;
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ChangedIoOutState(IoName, bStateCurrent)));
            }
            else
            {


            }

        }
        public void ChagedPrItem(string name)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => { ChagedPrItem(name); }));
            }
            else
            {

                foreach (var tem in VisionMgr.GetInstance().GetItemNamesAndTypes())
                {
                }
            }
        }
        bool pr(string strVisionPrName, VisionControl visionControl1)
        {

            object objresults = VisionMgr.GetInstance().GetResult(strVisionPrName);
            IOperateParam visionShapParams = (IOperateParam)objresults;
            visionShapParams.ClearResult();
            string camname = VisionMgr.GetInstance().GetCamName(strVisionPrName);
            CameraMgr.GetInstance().BindWindow(camname, visionControl1);
            CameraMgr.GetInstance().ClaerPr(camname);

            CameraMgr.GetInstance().GetCamera(camname).SetTriggerMode(CameraModeType.Software);


            double? Expouse = VisionMgr.GetInstance().GetExpourseTime(strVisionPrName);
            double? Gain = VisionMgr.GetInstance().GetGain(strVisionPrName);

            CameraMgr.GetInstance().SetCamExposure(camname, (double)Expouse);
            CameraMgr.GetInstance().SetCamGain(camname, (double)Gain);
            CameraMgr.GetInstance().GetCamera(camname).GarbBySoftTrigger();
            HObject img = CameraMgr.GetInstance().GetCamera(camname).GetImage();
            VisionMgr.GetInstance().ProcessImage(strVisionPrName, img, visionControl1);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            bool bfail = false;
            while (true)
            {
                object objresult = VisionMgr.GetInstance().GetResult(strVisionPrName);
                if (objresult != null)
                {
                    IOperateParam visionShapParam = (IOperateParam)objresult;
                    if (visionShapParam.GetResultNum() > 0)
                    {
                        img.Dispose();
                        return true;
                    }
                }
                if (stopwatch.ElapsedMilliseconds > 5000)
                {
                    bfail = true;
                    img.Dispose();
                    return false;
                }

            }
            if (bfail)
                return false;
        }
        public bool UaxisRun(string posName, bool bmanual)
        {
            StationAA stationAAT = (StationAA)StationMgr.GetInstance().GetStation("AA站");
            if (!stationAAT.CheckLRuvCliyder(bmanual))
                return false;
            StationTable stationTable = (StationTable)StationMgr.GetInstance().GetStation("转盘站");
            return stationTable.Urun(posName, bmanual);


        }
        #endregion


        #region 手动AA步骤
        List<BtnStepAA> BtnAAStepList = new List<BtnStepAA>();
        private void ChangeTypeAA(object sender, EventArgs e)
        {
            //复原
            //BtnStepList[0].ColorValue;
            if (btnStep_0.BackColor == Color.Green)
            {
                btnStep_0.BackColor = Color.Cyan;
            }
            if (btnStep_1.BackColor == Color.Green)
            {
                btnStep_1.BackColor = Color.Cyan;
            }
            if (btnStep_2.BackColor == Color.Green)
            {
                btnStep_2.BackColor = Color.Cyan;
            }
            if (btnStep_3.BackColor == Color.Green)
            {
                btnStep_3.BackColor = Color.Cyan;
            }
            if (btnStep_4.BackColor == Color.Green)
            {
                btnStep_4.BackColor = Color.Cyan;
            }
            if (btnStep_5.BackColor == Color.Green)
            {
                btnStep_5.BackColor = Color.Cyan;
            }
            if (btnStep_6.BackColor == Color.Green)
            {
                btnStep_6.BackColor = Color.Cyan;
            }
            if (btnStep_7.BackColor == Color.Green)
            {
                btnStep_7.BackColor = Color.Cyan;
            }
            Button button = (Button)sender;
            button.BackColor = Color.Green;
        }
        private void EnableAA(List<int> EnableList)
        {
            btnStep_0.Enabled = false;
            btnStep_1.Enabled = false;
            btnStep_2.Enabled = false;
            btnStep_3.Enabled = false;
            btnStep_4.Enabled = false;
            btnStep_5.Enabled = false;
            btnStep_6.Enabled = false;
            btnStep_7.Enabled = false;
            btnStep_0.BackColor = Color.Gray;
            btnStep_1.BackColor = Color.Gray;
            btnStep_2.BackColor = Color.Gray;
            btnStep_3.BackColor = Color.Gray;
            btnStep_4.BackColor = Color.Gray;
            btnStep_5.BackColor = Color.Gray;
            btnStep_6.BackColor = Color.Gray;
            btnStep_7.BackColor = Color.Gray;
            foreach (int a in EnableList)
            {
                switch (a)
                {
                    case 0:
                        btnStep_0.Enabled = true; btnStep_0.BackColor = Color.Cyan; break;
                    case 1:
                        btnStep_1.Enabled = true; btnStep_1.BackColor = Color.Cyan; break;
                    case 2:
                        btnStep_2.Enabled = true; btnStep_2.BackColor = Color.Cyan; break;
                    case 3:
                        btnStep_3.Enabled = true; btnStep_3.BackColor = Color.Cyan; break;
                    case 4:
                        btnStep_4.Enabled = true; btnStep_4.BackColor = Color.Cyan; break;
                    case 5:
                        btnStep_5.Enabled = true; btnStep_5.BackColor = Color.Cyan; break;
                    case 6:
                        btnStep_6.Enabled = true; btnStep_6.BackColor = Color.Cyan; break;
                    case 7:
                        btnStep_7.Enabled = true; btnStep_7.BackColor = Color.Cyan; break;
                }

            }
        }
        private int GetSetpAA()
        {
            int count = 0;
            if (btnStep_0.BackColor == Color.Green)
            {
                count++; return 0;
            }
            if (btnStep_1.BackColor == Color.Green)
            {
                count++; return 1;
            }
            if (btnStep_2.BackColor == Color.Green)
            {
                count++; return 2;
            }
            if (btnStep_3.BackColor == Color.Green)
            {
                count++; return 3;
            }
            if (btnStep_4.BackColor == Color.Green)
            {
                count++; return 4;
            }
            if (btnStep_5.BackColor == Color.Green)
            {
                count++; return 5;
            }
            if (btnStep_6.BackColor == Color.Green)
            {
                count++; return 6;
            }
            if (btnStep_7.BackColor == Color.Green)
            {
                count++; return 7;
            }
            if (count != 1)
            {
                return -1;
            }
            return 0;
        }
        private void SetNestSetpAA(int step, EventArgs e)
        {
            switch (step)
            {
                case 0:
                    btnStep_0.BackColor = Color.Green; break;
                case 1:
                    btnStep_1.BackColor = Color.Green; break;
                case 2:
                    btnStep_2.BackColor = Color.Green; break;
                case 3:
                    btnStep_3.BackColor = Color.Green; break;
                case 4:
                    btnStep_4.BackColor = Color.Green; break;
                case 5:
                    btnStep_5.BackColor = Color.Green; break;
                case 6:
                    btnStep_6.BackColor = Color.Green; break;
                case 7:
                    btnStep_7.BackColor = Color.Green; break;
                default:
                    btnStep_7.BackColor = Color.Green; break;
            }
        }
        static int AAcount = 0;
        private async void RunAAStep_Click(object sender, EventArgs e)
        {
            UserTest.RunLog.Write($"点击【单步执行】", LogType.Info, PathHelper.LogPathManual);
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许单步执行！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            int StationIndex = 0;
            string Soket = "A";
            PathHelper.AA_ID = 0;
            if (radioBtnB.Checked)
            {
                PathHelper.AA_ID = 1;
                StationIndex = 1;
                Soket = "B";
            }


            // UserTest.ContiuneWhlie = false;
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
            StationDisp dospT = (StationDisp)StationMgr.GetInstance().GetStation("点胶站");
            if (MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisX) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisY) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisZ) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("点胶站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            StationAA stationAAT = (StationAA)StationMgr.GetInstance().GetStation("AA站");
            if (MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisX) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisY) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisZ) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("AA站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            StationTable stationTableTT = (StationTable)StationMgr.GetInstance().GetStation("转盘站");
            if (MotionMgr.GetInstace().GetHomeFinishFlag(stationTableTT.AxisU) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("转盘站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            //运行该步骤
            int t = GetSetpAA();
            if (t == -1)
            {
                MessageBox.Show("步骤异常，请关闭软件重新初始化！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            ManualStepAA.Enabled = false;
            ManualStepDisp.Enabled = false;
            AAGroupBox.Enabled = false;
            MFTest.Enabled = false;
            string AB = "A";
            string TabPosLens = "A工位夹取位";
            string TabPosAA = "A工位AA位";
            string TabPosEnd = "B工位AA位";
            bool checkBEnabled = false;
            bool isFail = false;
            int isFailChangeT = 0;
            if (radioBtnB.Checked)
            {
                AB = "B";
                TabPosLens = "B工位夹取位";
                TabPosAA = "B工位AA位";
                TabPosEnd = "A工位AA位";
            }

            await Task.Run(() =>
            {

                switch (t)
                {
                    case 0:
                        Form_Auto.EvenGetSN(StationIndex);
                        stationAAT.GoAAReadySafe(true);//AA z轴到安全位置
                        dospT.GoSanpReadySafe(true);   //点胶 z轴到安全位置
                        if (!stationTableTT.GoTableReadySafe(true))  //转盘旋转角度
                        {
                            isFailChangeT = -1;
                            isFail = true;
                            return;
                        }
                        checkBEnabled = true;
                        int SetSNA = ParamSetMgr.GetInstance().GetIntParam("A工位SetSN_ID");
                        int SetSNB = ParamSetMgr.GetInstance().GetIntParam("B工位SetSN_ID");
                        AAcount = 1;
                        ModuleMgr.Instance.SetSN(0, SetSNA);
                        ModuleMgr.Instance.SetSN(1, SetSNB);
                        // string PlayIni = ParamSetMgr.GetInstance().GetStringParam("点亮文件名称");
                        string playPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\Play\\play.ini";
                        //ModuleMgr.Instance.Init(DeviceIDA, playPath);
                        //ModuleMgr.Instance.Init(DeviceIDB, playPath);
                        if (!UaxisRun(TabPosEnd, true))
                        {
                            isFailChangeT = -1;
                            isFail = true;
                            return;

                        }//转盘初始位置
                        IOMgr.GetInstace().WriteIoBit($"{AB}Lens升降气缸", true);
                        break;
                    case 1:
                        string ab = "A";
                        if (radioBtnB.Checked)
                        {
                            IOMgr.GetInstace().WriteIoBit($"B模组上电", true);
                            Thread.Sleep(2000);
                            ab = "B";
                            Task.Run(() => Form_Auto.RunB());
                        }
                        else
                        {
                            IOMgr.GetInstace().WriteIoBit($"A模组上电", true);
                            Thread.Sleep(2000);
                            Task.Run(() => Form_Auto.RunA());
                        }
                        if (!IOMgr.GetInstace().ReadIoInBit($"{ab}治具LENS检测"))
                        {
                            MessageBox.Show($"{ab}治具没有检测到LENS", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            isFailChangeT = -1;
                            isFail = true;
                            return;
                        }
                        if (!UaxisRun(TabPosLens, true))
                        {
                            isFailChangeT = -1;
                            isFail = true;
                            return;
                        };//转盘转到夹取位置

                        if (stationAAT.StepLensPickRun(true) == StationAA.StationStep.Step_End)   //去夹取Lens
                        {
                            isFailChangeT = -1;
                            isFail = true;
                            return;
                        }
                        if (!UaxisRun(TabPosAA, true))
                        {
                            isFailChangeT = -1;
                            isFail = true;
                            return;
                        }//转盘转到AA位置
                        if (stationAAT.StepAAPositionRun(true) == StationAA.StationStep.Step_End)     //放到AA位
                        {
                            isFailChangeT = -1;
                            isFail = true;
                            return;
                        }
                        break;
                    case 2:
                        StationAA.StationStep centerStep = stationAAT.StepFindCenterRun(AAcount, null, true);
                        if (centerStep == StationAA.StationStep.Step_End)
                        {
                            isFailChangeT = -1;
                            isFail = true;
                            return;
                        }
                        if (centerStep == StationAA.StationStep.Step_Check)
                        {
                            isFailChangeT = 5;
                            isFail = true;
                            return;
                        }

                        //对心
                        break;
                    case 3:
                        if (stationAAT.StepThroughFocusRun(AAcount, null, true) == StationAA.StationStep.Step_End)
                        {
                            isFailChangeT = -1;
                            isFail = true;
                            return;
                        }

                        //TF
                        break;
                    case 4:
                        StationAA.StationStep tiltStep = stationAAT.StepTiltRun(AAcount, null, true);
                        if (tiltStep == StationAA.StationStep.Step_End)
                        {
                            isFailChangeT = -1;
                            isFail = true;
                            return;
                        }
                        if (tiltStep == StationAA.StationStep.Step_FindCenter)
                        {
                            AAcount++;
                            isFailChangeT = 2;
                            isFail = true;
                            return;
                        }

                        //Tilt
                        break;
                    case 5:
                        if (stationAAT.StepAACheckRun(null, true) == StationAA.StationStep.Step_End)
                        {
                            isFailChangeT = -1;
                            isFail = true;
                            return;
                        }
                        //Check
                        break;
                    case 6:
                        //UV
                        if (stationAAT.StepUVRun(null, true) == StationAA.StationStep.Step_End)
                        {
                            //isFailChangeT = -1;
                            //isFail = true;
                            //return;
                        }
                        break;
                    case 7:
                        //AA end
                        stationAAT.StepEndRun(null, true);
                        dospT.GoSanpReadySafe(true);   //点胶 z轴到安全位置
                        UaxisRun(TabPosEnd, true);//转盘转到AA位置
                        checkBEnabled = true;
                        string errCsv = CSVHelper.Instance.SaveToCSVPath(PathHelper.TestResultCsvPath, UserTest.TestResultAB[StationIndex]);
                        break;
                }





            });

            if (radioBtnB.Checked)
            {
                GlobelManualResetEvent.ContinueShowB.Set();
            }
            else
            {
                GlobelManualResetEvent.ContinueShowA.Set();
            }

            radioBtnB.Enabled = checkBEnabled;
            radioBtnA.Enabled = checkBEnabled;
            //    
            if (!isFail)//return 跳不出去
            {
                int indexList = BtnAAStepList.FindIndex(p => p.Index == t);
                EnableAA(BtnAAStepList[indexList].NextStepList);
                //   BtnStepList[indexList]
                SetNestSetpAA(BtnAAStepList[indexList].NextStep, e);
                if (t == 5)
                {
                    MFTest.Enabled = true;
                }
            }
            else
            {
                if (isFailChangeT != -1)
                {
                    if (isFailChangeT == 0)
                        isFailChangeT = 1;//设定为前一步骤
                    int indexList = BtnAAStepList.FindIndex(p => p.Index == isFailChangeT - 1);
                    EnableAA(BtnAAStepList[indexList].NextStepList);
                    //   BtnStepList[indexList]
                    SetNestSetpAA(BtnAAStepList[indexList].NextStep, e);
                }
                else
                {
                    MessageBox.Show($"{BtnAAStepList.Find(p => p.Index == t).ButtonValue.Text},失败！", "err", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            AAGroupBox.Enabled = true;
            ManualStepAA.Enabled = true;
            ManualStepDisp.Enabled = true;
        }
        private void CheckAB(object sender, EventArgs e)
        {
            radioBtnA.BackColor = Control.DefaultBackColor;
            radioBtnB.BackColor = Control.DefaultBackColor;
            if (radioBtnA.Checked)
                radioBtnA.BackColor = Color.Green;
            if (radioBtnB.Checked)
                radioBtnB.BackColor = Color.Green;
        }
        private void TableRunManual_Click(object sender, EventArgs e)
        {
            UserTest.RunLog.Write($"点击【转到上料位置】", LogType.Info, PathHelper.LogPathManual);
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许手动旋转！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (radioBtnA.Checked && radioBtnB.Checked)
            {
                MessageBox.Show("两个都被选择，异常！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            var result = MessageBox.Show("点击确实后，转盘会选在", "Warn", MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            if (result != DialogResult.OK)
            {
                return;
            }
            StationDisp dospT = (StationDisp)StationMgr.GetInstance().GetStation("点胶站");
            if (MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisX) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisY) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisZ) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("点胶站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            StationAA stationAAT = (StationAA)StationMgr.GetInstance().GetStation("AA站");
            if (MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisX) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisY) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisZ) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("AA站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            StationTable stationTableTT = (StationTable)StationMgr.GetInstance().GetStation("转盘站");
            if (MotionMgr.GetInstace().GetHomeFinishFlag(stationTableTT.AxisU) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("转盘站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            ManualStepAA.Enabled = false;
            AAGroupBox.Enabled = false;
            ManualStepDisp.Enabled = false;
            string TabPos = "B工位AA位";
            if (radioBtnB.Checked)
            {
                TabPos = "A工位AA位";
            }
            UaxisRun(TabPos, true);
            AAGroupBox.Enabled = true;
            ManualStepAA.Enabled = true;
            ManualStepDisp.Enabled = true;
        }
        private void CheckModule_CheckedChanged(object sender, EventArgs e)
        {
            radioBtnPreak.BackColor = Control.DefaultBackColor;
            radioBtnAve.BackColor = Control.DefaultBackColor;
            ParamSet paramSet = new ParamSet();
            ParamSetMgr.GetInstance().GetParam("AA模式选择", out paramSet);
            string currentProductFile = ParamSetMgr.GetInstance().CurrentWorkDir + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + (".xml");
            if (radioBtnPreak.Checked)
            {
                paramSet._strParamVal = "Peak";
                radioBtnPreak.BackColor = Color.Green;
            }
            if (radioBtnAve.Checked)
            {
                paramSet._strParamVal = "Average";
                radioBtnAve.BackColor = Color.Green;
            }
            ParamSetMgr.GetInstance().SetParam("AA模式选择", paramSet);
            ParamSetMgr.GetInstance().SaveParam(currentProductFile);
        }
        private void checkBoxA_CheckedChanged(object sender, EventArgs e)
        {

            checkBoxA.BackColor = Control.DefaultBackColor;
            ParamSet paramSetA = new ParamSet();

            ParamSetMgr.GetInstance().GetParam("屏蔽A工位", out paramSetA);
            string currentProductFile = ParamSetMgr.GetInstance().CurrentWorkDir + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + (".xml");

            if (checkBoxA.Checked)
            {
                paramSetA._strParamVal = false;
                checkBoxA.BackColor = Color.Green;
            }
            else
            {
                paramSetA._strParamVal = true;
            }
            ParamSetMgr.GetInstance().SetParam("屏蔽A工位", paramSetA);
            ParamSetMgr.GetInstance().SaveParam(currentProductFile);
        }
        private void checkBoxB_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxB.BackColor = Control.DefaultBackColor;
            ParamSet paramSetB = new ParamSet();
            ParamSetMgr.GetInstance().GetParam("屏蔽B工位", out paramSetB);
            string currentProductFile = ParamSetMgr.GetInstance().CurrentWorkDir + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + (".xml");
            if (checkBoxB.Checked)
            {
                paramSetB._strParamVal = false;
                checkBoxB.BackColor = Color.Green;
            }
            else
            {
                paramSetB._strParamVal = true;
            }
            ParamSetMgr.GetInstance().SetParam("屏蔽B工位", paramSetB);
            ParamSetMgr.GetInstance().SaveParam(currentProductFile);
        }
        private async void MFTest_Click(object sender, EventArgs e)
        {
            UserTest.RunLog.Write($"点击【MF标定】", LogType.Info, PathHelper.LogPathManual);
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许MF校准！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            ManualStepAA.Enabled = false;
            ManualStepDisp.Enabled = false;
            AAGroupBox.Enabled = false;
            MFTest.Enabled = false;
            int StationIndex = 0;
            if (radioBtnB.Checked)
                StationIndex = 1;
            if (StationIndex == 0)
            {
                GlobelManualResetEvent.ContinueShowA.Reset();
            }
            else
            {
                GlobelManualResetEvent.ContinueShowB.Reset();
            }
            StationAA stationAAT = (StationAA)StationMgr.GetInstance().GetStation("AA站");
            await Task.Run(() =>
            {
                if (!stationAAT.MFTest())
                {
                    MessageBox.Show($"MF标定失败！，请重新标定", "err", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                }
            });

            if (StationIndex == 0)
            {
                GlobelManualResetEvent.ContinueShowA.Set();
            }
            else
            {
                GlobelManualResetEvent.ContinueShowB.Set();
            }
            ManualStepAA.Enabled = true;
            ManualStepDisp.Enabled = true;
            AAGroupBox.Enabled = true;
            MFTest.Enabled = true;

        }
        //  [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private async void TestCompete_Click(object sender, EventArgs e)
        {
            UserTest.RunLog.Write($"点击【成品检测】", LogType.Info, PathHelper.LogPathManual);
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许成品检测！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            StationDisp dospT = (StationDisp)StationMgr.GetInstance().GetStation("点胶站");
            if (MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisX) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisY) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisZ) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("点胶站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            StationAA stationAAT = (StationAA)StationMgr.GetInstance().GetStation("AA站");
            if (MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisX) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisY) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisZ) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("AA站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            StationTable stationTableTT = (StationTable)StationMgr.GetInstance().GetStation("转盘站");
            if (MotionMgr.GetInstace().GetHomeFinishFlag(stationTableTT.AxisU) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("转盘站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            IOMgr.GetInstace().WriteIoBit("NG指示红灯", false);
            IOMgr.GetInstace().WriteIoBit("OK指示绿灯", false);
            StationMgr.GetInstance().Stop();
            IOMgr.GetInstace().WriteIoBit($"A模组上电", true);
            IOMgr.GetInstace().WriteIoBit($"B模组上电", true);
            int socketNum = 0;
            string slcketName = "A";
            string TabPosBefore = "B工位AA位";
            string TabPosTest = "A工位AA位";
            string TabPosAfter = "B工位AA位";
            ManualStepAA.Enabled = false;
            ManualStepDisp.Enabled = false;
            AAGroupBox.Enabled = false;
            TestCompete.Enabled = false;
            UserTest.ProductCheckResultAB[0] = new ProductCheckResult();
            UserTest.ProductCheckResultAB[1] = new ProductCheckResult();

            UserTest.ProductCheckResultAB[0].StarTime = DateTime.Now;
            UserTest.ProductCheckResultAB[1].StarTime = DateTime.Now;

            if (radioBtnB.Checked)
            {
                TabPosBefore = "A工位AA位";
                TabPosTest = "B工位AA位";
                TabPosAfter = "A工位AA位";
                socketNum = 1;
                slcketName = "B";
            }
            await Task.Run(() =>
            {
                dospT.GoSanpReadySafe(true);
                stationAAT.GoAAReadySafe(true);
                if (!UaxisRun(TabPosTest, true))
                {
                    return;
                }

                Form_Auto.EvenGetSN(socketNum);
                if (UserTest.TestResultAB[socketNum].SerialNumber == "NOSN")
                {
                    MessageBox.Show("请输入SN 或者屏蔽SN", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                UserTest.TestResultAB[socketNum].SocketerNumber = socketNum == 0 ? "A" : "B";


                int SetSNA = ParamSetMgr.GetInstance().GetIntParam("A工位SetSN_ID");
                int SetSNB = ParamSetMgr.GetInstance().GetIntParam("B工位SetSN_ID");
                //string PlayIni = ParamSetMgr.GetInstance().GetStringParam("点亮文件名称");
                string playPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\Play\\play.ini";
                if (socketNum == 0)
                    ModuleMgr.Instance.SetSN(0, SetSNA);
                else
                    ModuleMgr.Instance.SetSN(1, SetSNB);
                if (!ModuleMgr.Instance.Init(socketNum, playPath))
                {
                    //  MessageBox.Show("加载A工位点亮文件失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    // return;
                }

                Thread.Sleep(2000);

                ModuleMgr.Instance.Play(socketNum);
                //
                if (ParamSetMgr.GetInstance().GetBoolParam("是否关闭曝光") && ParamSetMgr.GetInstance().GetStringParam("工位点亮类型").Contains("DT"))
                {
                    int SlaveID = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("SlaveID"), 16);
                    int addr = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("关闭曝光地址"), 16);
                    int value = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("曝光值"), 16);
                    ModuleMgr.Instance.WriteI2C(socketNum, Convert.ToByte(SlaveID), 0, addr, value);
                }

                stationAAT.GoProductcheck(socketNum, CheckType.Product);
                stationAAT.StepEndRun(null, true);
                UserTest.ProductCheckResultAB[socketNum].EndTime = DateTime.Now;
                UserTest.ProductCheckResultAB[socketNum].Product = slcketName;
                UserTest.ProductCheckResultAB[socketNum].TestTime = (DateTime.Now - UserTest.ProductCheckResultAB[socketNum].StarTime).TotalSeconds;
                CSVHelper.Instance.SaveToCSVPath(PathHelper.ProductCsvPath, UserTest.ProductCheckResultAB[socketNum]);
                if (UserTest.ProductCheckResultAB[socketNum].Result)
                {
                    IOMgr.GetInstace().WriteIoBit("NG指示红灯", false);
                    IOMgr.GetInstace().WriteIoBit("OK指示绿灯", true);
                }
                else
                {
                    IOMgr.GetInstace().WriteIoBit("NG指示红灯", true);
                    IOMgr.GetInstace().WriteIoBit("OK指示绿灯", false);
                }
            });




            UaxisRun(TabPosAfter, true);
            TestCompete.Enabled = true;
            ManualStepAA.Enabled = true;
            ManualStepDisp.Enabled = true;
            AAGroupBox.Enabled = true;
        }
        #endregion

        #region 手动点胶步骤
        List<BtnStepAA> BtnDispStepList = new List<BtnStepAA>();
        private void ChangeTypeDisp(object sender, EventArgs e)
        {
            if (btnDisp_0.BackColor == Color.Green)
            {
                btnDisp_0.BackColor = Color.Cyan;
            }
            if (btnDisp_1.BackColor == Color.Green)
            {
                btnDisp_1.BackColor = Color.Cyan;
            }
            if (btnDisp_2.BackColor == Color.Green)
            {
                btnDisp_2.BackColor = Color.Cyan;
            }
            if (btnDisp_3.BackColor == Color.Green)
            {
                btnDisp_3.BackColor = Color.Cyan;
            }
            if (btnDisp_4.BackColor == Color.Green)
            {
                btnDisp_4.BackColor = Color.Cyan;
            }
            if (btnDisp_5.BackColor == Color.Green)
            {
                btnDisp_5.BackColor = Color.Cyan;
            }
            if (btnDisp_6.BackColor == Color.Green)
            {
                btnDisp_6.BackColor = Color.Cyan;
            }
            if (btnDisp_7.BackColor == Color.Green)
            {
                btnDisp_7.BackColor = Color.Cyan;
            }
            Button button = (Button)sender;
            button.BackColor = Color.Green;
        }
        private void EnableDisp(List<int> EnableList)
        {
            btnDisp_0.Enabled = false;
            btnDisp_1.Enabled = false;
            btnDisp_2.Enabled = false;
            btnDisp_3.Enabled = false;
            btnDisp_4.Enabled = false;
            btnDisp_5.Enabled = false;
            btnDisp_6.Enabled = false;
            btnDisp_7.Enabled = false;
            btnDisp_0.BackColor = Color.Gray;
            btnDisp_1.BackColor = Color.Gray;
            btnDisp_2.BackColor = Color.Gray;
            btnDisp_3.BackColor = Color.Gray;
            btnDisp_4.BackColor = Color.Gray;
            btnDisp_5.BackColor = Color.Gray;
            btnDisp_6.BackColor = Color.Gray;
            btnDisp_7.BackColor = Color.Gray;
            foreach (int a in EnableList)
            {
                switch (a)
                {
                    case 0:
                        btnDisp_0.Enabled = true; btnDisp_0.BackColor = Color.Cyan; break;
                    case 1:
                        btnDisp_1.Enabled = true; btnDisp_1.BackColor = Color.Cyan; break;
                    case 2:
                        btnDisp_2.Enabled = true; btnDisp_2.BackColor = Color.Cyan; break;
                    case 3:
                        btnDisp_3.Enabled = true; btnDisp_3.BackColor = Color.Cyan; break;
                    case 4:
                        btnDisp_4.Enabled = true; btnDisp_4.BackColor = Color.Cyan; break;
                    case 5:
                        btnDisp_5.Enabled = true; btnDisp_5.BackColor = Color.Cyan; break;
                    case 6:
                        btnDisp_6.Enabled = true; btnDisp_6.BackColor = Color.Cyan; break;
                    case 7:
                        btnDisp_7.Enabled = true; btnDisp_7.BackColor = Color.Cyan; break;
                }

            }
        }
        private int GetSetpDisp()
        {
            int count = 0;
            if (btnDisp_0.BackColor == Color.Green)
            {
                count++; return 0;
            }
            if (btnDisp_1.BackColor == Color.Green)
            {
                count++; return 1;
            }
            if (btnDisp_2.BackColor == Color.Green)
            {
                count++; return 2;
            }
            if (btnDisp_3.BackColor == Color.Green)
            {
                count++; return 3;
            }
            if (btnDisp_4.BackColor == Color.Green)
            {
                count++; return 4;
            }
            if (btnDisp_5.BackColor == Color.Green)
            {
                count++; return 5;
            }
            if (btnDisp_6.BackColor == Color.Green)
            {
                count++; return 6;
            }
            if (btnDisp_7.BackColor == Color.Green)
            {
                count++; return 7;
            }
            if (count != 1)
            {
                return -1;
            }
            return 0;
        }
        private void SetNestSetpDisp(int step, EventArgs e)
        {
            switch (step)
            {
                case 0:
                    btnDisp_0.BackColor = Color.Green; break;
                case 1:
                    btnDisp_1.BackColor = Color.Green; break;
                case 2:
                    btnDisp_2.BackColor = Color.Green; break;
                case 3:
                    btnDisp_3.BackColor = Color.Green; break;
                case 4:
                    btnDisp_4.BackColor = Color.Green; break;
                case 5:
                    btnDisp_5.BackColor = Color.Green; break;
                case 6:
                    btnDisp_6.BackColor = Color.Green; break;
                case 7:
                    btnDisp_7.BackColor = Color.Green; break;
                default:
                    btnDisp_7.BackColor = Color.Green; break;
            }
        }
        private async void RunDispStep_Click(object sender, EventArgs e)
        {
            //if (GlobalVariable.g_StationState != StationState.StationStateStop)
            //{
            //    MessageBox.Show("当前程序在运行，不允许单步执行！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}
            //int StationIndex = 0;
            //string Soket = "A";
            //PathHelper.Disp_ID = 0;
            //if (radioBtnB.Checked)
            //{
            //    PathHelper.Disp_ID = 1;
            //    StationIndex = 1;
            //    Soket = "B";
            //}


            //// UserTest.ContiuneWhlie = false;
            //if (!IOMgr.GetInstace().ReadIoInBit("急停"))
            //{
            //    MessageBox.Show("设备急停", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}
            //if (!IOMgr.GetInstace().ReadIoInBit("气源检测"))
            //{
            //    MessageBox.Show("气源检测异常", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}
            //if (!IOMgr.GetInstace().ReadIoInBit($"{Soket}治具盖上检测"))
            //{
            //    MessageBox.Show($"{Soket}治具盖上没有盖上", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}
            //StationDisp dospT = (StationDisp)StationMgr.GetInstance().GetStation("点胶站");
            //if (MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisX) != AxisHomeFinishFlag.Homed ||
            //    MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisY) != AxisHomeFinishFlag.Homed ||
            //    MotionMgr.GetInstace().GetHomeFinishFlag(dospT.AxisZ) != AxisHomeFinishFlag.Homed)
            //{
            //    MessageBox.Show("点胶站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}
            //StationAA stationAAT = (StationAA)StationMgr.GetInstance().GetStation("AA站");
            //if (MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisX) != AxisHomeFinishFlag.Homed ||
            //    MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisY) != AxisHomeFinishFlag.Homed ||
            //    MotionMgr.GetInstace().GetHomeFinishFlag(stationAAT.AxisZ) != AxisHomeFinishFlag.Homed)
            //{
            //    MessageBox.Show("AA站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}

            //StationTable stationTableTT = (StationTable)StationMgr.GetInstance().GetStation("转盘站");
            //if (MotionMgr.GetInstace().GetHomeFinishFlag(stationTableTT.AxisU) != AxisHomeFinishFlag.Homed)
            //{
            //    MessageBox.Show("转盘站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}
            ////运行该步骤
            //int t = GetSetpDisp();
            //if (t == -1)
            //{
            //    MessageBox.Show("步骤异常，请关闭软件重新初始化！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    return;
            //}
            //ManualStepAA.Enabled = false;
            //ManualStepDisp.Enabled = false;
            //AAGroupBox.Enabled = false;
            //MFTest.Enabled = false;
            //string AB = "A";
            //string TabPosAA = "A工位AA位";
            //string TabPosEnd = "B工位AA位";
            //bool checkBEnabled = false;
            //bool isFail = false;
            //int isFailChangeT = 0;
            //if (radioBtnB.Checked)
            //{
            //    AB = "B";
            //    TabPosAA = "B工位AA位";
            //    TabPosEnd = "A工位AA位";
            //}

            //await Task.Run(() =>
            //{

            //    switch (t)
            //    {
            //        case 0:
            //            Form_Auto.EvenGetSN(StationIndex);
            //            stationAAT.GoAAReadySafe(true);//AA z轴到安全位置
            //            dospT.GoSanpReadySafe(true);   //点胶 z轴到安全位置
            //            if (!stationTableTT.GoTableReadySafe(true))  //转盘旋转角度
            //            {
            //                isFailChangeT = -1;
            //                isFail = true;
            //                return;
            //            }
            //            checkBEnabled = true;
            //            if (!UaxisRun(TabPosEnd, true))
            //            {
            //                isFailChangeT = -1;
            //                isFail = true;
            //                return;

            //            }//转盘初始位置
            //            break;
            //        case 1:
            //            string ab = "A";
            //            if (radioBtnB.Checked)
            //            {
            //                ab = "B";
            //                Task.Run(() => Form_Auto.RunB());
            //            }
            //            else
            //            {
            //                Task.Run(() => Form_Auto.RunA());
            //            }
            //            if (!IOMgr.GetInstace().ReadIoInBit($"{ab}治具LENS检测"))
            //            {
            //                MessageBox.Show($"{ab}治具没有检测到LENS", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //                isFailChangeT = -1;
            //                isFail = true;
            //                return;
            //            }

            //            if (stationAAT.StepLensPickRun(true) == StationAA.StationStep.Step_End)   //去夹取Lens
            //            {
            //                isFailChangeT = -1;
            //                isFail = true;
            //                return;
            //            }
            //            if (!UaxisRun(TabPosAA, true))
            //            {
            //                isFailChangeT = -1;
            //                isFail = true;
            //                return;
            //            }//转盘转到AA位置
            //            if (stationAAT.StepAAPositionRun(true) == StationAA.StationStep.Step_End)     //放到AA位
            //            {
            //                isFailChangeT = -1;
            //                isFail = true;
            //                return;
            //            }
            //            break;
            //        case 2:
            //            StationAA.StationStep centerStep = stationAAT.StepFindCenterRun(AAcount, null, true);
            //            if (centerStep == StationAA.StationStep.Step_End)
            //            {
            //                isFailChangeT = -1;
            //                isFail = true;
            //                return;
            //            }
            //            if (centerStep == StationAA.StationStep.Step_Check)
            //            {
            //                isFailChangeT = 5;
            //                isFail = true;
            //                return;
            //            }

            //            //对心
            //            break;
            //        case 3:
            //            if (stationAAT.StepThroughFocusRun(AAcount, null, true) == StationAA.StationStep.Step_End)
            //            {
            //                isFailChangeT = -1;
            //                isFail = true;
            //                return;
            //            }

            //            //TF
            //            break;
            //        case 4:
            //            StationAA.StationStep tiltStep = stationAAT.StepTiltRun(AAcount, null, true);
            //            if (tiltStep == StationAA.StationStep.Step_End)
            //            {
            //                isFailChangeT = -1;
            //                isFail = true;
            //                return;
            //            }
            //            if (tiltStep == StationAA.StationStep.Step_FindCenter)
            //            {
            //                AAcount++;
            //                isFailChangeT = 2;
            //                isFail = true;
            //                return;
            //            }

            //            //Tilt
            //            break;
            //        case 5:
            //            if (stationAAT.StepAACheckRun(null, true) == StationAA.StationStep.Step_End)
            //            {
            //                isFailChangeT = -1;
            //                isFail = true;
            //                return;
            //            }
            //            //Check
            //            break;
            //        case 6:
            //            //UV
            //            if (stationAAT.StepUVRun(null, true) == StationAA.StationStep.Step_End)
            //            {
            //                //isFailChangeT = -1;
            //                //isFail = true;
            //                //return;
            //            }
            //            break;
            //        case 7:
            //            //AA end
            //            stationAAT.StepEndRun(null, true);
            //            dospT.GoSanpReadySafe(true);   //点胶 z轴到安全位置
            //            UaxisRun(TabPosEnd, true);//转盘转到AA位置
            //            checkBEnabled = true;
            //            string errCsv = CSVHelper.Instance.SaveToCSVPath(PathHelper.TestResultCsvPath, UserTest.TestResultAB[StationIndex]);
            //            break;
            //    }





            //});

            //if (radioBtnB.Checked)
            //{
            //    GlobelManualResetEvent.ContinueShowB.Set();
            //}
            //else
            //{
            //    GlobelManualResetEvent.ContinueShowA.Set();
            //}

            //radioBtnB.Enabled = checkBEnabled;
            //radioBtnA.Enabled = checkBEnabled;
            ////    
            //if (!isFail)//return 跳不出去
            //{
            //    int indexList = BtnAAStepList.FindIndex(p => p.Index == t);
            //    EnableAA(BtnAAStepList[indexList].NextStepList);
            //    //   BtnStepList[indexList]
            //    SetNestSetpAA(BtnAAStepList[indexList].NextStep, e);
            //    if (t == 5)
            //    {
            //        MFTest.Enabled = true;
            //    }
            //}
            //else
            //{
            //    if (isFailChangeT != -1)
            //    {
            //        if (isFailChangeT == 0)
            //            isFailChangeT = 1;//设定为前一步骤
            //        int indexList = BtnAAStepList.FindIndex(p => p.Index == isFailChangeT - 1);
            //        EnableAA(BtnAAStepList[indexList].NextStepList);
            //        //   BtnStepList[indexList]
            //        SetNestSetpAA(BtnAAStepList[indexList].NextStep, e);
            //    }
            //    else
            //    {
            //        MessageBox.Show($"{BtnAAStepList.Find(p => p.Index == t).ButtonValue.Text},失败！", "err", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //    }
            //}
            //AAGroupBox.Enabled = true;
            //ManualStepAA.Enabled = true;
            //ManualStepDisp.Enabled = true;
        }
        private void 单次采集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox_SelCamera.SelectedItem == null)
                return;
            for (int i = 0; i < comboBox_SelCamera.Items.Count; i++)
            {
                CameraMgr.GetInstance().ClaerPr(comboBox_SelCamera.Items[i].ToString());
                CameraMgr.GetInstance().SetTriggerSoftMode(comboBox_SelCamera.Items[i].ToString());
            }
            CameraMgr.GetInstance().BindWindow(comboBox_SelCamera.Text, visionControl1);
            CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).SetTriggerMode(CameraModeType.Software);
            Thread.Sleep(100);
            //CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).wnd = hWindowControl1.HalconID;
            CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).StartGrab();
            CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).GarbBySoftTrigger();

        }
        private void 连续采集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox_SelCamera.SelectedItem == null)
                return;
            for (int i = 0; i < comboBox_SelCamera.Items.Count; i++)
            {
                CameraMgr.GetInstance().ClaerPr(comboBox_SelCamera.Items[i].ToString());
                CameraMgr.GetInstance().SetTriggerSoftMode(comboBox_SelCamera.Items[i].ToString());
            }
            CameraMgr.GetInstance().BindWindow(comboBox_SelCamera.Text, visionControl1);
            CameraBase pCamer = CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString());
            pCamer.SetTriggerMode(CameraModeType.Software);
            Thread.Sleep(100);
            CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).SetAcquisitionMode();
            // CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).wnd = hWindowControl1.HalconID;
            CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).StartGrab();

        }
        private void 保存原图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                HObject img = CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).GetImage();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "原图另存";
                saveFileDialog.Filter = "Color BMP Files (*.bmp)|*.bmp|Jpeg Files (*.jpg)|*.jpg|Color TIF Files (*.tif)|*.tif";
                saveFileDialog.FilterIndex = 1;//设置默认文件类型显示顺序 
                saveFileDialog.RestoreDirectory = true; //点了保存按钮进入if (picBox1.Image != null)
                saveFileDialog.FileName = "Image";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (img != null)
                    {
                        //保存到磁盘文件
                        HOperatorSet.WriteImage(img.Clone(), "bmp", 0, saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存原图失败，{ex}", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

        }

        private async void GoDisp_Click(object sender, EventArgs e)
        {
            UserTest.RunLog.Write($"点击【点胶测试】", LogType.Info, PathHelper.LogPathManual);
            if (GlobalVariable.g_StationState != StationState.StationStateStop)
            {
                MessageBox.Show("当前程序在运行，不允许手动点胶！请先停止", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            StationDisp dosp = (StationDisp)StationMgr.GetInstance().GetStation("点胶站");
            if (MotionMgr.GetInstace().GetHomeFinishFlag(dosp.AxisX) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(dosp.AxisY) != AxisHomeFinishFlag.Homed ||
                MotionMgr.GetInstace().GetHomeFinishFlag(dosp.AxisZ) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("点胶站不是所有轴都回到原点", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            bool result = true;
            int StationIndex = TableData.GetInstance().GetSocketNum(1, 0.5) - 1;
            if (StationIndex < 0)
            {
                MessageBox.Show("转盘未转到点胶位置！", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            ManualStepAA.Enabled = false;
            ManualStepDisp.Enabled = false;
            AAGroupBox.Enabled = false;
            TestCompete.Enabled = false;
            GoDisp.Enabled = false;
            await Task.Run(() =>
            {
                dosp.StepGoSnap(this.visionControl1, true);
                dosp.GoSanpReadySafe(true);
            });
            MessageBox.Show(result ? "点胶检查OK" : "点胶检查NG", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            GoDisp.Enabled = true;
            ManualStepAA.Enabled = true;
            ManualStepDisp.Enabled = true;
            AAGroupBox.Enabled = true;
            TestCompete.Enabled = true;
            //ButtonEnable(false);
            //object[] objs = new object[2] { new object(), new object() };
            ////   await Task.Run(() => { Form_Auto.showShomethingOnAutoScreenHandler("BeamproFile_Read", objs); });
            // Form_Auto.showShomethingOnAutoScreenHandler("BeamproFile_Read", objs); 
            //textXWidthVal.Text = ((double)objs[0]).ToString();
            //textYWidthVal.Text = ((double)objs[1]).ToString();
            //// string strwidthX = "", strwidthY = "";
            //// double deamprofilex = 0;
            //// double deamprofiley = 0;
            //// //  My my = new My(NST_CrossLaserAATest.CrossLaserAATest.GetBeamWidth);
            //// //  my.BeginInvoke(ref deamprofilex, ref deamprofiley,null,null);
            //ButtonEnable(true);
            //// IAsyncResult asyncResult= this.BeginInvoke(new Action(() => {
            ////     NST_CrossLaserAATest.CrossLaserAATest.GetBeamWidth(ref deamprofilex, ref deamprofiley);
            ////    textXWidthVal.Text = deamprofilex.ToString();
            ////    textYWidthVal.Text = deamprofiley.ToString();
            ////}));

            //// textXWidthVal.Text = deamprofilex.ToString();
            //// textYWidthVal.Text = deamprofiley.ToString();
        }

        #endregion

        private async void btnEnumAllCam_Click(object sender, EventArgs e)
        {
            ModuleMgr.Instance.Stop(0, 1);

            List<string> strSns = new List<string>();
            int nCams = 0;
            bool bRtn = ModuleMgr.Instance.Enumerate(0, ref nCams, strSns, 1);
            cbxAllUSB.Items.Clear();
            if (strSns == null || strSns.Count <= 0)
                return;
            cbxAllUSB.Items.AddRange(strSns.ToArray());

        }

        private async void buttonTest_Click(object sender, EventArgs e)
        {
         //  int DeviceID_Calib = ParamSetMgr.GetInstance().GetIntParam("校准相机SetSN_ID");
            List<string> strSns = new List<string>();
            for (int i = 0; i < cbxAllUSB.Items.Count; i++)
                strSns.Add(cbxAllUSB.Items[i].ToString());
            int nSel = -1;

            nSel = cbxAllUSB.SelectedIndex;
            if (nSel < 0 || nSel >= strSns.Count)
                return;
            string selSn = "";
            await Task.Run(() =>
            {
                Bitmap bt = null;
                int i = nSel;
                bt = null;
                ModuleMgr.Instance.Stop(0, 1);
                ModuleMgr.Instance.SetSN(0, nSel, 1);
                Thread.Sleep(500);
                if (ModuleMgr.Instance.Init(0, "", 1))
                {
                    Thread.Sleep(300);
                    if (ModuleMgr.Instance.CaptureToBmpRGB(0, 1, ref bt, 1))
                        if (bt != null)
                        {
                            bt.Dispose();
                            selSn = strSns[i];
                            ParamSetMgr.GetInstance().SetStringParam("标准校准相机Sn", selSn);
                            MessageBox.Show($"标准校准相机Sn：{selSn}", "Err", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        else
                        {
                            MessageBox.Show($"标准校准相机Sn：没有找到", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }


                }
            });

        }
    }
    public class BtnStepAA
    {
        public int Index;
        public Button ButtonValue;
        public Color ColorValue;
        public int NextStep;
        public List<int> NextStepList;
    }
    public class BtnStepDisp
    {
        public int Index;
        public Button ButtonValue;
        public Color ColorValue;
        public int NextStep;
        public List<int> NextStepList;
    }
}
