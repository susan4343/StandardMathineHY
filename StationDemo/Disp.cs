using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using CameraLib;
//using HalconLib;
using MotionIoLib;
using System.IO;
using CommonTools;

using BaseDll;
using HalconDotNet;
using UserCtrl;
using UserData;
using OtherDevice;
using System.Threading.Tasks;
using System.Diagnostics;
using ModuleCapture;

namespace StationDemo
{
    public class StationDisp : CommonTools.Stationbase
    {
        #region 变量
        static bool isOpenSocket = false;
        int SocketNumOfUnloadLoad = 0;// 上料站对应夹具号
        string strStationName = "";
        #endregion

        #region 主流程
        public StationDisp(string strStationName, int[] arrAxis, string[] axisname, params string[] CameraName)
    : base(strStationName, arrAxis, axisname, CameraName)
        {

        }
        public StationDisp(CommonTools.Stationbase pb) : base(pb)
        {
            m_listIoInput.Add("急停");
            m_listIoInput.Add("气源检测");
            m_listIoInput.Add("安全门");
            m_listIoInput.Add("安全光栅");
            m_listIoInput.Add("点胶针头定位");
            m_listIoInput.Add("点胶液位感应");


            m_listIoOutput.Add("相机光源");
            m_listIoOutput.Add("点胶机");
            m_listIoOutput.Add("12V开启");
            m_listIoOutput.Add("蜂鸣");

        }
        public enum StationStep
        {
            [Description("0.初始化步骤")]
            Step_Init = 100,
            [Description("1.等待下料和上料")]
            Step_CheckIpos,
            [Description("2.去点胶流程")]
            Step_GoSnap,
            [Description("3.NG停止流程")]
            Step_Stop,

        }
        protected override bool InitStation()
        {
            ParamSetMgr.GetInstance().SetBoolParam("点胶工站初始化完成", false);
            //IOMgr.GetInstace().WriteIoBit("点胶机", false);
            IOMgr.GetInstace().WriteIoBit("相机光源", false);
            TableData.GetInstance().ResetStartCmd("A_UnLoadLoad");
            TableData.GetInstance().ResetStartCmd("B_UnLoadLoad");
            ClearAllStep();
            PushMultStep((int)StationStep.Step_Init);
            //  x = dispCalibParam.pointDispenseCalibs.Find(t => t.strPointName == "吐胶点").MachinePoint.x;
            //  y = dispCalibParam.pointDispenseCalibs.Find(t => t.strPointName == "吐胶点").MachinePoint.y;
            //  z = dispCalibParam.pointDispenseCalibs.Find(t => t.strPointName == "吐胶点").MachinePoint.z;
            Info("点胶站加载完成");
            return true;
        }
        protected override void StationWork(int step)
        {
            switch (step)
            {
                case (int)StationStep.Step_Init:
                    DelCurrentStep();
                    PushMultStep((int)StepInitRun());
                    break;
                case (int)StationStep.Step_CheckIpos:
                    DelCurrentStep();
                    PushMultStep((int)StepCheckIpos());
                    Thread.Sleep(10);
                    break;
                case (int)StationStep.Step_GoSnap:
                    UserTest.TestTimeInfo.DispBegin = DateTime.Now;
                    DelCurrentStep();
                    int StationIndex = TableData.GetInstance().GetSocketNum(1, 0.5) - 1;
                    UserTest.TestResultAB[StationIndex].AACount = 1;
                    PathHelper.Disp_ID = StationIndex;
                    PushMultStep((int)StepGoSnap(this.VisionControl));
                    UserTest.TestTimeInfo.DispEnd = DateTime.Now;
                    UserTest.TestTimeInfo.DispTime = (DateTime.Now - UserTest.TestTimeInfo.DispBegin).TotalSeconds;
                    break;
                case (int)StationStep.Step_Stop:
                    DelCurrentStep();
                    break;
            }


        }
        public StationStep StepInitRun(bool bmanual = false)
        {
            StationStep step = StationStep.Step_Stop;
            // Form_Auto.ShowEventOnAutoScreen("回零", null);
            ParamSetMgr.GetInstance().SetBoolParam("点胶工站初始化完成", false);
            ParamSetMgr.GetInstance().SetBoolParam("重新上料", false);
            // IOMgr.GetInstace().WriteIoBit("点胶机", false);
            IOMgr.GetInstace().WriteIoBit("相机光源", false);
            IOMgr.GetInstace().WriteIoBit($"ALens升降气缸", true);
            IOMgr.GetInstace().WriteIoBit($"BLens升降气缸", true);
            if (!GoSanpHome(bmanual))
            {
                Err("点胶站 回零失败");
                throw new Exception("点胶轴回零失败！");
            }
            GoSanpReadySafe(bmanual);
            ParamSetMgr.GetInstance().SetBoolParam("点胶工站初始化完成", true);
            step = StationStep.Step_CheckIpos;
            return step;


        }
        public StationStep StepCheckIpos(bool bmanual = false)
        {
            StationStep step = StationStep.Step_CheckIpos;
            bool dEnableA = ParamSetMgr.GetInstance().GetBoolParam("屏蔽A工位");
            bool dEnableB = ParamSetMgr.GetInstance().GetBoolParam("屏蔽B工位");
            bool bA_UnLoadLoadStart = TableData.GetInstance().GetStationStartCmd("A_UnLoadLoad") && !dEnableA;
            bool bB_UnLoadLoadStart = TableData.GetInstance().GetStationStartCmd("B_UnLoadLoad") && !dEnableB;
            if (bA_UnLoadLoadStart || bB_UnLoadLoadStart || ParamSetMgr.GetInstance().GetBoolParam("重新上料"))
            {
                ParamSetMgr.GetInstance().SetBoolParam("重新上料", false);
                TableData.GetInstance().ResetStartCmd("A_UnLoadLoad");
                TableData.GetInstance().ResetStartCmd("B_UnLoadLoad");
                strStationName = TableData.GetInstance().GetStationName();
                if (strStationName == "A_Pick" || strStationName == "B_Pick")
                {
                    TableData.GetInstance().SetStationResult("A_UnLoadLoad", true);
                    TableData.GetInstance().SetStationResult("B_UnLoadLoad", true);
                    return step;
                }
                SocketNumOfUnloadLoad = TableData.GetInstance().GetSocketNum(1, 0.5) - 1;
                if (SocketNumOfUnloadLoad == 0 && dEnableA)
                {
                    TableData.GetInstance().SetStationResult("A_UnLoadLoad", true);
                    TableData.GetInstance().SetStationResult("B_UnLoadLoad", true);
                    return step;
                }
                if (SocketNumOfUnloadLoad == 1 && dEnableB)
                {
                    TableData.GetInstance().SetStationResult("A_UnLoadLoad", true);
                    TableData.GetInstance().SetStationResult("B_UnLoadLoad", true);
                    return step;
                }
                IOMgr.GetInstace().WriteIoBit("NG指示红灯", false);
                IOMgr.GetInstace().WriteIoBit("OK指示绿灯", true);
                Info("开始上下料，安全光栅开始屏蔽");
                ParamSetMgr.GetInstance().SetBoolParam("启用安全光栅", false);
                ParamSetMgr.GetInstance().SetBoolParam("可以上下料", true);

                SocketState state = SocketMgr.GetInstance().socketArr[SocketNumOfUnloadLoad].socketState;
                if (state == SocketState.HaveOK || state == SocketState.HaveNG)
                {
                    string lightColor = state == SocketState.HaveOK ? "OK指示绿灯" : "NG指示红灯";
                    string fp = state == SocketState.HaveOK ? "P" : "F";
                    UserTest.TestResultAB[SocketNumOfUnloadLoad].SocketerNumber = SocketNumOfUnloadLoad == 0 ? "A" : "B";
                    #region 计算CT赋值
                    if (UserTest.ProductCount.CountCTAll == 0)
                    {
                        UserTest.ProductCount.StarCTTime = DateTime.Now;
                        UserTest.ProductCount.EndCTTime = DateTime.Now;
                        UserTest.ProductCount.CountCTTime = 0;
                    }
                    else
                    {
                        UserTest.ProductCount.CountCTTime += (DateTime.Now - UserTest.ProductCount.EndCTTime).TotalSeconds;
                        UserTest.ProductCount.EndCTTime = DateTime.Now;
                        if ((DateTime.Now - UserTest.ProductCount.StarCTTime).TotalMinutes > ParamSetMgr.GetInstance().GetDoubleParam("UPH计算时长"))
                        {
                            if (UserTest.ProductCount.CountCTAll > ParamSetMgr.GetInstance().GetDoubleParam("UPH计算时长范围内最少个数"))
                                UserTest.ProductCount.UPH = (UserTest.ProductCount.CountCTAll) * 3600 / UserTest.ProductCount.CountCTTime;
                            //清除
                            UserTest.ProductCount.CountCTAll = -1;
                        }

                    }
                    UserTest.ProductCount.CountCTAll++;
                    if (SocketNumOfUnloadLoad == 0)
                    {
                        if (fp == "P")
                            UserTest.ProductCount.OKA++;
                        else
                        {
                            UserTest.ProductCount.NGA++;
                            if (!UserTest.FailResultAB.Play)
                            {
                                UserTest.ProductCount.PlayFailA++;
                                UserTest.FailResultAB.OC = true;
                                UserTest.FailResultAB.SFR = true;
                                UserTest.FailResultAB.Tilt = true;
                            }
                            if (!UserTest.FailResultAB.OC)
                            {
                                UserTest.ProductCount.OCFailA++;
                                UserTest.FailResultAB.Play = true;
                                UserTest.FailResultAB.SFR = true;
                                UserTest.FailResultAB.Tilt = true;
                            }
                            if (!UserTest.FailResultAB.Tilt)
                            {
                                UserTest.ProductCount.TiltFailA++;
                                UserTest.FailResultAB.OC = true;
                                UserTest.FailResultAB.SFR = true;
                                UserTest.FailResultAB.Play = true;
                            }
                            if (!UserTest.FailResultAB.SFR)
                            {
                                UserTest.ProductCount.SFRFailA++;
                                UserTest.FailResultAB.OC = true;
                                UserTest.FailResultAB.Play = true;
                                UserTest.FailResultAB.Tilt = true;
                            }
                            if (UserTest.FailResultAB.Play && UserTest.FailResultAB.OC && UserTest.FailResultAB.Tilt && UserTest.FailResultAB.SFR)
                            {
                                UserTest.ProductCount.OtherFailA++;
                            }

                        }

                    }
                    else
                    {
                        if (fp == "P")
                            UserTest.ProductCount.OKB++;
                        else
                        {
                            UserTest.ProductCount.NGB++;
                            if (!UserTest.FailResultAB.Play)
                            {
                                UserTest.ProductCount.PlayFailB++;
                                UserTest.FailResultAB.OC = true;
                                UserTest.FailResultAB.SFR = true;
                                UserTest.FailResultAB.Tilt = true;
                            }
                            if (!UserTest.FailResultAB.OC)
                            {
                                UserTest.ProductCount.OCFailB++;
                                UserTest.FailResultAB.Play = true;
                                UserTest.FailResultAB.SFR = true;
                                UserTest.FailResultAB.Tilt = true;
                            }
                            if (!UserTest.FailResultAB.Tilt)
                            {
                                UserTest.ProductCount.TiltFailB++;
                                UserTest.FailResultAB.OC = true;
                                UserTest.FailResultAB.SFR = true;
                                UserTest.FailResultAB.Play = true;
                            }
                            if (!UserTest.FailResultAB.SFR)
                            {
                                UserTest.ProductCount.SFRFailB++;
                                UserTest.FailResultAB.OC = true;
                                UserTest.FailResultAB.Play = true;
                                UserTest.FailResultAB.Tilt = true;
                            }
                            if (UserTest.FailResultAB.Play && UserTest.FailResultAB.OC && UserTest.FailResultAB.Tilt && UserTest.FailResultAB.SFR)
                            {
                                UserTest.ProductCount.OtherFailB++;
                            }

                        }

                    }


                    #endregion
                    Form_Auto.EvenShowCT(SocketNumOfUnloadLoad + 1);
                    UserTest.TestResultAB[SocketNumOfUnloadLoad].Result = fp == "P" ? true : false;
                    if (fp == "P")
                        UserTest.TestResultAB[SocketNumOfUnloadLoad].FailStep = "Pass";
                    UserTest.TestResultAB[SocketNumOfUnloadLoad].EndTime = DateTime.Now;
                    UserTest.TestResultAB[SocketNumOfUnloadLoad].TestTime = (UserTest.TestResultAB[SocketNumOfUnloadLoad].EndTime - UserTest.TestResultAB[SocketNumOfUnloadLoad].StarTime).TotalSeconds;
                    string errCsv = CSVHelper.Instance.SaveToCSVPath(PathHelper.TestResultCsvPath, UserTest.TestResultAB[SocketNumOfUnloadLoad]);
                    IOMgr.GetInstace().WriteIoBit("OK指示绿灯", false);
                    IOMgr.GetInstace().WriteIoBit("NG指示红灯", false);
                    IOMgr.GetInstace().WriteIoBit(lightColor, true);
                    Info($"保存OK结果：cvs={errCsv}.");
                    if (SocketNumOfUnloadLoad == 0)
                    {
                        IOMgr.GetInstace().WriteIoBit($"ALens升降气缸", true);
                    }
                    else
                    {
                        IOMgr.GetInstace().WriteIoBit($"BLens升降气缸", true);
                    }
                    //PlaceToSocket(SocketNumOfUnloadLoad);
                    UserTest.CTTestAB[SocketNumOfUnloadLoad].Star = false;
                    UserTest.CTTestAB[SocketNumOfUnloadLoad].End = false;
                    UserTest.CTTestAB[SocketNumOfUnloadLoad].Show = true;
                }
                //同意下料接口
                SysFunConfig.LodUnloadPatten.ULoad(SocketNumOfUnloadLoad== 0? "A":"B", bmanual);
            retry_check_Start:
                ParamSetMgr.GetInstance().SetBoolParam("启用安全光栅", false);
                WaranResult waranResult = doWhileCheckStartSignal.doSomething(this, doWhileCheckStartSignal, false, new object[] { this });
                if (waranResult == WaranResult.Retry)
                    goto retry_check_Start;
                IOMgr.GetInstace().WriteIoBit($"相机光源", true);
                IOMgr.GetInstace().WriteIoBit("启动按钮灯", false);
                IOMgr.GetInstace().WriteIoBit("OK指示绿灯", false);
                IOMgr.GetInstace().WriteIoBit("NG指示红灯", false);
                if (!IOMgr.GetInstace().ReadIoInBit($"点胶液位感应") && ParamSetMgr.GetInstance().GetBoolParam("点胶液位检测"))
                {
                    MessageBox.Show($"点胶液位感应有信号，胶水已经用完！请更换，或者屏蔽[点胶液位检测]。", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    goto retry_check_Start;
                }
                Form_Auto.EvenGetSN(SocketNumOfUnloadLoad);
                if (UserTest.TestResultAB[SocketNumOfUnloadLoad].SerialNumber == "NOSN")
                {
                    MessageBox.Show("请输入SN 或者屏蔽SN,重新启动", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    goto retry_check_Start;
                }
                if (SocketNumOfUnloadLoad == 0)
                {
                    UserTest.ProductCount.CompeteA++;
                }
                else
                {
                    UserTest.ProductCount.CompeteB++;
                }
                ParamSetMgr.GetInstance().SetBoolParam("启用安全光栅", true);
                ParamSetMgr.GetInstance().SetBoolParam("可以上下料", false);
                ParamSetMgr.GetInstance().SetBoolParam("AA完成", false);
                ParamSetMgr.GetInstance().SetBoolParam("点胶完成", false);
                ParamSetMgr.GetInstance().SetBoolParam("启动AA", false);
                ParamSetMgr.GetInstance().SetBoolParam("启动点胶", false);
                Info("开始上下料，安全光栅开始启用");
                ParamSetMgr.GetInstance().SetBoolParam("启用安全光栅", true);
                UserTest.CTTestAB[SocketNumOfUnloadLoad].Star = true;
                UserTest.CTTestAB[SocketNumOfUnloadLoad].End = false;
                UserTest.CTTestAB[SocketNumOfUnloadLoad].Show = false;
          
                step = StationStep.Step_GoSnap;
            }
            return step;


        }
        public StationStep StepGoSnap(VisionControl visionControl, bool bmanual = false)
        {
            StationStep step = StationStep.Step_Stop;
            //拍照识别是否有料,计算偏差（6月后做）
            ParamSetMgr.GetInstance().SetBoolParam("启动点胶", true);
            //开始点胶(优先实现画轨迹)
            //获取当前夹取位置是A工位还是B工位
            int StationIndex = TableData.GetInstance().GetSocketNum(1, 0.5) - 1;
            PathHelper.Disp_ID = StationIndex;
            string stationAAName = StationIndex == 0 ? "A" : "B";
            ParamSetMgr.GetInstance().SetBoolParam($"{stationAAName}工位点胶", true);
            double CenterX = ParamSetMgr.GetInstance().GetDoubleParam("产品点胶X轴半径");
            double CenterY = ParamSetMgr.GetInstance().GetDoubleParam("产品点胶Y轴半径");
            double DelZ = ParamSetMgr.GetInstance().GetDoubleParam("点胶Z轴上升高度偏差");
            int DispDelay = ParamSetMgr.GetInstance().GetIntParam("出胶延迟");
            double DispRunAngle = ParamSetMgr.GetInstance().GetDoubleParam("画胶角度");
            double DispEndAngle = ParamSetMgr.GetInstance().GetDoubleParam("收胶角度");
            bool DispEnable = ParamSetMgr.GetInstance().GetBoolParam("屏蔽点胶");
            bool DispPhoto = ParamSetMgr.GetInstance().GetBoolParam("点胶相机拍照定位");
            double SafeZ = GetStationPointDic()[$"安全位置"].pointZ;
            double X = 0;
            double Y = 0;
            double DispPhotoX = GetStationPointDic()[$"{stationAAName}工位拍照位"].pointX;
            double DispPhotoY = GetStationPointDic()[$"{stationAAName}工位拍照位"].pointY;
            double DispPhotoZ = GetStationPointDic()[$"{stationAAName}工位拍照位"].pointZ;
            double DispPosX = GetStationPointDic()[$"{stationAAName}工位点胶位"].pointX;
            double DispPosY = GetStationPointDic()[$"{stationAAName}工位点胶位"].pointY;
            double DispPosZ = GetStationPointDic()[$"{stationAAName}工位点胶位"].pointZ;
            double x1 = GetStationPointDic()["安全位置"].pointX;
            CameraBase cam = null;
            if (!ParamSetMgr.GetInstance().GetBoolParam("屏蔽上相机"))
            {
                cam = CameraMgr.GetInstance().GetCamera("Top");
                cam.BindWindow(visionControl);
                Task.Run(() =>
                {
                    cam.StopGrap();
                    cam.SetTriggerMode(CameraModeType.Software);
                    cam.SetGain(ParamSetMgr.GetInstance().GetIntParam("点胶相机增益"));
                    cam.SetExposureTime(ParamSetMgr.GetInstance().GetIntParam("点胶相机曝光"));
                    cam.StartGrab();
                });
            }
            IOMgr.GetInstace().WriteIoBit($"相机光源", true);
        retry_uplens:
            IOMgr.GetInstace().WriteIoBit($"{stationAAName}Lens升降气缸", true);
            WaranResult waranResult1 = CheckIobyName($"{stationAAName}Lens上升到位", true, $"{stationAAName}Lens上升到位", bmanual);
            if (waranResult1 == WaranResult.Retry)
                goto retry_uplens;
            MoveSigleAxisPosWaitInpos(AxisZ, SafeZ, (double)SpeedType.High, 0.005, bmanual, this);
            MoveY(DispPhotoY, SpeedType.High);

            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisZ }, new double[] { DispPhotoX, DispPhotoZ }, new double[] { (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);
            HObject img = null;




            if (!ParamSetMgr.GetInstance().GetBoolParam("屏蔽上相机"))
            {
                img = cam.GetImage();
                if (img == null || !img.IsInitialized())
                {
                    img = cam.GetImage();
                }
                else
                {
                    ImageHelper.Instance.SaveImage($"{PathHelper.ImagePathDisp}{DateTime.Now.ToString("HHmmssfff")}.bmp", "bmp", img.Clone());
                }
            }


            IOMgr.GetInstace().WriteIoBit($"相机光源", false);
            //去画胶
            Task.Run(() =>
            {
                if (ParamSetMgr.GetInstance().GetBoolParam("是否选择程控电源"))
                {
                    double valueVoltage = ParamSetMgr.GetInstance().GetDoubleParam("程控电源电压");
                    OtherDevices.ckPower.SetVoltage(StationIndex + 1, valueVoltage);
                    double valueCurrent = ParamSetMgr.GetInstance().GetDoubleParam("程控电源电流");
                    OtherDevices.ckPower.SetCurrent(StationIndex + 1, valueCurrent);
                }
                IOMgr.GetInstace().WriteIoBit($"{stationAAName}模组上电", true);
                IOMgr.GetInstace().WriteIoBit($"12V开启", ParamSetMgr.GetInstance().GetBoolParam("是否开启非程控12V"));
            });
            if (DispEnable)
            {
                step = StationStep.Step_CheckIpos;
                MoveSigleAxisPosWaitInpos(AxisZ, SafeZ, (double)SpeedType.High, 0.005, bmanual, this);
                TableData.GetInstance().SetStationResult("A_UnLoadLoad", true);
                TableData.GetInstance().SetStationResult("B_UnLoadLoad", true);
                return step;
            }
            if (DispPhoto)
            {

                ////MoveY(DispPhotoY, SpeedType.High);
                ////MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisZ }, new double[] { DispPhotoX, DispPhotoZ }, new double[] { (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);

                //拍照计算 +半径
                double CenterdelX = 0;
                double CenterdelY = 0;
                X = CenterX + CenterdelX;
                Y = CenterdelY;
                DispPosX = DispPhotoX - X;
                DispPosY = DispPhotoY + CenterdelY;
            }
            else
            {
                X = CenterX;
                Y = 0;
            }
            //  IOMgr.GetInstace().WriteIoBit("点胶机", false);
            bool brtnExc = true;
            MotionMgr.GetInstace().AddAxisToGroup("点胶群组", 2, new int[] { AxisX, AxisY });

            MoveY(DispPosY, SpeedType.High);
            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisZ }, new double[] { DispPosX, DispPosZ + DelZ + 10 }, new double[] { (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);

            //    MoveMulitAxisPosWaitInpos(new int[] { AxisX }, new double[] { DispPosX }, new double[] { (double)SpeedType.High }, 0.005, bmanual, this);
            IOMgr.GetInstace().WriteIoBit("点胶机", false);
            MoveMulitAxisPosWaitInpos(new int[] { AxisZ }, new double[] { DispPosZ + DelZ }, new double[] { (double)SpeedType.High }, 0.005, bmanual, this);
            if (ParamSetMgr.GetInstance().GetStringParam("点胶轨迹") == "Circle")
            {
                DateTime dateTime = DateTime.Now;
                if (DispDelay >= 0)
                {
                    IOMgr.GetInstace().WriteIoBit("点胶机", true);
                    Thread.Sleep(DispDelay);
                }
                else
                {
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            if ((DateTime.Now - dateTime).TotalSeconds > Math.Abs(DispDelay) / 1000.0)
                            {
                                IOMgr.GetInstace().WriteIoBit("点胶机", true);
                                break;
                            }
                            Thread.Sleep(10);
                        }


                    });

                }






                //走

                brtnExc &= MotionMgr.GetInstace().ClearBufMove("点胶群组");
                brtnExc &= MotionMgr.GetInstace().RestGpErr("点胶群组");
                //  GpState gps = MotionMgr.GetInstace().GetGpState("点胶群组");
                brtnExc &= MotionMgr.GetInstace().AddBufMove("点胶群组", BufMotionType.buf_Arc2dAbsAngleCW, 1, 2, (double)SpeedType.Mid, (double)SpeedType.Mid, new double[2] { X, Y }, new double[2] { DispPosZ, 0 });
                brtnExc &= MotionMgr.GetInstace().BufTrans("点胶群组");//M314没有buf运动，所以放在这里star
                brtnExc &= MotionMgr.GetInstace().BufStart("点胶群组");
                Thread.Sleep(400);
                if (!WaitXY(DispPosX, DispPosY))
                {
                    IOMgr.GetInstace().WriteIoBit("点胶机", false);
                    return step;
                }
                IOMgr.GetInstace().WriteIoBit("点胶机", false);
            }
            else
            {
                IOMgr.GetInstace().WriteIoBit("点胶机", true);
                Thread.Sleep(DispDelay);
                MoveY(DispPosY + CenterY * 10, SpeedType.Mid);
                MoveMulitAxisPosWaitInpos(new int[] { AxisX }, new double[] { DispPosX + CenterX * 2 }, new double[] { (double)SpeedType.Mid }, 0.005, bmanual, this);
                MoveY(DispPosY - CenterY * 10, SpeedType.Mid);
                MoveMulitAxisPosWaitInpos(new int[] { AxisX }, new double[] { DispPosX }, new double[] { (double)SpeedType.Mid }, 0.005, bmanual, this);
                MoveY(DispPosY, SpeedType.Mid);
                IOMgr.GetInstace().WriteIoBit("点胶机", false);
            }
            IOMgr.GetInstace().WriteIoBit($"相机光源", true);

            MoveMulitAxisPosWaitInpos(new int[] { AxisZ }, new double[] { DispPosZ + 3 }, new double[] { (double)SpeedType.Mid }, 0.005, bmanual, this);

            MoveMulitAxisPosWaitInpos(new int[] { AxisZ }, new double[] { DispPosZ + 10 }, new double[] { (double)SpeedType.High }, 0.005, bmanual, this);
            if (!ParamSetMgr.GetInstance().GetBoolParam("屏蔽上相机"))
            {
                MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisZ }, new double[] { DispPhotoX, DispPhotoZ }, new double[] { (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);
                // MoveSigleAxisPosWaitInpos(AxisX, DispPhotoX, (double)SpeedType.High, 0.005, bmanual, this);
                MoveY(DispPhotoY, SpeedType.High);
                HObject img2 = cam.GetImage();
                if (img2 == null || !img2.IsInitialized())
                {
                    img2 = cam.GetImage();
                }
                else
                {
                    ImageHelper.Instance.SaveImage($"{PathHelper.ImagePathDisp}{DateTime.Now.ToString("HHmmssfff")}.bmp", "bmp", img2.Clone());
                }
            }
            IOMgr.GetInstace().WriteIoBit($"相机光源", false);

            MoveMulitAxisPosWaitInpos(new int[] { AxisZ }, new double[] { SafeZ }, new double[] { (double)SpeedType.High }, 0.005, bmanual, this);
            MoveSigleAxisPosWaitInpos(AxisX, x1, (double)SpeedType.High, 0.005, bmanual, this);
            step = StationStep.Step_CheckIpos;
            //if (DialogResult.OK!= MessageBox.Show("请确定点胶效果是否OK，OK按确定", "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly))
            //{
            //    ParamSetMgr.GetInstance().SetBoolParam("重新上料", true);
            //    return step;
            //}
            ParamSetMgr.GetInstance().SetBoolParam("重新上料", false);
            SocketMgr.GetInstance().SetSocketState(SocketNumOfUnloadLoad, SocketState.Have);
            TableData.GetInstance().SetStationResult("A_UnLoadLoad", true);
            TableData.GetInstance().SetStationResult("B_UnLoadLoad", true);
            ParamSetMgr.GetInstance().SetBoolParam("点胶完成", true);
            return step;


        }
        #endregion

        #region 方法
        public bool GoSanpHome(bool bmanual = false)
        {
            IOMgr.GetInstace().WriteIoBit($"相机光源", true);
            Info("点胶开始回零");
            ParamSetMgr.GetInstance().SetBoolParam("点胶回零完成", false);
            MotionMgr.GetInstace().ServoOn((short)AxisZ);
            MotionMgr.GetInstace().ServoOn((short)AxisX);
            MotionMgr.GetInstace().ServoOn((short)AxisY);
            if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisZ) != AxisHomeFinishFlag.Homed)
            {
                HomeSigleAxisPosWaitInpos(AxisZ, this, 120000, bmanual);
            }
            if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisX) != AxisHomeFinishFlag.Homed)
            {
                HomeSigleAxisPosWaitInpos(AxisX, this, 120000, bmanual);
            }
            if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisY) != AxisHomeFinishFlag.Homed)
            {
                HomeSigleAxisPosWaitInpos(AxisY, this, 120000, bmanual);
            }
            ParamSetMgr.GetInstance().SetBoolParam("点胶回零完成", true);
            Info("点胶回零完成");
            return true;

        }
        public void GoSanpReadySafe(bool bmanual = false)
        {
            double x1 = GetStationPointDic()["安全位置"].pointX;
            double y1 = GetStationPointDic()["安全位置"].pointY;
            double z1 = GetStationPointDic()["安全位置"].pointZ;
            Info("点胶站：移动到准备位置");
            MoveSigleAxisPosWaitInpos(AxisZ, z1, (double)SpeedType.High, 0.05, bmanual);//先往上移动
            MoveY(y1, SpeedType.High);
            MoveMulitAxisPosWaitInpos(new int[] { AxisX }, new double[] { x1 }, new double[] { (double)SpeedType.High }, 0.05, bmanual);
        }
        public void PlaceToSocket(int index, bool bmanual = false)
        {
            WaranResult waranResult;
            string name = index == 1 ? "A" : "B";
            string strCloseSocketIoControlName = $"{name}Lens升降气缸";

        retry_up:
            IOMgr.GetInstace().WriteIoBit(strCloseSocketIoControlName, true);
            Thread.Sleep(20);
            waranResult = CheckIobyName($"{name}Lens上升到位", true, $"手动取料时,{name}#Lens气缸上升失败 ", bmanual);
            if (waranResult == WaranResult.Retry)
                goto retry_up;


        }
        public bool GlueCheck(HObject Image1, HObject Image2)
        {
            bool result = true;
            string pathImage = ParamSetMgr.GetInstance().GetStringParam("保存相机图片路径");
            string path = pathImage + "\\" + DateTime.Now.ToString("yy-MM-dd") + "\\Fail\\";
            HObject ModleImage = null; HTuple Number1 = null; HTuple Number2 = null; HTuple Number3 = null;
            HObject modelROI = null; HTuple ModelID = null;
            HObject ModleRoiCircle = null; HObject RoiCircle1 = null; HObject RoiCircle2 = null;
            HObject ModleRoiRectangle = null; HObject RoiRectangle1 = null; HObject RoiRectangle2 = null;
            HObject RegionDifference = null; HObject RegionDifference1 = null; HObject RegionDifference2 = null;
            HObject ImageReduced = null; HObject ImageReduced1 = null; HObject ImageReduced2 = null;
            HObject ImageSub = null; HObject RegionThreshold = null; HObject ConnectedRegions = null;
            HObject SelectedRegions = null; HObject RegionClosing1 = null; HObject RegionClosing2 = null;
            HObject Skeleton = null; HObject SelectedSkeleton = null; HObject SelectedSkeleton2 = null; HObject RegionUnion = null;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //读取模板路径，模板参数
                //string Modelpath = "D:\ProductFile\DF\Disp\GlueCheck\ModelImage.bmp";
                //HTuple threshold = 30;
                //HTuple closing = 2;
                //HTuple modelCircleRow = 512.71; HTuple modelCircleCol = 838.492; HTuple modelCircleR = 97.1676;
                //HTuple RoiCircleRow = 515.131; HTuple RoiCircleCol = 635.466; HTuple RoiCircleR = 61.896;
                //HTuple RoiRectangleRow1 = 411.037; HTuple RoiRectangleCol1 = 542.693; HTuple RoiRectangleRow2 = 611.963; HTuple RoiRectangleCol2 = 724.206;

                string Modelpath = ParamSetMgr.GetInstance().GetStringParam("点胶检查路径模板").Replace("\\", "\\\\");
                HTuple threshold = ParamSetMgr.GetInstance().GetIntParam("点胶检查阈值");
                HTuple closing = ParamSetMgr.GetInstance().GetIntParam("点胶检查闭合像素");

                HTuple modelCircleRow = ParamSetMgr.GetInstance().GetDoubleParam("点胶检查模板Row");
                HTuple modelCircleCol = ParamSetMgr.GetInstance().GetDoubleParam("点胶检查模板Col");
                HTuple modelCircleR = ParamSetMgr.GetInstance().GetDoubleParam("点胶检查模板R");
                HTuple RoiCircleRow = ParamSetMgr.GetInstance().GetDoubleParam("点胶检查CircleRow");
                HTuple RoiCircleCol = ParamSetMgr.GetInstance().GetDoubleParam("点胶检查CircleCol");
                HTuple RoiCircleR = ParamSetMgr.GetInstance().GetDoubleParam("点胶检查CircleR");
                HTuple RoiRectangleRow1 = ParamSetMgr.GetInstance().GetDoubleParam("点胶检查RectangleRow1");
                HTuple RoiRectangleCol1 = ParamSetMgr.GetInstance().GetDoubleParam("点胶检查RectangleCol1");
                HTuple RoiRectangleRow2 = ParamSetMgr.GetInstance().GetDoubleParam("点胶检查RectangleRow2");
                HTuple RoiRectangleCol2 = ParamSetMgr.GetInstance().GetDoubleParam("点胶检查RectangleCol2");

                HTuple Row1 = null; HTuple Column1 = null; HTuple Angle1 = null; HTuple Score1 = null;
                HTuple Row2 = null; HTuple Column2 = null; HTuple Angle2 = null; HTuple Score2 = null;
                HOperatorSet.ReadImage(out ModleImage, Modelpath);
                HOperatorSet.GenCircle(out modelROI, modelCircleRow, modelCircleCol, modelCircleR);
                HOperatorSet.GenCircle(out ModleRoiCircle, RoiCircleRow, RoiCircleCol, RoiCircleR);
                HOperatorSet.GenRectangle1(out ModleRoiRectangle, RoiRectangleRow1, RoiRectangleCol1, RoiRectangleRow2, RoiRectangleCol2);
                HOperatorSet.Difference(ModleRoiRectangle, ModleRoiCircle, out RegionDifference);
                HOperatorSet.ReduceDomain(Image1, modelROI, out ImageReduced);
                HOperatorSet.CreateNccModel(ImageReduced, 3, -0.39, 0.79, 0.1, "use_polarity", out ModelID);
                HOperatorSet.FindNccModel(Image1, ModelID, -0.39, 0.78, 0.8, 1, 0.5, "true", 0, out Row1, out Column1, out Angle1, out Score1);
                HOperatorSet.FindNccModel(Image2, ModelID, -0.39, 0.78, 0.8, 1, 0.5, "true", 0, out Row2, out Column2, out Angle2, out Score2);
                HOperatorSet.GenCircle(out RoiCircle1, RoiCircleRow - modelCircleRow + Row1, RoiCircleCol - modelCircleCol + Column1, RoiCircleR);
                HOperatorSet.GenCircle(out RoiCircle2, RoiCircleRow - modelCircleRow + Row2, RoiCircleCol - modelCircleCol + Column2, RoiCircleR);
                HOperatorSet.GenRectangle1(out RoiRectangle1, RoiRectangleRow1 - modelCircleRow + Row1, RoiRectangleCol1 - modelCircleCol + Column1, RoiRectangleRow2 - modelCircleRow + Row1, RoiRectangleCol2 - modelCircleCol + Column1);
                HOperatorSet.GenRectangle1(out RoiRectangle2, RoiRectangleRow1 - modelCircleRow + Row2, RoiRectangleCol1 - modelCircleCol + Column2, RoiRectangleRow2 - modelCircleRow + Row2, RoiRectangleCol2 - modelCircleCol + Column2);
                HOperatorSet.Difference(RoiRectangle1, RoiCircle1, out RegionDifference1);
                HOperatorSet.Difference(RoiRectangle2, RoiCircle2, out RegionDifference2);
                HOperatorSet.ReduceDomain(Image1, RegionDifference1, out ImageReduced1);
                HOperatorSet.ReduceDomain(Image2, RegionDifference2, out ImageReduced2);
                HOperatorSet.SubImage(ImageReduced1, ImageReduced2, out ImageSub, 1, 0);
                HOperatorSet.Threshold(ImageSub, out RegionThreshold, threshold, 255);
                HOperatorSet.ClosingCircle(RegionThreshold, out RegionClosing1, closing);
                HOperatorSet.Connection(RegionClosing1, out ConnectedRegions);
                HOperatorSet.SelectShape(ConnectedRegions, out SelectedRegions, "area", "and", 800, 99999);
                HOperatorSet.CountObj(SelectedRegions, out Number1);
                if (Number1.I < 1)
                {
                    HOperatorSet.WriteImage(Image1.Clone(), "bmp", 0, $"{path}LoseGlue1_{DateTime.Now.ToString("yyMMdd_HHmmssfff")}.bmp");
                    HOperatorSet.WriteImage(Image2.Clone(), "bmp", 0, $"{path}LoseGlue2_{DateTime.Now.ToString("yyMMdd_HHmmssfff")}.bmp");
                    return false;//无特征，漏胶
                }
                HOperatorSet.ClosingCircle(SelectedRegions, out RegionClosing2, closing);
                HOperatorSet.Skeleton(RegionClosing2, out Skeleton);
                HOperatorSet.SelectShape(Skeleton, out SelectedSkeleton, (((new HTuple("area")).TupleConcat("holes_num")).TupleConcat("column")).TupleConcat("row"), "and", (((new HTuple(300)).TupleConcat(0)).TupleConcat(300)).TupleConcat(300), (((new HTuple(1000000)).TupleConcat(1000)).TupleConcat(1000)).TupleConcat(1000));
                // HOperatorSet.Union1(SelectedSkeleton, out RegionUnion);
                HOperatorSet.CountObj(SelectedSkeleton, out Number2);
                if (Number2.I < 1)
                {
                    HOperatorSet.WriteImage(Image1.Clone(), "bmp", 0, $"{path}OpenGlueNoSkeleton1_{DateTime.Now.ToString("yyMMdd_HHmmssfff")}.bmp");
                    HOperatorSet.WriteImage(Image2.Clone(), "bmp", 0, $"{path}OpenGlueNoSkeleton2_{DateTime.Now.ToString("yyMMdd_HHmmssfff")}.bmp");
                    return false;//无特征，断胶
                }
                else
                {
                    HOperatorSet.SelectShape(SelectedSkeleton, out SelectedSkeleton2, "area_holes", "and", 10000, 99999999);
                    HOperatorSet.CountObj(SelectedSkeleton2, out Number3);
                    if (Number3.I != 1)
                    {
                        HOperatorSet.WriteImage(Image1.Clone(), "bmp", 0, $"{path}OpenGlue1AreaNum_{DateTime.Now.ToString("yyMMdd_HHmmssfff")}.bmp");
                        HOperatorSet.WriteImage(Image2.Clone(), "bmp", 0, $"{path}OpenGlue2AreaNum_{DateTime.Now.ToString("yyMMdd_HHmmssfff")}.bmp");
                        return false;//无特征，断胶
                    }

                }
            }
            catch
            {
                if (Image1 != null)
                {
                    HOperatorSet.WriteImage(Image1.Clone(), "bmp", 0, $"{path}GlueErr1_{DateTime.Now.ToString("yyMMdd_HHmmssfff")}.bmp");
                }
                if (Image2 != null)
                {
                    HOperatorSet.WriteImage(Image2.Clone(), "bmp", 0, $"{path}GlueErr2_{DateTime.Now.ToString("yyMMdd_HHmmssfff")}.bmp");
                }
                result = false;
            }
            finally
            {
                if (ModleImage != null) ModleImage.Dispose();
                if (modelROI != null) modelROI.Dispose();
                if (ModleRoiCircle != null) ModleRoiCircle.Dispose();
                if (RoiCircle1 != null) RoiCircle1.Dispose();
                if (RoiCircle2 != null) RoiCircle2.Dispose();
                if (ModleRoiRectangle != null) ModleRoiRectangle.Dispose();
                if (RoiRectangle1 != null) RoiRectangle1.Dispose();
                if (RoiRectangle2 != null) RoiRectangle2.Dispose();
                if (RegionDifference != null) RegionDifference.Dispose();
                if (RegionDifference1 != null) RegionDifference1.Dispose();
                if (RegionDifference2 != null) RegionDifference2.Dispose();
                if (ImageReduced != null) ImageReduced.Dispose();
                if (ImageReduced1 != null) ImageReduced1.Dispose();
                if (ImageReduced2 != null) ImageReduced2.Dispose();
                if (ImageSub != null) ImageSub.Dispose();
                if (RegionThreshold != null) RegionThreshold.Dispose();
                if (ConnectedRegions != null) ConnectedRegions.Dispose();
                if (SelectedRegions != null) SelectedRegions.Dispose();
                if (RegionClosing1 != null) RegionClosing1.Dispose();
                if (RegionClosing2 != null) RegionClosing2.Dispose();
                if (Skeleton != null) Skeleton.Dispose();
                if (SelectedSkeleton != null) SelectedSkeleton.Dispose();
                if (RegionUnion != null) RegionUnion.Dispose();
                if (Image1 != null) Image1.Dispose();
                if (Image2 != null) Image2.Dispose();
            }
            return result;
        }
        public bool WaitXY(double posX, double posY)
        {
            bool result = false;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            do
            {
                if (stopwatch.ElapsedMilliseconds > 10000)
                {
                    return false;
                }
                if (MotionMgr.GetInstace().IsAxisNormalStop(AxisY) == AxisState.NormalStop && MotionMgr.GetInstace().IsAxisNormalStop(AxisX) == AxisState.NormalStop)
                {
                    return true;
                }
                //if (Math.Abs(MotionMgr.GetInstace().GetAxisPos(AxisX) - posX) < 0.01 && Math.Abs(MotionMgr.GetInstace().GetAxisPos(AxisY) - posY) < 0.01)
                //{
                //    return true;
                //}
                Thread.Sleep(10);

            }
            while (true);



        }
        public bool MoveY(double pos, SpeedType speedType)
        {
            bool result = false;
            MotionMgr.GetInstace().AbsMove(AxisY, pos, (double)speedType);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            do
            {
                if (stopwatch.ElapsedMilliseconds > 30000)
                {
                    return false;
                }
                if (MotionMgr.GetInstace().IsAxisNormalStop(AxisY) == AxisState.NormalStop)
                {
                    return true;
                }

                Thread.Sleep(10);

            }
            while (true);



        }
        /// <summary>
        /// 检测启动
        /// </summary>
        DoWhile doWhileCheckStartSignal = new DoWhile((time, dowhileobj, bmanual, paramobjs) =>
        {
            int nTimeWaitStartSinal = 60000;
            nTimeWaitStartSinal = (int)(ParamSetMgr.GetInstance().GetDoubleParam("等待启动时间") * 1000);
            StationDisp stationDisp = null;
            int SocketNumOfUnloadLoad = TableData.GetInstance().GetSocketNum(1, 0.5) - 1;
            string Soket = SocketNumOfUnloadLoad == 0 ? "A" : "B";
            if (paramobjs.Length > 0)
                stationDisp = (StationDisp)paramobjs[0];
            if (/*!IOMgr.GetInstace().ReadIoInBit($"{Soket}治具盖上检测") */
            SysFunConfig.LodUnloadPatten.IsUload(Soket)
            && !IOMgr.GetInstace().ReadIoInBit("安全光栅"))
            {
                IOMgr.GetInstace().WriteIoBit("启动按钮灯", true);
                isOpenSocket = true;
            }
            if (isOpenSocket)
            {
                bool starMode = ParamSetMgr.GetInstance().GetBoolParam("是否用光栅启动");
                if (starMode)
                {
                    Thread.Sleep(200);
                    if (IOMgr.GetInstace().ReadIoInBit("安全光栅"))
                    {

                        //if (IOMgr.GetInstace().ReadIoInBit($"{Soket}治具盖上检测") && IOMgr.GetInstace().ReadIoInBit($"{Soket}治具LENS检测"))
                        //{
                        //    isOpenSocket = false;
                        //    Thread.Sleep(1000);
                        //    return WaranResult.Run;
                        //}
                        SysFunConfig.LodUnloadPatten.Load(Soket, bmanual);
                        if (SysFunConfig.LodUnloadPatten.IsLoadOK(Soket, bmanual) )
                        {
                            isOpenSocket = false;
                            Thread.Sleep(1000);
                            return WaranResult.Run;
                        }
                    }
                }
                else
                {
                    if (IOMgr.GetInstace().ReadIoInBit("左启动按钮") && IOMgr.GetInstace().ReadIoInBit("右启动按钮"))
                    {

                        //if (IOMgr.GetInstace().ReadIoInBit($"{Soket}治具LENS检测") && IOMgr.GetInstace().ReadIoInBit($"{Soket}治具盖上检测") && IOMgr.GetInstace().ReadIoInBit("安全光栅"))
                        //{
                        //    isOpenSocket = false;
                        //    return WaranResult.Run;
                        //}
                        SysFunConfig.LodUnloadPatten.Load(Soket, bmanual);
                        if (SysFunConfig.LodUnloadPatten.IsLoadOK(Soket, bmanual ) && IOMgr.GetInstace().ReadIoInBit("安全光栅"))
                        {
                            isOpenSocket = false;
                            return WaranResult.Run;
                        }
                    }
                }

            }
            if (time > nTimeWaitStartSinal)
            {
                WaranResult waranResult = AlarmMgr.GetIntance().WarnWithDlg("等待启动超时", stationDisp, CommonDlg.DlgWaranType.Waran_Stop_Retry, dowhileobj, bmanual);
                AlarmMgr.GetIntance().StopAlarmBeet();
                return waranResult;
            }
            else
                return WaranResult.CheckAgain;

        }, int.MaxValue);
        #endregion



    }
}