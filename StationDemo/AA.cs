using AlgorithmNamespace;
using BaseDll;
using CameraLib;
using CommonTools;
using HalconDotNet;

using log4net;
using ModuleCapture;
//using HalconLib;
using MotionIoLib;
using OtherDevice;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UserCtrl;
using UserData;

namespace StationDemo
{
    public class StationAA : CommonTools.Stationbase
    {
        #region 变量
        int StationIndex = 0;//工位选择
        #endregion

        #region 主流程
        public StationAA(string strStationName, int[] arrAxis, string[] axisname, params string[] CameraName)
            : base(strStationName, arrAxis, axisname, CameraName)
        {

        }
        public StationAA(CommonTools.Stationbase pb) : base(pb)
        {
            m_listIoInput.Add("急停");
            m_listIoInput.Add("气源检测");
            m_listIoInput.Add("安全门");
            m_listIoInput.Add("安全光栅");
            m_listIoInput.Add("夹爪气缸到位");
            m_listIoInput.Add("夹爪气缸原位");
            m_listIoInput.Add("ALens下降到位");
            m_listIoInput.Add("ALens上升到位");
            m_listIoInput.Add("BLens下降到位");
            m_listIoInput.Add("BLens上升到位");
            m_listIoInput.Add("平行光管下降到位");
            m_listIoInput.Add("平行光管上升到位");
            m_listIoInput.Add("A治具盖上检测");
            m_listIoInput.Add("B治具盖上检测");
            m_listIoInput.Add("A治具LENS检测");
            m_listIoInput.Add("B治具LENS检测");


            m_listIoOutput.Add("蜂鸣");
            m_listIoOutput.Add("UV固化");
            m_listIoOutput.Add("平行光管光源");
            m_listIoOutput.Add("夹爪气缸电磁阀");
            m_listIoOutput.Add("12V开启");
            m_listIoOutput.Add("A模组上电");
            m_listIoOutput.Add("B模组上电");
            m_listIoOutput.Add("ALens升降气缸");
            m_listIoOutput.Add("BLens升降气缸");
            if (ParamSetMgr.GetInstance().GetBoolParam("是否用平行光管或中继镜气缸"))
            {
                m_listIoOutput.Add("平行光管升降气缸");
            }
        }
        public enum StationStep
        {

            [Description("0.初始化步骤")]
            StepInit = 100,
            [Description("1.获取转盘通知")]
            Step_GetCmdFromTable,//d等待转盘通知 夹取Lens 还是去AA
            [Description("2.夹取Lens")]
            Step_LensPick,
            [Description("3.移动到AA位")]
            Step_Position,
            [Description("4.对心")]
            Step_FindCenter,
            [Description("5.ThroughFocus")]
            Step_ThroughFocus,//按照对焦次数来分，跳转到Step_WorkStaionAA_FindCenter或者Step_WorkStaionAA_Check
            [Description("6.AA结果检测")]
            Step_Check,
            [Description("7.AA结果检测")]
            Step_Tilt,
            [Description("8.UV固化")]
            Step_UV,
            [Description("9.退出步骤")]
            Step_End,
            [Description("10.NG停止流程")]
            Step_Stop,

        }
        protected override bool InitStation()
        {
            IOMgr.GetInstace().WriteIoBit("UV固化", false);
            SocketMgr.GetInstance().SetSocketState(0, SocketState.None);
            SocketMgr.GetInstance().SetSocketState(1, SocketState.None); ;
            SocketMgr.GetInstance().socketArr[0].socketState = SocketState.None;
            SocketMgr.GetInstance().socketArr[1].socketState = SocketState.None;
            ParamSetMgr.GetInstance().SetBoolParam("AA工站初始化完成", false);
            ParamSetMgr.GetInstance().SetBoolParam("AA工站初始化完成", false);
            ParamSetMgr.GetInstance().SetBoolParam("AA工站初始化完成", false);
            ClearAllStep();
            PushMultStep((int)StationStep.StepInit);
            Info("AA站加载完成");
            return true;
        }
        protected override void StationWork(int step)
        {
            switch (step)
            {
                case (int)StationStep.StepInit:
                    DelCurrentStep();
                    PushMultStep((int)StepInitRun());
                    break;
                case (int)StationStep.Step_GetCmdFromTable:
                    DelCurrentStep();
                    PushMultStep((int)StepGetCmdFromTableRun());
                    break;
                case (int)StationStep.Step_LensPick:
                    DelCurrentStep();
                    //夹取后 在禁止按清料
                    PushMultStep((int)StepLensPickRun());
                    break;
                case (int)StationStep.Step_Position:
                    DelCurrentStep();
                    PushMultStep((int)StepAAPositionRun());
                    StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
                    UserTest.TestResultAB[StationIndex].AACount = 1;
                    PathHelper.AA_ID = StationIndex;
                    break;
                case (int)StationStep.Step_FindCenter:
                    DelCurrentStep();
                    PushMultStep((int)StepFindCenterRun(UserTest.TestResultAB[StationIndex].AACount, this.VisionControl));
                    break;
                case (int)StationStep.Step_ThroughFocus:
                    DelCurrentStep();
                    PushMultStep((int)StepThroughFocusRun(UserTest.TestResultAB[StationIndex].AACount, this.VisionControl));
                    break;
                case (int)StationStep.Step_Tilt:
                    DelCurrentStep();
                    PushMultStep((int)StepTiltRun(UserTest.TestResultAB[StationIndex].AACount, this.VisionControl));
                    break;
                case (int)StationStep.Step_Check:
                    DelCurrentStep();
                    PushMultStep((int)StepAACheckRun(this.VisionControl));
                    break;
                case (int)StationStep.Step_UV:
                    DelCurrentStep();
                    PushMultStep((int)StepUVRun(this.VisionControl));
                    break;
                case (int)StationStep.Step_End:
                    DelCurrentStep();
                    PushMultStep((int)StepEndRun());
                    break;
                case (int)StationStep.Step_Stop:
                    //处在NG位置，等待处理
                    DelCurrentStep();
                    break;
            }


        }
        public bool CheckLRuvCliyder(bool bmanual)
        {
            int nMaxCountRetry = 3;
            int nCountNow = 0;
            if (ParamSetMgr.GetInstance().GetBoolParam("是否侧向UV"))
            {
                IOMgr.GetInstace().WriteIoBit("侧向UV气缸", false);
            retry_LRuv:
                nCountNow++;
                WaranResult waran = CheckIobyName("左侧UV原位", true, "左侧UV原位失败,可以重试3次 ", bmanual);
                if (waran == WaranResult.Retry)
                {
                    IOMgr.GetInstace().WriteIoBit("侧向UV气缸", false);
                    goto retry_LRuv;
                }
                waran = CheckIobyName("右侧UV原位", true, "右侧UV原位失败，可以重试3次", bmanual);
                if (waran == WaranResult.Retry)
                {
                    IOMgr.GetInstace().WriteIoBit("侧向UV气缸", false);
                    goto retry_LRuv;
                }
                if (nCountNow >= nMaxCountRetry)
                {
                    Err("左右侧UV原位归位失败");
                    return false;
                }


            }
            return true;
        }
        public StationStep StepInitRun(bool bmanual = false)
        {
            StationStep step = StationStep.Step_Stop;
            // Form_Auto.ShowEventOnAutoScreen("回零", null);
            ParamSetMgr.GetInstance().SetBoolParam("AA工站初始化完成", false);
            UserTest.ProductCount.CountCTTime = 0;
            UserTest.ProductCount.CountCTAll = 0;
            //重新加载算法文件
            string currentProductFile = ParamSetMgr.GetInstance().CurrentWorkDir + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + (".xml");
            ParamSetMgr.GetInstance().SaveParam(currentProductFile);
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}NST_ActiveAlignment.ini";
            AlgorithmMgr.Instance.LoadConfig(path);
            if (!CheckLRuvCliyder(bmanual))
            {
                Err("AA站 回零失败");
                throw new Exception("6轴回零失败！");
            }
            if (!GoAAHome(bmanual))
            {
                Err("AA站 回零失败");
                throw new Exception("6轴回零失败！");
            }
            if (ParamSetMgr.GetInstance().GetBoolParam("是否选择程控电源"))
            {
                OtherDevices.ckPower.SetAllCKPowerOn();
                double valueVoltage = ParamSetMgr.GetInstance().GetDoubleParam("程控电源电压");
                OtherDevices.ckPower.SetVoltage(1, 0);
                OtherDevices.ckPower.SetVoltage(2, 0);
                double valueCurrent = ParamSetMgr.GetInstance().GetDoubleParam("程控电源电流");
                OtherDevices.ckPower.SetCurrent(1, 0);
                OtherDevices.ckPower.SetCurrent(2, 0);
            }
            if (ParamSetMgr.GetInstance().GetStringParam("算法类型") == "Collimator")
            {
                IOMgr.GetInstace().WriteIoBit($"平行光管光源", true);
            }
            IOMgr.GetInstace().WriteIoBit($"12V开启", false);
            IOMgr.GetInstace().WriteIoBit($"A模组上电", false);
            IOMgr.GetInstace().WriteIoBit($"B模组上电", false);
            int SetSNA = ParamSetMgr.GetInstance().GetIntParam("A工位SetSN_ID");
            int SetSNB = ParamSetMgr.GetInstance().GetIntParam("B工位SetSN_ID");
            ModuleMgr.Instance.SetSN(0, SetSNA);
            ModuleMgr.Instance.SetSN(1, SetSNB);

            string playPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\Play\\play.ini";
            //    ModuleMgr.Instance.Init(StationIndex, playPath);
            GoAAReadySafe(bmanual);


            ParamSetMgr.GetInstance().SetBoolParam("AA工站初始化完成", true);
            step = StationStep.Step_GetCmdFromTable;
            return step;
        }
        public StationStep StepGetCmdFromTableRun(bool bmanual = false)
        {

            StationStep step = StationStep.Step_GetCmdFromTable;
            string err = "";
            //获取当前夹取位置是A工位还是B工位
            int StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
            string stationLensName = StationIndex == 0 ? "A" : "B";
            bool dEnableA = ParamSetMgr.GetInstance().GetBoolParam("屏蔽A工位");
            bool dEnableB = ParamSetMgr.GetInstance().GetBoolParam("屏蔽B工位");
            bool A_AA = TableData.GetInstance().GetStationStartCmd($"A_AA");
            bool A_Lens = TableData.GetInstance().GetStationStartCmd($"A_Pick");
            bool B_AA = TableData.GetInstance().GetStationStartCmd($"B_AA");
            bool B_Lens = TableData.GetInstance().GetStationStartCmd($"B_Pick");
            if (A_AA || A_Lens || B_AA || B_Lens)
            {
                TableData.GetInstance().ResetStartCmd("A_AA");
                TableData.GetInstance().ResetStartCmd("A_Pick");
                TableData.GetInstance().ResetStartCmd("B_AA");
                TableData.GetInstance().ResetStartCmd("B_Pick");
                string strStationName = TableData.GetInstance().GetStationName();
                Info(string.Format("AA 工位[{0}]流程开始", strStationName));
                if (StationIndex == 0 && dEnableA)
                {
                    ParamSetMgr.GetInstance().SetBoolParam("AA完成", true);
                    Info(string.Format("AA 工位[{0}]屏蔽", stationLensName));
                    TableData.GetInstance().SetStationResult("A_AA", true);
                    TableData.GetInstance().SetStationResult("B_AA", true);
                    TableData.GetInstance().SetStationResult("A_Pick", true);
                    TableData.GetInstance().SetStationResult("B_Pick", true);
                    SocketMgr.GetInstance().socketArr[StationIndex].socketState = SocketState.HaveOK;
                    return step;
                }
                if (StationIndex == 1 && dEnableB)
                {
                    ParamSetMgr.GetInstance().SetBoolParam("AA完成", true);
                    Info(string.Format("AA 工位[{0}]屏蔽", stationLensName));
                    TableData.GetInstance().SetStationResult("A_AA", true);
                    TableData.GetInstance().SetStationResult("B_AA", true);
                    TableData.GetInstance().SetStationResult("A_Pick", true);
                    TableData.GetInstance().SetStationResult("B_Pick", true);
                    SocketMgr.GetInstance().socketArr[StationIndex].socketState = SocketState.HaveOK;
                    return step;
                }
                switch (strStationName)
                {

                    case "A_Pick":
                    case "B_Pick":
                        if (SocketMgr.GetInstance().socketArr[StationIndex].socketState == SocketState.Have)
                        {
                            step = StationStep.Step_LensPick;
                        }
                        else
                        {
                            TableData.GetInstance().SetStationResult("A_AA", true);
                            TableData.GetInstance().SetStationResult("B_AA", true);
                            TableData.GetInstance().SetStationResult("A_Pick", true);
                            TableData.GetInstance().SetStationResult("B_Pick", true);
                        }
                        break;
                    case "A_AA":
                    case "B_AA":
                        if (SocketMgr.GetInstance().socketArr[StationIndex].socketState == SocketState.Picked)
                        {
                            step = StationStep.Step_Position;
                        }
                        else
                        {
                            TableData.GetInstance().SetStationResult("A_AA", true);
                            TableData.GetInstance().SetStationResult("B_AA", true);
                            TableData.GetInstance().SetStationResult("A_Pick", true);
                            TableData.GetInstance().SetStationResult("B_Pick", true);
                        }
                        break;
                    default:
                        Info(string.Format("AA 工位[{0}]default.", strStationName));
                        break;

                }

            }
            return step;
        }
        public StationStep StepLensPickRun(bool bmanual = false)
        {
            IOMgr.GetInstace().WriteIoBit("UV固化", false);
            ParamSetMgr.GetInstance().SetBoolParam("抓取启动", true);
            UserTest.TestTimeInfo.AAbegin = DateTime.Now;
            WaranResult waranResult = WaranResult.Run;
            StationStep step = StationStep.Step_End;
            //获取当前夹取位置是A工位还是B工位
            int StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
            if (!bmanual)
            {
                Task.Run(() =>
                {
                    GlobelManualResetEvent.AutoPlay.Reset();
                    int SetSN = ParamSetMgr.GetInstance().GetIntParam("A工位SetSN_ID");
                    if (StationIndex == 1)
                        SetSN = ParamSetMgr.GetInstance().GetIntParam("B工位SetSN_ID");

                    string playPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\Play\\play.ini";
                    ModuleMgr.Instance.SetSN(StationIndex, SetSN);
                    // ModuleMgr.Instance.Init(StationIndex, playPath);
                    ModuleMgr.Instance.Stop(StationIndex);
                    Thread.Sleep(200);
                    if (!ModuleMgr.Instance.Play(StationIndex))
                    {
                        UserTest.FailResultAB.Play = false;
                        Err("点亮失败");
                    }
                    if (ParamSetMgr.GetInstance().GetBoolParam("是否关闭曝光") && ParamSetMgr.GetInstance().GetStringParam("工位点亮类型").Contains("DT"))
                    {
                        int SlaveID = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("SlaveID"), 16);
                        int addr = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("关闭曝光地址"), 16);
                        int value = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("曝光值"), 16);
                        ModuleMgr.Instance.WriteI2C(StationIndex, Convert.ToByte(SlaveID), 0, addr, value);
                    }
                    Thread.Sleep(100);
                    GlobelManualResetEvent.AutoPlay.Set();
                });
            }
            string stationLensName = StationIndex == 0 ? "A" : "B";
            // UserTest.ContiuneWhlie = false;
            //if (!IOMgr.GetInstace().ReadIoInBit($"{stationLensName}治具盖上检测"))
            if (SysFunConfig.LodUnloadPatten.IsOpenSocket(stationLensName))
            {
                Err($"{stationLensName}治具没有盖好");
                return step;
            }
        retry_openclmp:
            IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", false);
            waranResult = CheckIobyName("夹爪气缸原位", true, "夹爪气缸打开失败", bmanual);
            if (waranResult == WaranResult.Retry)
                goto retry_openclmp;


            double safeX = GetStationPointDic()["安全位置"].pointX;
            double safeY = GetStationPointDic()["安全位置"].pointY;
            double safeZ = GetStationPointDic()["安全位置"].pointZ;
            double safeU = GetStationPointDic()["安全位置"].pointU;
            double safeTx = GetStationPointDic()["安全位置"].pointTx;
            double safeTy = GetStationPointDic()["安全位置"].pointTy;
            //6轴平台移动到夹取位置，先移动XY后再移动Z
            double lensX = GetStationPointDic()[$"{stationLensName}工位夹取位"].pointX;
            double lensY = GetStationPointDic()[$"{stationLensName}工位夹取位"].pointY;
            double lensZ = GetStationPointDic()[$"{stationLensName}工位夹取位"].pointZ;
            double lensU = GetStationPointDic()[$"{stationLensName}工位夹取位"].pointU;
            double lensTx = GetStationPointDic()[$"{stationLensName}工位夹取位"].pointTx;
            double lensTy = GetStationPointDic()[$"{stationLensName}工位夹取位"].pointTy;
            MoveSigleAxisPosWaitInpos(AxisZ, safeZ, (double)SpeedType.High, 0.005, bmanual, this, 30000);//Z轴上抬到等待位置
            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisU, AxisTx, AxisTy }, new double[] { safeX, safeY, safeU, safeTx, safeTy }, new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);

            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisU, AxisTx, AxisTy }, new double[] { lensX, lensY, lensU, lensTx, lensTy }, new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);
            MoveSigleAxisPosWaitInpos(AxisZ, lensZ, (double)SpeedType.High, 0.005, bmanual, this, 30000);
            //移动完成后，夹紧Lens

            Thread.Sleep(50);
        retry_closeclmp:
            IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", true);
            waranResult = CheckIobyName("夹爪气缸到位", true, "夹爪气缸夹紧失败", bmanual);
            if (waranResult == WaranResult.Retry)
                goto retry_closeclmp;
            if (ParamSetMgr.GetInstance().GetBoolParam("夹爪夹取两次"))
            {
                Thread.Sleep(100);
            retry_openclmp2:
                IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", false);
                waranResult = CheckIobyName("夹爪气缸原位", true, "夹爪气缸打开失败", bmanual);
                if (waranResult == WaranResult.Retry)
                    goto retry_openclmp2;
                Thread.Sleep(500);
            retry_closeclmp2:
                IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", true);
                waranResult = CheckIobyName("夹爪气缸到位", true, "夹爪气缸夹紧失败", bmanual);
                if (waranResult == WaranResult.Retry)
                    goto retry_closeclmp2;
            }

            Thread.Sleep(300);
            MoveSigleAxisPosWaitInpos(AxisZ, safeZ, (double)SpeedType.High, 0.005, bmanual, this, 30000);//Z轴上抬到等待位置
        retry_downlens:
            IOMgr.GetInstace().WriteIoBit($"{stationLensName}Lens升降气缸", false);
            waranResult = CheckIobyName($"{stationLensName}Lens下降到位", true, "Lens下降到位失败", bmanual);
            if (waranResult == WaranResult.Retry)
                goto retry_downlens;
            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisU, AxisTx, AxisTy }, new double[] { safeX, safeY, safeU, safeTx, safeTy }, new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);
            ParamSetMgr.GetInstance().SetBoolParam("抓取完成", true);
            ParamSetMgr.GetInstance().SetBoolParam("抓取启动", false);
            Info(string.Format("AA 工位{0}抓取完成", stationLensName));
            step = StationStep.Step_GetCmdFromTable;
            SocketMgr.GetInstance().socketArr[StationIndex].socketState = SocketState.Picked;
            TableData.GetInstance().SetStationResult("A_AA", true);
            TableData.GetInstance().SetStationResult("B_AA", true);
            TableData.GetInstance().SetStationResult("A_Pick", true);
            TableData.GetInstance().SetStationResult("B_Pick", true);

            step = StationStep.Step_GetCmdFromTable;
            return step;
        }
        public StationStep StepAAPositionRun(bool bmanual = false)
        {
            StationStep step = StationStep.Step_End;
            //获取当前夹取位置是A工位还是B工位
            int StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
            string stationAAName = StationIndex == 0 ? "A" : "B";
            double safeX = GetStationPointDic()["安全位置"].pointX;
            double safeY = GetStationPointDic()["安全位置"].pointY;
            double safeZ = GetStationPointDic()["安全位置"].pointZ;
            double safeU = GetStationPointDic()["安全位置"].pointU;
            double safeTx = GetStationPointDic()["安全位置"].pointTx;
            double safeTy = GetStationPointDic()["安全位置"].pointTy;
            //6轴平台移动到夹取位置，先移动XY后再移动Z
            double AAX = GetStationPointDic()[$"{stationAAName}工位AA位"].pointX;
            double AAY = GetStationPointDic()[$"{stationAAName}工位AA位"].pointY;
            double AAZ = GetStationPointDic()[$"{stationAAName}工位AA位"].pointZ;
            double AAU = GetStationPointDic()[$"{stationAAName}工位AA位"].pointU;
            double AATx = GetStationPointDic()[$"{stationAAName}工位AA位"].pointTx;
            double AATy = GetStationPointDic()[$"{stationAAName}工位AA位"].pointTy;
            MoveSigleAxisPosWaitInpos(AxisZ, safeZ, (double)SpeedType.High, 0.005, bmanual, this, 30000);//Z轴上抬到等待位置
            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisU, AxisTx, AxisTy }, new double[] { safeX, safeY, safeU, safeTx, safeTy }, new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);
            //6轴平台移动到夹取位置，先移动XY后再移动Z
            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisU, AxisTx, AxisTy }, new double[] { AAX, AAY, AAU, AATx, AATy }, new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);
            MoveSigleAxisPosWaitInpos(AxisZ, AAZ, (double)SpeedType.High, 0.005, bmanual, this, 30000);
            //平行光管下降
            if (ParamSetMgr.GetInstance().GetBoolParam("是否用平行光管或中继镜气缸"))
            {
            retry_open:
                IOMgr.GetInstace().WriteIoBit("平行光管升降气缸", true);
                WaranResult waranResult = CheckIobyName("平行光管下降到位", true, "平行光管下降到位失败", bmanual, 3000);
                if (waranResult == WaranResult.Retry)
                    goto retry_open;
                Thread.Sleep(2000);
            }

            Thread.Sleep(200);
            step = StationStep.Step_FindCenter;

            return step;
        }
        public StationStep StepFindCenterRun(int AACount, VisionControl visionControl = null, bool bmanual = false)
        {
            ParamSetMgr.GetInstance().SetBoolParam("抓取完成", false);
            ParamSetMgr.GetInstance().SetBoolParam("启动AA", true);
            if (AACount == 1)
            {
                UserTest.TestTimeInfo.Center_1_Begin = DateTime.Now;
            }
            else if (AACount == 2)
            {
                UserTest.TestTimeInfo.Center_2_Begin = DateTime.Now;
            }
            else
            {
                UserTest.TestTimeInfo.Center_3_Begin = DateTime.Now;
            }
            if (!bmanual)
                GlobelManualResetEvent.AutoPlay.WaitOne();
            //获取当前夹取位置是A工位还是B工位
            int StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
            string stationAAName = StationIndex == 0 ? "A" : "B";
            if (stationAAName == "A")
            {
                GlobelManualResetEvent.ContinueShowA.Reset();
            }
            else
            {
                GlobelManualResetEvent.ContinueShowB.Reset();
            }
            StationStep step = StationStep.Step_End;
            int index = 1;
            if (AACount > 1)
            {
                index = 2;
            }
            ParamSetMgr.GetInstance().SetBoolParam($"{stationAAName}工位第{index}次对心", true);
            double AAX = MotionMgr.GetInstace().GetAxisPos(AxisX);
            double AAY = MotionMgr.GetInstace().GetAxisPos(AxisY);
            double AAZ = MotionMgr.GetInstace().GetAxisPos(AxisZ);
            double AAOriPosX = 0;
            double AAOriPosY = 0;
            double AAOriPosZ = 0;
            double AAOriPosTX = 0;
            double AAOriPosTY = 0;
            double AAOriPosTZ = 0;
            if (index == 1)
            {
                AAOriPosX = GetStationPointDic()[$"{stationAAName}工位AA位"].pointX;
                AAOriPosY = GetStationPointDic()[$"{stationAAName}工位AA位"].pointY;
                AAOriPosZ = GetStationPointDic()[$"{stationAAName}工位AA位"].pointZ;
                AAOriPosTX = GetStationPointDic()[$"{stationAAName}工位AA位"].pointTx;
                AAOriPosTY = GetStationPointDic()[$"{stationAAName}工位AA位"].pointTy;
                AAOriPosTZ = GetStationPointDic()[$"{stationAAName}工位AA位"].pointU;
                Info($"第{AACount}次对心：{stationAAName}工位AA站回初始位置");
            }
            else
            {
                AAOriPosX = MotionMgr.GetInstace().GetAxisPos(AxisX);
                AAOriPosY = MotionMgr.GetInstace().GetAxisPos(AxisY);
                AAOriPosZ = MotionMgr.GetInstace().GetAxisPos(AxisZ);
                AAOriPosTX = MotionMgr.GetInstace().GetAxisPos(AxisTx);
                AAOriPosTY = MotionMgr.GetInstace().GetAxisPos(AxisTy);
                AAOriPosTZ = MotionMgr.GetInstace().GetAxisPos(AxisU);
                Info($"第{AACount}次对心：{stationAAName}工位AA站回当前位置");
            }

            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisZ, AxisTx, AxisTy, AxisU },
            new double[] { AAOriPosX, AAOriPosY, AAOriPosZ, AAOriPosTX, AAOriPosTY, AAOriPosTZ },
            new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);

            double dx = 0;
            double dy = 0;
            List<double> Array = new List<double>();
            Thread.Sleep(200);
            Info($"{stationAAName}工位AA站对心");
            Bitmap bt = null;
            string strXoffset = $"第{1}次对心偏差X";
            string strYoffset = $"第{1}次对心偏差Y";
            UserTest.FailResultAB.Play = true;
            UserTest.FailResultAB.SFR = true;
            Thread.Sleep(ParamSetMgr.GetInstance().GetIntParam("电机到位延时"));
            if (!ModuleMgr.Instance.CaptureToBmpRGB(StationIndex, 1, ref bt))
            {
                Err("取图失败！");
                if (AACount == 1)
                {
                    if (StationIndex == 0)
                    {
                        IOMgr.GetInstace().WriteIoBit($"A模组上电", false);
                        Thread.Sleep(3000);
                        IOMgr.GetInstace().WriteIoBit($"A模组上电", true);
                    }
                    else
                    {
                        IOMgr.GetInstace().WriteIoBit($"B模组上电", false);
                        Thread.Sleep(3000);
                        IOMgr.GetInstace().WriteIoBit($"B模组上电", true);
                    }

                    Thread.Sleep(6000);
                    int SetSN = ParamSetMgr.GetInstance().GetIntParam("A工位SetSN_ID");
                    if (StationIndex == 1)
                        SetSN = ParamSetMgr.GetInstance().GetIntParam("B工位SetSN_ID");


                    string playPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\Play\\play.ini";

                    ModuleMgr.Instance.SetSN(StationIndex, SetSN);
                    // ModuleMgr.Instance.Init(StationIndex, playPath);


                    ModuleMgr.Instance.Stop(StationIndex);
                    Thread.Sleep(500);
                    ModuleMgr.Instance.Play(StationIndex);
                    Thread.Sleep(200);
                    if (ParamSetMgr.GetInstance().GetBoolParam("是否关闭曝光") && ParamSetMgr.GetInstance().GetStringParam("工位点亮类型").Contains("DT"))
                    {
                        int SlaveID = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("SlaveID"), 16);
                        int addr = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("关闭曝光地址"), 16);
                        int value = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("曝光值"), 16);
                        ModuleMgr.Instance.WriteI2C(StationIndex, Convert.ToByte(SlaveID), 0, addr, value);
                    }
                    if (!ModuleMgr.Instance.CaptureToBmpRGB(StationIndex, 1, ref bt))
                    {
                        Err("二次点亮，取图失败！");
                        UserTest.FailResultAB.Play = false;
                        UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次对心移动前拍照异常";
                        return step;
                    }
                }
                else
                {
                    UserTest.FailResultAB.Play = false;
                    UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次对心移动前拍照异常";
                    return step;
                }

            }
            if (!AlgorithmMgr.Instance.Findcenter((Bitmap)bt.Clone(), ref dx, ref dy, ref Array, ParamSetMgr.GetInstance().GetBoolParam("选择MF快速调焦算法")))
            {
                if (AACount == 1)
                {
                    if (StationIndex == 0)
                    {
                        IOMgr.GetInstace().WriteIoBit($"A模组上电", false);
                        Thread.Sleep(3000);
                        IOMgr.GetInstace().WriteIoBit($"A模组上电", true);
                    }
                    else
                    {
                        IOMgr.GetInstace().WriteIoBit($"B模组上电", false);
                        Thread.Sleep(3000);
                        IOMgr.GetInstace().WriteIoBit($"B模组上电", true);
                    }
                    Thread.Sleep(6000);
                    int SetSN = ParamSetMgr.GetInstance().GetIntParam("A工位SetSN_ID");
                    if (StationIndex == 1)
                        SetSN = ParamSetMgr.GetInstance().GetIntParam("B工位SetSN_ID");

                    string playPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\Play\\play.ini";
                    ModuleMgr.Instance.SetSN(StationIndex, SetSN);
                    //   ModuleMgr.Instance.Init(StationIndex, playPath);


                    ModuleMgr.Instance.Stop(StationIndex);
                    ModuleMgr.Instance.Stop(StationIndex);
                    Thread.Sleep(500);
                    ModuleMgr.Instance.Play(StationIndex);
                    Thread.Sleep(200);
                    if (!ModuleMgr.Instance.CaptureToBmpRGB(StationIndex, 1, ref bt))
                    {
                        UserTest.FailResultAB.Play = false;
                        UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次对心移动前拍照异常";
                        return step;
                    }
                    if (!AlgorithmMgr.Instance.Findcenter((Bitmap)bt.Clone(), ref dx, ref dy, ref Array, ParamSetMgr.GetInstance().GetBoolParam("选择MF快速调焦算法")))
                    {
                        UserTest.FailResultAB.OC = false;
                        UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次对心移动前计算异常";
                        return step;
                    }
                }
                else
                {
                    UserTest.FailResultAB.OC = false;
                    UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次对心移动前计算异常";
                    return step;
                }
            }
            if (bmanual)
                CSVHelper.Instance.SaveToCSVPath(PathHelper.FindCenterCsvPath, new FindCenterResult { Name = "对心前", DX = dx.ToString("0.00000"), DY = dy.ToString("0.00000") });
            ImageHelper.Instance.SaveImage($"{PathHelper.ImagePathFindcenter}对心移动前_{UserTest.TestResultAB[StationIndex].SerialNumber}_{DateTime.Now.ToString("HHmmssfff")}.bmp", (Bitmap)bt.Clone());
            Form_Auto.EvenShowImageDelegate(bt, PathHelper.ImagePathFindcenter + $"Result_对心移动前_{UserTest.TestResultAB[StationIndex].SerialNumber}_{DateTime.Now.ToString("HHmmssfff")}" + ".png", true);
            if (index == 1)
            {
                UserTest.TestResultAB[StationIndex].Find_1_BeforeX = dx;
                UserTest.TestResultAB[StationIndex].Find_1_BeforeY = dy;
            }
            else
            {
                UserTest.TestResultAB[StationIndex].Find_2_BeforeX = dx;
                UserTest.TestResultAB[StationIndex].Find_2_BeforeY = dy;
            }
            Info($"{stationAAName}工位第{index}次对心结果：(OC_X:{dx},OC_Y:{dy})规格：({ ParamSetMgr.GetInstance().GetDoubleParam(strXoffset)} ,{ParamSetMgr.GetInstance().GetDoubleParam(strYoffset)}) ");
            UserTest.RunLog.Write($"{stationAAName}工位第{index}次对心结果： ", LogType.Info, PathHelper.LogPathAuto);
            UserTest.RunLog.Write($"(OC_X:{dx.ToString("0.00000")},OC_Y:{dy.ToString("0.00000")})规格：({ ParamSetMgr.GetInstance().GetDoubleParam(strXoffset)} ,{ParamSetMgr.GetInstance().GetDoubleParam(strYoffset)}) ", LogType.Wran, PathHelper.LogPathAuto);

            if (Math.Abs(dx) < ParamSetMgr.GetInstance().GetDoubleParam(strXoffset) && Math.Abs(dy) < ParamSetMgr.GetInstance().GetDoubleParam(strYoffset))
            {
                double MoveXDistance = dx;
                double MoveYDistance = dy;
                if (ParamSetMgr.GetInstance().GetBoolParam("XY对调"))
                {
                    MoveXDistance = dy; MoveYDistance = dx;
                }
                if (ParamSetMgr.GetInstance().GetBoolParam("X是否负"))
                    MoveXDistance = -MoveXDistance;
                if (ParamSetMgr.GetInstance().GetBoolParam("Y是否负"))
                    MoveYDistance = -MoveYDistance;

                double XGain = ParamSetMgr.GetInstance().GetDoubleParam("XGain");
                double YGain = ParamSetMgr.GetInstance().GetDoubleParam("YGain");
                MoveXDistance = MoveXDistance * XGain;
                MoveYDistance = MoveYDistance * YGain;
                AAX = MotionMgr.GetInstace().GetAxisPos(AxisX);
                AAY = MotionMgr.GetInstace().GetAxisPos(AxisY);
                AAZ = MotionMgr.GetInstace().GetAxisPos(AxisZ);
                MoveXDistance = MoveXDistance + AAX;
                MoveYDistance = MoveYDistance + AAY;

                Info($"{stationAAName}工位第{index}次对心 计算结果（OK）：(OC_X:{dx},OC_Y:{dy}) X轴偏差:{(MoveXDistance - AAX).ToString("0.0000")}, X轴偏差:{(MoveYDistance - AAY).ToString("0.0000")}. ");
                Info($"{stationAAName}工位第{index}次对心,移动前 轴位置：AAX{MotionMgr.GetInstace().GetAxisPos(AxisX)}, AAY{MotionMgr.GetInstace().GetAxisPos(AxisY)}");
                MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY }, new double[] { MoveXDistance, MoveYDistance }, new double[] { (double)SpeedType.Mid, (double)SpeedType.Mid }, 0.005, bmanual, this);
                Info($"{stationAAName}工位第{index}次对心结束,移动后 轴位置：AAX{MotionMgr.GetInstace().GetAxisPos(AxisX)}, AAY{MotionMgr.GetInstace().GetAxisPos(AxisY)}");

            }
            else
            {
                UserTest.FailResultAB.OC = false;
                ParamSetMgr.GetInstance().SetBoolParam($"{stationAAName}工位第{index}次对心", false);
                Err($"{stationAAName}工位第{index}次对心,计算的dx,dy 大于第{index}次对心设定值,将会退出AA流程");
                UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次对心移动前计算超规格";
                return step;
            }

            Info($"{stationAAName}工位第{index}次对心,对心完成后取图测试");
            dx = 0; dy = 0;
            if (AACount > ParamSetMgr.GetInstance().GetIntParam("AA次数"))
            {
                strXoffset = $"第{2}次对心偏差X";
                strYoffset = $"第{2}次对心偏差Y";
                step = StationStep.Step_Check;//Check 最后一次对心超规格也去Check
            }
            bt = null;
            UserTest.FailResultAB.Play = true;
            UserTest.FailResultAB.SFR = true;
            Thread.Sleep(ParamSetMgr.GetInstance().GetIntParam("电机到位延时"));
            if (!ModuleMgr.Instance.CaptureToBmpRGB(StationIndex, 1, ref bt))
            {
                UserTest.FailResultAB.Play = false;
                UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次对心移动后拍照异常";
                return step;
            }
            if (!AlgorithmMgr.Instance.Findcenter((Bitmap)bt.Clone(), ref dx, ref dy, ref Array, false))
            {
                UserTest.FailResultAB.OC = false;
                UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次对心移动后计算异常";
                return step;
            }
            ImageHelper.Instance.SaveImage($"{PathHelper.ImagePathFindcenter}对心移动后_{UserTest.TestResultAB[StationIndex].SerialNumber}_{DateTime.Now.ToString("HHmmssfff")}.bmp", (Bitmap)bt.Clone());
            Form_Auto.EvenShowImageDelegate(bt, PathHelper.ImagePathFindcenter + $"Result_对心移动后_{UserTest.TestResultAB[StationIndex].SerialNumber}_{DateTime.Now.ToString("HHmmssfff")}" + ".png", true);
            if (index == 1)
            {
                UserTest.TestResultAB[StationIndex].Find_1_AfterX = dx;
                UserTest.TestResultAB[StationIndex].Find_1_AfterY = dy;
            }
            else
            {
                UserTest.TestResultAB[StationIndex].Find_2_AfterX = dx;
                UserTest.TestResultAB[StationIndex].Find_2_AfterY = dy;
            }

            Info($"{stationAAName}工位第{index}次对心后,取图测试结果 ：");
            Info($"(OC_X:{dx.ToString("0.00000")},OC_Y:{dy.ToString("0.00000")}), 规格：({ ParamSetMgr.GetInstance().GetDoubleParam(strXoffset)} ,{ParamSetMgr.GetInstance().GetDoubleParam(strYoffset)})");
            if (bmanual)
                CSVHelper.Instance.SaveToCSVPath(PathHelper.FindCenterCsvPath, new FindCenterResult { Name = "对心后", DX = dx.ToString("0.00000"), DY = dy.ToString("0.00000") });

            UserTest.RunLog.Write($"{stationAAName}工位第{index}次对心后,取图测试结果 ：", LogType.Info, PathHelper.LogPathAuto);
            UserTest.RunLog.Write($"(OC_X:{dx},OC_Y:{dy}), 规格：({ ParamSetMgr.GetInstance().GetDoubleParam(strXoffset)} ,{ParamSetMgr.GetInstance().GetDoubleParam(strYoffset)})", LogType.Wran, PathHelper.LogPathAuto);

            //取图计算
            if (Math.Abs(dx) > ParamSetMgr.GetInstance().GetDoubleParam(strXoffset) || Math.Abs(dy) > ParamSetMgr.GetInstance().GetDoubleParam(strYoffset))
            {
                //strXoffset = $"第{1}次对心偏差X";
                //strYoffset = $"第{1}次对心偏差Y";
                //if (Math.Abs(dx) > ParamSetMgr.GetInstance().GetDoubleParam(strXoffset) || Math.Abs(dy) > ParamSetMgr.GetInstance().GetDoubleParam(strYoffset))
                //{

                //}
                UserTest.FailResultAB.OC = false;
                ParamSetMgr.GetInstance().SetBoolParam($"第{index}次对心", false);
                Err($"{stationAAName}工位第{index}次对心,计算的dx,dy 大于第{index}次对心设定值，将会退出AA流程");
                UserTest.TestResultAB[StationIndex].FailStep = $"第{index}次对心移动后计算超规格";
                return step;
            }
            else
            {
                Info($"{stationAAName}工位第{index}次对心完成后取图测试结果 OK，对心成功");
            }
            //如果不等于0 快速抓到最清晰位置
            double del = 0;
            if (AACount <= ParamSetMgr.GetInstance().GetIntParam("AA次数"))
            {
                if (ParamSetMgr.GetInstance().GetBoolParam("选择MF快速调焦算法") && (UserTest.mFHelpers.Count != 0))
                {
                    int count = 1;
                retryMF:
                    int Step = ParamSetMgr.GetInstance().GetIntParam("MF_校准步数");
                    double Distance = ParamSetMgr.GetInstance().GetDoubleParam("MF_校准范围");
                    double dstepdistance = Distance / (Step * 1.0);
                    int numSet = 0; int numGet = 0; double minSet = 1000000; double minGet = 1000000; ; double minSetValue = 0; double minGetValue = 0;
                    if (Array.Count > 10)
                    {
                        for (int t = 1; t < UserTest.mFHelpers.Count; t++)
                        {
                            //第一种    minSetValue = Math.Sqrt(Math.Pow(UserTest.mFHelpers[t].N0_Up_L_R - UserTest.mFHelpers[0].N0_Up_L_R, 2) + Math.Pow(UserTest.mFHelpers[t].N1_Right_U_D - UserTest.mFHelpers[0].N1_Right_U_D, 2) + Math.Pow(UserTest.mFHelpers[t].N2_Down_R_L - UserTest.mFHelpers[0].N2_Down_R_L, 2) + Math.Pow(UserTest.mFHelpers[t].N3_Left_D_W - UserTest.mFHelpers[0].N3_Left_D_W, 2) + Math.Pow(UserTest.mFHelpers[t].N4_UR_DL - UserTest.mFHelpers[0].N4_UR_DL, 2) + Math.Pow(UserTest.mFHelpers[t].N5_UL_DR - UserTest.mFHelpers[0].N5_UL_DR, 2));
                            //    minGetValue = Math.Sqrt(Math.Pow(UserTest.mFHelpers[t].N0_Up_L_R - Array[0], 2) + Math.Pow(UserTest.mFHelpers[t].N1_Right_U_D - Array[1], 2) + Math.Pow(UserTest.mFHelpers[t].N2_Down_R_L - Array[2], 2) + Math.Pow(UserTest.mFHelpers[t].N3_Left_D_W - Array[3], 2) + Math.Pow(UserTest.mFHelpers[t].N4_UR_DL - Array[4], 2) + Math.Pow(UserTest.mFHelpers[t].N5_UL_DR - Array[5], 2));
                            minSetValue = Math.Sqrt(Math.Pow(UserTest.mFHelpers[t].N4_UR_DL - UserTest.mFHelpers[0].N4_UR_DL, 2) + Math.Pow(UserTest.mFHelpers[t].N5_UL_DR - UserTest.mFHelpers[0].N5_UL_DR, 2));
                            minGetValue = Math.Sqrt(Math.Pow(UserTest.mFHelpers[t].N4_UR_DL - Array[4], 2) + Math.Pow(UserTest.mFHelpers[t].N5_UL_DR - Array[5], 2));
                            if (minSet > minSetValue)
                            {
                                numSet = t;
                                minSet = minSetValue;
                            }
                            if (minGet > minGetValue)
                            {
                                numGet = t;
                                minGet = minGetValue;
                            }
                        }
                        del = Distance / Step * (numSet - numGet);
                        int numSet2 = 0; int numGet2 = 0; double minSet2 = 1000000; double minGet2 = 1000000; double minSetValue2 = 0; double minGetValue2 = 0;
                        if (del < 0)
                        {
                            for (int t = numSet; t < UserTest.mFHelpers.Count; t++)
                            {
                                minSetValue2 = Math.Abs(UserTest.mFHelpers[t].N6_CT - UserTest.mFHelpers[0].N6_CT);
                                minGetValue2 = Math.Abs(UserTest.mFHelpers[t].N6_CT - Array[6]);
                                if (minSet2 > minSetValue2)
                                {
                                    numSet2 = t;
                                    minSet2 = minSetValue2;
                                }
                                if (minGet2 > minGetValue2)
                                {
                                    numGet2 = t;
                                    minGet2 = minGetValue2;
                                }
                            }

                        }
                        else
                        {
                            for (int t = 1; t < numSet; t++)
                            {
                                minSetValue2 = Math.Abs(UserTest.mFHelpers[t].N6_CT - UserTest.mFHelpers[0].N6_CT);
                                minGetValue2 = Math.Abs(UserTest.mFHelpers[t].N6_CT - Array[6]);
                                if (minSet2 > minSetValue2)
                                {
                                    numSet2 = t;
                                    minSet2 = minSetValue2;
                                }
                                if (minGet2 > minGetValue2)
                                {
                                    numGet2 = t;
                                    minGet2 = minGetValue2;
                                }
                            }
                        }

                        del = Distance / Step * (numSet2 - numGet2);
                    }

                    if ((Math.Abs(del) > Math.Abs(Distance) / 2.5) || del == 0)
                    {
                        if (count < 2)
                        {
                            MoveSigleAxisPosWaitInpos(AxisZ, AAOriPosZ + del, (double)SpeedType.Mid, 0.005, bmanual, this);
                            UserTest.FailResultAB.Play = true;
                            UserTest.FailResultAB.SFR = true;
                            Thread.Sleep(ParamSetMgr.GetInstance().GetIntParam("电机到位延时") * 2);
                            if (!ModuleMgr.Instance.CaptureToBmpRGB(StationIndex, 1, ref bt))
                            {
                                UserTest.FailResultAB.Play = false;
                                UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次对心移动后拍照异常";
                                return step;
                            }
                            Array = new List<double>();
                            if (!AlgorithmMgr.Instance.Findcenter((Bitmap)bt.Clone(), ref dx, ref dy, ref Array, ParamSetMgr.GetInstance().GetBoolParam("选择MF快速调焦算法")))
                            {
                                UserTest.FailResultAB.OC = false;
                                UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次对心移动后计算异常";
                                return step;
                            }
                            count++;
                            goto retryMF;
                        }

                        Err($"{stationAAName}工位第{index}次MF结果{del}：超过规格{Distance}) ");
                        del = 0;
                    }
                }
                else
                {
                    if (!ParamSetMgr.GetInstance().GetBoolParam("选择MF快速调焦算法"))
                    {
                        Warn($"未选择MF快速调焦算法");
                    }
                    else
                    {
                        Warn($"MF文件异常，请检查！ ");
                    }

                }
            }
            UserTest.RunLog.Write($"调整偏差{del}", LogType.Info, PathHelper.LogPathAuto);
            AAOriPosZ = AAOriPosZ + del;
            MoveSigleAxisPosWaitInpos(AxisZ, AAOriPosZ, (double)SpeedType.Mid, 0.005, bmanual, this);
            Thread.Sleep(200);
            if (AACount <= ParamSetMgr.GetInstance().GetIntParam("AA次数"))
            {
                step = StationStep.Step_ThroughFocus;
            }
            else
            {
                step = StationStep.Step_Check;//Check
            }
            if (AACount == 1)
            {
                UserTest.TestTimeInfo.Center_1 = (DateTime.Now - UserTest.TestTimeInfo.Center_1_Begin).TotalSeconds;
            }
            else if (AACount == 2)
            {
                UserTest.TestTimeInfo.Center_2 = (DateTime.Now - UserTest.TestTimeInfo.Center_2_Begin).TotalSeconds;
            }
            else
            {
                UserTest.TestTimeInfo.Center_3 = (DateTime.Now - UserTest.TestTimeInfo.Center_3_Begin).TotalSeconds;
            }
            return step;
        }
        public StationStep StepThroughFocusRun(int AACount, VisionControl visionControl = null, bool bmanual = false)
        {
            bool TryAgin = true;
        try_TF:
            if (AACount <= 1)
            {
                UserTest.TestTimeInfo.TF_1_Begin = DateTime.Now;
            }
            else
            {
                UserTest.TestTimeInfo.TF_2_Begin = DateTime.Now;
            }

            //获取当前夹取位置是A工位还是B工位
            int StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
            string stationAAName = StationIndex == 0 ? "A" : "B";
            if (stationAAName == "A")
            {
                GlobelManualResetEvent.ContinueShowA.Reset();
            }
            else
            {
                GlobelManualResetEvent.ContinueShowB.Reset();
            }
            StationStep step = StationStep.Step_End;
            int index = 1;
            if (AACount > 1)
            {
                index = 2;
            }
            ParamSetMgr.GetInstance().SetBoolParam($"{stationAAName}工位第{index}次ThroughFocus", true);
            string strStepNum = $"第{index}次Throughfocus步数";
            string strExtent = $"第{index}次Throughfocus范围";
            if (ParamSetMgr.GetInstance().GetBoolParam("是否侧向UV"))
                IOMgr.GetInstace().WriteIoBit("侧向UV气缸", true);
            double upDistance = ParamSetMgr.GetInstance().GetDoubleParam("第二次AA上升距离");
            double PosZ = 0;
            if (AACount == 1)
            {
                // PosZ = GetStationPointDic()[$"{stationAAName}工位AA位"].pointZ;//获取AA位置
                PosZ = MotionMgr.GetInstace().GetAxisPos(AxisZ);
            }
            else if (AACount == 2)
            {
                PosZ = MotionMgr.GetInstace().GetAxisPos(AxisZ) + upDistance;
            }
            else
            {
                PosZ = MotionMgr.GetInstace().GetAxisPos(AxisZ);
            }
            double MoveZ = PosZ;
            if (ParamSetMgr.GetInstance().GetBoolParam("AA位置为清晰位"))
            {
                MoveZ = PosZ - (ParamSetMgr.GetInstance().GetDoubleParam(strExtent)) / 2.0;
            }
            double dstepdistance = 1.0 * (ParamSetMgr.GetInstance().GetDoubleParam(strExtent)) / ParamSetMgr.GetInstance().GetIntParam(strStepNum);
            MoveSigleAxisPosWaitInpos(AxisZ, MoveZ, (double)SpeedType.High, 0.005, bmanual, this);
            Thread.Sleep(100);
            if (index == 1)
            {
                UserTest.TestResultAB[StationIndex].AA_1_BeforePosZ = MoveZ;
            }
            else
            {
                UserTest.TestResultAB[StationIndex].AA_2_BeforePosZ = MoveZ;
            }
            Info($"{stationAAName}工位第{AACount}次ThroughFocus Z开始{MoveZ}, 步数:{ParamSetMgr.GetInstance().GetIntParam(strStepNum)},范围：{ParamSetMgr.GetInstance().GetDoubleParam(strExtent)}");

            int nTimeDelay = 0;
            nTimeDelay = ParamSetMgr.GetInstance().GetIntParam("电机到位延时");
            if (nTimeDelay < 0 || nTimeDelay > 3000)
                nTimeDelay = 500;
            int nCount = ParamSetMgr.GetInstance().GetIntParam(strStepNum);
            if (nCount < 0 || nCount > 30)
                nCount = 20;
            Bitmap bt = null;
            UserTest.FailResultAB.Tilt = true;
            DateTime old = DateTime.Now;
            List<SFRValue> sFRInfos = new List<SFRValue>();
            SFRValue sFRValue = new SFRValue();
            RectInfo rectInfo = new RectInfo();
            LightValue lightValue = new LightValue();
            Rectangle[] rectangles = new Rectangle[13];
            int isRetryAgain = 0;
            for (int i = 0; i < nCount; i++)
            {
                if (nCount > ParamSetMgr.GetInstance().GetIntParam(strStepNum) * 2 + 3)
                {
                    UserTest.FailResultAB.Tilt = false;
                    Err($"{stationAAName}工位第{AACount}次ThroughFoucs_{nCount} 超过2倍步数，异常 ");
                    return step;
                }
                DateTime move = DateTime.Now;
                double pos = MoveZ + i * dstepdistance;
                MoveSigleAxisPosWaitInpos(AxisZ, pos, (double)SpeedType.Mid, 0.005, bmanual, this);
                Thread.Sleep(nTimeDelay);
                DateTime moveEnd = DateTime.Now;
                UserTest.FailResultAB.Play = true;
                UserTest.FailResultAB.SFR = true;
                if (!ModuleMgr.Instance.CaptureToBmpRGB(StationIndex, 1, ref bt))
                {
                    UserTest.FailResultAB.Play = false;
                    MoveSigleAxisPosWaitInpos(AxisZ, PosZ, (double)SpeedType.Mid, 0.005, bmanual, this);//回到初始位置
                    Err($"{stationAAName}工位第{AACount}次ThroughFoucs_{i} 拍照异常 ");
                    UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{nCount}次Throughfocus拍照异常";
                    return step;
                }
                DateTime img = DateTime.Now;
                if (!AlgorithmMgr.Instance.GetSFRValue((Bitmap)bt.Clone(), ref sFRValue, ref rectInfo, rectangles, ref lightValue, true))
                {
                    UserTest.FailResultAB.SFR = false;
                    MoveSigleAxisPosWaitInpos(AxisZ, PosZ, (double)SpeedType.Mid, 0.005, bmanual, this);//回到初始位置
                    Err($"{stationAAName}工位第{AACount}次ThroughFoucs_{i} 算法数据回传错误 ");
                    return step;
                }
                DateTime alg = DateTime.Now;
                ImageHelper.Instance.SaveImage($"{PathHelper.ImagePathThroughFocus}{AACount}_Throughfocus_{i}_{UserTest.TestResultAB[StationIndex].SerialNumber}_{pos}_{DateTime.Now.ToString("HHmmssfff")}.bmp", (Bitmap)bt.Clone());
                Form_Auto.EvenShowImageDelegate(bt, $"{PathHelper.ImagePathSFRResult}{AACount}_Throughfocus_SFR_{i}_{pos}.png", false, sFRValue, rectInfo, rectangles, lightValue);
                sFRValue.dZ = MotionMgr.GetInstace().GetAxisPos(AxisZ);
                sFRInfos.Add(sFRValue);
                if (i == nCount - 1)
                {
                    double del = ParamSetMgr.GetInstance().GetDoubleParam("TF_sfr步骤结束最小差值");
                    double minCT = 0; double minUL = 0; double minUR = 0; double minDL = 0; double minDR = 0;
                    for (int k = 0; k < sFRInfos.Count; k++)
                    {
                        if (minCT < sFRInfos[k].block[0].dValue + del)
                        {
                            minCT = sFRInfos[k].block[0].dValue;
                            if (k == (sFRInfos.Count - 1))
                            {
                                nCount = nCount + 3;
                                break;
                            }
                        }
                        if (minUL < sFRInfos[k].block[1].dValue + del)
                        {
                            minUL = sFRInfos[k].block[1].dValue;
                            if (k == (sFRInfos.Count - 1))
                            {
                                nCount = nCount + 3;
                                break;
                            }
                        }
                        if (minUR < sFRInfos[k].block[2].dValue + del)
                        {
                            minUR = sFRInfos[k].block[2].dValue;
                            if (k == (sFRInfos.Count - 1))
                            {
                                nCount = nCount + 3;
                                break;
                            }
                        }
                        if (minDL < sFRInfos[k].block[3].dValue + del)
                        {
                            minDL = sFRInfos[k].block[3].dValue;
                            if (k == (sFRInfos.Count - 1))
                            {
                                nCount = nCount + 3;
                                break;
                            }
                        }
                        if (minDR < sFRInfos[k].block[4].dValue + del)
                        {
                            minDR = sFRInfos[k].block[4].dValue;
                            if (k == (sFRInfos.Count - 1))
                            {
                                nCount = nCount + 3;
                                break;
                            }
                        }
                    }
                    string path = $"{PathHelper.ImagePathChart}{UserTest.TestResultAB[StationIndex].AACount}_{DateTime.Now.ToString("HHmmssfff")}.bmp";
                    Form_Auto.EvenShowChartDelegate(sFRInfos, index, pos, dstepdistance, path);//最后一次刷新，要存最后一张Chart图
                    if (bmanual)
                        Form_Manual.EvenShowChartDelegate(sFRInfos, index, pos, dstepdistance, "");
                }
                if (i == 3)
                {
                    double lowDel = ParamSetMgr.GetInstance().GetDoubleParam("TF_sfr步骤开始最小差值");
                    bool isLow = (sFRInfos[0].block[0].dValue > sFRInfos[1].block[0].dValue + lowDel) && (sFRInfos[1].block[0].dValue > sFRInfos[2].block[0].dValue + lowDel);
                    isLow |= (sFRInfos[0].block[1].dValue > sFRInfos[1].block[1].dValue + lowDel) && (sFRInfos[1].block[1].dValue > sFRInfos[2].block[1].dValue + lowDel);
                    isLow |= (sFRInfos[0].block[2].dValue > sFRInfos[1].block[2].dValue + lowDel) && (sFRInfos[1].block[2].dValue > sFRInfos[2].block[2].dValue + lowDel);
                    isLow |= (sFRInfos[0].block[3].dValue > sFRInfos[1].block[3].dValue + lowDel) && (sFRInfos[1].block[3].dValue > sFRInfos[2].block[3].dValue + lowDel);
                    isLow |= (sFRInfos[0].block[4].dValue > sFRInfos[1].block[4].dValue + lowDel) && (sFRInfos[1].block[4].dValue > sFRInfos[2].block[4].dValue + lowDel);
                    if (isLow && (isRetryAgain < 2))//五条曲线，有下降的就重新扫描
                    {
                        i = 0;
                        isRetryAgain++;
                        MoveZ = PosZ - (ParamSetMgr.GetInstance().GetDoubleParam(strExtent)) * (1 + isRetryAgain) / 2.0;
                        MoveSigleAxisPosWaitInpos(AxisZ, MoveZ, (double)SpeedType.High, 0.005, bmanual, this);
                        Thread.Sleep(1000);
                        sFRInfos.Clear();
                    }
                }
                Form_Auto.EvenShowChartDelegate(sFRInfos, index, pos, dstepdistance, "");
                if (bmanual)
                    Form_Manual.EvenShowChartDelegate(sFRInfos, index, pos, dstepdistance, "");
                DateTime show = DateTime.Now;

                double time10 = (moveEnd - move).TotalSeconds;
                double time11 = (img - moveEnd).TotalSeconds;
                double time12 = (alg - img).TotalSeconds;
                double time13 = (show - alg).TotalSeconds;
            }

            double time1 = (DateTime.Now - old).TotalSeconds;
            CSVHelper.Instance.SaveSFRInfoData(UserTest.TestResultAB[StationIndex].SerialNumber, sFRInfos, index.ToString());
            int icount = nCount;
            double dtx = 0; double dty = 0; double preakZ = 0;
            if (!AlgorithmMgr.Instance.GetTiltValue(sFRInfos.ToArray(), ref preakZ, ref dtx, ref dty))
            {
                double posTFZ = PosZ; double maxSfr = 0;
                for (int p = 0; p < sFRInfos.Count; p++)
                {
                    if (sFRInfos[p].block[0].dValue > maxSfr)
                    {
                        maxSfr = sFRInfos[p].block[0].dValue;
                        posTFZ = sFRInfos[p].dZ;
                    }
                }
                if (TryAgin)
                {
                    TryAgin = false;

                    if (maxSfr != 0)
                    {
                        UserTest.RunLog.Write($"Tilt计算失败，重试！", LogType.Wran, PathHelper.LogPathAuto);
                        MoveSigleAxisPosWaitInpos(AxisZ, posTFZ, (double)SpeedType.Mid, 0.005, bmanual, this);//回到peak位置
                        goto try_TF;
                    }
                }
                UserTest.FailResultAB.Tilt = false;
                MoveSigleAxisPosWaitInpos(AxisZ, PosZ, (double)SpeedType.Mid, 0.005, bmanual, this);//回到初始位置
                Err($"{stationAAName}工位第{AACount}次ThroughFoucs 算法数据回传错误 ");
                UserTest.RunLog.Write($"{stationAAName}工位第{AACount}次ThroughFoucs 算法数据回传错误 ", LogType.Wran, PathHelper.LogPathAuto);

                UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次TF移动后计算异常";
                ParamSetMgr.GetInstance().SetBoolParam($"{stationAAName}工位第{index}次ThroughFocus", false);
                return step;
            }
            if (AACount > ParamSetMgr.GetInstance().GetIntParam("AA次数") - 1)
            {
                double maxValuePreak = preakZ;
                if (ParamSetMgr.GetInstance().GetStringParam("AA模式选择") == "Average")
                {
                    //计算四周值
                    double centerDel = ParamSetMgr.GetInstance().GetDoubleParam("中心四边阈值差允许最大值") * 1.5;
                    double cornerDel = ParamSetMgr.GetInstance().GetDoubleParam("四边阈值差允许最大值") * 1.5;
                    List<SFRValue> Infos = new List<SFRValue>();
                    double Max = 0;
                    foreach (var a in sFRInfos)
                    {
                        if ((Math.Abs(a.block[0].dValue - a.block[1].dValue) < centerDel) && (Math.Abs(a.block[0].dValue - a.block[1].dValue) < centerDel) && (Math.Abs(a.block[0].dValue - a.block[1].dValue) < centerDel) && (Math.Abs(a.block[0].dValue - a.block[1].dValue) < centerDel))
                        {
                            if ((Math.Abs(a.block[1].dValue - a.block[2].dValue) < cornerDel) && (Math.Abs(a.block[1].dValue - a.block[3].dValue) < cornerDel) && (Math.Abs(a.block[1].dValue - a.block[4].dValue) < cornerDel) && (Math.Abs(a.block[2].dValue - a.block[3].dValue) < cornerDel) && (Math.Abs(a.block[2].dValue - a.block[4].dValue) < cornerDel) && (Math.Abs(a.block[3].dValue - a.block[4].dValue) < cornerDel))
                            {
                                Infos.Add(a);
                            }
                        }
                    }
                    //double maxValue = 0;
                    //foreach (var b in Infos)
                    //{
                    //    if ((b.block[1].dValue + b.block[2].dValue + b.block[3].dValue + b.block[4].dValue) > maxValue)
                    //    {
                    //        maxValue = b.block[1].dValue + b.block[2].dValue + b.block[3].dValue + b.block[4].dValue;
                    //        preakZ = b.dZ;
                    //    }
                    //}
                    double max0 = 0; double max1 = 0; double max2 = 0; double max3 = 0; double max4 = 0;
                    double peak0 = 0; double peak1 = 0; double peak2 = 0; double peak3 = 0; double peak4 = 0;
                    foreach (var b in Infos)
                    {
                        if (b.block[0].dValue > max0)
                        {
                            max0 = b.block[0].dValue;
                            peak0 = b.dZ;
                        }
                        if (b.block[1].dValue > max1)
                        {
                            max1 = b.block[1].dValue;
                            peak1 = b.dZ;
                        }
                        if (b.block[2].dValue > max2)
                        {
                            max2 = b.block[2].dValue;
                            peak2 = b.dZ;
                        }
                        if (b.block[3].dValue > max3)
                        {
                            max3 = b.block[3].dValue;
                            peak3 = b.dZ;
                        }
                        if (b.block[4].dValue > max4)
                        {
                            max4 = b.block[4].dValue;
                            peak4 = b.dZ;
                        }
                    }
                    if (Infos != null && Infos.Count != 0)
                        preakZ = (peak0 + peak1 + peak2 + peak3 + peak4) / 5;
                    Info($"原来值{maxValuePreak}，Peak值{preakZ}。{peak0},{peak1},{peak2},{peak3},{peak4}.");
                }
                //  Info($"原来值{maxValuePreak}，Peak值{preakZ}。{peak0},{peak1},{peak2},{peak3},{peak4}.");
            }

            Warn($"{stationAAName}工位第{AACount}次ThroughFoucs ,Tx：{dtx},Ty：{dty}");
            Info($"{stationAAName}工位第{AACount}次ThroughFocus 步数:{ParamSetMgr.GetInstance().GetIntParam(strStepNum).ToString("F4")}," +
              $"范围：{ParamSetMgr.GetInstance().GetDoubleParam(strExtent).ToString("F4")}，" +
              $"AA前，轴位置 Z：{MotionMgr.GetInstace().GetAxisPos(AxisZ).ToString("F4")}");
            UserTest.RunLog.Write($"{stationAAName}工位第{AACount}次ThroughFocus 步数:{ParamSetMgr.GetInstance().GetIntParam(strStepNum).ToString("F4")}," +
              $"范围：{ParamSetMgr.GetInstance().GetDoubleParam(strExtent).ToString("F4")}，" +
              $"AA前，轴位置 Z：{MotionMgr.GetInstace().GetAxisPos(AxisZ).ToString("F4")}", LogType.Info, PathHelper.LogPathAuto);

            UserTest.RunLog.Write($"{stationAAName},第{AACount}次TF像素角度 ,Tx:{dtx.ToString("0.000")},Ty:{dty.ToString("0.000")}", LogType.Wran, PathHelper.LogPathAuto);


            string strtxmax = $"第{index}次最大Tx";
            string strtymax = $"第{index}次最大Ty";



            double MoveDTX = dtx;
            double MoveDTY = dty;
            if (ParamSetMgr.GetInstance().GetBoolParam("TXTY是否对调"))
            {
                MoveDTX = dty;
                MoveDTY = dtx;
            }
            if (ParamSetMgr.GetInstance().GetBoolParam("TX是否负")) MoveDTX = -MoveDTX;
            if (ParamSetMgr.GetInstance().GetBoolParam("TY是否负")) MoveDTY = -MoveDTY;

            double txgain = ParamSetMgr.GetInstance().GetDoubleParam("TXGain");
            double tygain = ParamSetMgr.GetInstance().GetDoubleParam("TYGain");
            double dtxTrue = MoveDTX * txgain;
            double dtyTrue = MoveDTY * tygain;
            Info($"，peckz:{preakZ.ToString("F4")},物理偏差Tx：{(dtxTrue).ToString("F4")},物理偏差Ty：{(dtyTrue).ToString("F4")},规格:{ParamSetMgr.GetInstance().GetDoubleParam(strtxmax)},{ParamSetMgr.GetInstance().GetDoubleParam(strtymax)} ");

            bool brtn = Math.Abs(dtxTrue) < ParamSetMgr.GetInstance().GetDoubleParam(strtxmax) && Math.Abs(dtyTrue) < ParamSetMgr.GetInstance().GetDoubleParam(strtymax);
            if (!brtn)
            {
                UserTest.FailResultAB.Tilt = false;
                Err($"{stationAAName}工位第{AACount}次ThroughFoucs dtx dty规格判断结果 NG");
                ParamSetMgr.GetInstance().SetBoolParam($"第{index}次ThroughFocus", false);
                MoveSigleAxisPosWaitInpos(AxisZ, PosZ, (double)SpeedType.Mid, 0.005, bmanual, this);//回到初始位置
                UserTest.TestResultAB[StationIndex].FailStep = $"{stationAAName}工位第{index}次TF计算超规格";
                Thread.Sleep(1000);
                return step;
            }
            else
            {
                Warn($"{stationAAName}工位第{AACount}次ThroughFoucs dtx dty规格判断结果 OK");
                ParamSetMgr.GetInstance().SetDoubleParam($"第{index}次dtx", dtx * txgain);
                ParamSetMgr.GetInstance().SetDoubleParam($"第{index}次dty", dty * tygain);
                //Z调整
                MoveSigleAxisPosWaitInpos(AxisZ, MoveZ, (double)SpeedType.Mid, 0.005, bmanual, this);
                Thread.Sleep(200);
                MoveSigleAxisPosWaitInpos(AxisZ, preakZ, (double)SpeedType.Mid, 0.005, bmanual, this);
                Info($"{stationAAName}工位第{index}次ThroughFoucs  Z 调整： z轴移动:{preakZ.ToString("F4")}，,");
            }
            if (index == 1)
            {
                UserTest.TestResultAB[StationIndex].AA_1_TiltX = dtxTrue;
                UserTest.TestResultAB[StationIndex].AA_1_TiltY = dtyTrue;
                UserTest.TestResultAB[StationIndex].AA_1_AfterPosZ = MotionMgr.GetInstace().GetAxisPos(AxisZ);
            }
            else
            {
                UserTest.TestResultAB[StationIndex].AA_2_TiltX = dtxTrue;
                UserTest.TestResultAB[StationIndex].AA_2_TiltY = dtyTrue;
                UserTest.TestResultAB[StationIndex].AA_2_AfterPosZ = MotionMgr.GetInstace().GetAxisPos(AxisZ);
            }
            step = StationStep.Step_Tilt;
            if (AACount <= 1)
            {
                UserTest.TestTimeInfo.TF_1 = (DateTime.Now - UserTest.TestTimeInfo.TF_1_Begin).TotalSeconds;
            }
            else
            {
                UserTest.TestTimeInfo.TF_2 = (DateTime.Now - UserTest.TestTimeInfo.TF_2_Begin).TotalSeconds;
            }
            if (bmanual)
                CSVHelper.Instance.SaveToCSVPath(PathHelper.TFCsvPath, new TFResult { Z = MotionMgr.GetInstace().GetAxisPos(AxisZ).ToString("0.0000"), TiltX = dtxTrue.ToString("0.00000"), TiltY = dtyTrue.ToString("0.00000") });

            return step;
        }
        public StationStep StepTiltRun(int AACount, VisionControl visionControl = null, bool bmanual = false)
        {  //获取当前夹取位置是A工位还是B工位
            if (AACount <= 1)
            {
                UserTest.TestTimeInfo.Tilt_1_Begin = DateTime.Now;
            }
            else
            {
                UserTest.TestTimeInfo.Tilt_2_Begin = DateTime.Now;
            }

            int StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
            string stationAAName = StationIndex == 0 ? "A" : "B";
            if (stationAAName == "A")
            {
                GlobelManualResetEvent.ContinueShowA.Reset();
            }
            else
            {
                GlobelManualResetEvent.ContinueShowB.Reset();
            }

            StationStep step = StationStep.Step_End;
            int index = 1;
            if (AACount > 1)
            {
                index = 2;
            }
            double txcurpos = MotionMgr.GetInstace().GetAxisPos(AxisTx);
            double tycurpos = MotionMgr.GetInstace().GetAxisPos(AxisTy);
            double txdstpos = 0;
            double tydstpos = 0;
            if (index == 1)
            {
                txdstpos = txcurpos + UserTest.TestResultAB[StationIndex].AA_1_TiltX;
                tydstpos = tycurpos + UserTest.TestResultAB[StationIndex].AA_1_TiltY;
                UserTest.RunLog.Write($"第{AACount}次AA Tilt({UserTest.TestResultAB[StationIndex].AA_1_TiltX},{UserTest.TestResultAB[StationIndex].AA_1_TiltY})", LogType.Info, PathHelper.LogPathAuto);
            }
            else
            {
                txdstpos = txcurpos + UserTest.TestResultAB[StationIndex].AA_2_TiltX;
                tydstpos = tycurpos + UserTest.TestResultAB[StationIndex].AA_2_TiltY;
                UserTest.RunLog.Write($"第{AACount}次AA Tilt({UserTest.TestResultAB[StationIndex].AA_2_TiltX},{UserTest.TestResultAB[StationIndex].AA_2_TiltY})", LogType.Info, PathHelper.LogPathAuto);

            }
            //tilt调整
            if (index == ParamSetMgr.GetInstance().GetIntParam("AA次数") && !bmanual)
            {
                Info($"第{index}次ThroughFoucs Tilt 不调整；");
            }
            else
            {
                MoveMulitAxisPosWaitInpos(new int[] { AxisTx, AxisTy }, new double[] { txdstpos, tydstpos }, new double[] { (double)SpeedType.Mid, (double)SpeedType.Mid }, 0.003, bmanual, this);
                Info($"第{index}次ThroughFoucs  Tilt 调整：TX移动后坐标:{txdstpos.ToString("0.000")},TY移动坐标:{ tydstpos.ToString("0.000")} ");
                Thread.Sleep(200);
            }

            ParamSetMgr.GetInstance().SetBoolParam($"{stationAAName}工位第{index}次ThroughFocus", false);
            if (AACount <= ParamSetMgr.GetInstance().GetIntParam("AA次数"))//移动Tilt次数为准
            {
                UserTest.TestResultAB[StationIndex].AACount++;
            }
            else
            {
                UserTest.TestResultAB[StationIndex].AACount = 1;
            }
            if (AACount <= 1)
            {
                UserTest.TestTimeInfo.Tilt_1 = (DateTime.Now - UserTest.TestTimeInfo.Tilt_1_Begin).TotalSeconds;
            }
            else
            {
                UserTest.TestTimeInfo.Tilt_2 = (DateTime.Now - UserTest.TestTimeInfo.Tilt_2_Begin).TotalSeconds;
            }
            step = StationStep.Step_FindCenter;
            return step;
        }
        public StationStep StepAACheckRun(VisionControl visionControl = null, bool bmanual = false)
        {
            StationStep step = StationStep.Step_End;
            int StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
            if (StationIndex == 0)
            {
                GlobelManualResetEvent.ContinueShowA.Reset();
            }
            else
            {
                GlobelManualResetEvent.ContinueShowB.Reset();
            }
            UserTest.TestResultAB[StationIndex].Result = AAcheck(StationIndex, CheckType.UVBefore, bmanual);
            step = StationStep.Step_UV;//Check
            return step;
        }
        public StationStep StepUVRun(VisionControl visionControl = null, bool bmanual = false)
        {
            UserTest.TestTimeInfo.UVTime_Begin = DateTime.Now;
            StationStep step = StationStep.Step_End;
            //获取当前夹取位置是A工位还是B工位
            int StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
            string stationLensName = StationIndex == 0 ? "A" : "B";
            if (stationLensName == "A")
            {
                GlobelManualResetEvent.ContinueShowA.Reset();
            }
            else
            {
                GlobelManualResetEvent.ContinueShowB.Reset();
            }
            bool uvResult = true;
            Info("AA  UV");
            if (ParamSetMgr.GetInstance().GetBoolParam("屏蔽UV"))
                return step;

            if (ParamSetMgr.GetInstance().GetBoolParam("是否侧向UV"))
            {

            retry_LRuv:
                IOMgr.GetInstace().WriteIoBit("侧向UV气缸", true);
                WaranResult waran = CheckIobyName("左侧UV到位", true, "左侧UV到位失败", bmanual);
                if (waran == WaranResult.Retry)
                    goto retry_LRuv;

                waran = CheckIobyName("右侧UV到位", true, "右侧UV到位失败", bmanual);
                if (waran == WaranResult.Retry)
                    goto retry_LRuv;
            }



            int time = ParamSetMgr.GetInstance().GetIntParam("失败UV时间");
            if (UserTest.TestResultAB[StationIndex].Result)
            {
                time = ParamSetMgr.GetInstance().GetIntParam("UV时间");
            }
            if (time == 0)
                return step;
            //胶缩补偿
            double Z = MotionMgr.GetInstace().GetAxisPos(AxisZ);
            double del = ParamSetMgr.GetInstance().GetDoubleParam("胶缩补偿");
            MoveSigleAxisPosWaitInpos(AxisZ, Z + del, (double)SpeedType.Mid, 0.005, bmanual, this);//到胶缩补偿位置
            bool DispEnable = ParamSetMgr.GetInstance().GetBoolParam("屏蔽点胶");
            if (!DispEnable)
            {
                ParamSetMgr.GetInstance().SetBoolParam("UV", true);
                IOMgr.GetInstace().WriteIoBit("UV固化", true);
                Thread.Sleep(100);
                IOMgr.GetInstace().WriteIoBit("UV固化", false);
                Thread.Sleep(100);
                IOMgr.GetInstace().WriteIoBit("UV固化", true);
                UserTest.RunLog.Write($"{stationLensName}工位打开UV灯", LogType.Info, PathHelper.LogPathAuto);
                Thread.Sleep(time);
                IOMgr.GetInstace().WriteIoBit("UV固化", false);
                ParamSetMgr.GetInstance().SetBoolParam("UV", false);
                if (ParamSetMgr.GetInstance().GetBoolParam("UV后Check"))
                {
                    Thread.Sleep(500);
                    if (!AAcheck(StationIndex, CheckType.UVAfter, bmanual))
                    {
                        UserTest.TestResultAB[StationIndex].Result = false;
                    }
                }
            }

        retry_open:
            IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", false);
            WaranResult waranResult = CheckIobyName("夹爪气缸原位", true, "夹爪气缸回原位失败", bmanual, 3000);
            if (waranResult == WaranResult.Retry)
                goto retry_open;
            if (ParamSetMgr.GetInstance().GetBoolParam("是否侧向UV"))
            {

            retry_LRuv:
                IOMgr.GetInstace().WriteIoBit("侧向UV气缸", false);
                WaranResult waran = CheckIobyName("左侧UV原位", true, "左侧UV原位失败", bmanual);
                if (waran == WaranResult.Retry)
                    goto retry_LRuv;

                waran = CheckIobyName("右侧UV原位", true, "右侧UV原位失败", bmanual);
                if (waran == WaranResult.Retry)
                    goto retry_LRuv;


            }
            Thread.Sleep(1000);
            if (ParamSetMgr.GetInstance().GetBoolParam("夹爪松开Check") && !DispEnable)
            {
                if (!AAcheck(StationIndex, CheckType.GripOpen, bmanual))
                {
                    UserTest.TestResultAB[StationIndex].Result = false;
                }
            }
            UserTest.TestTimeInfo.UVTime = (DateTime.Now - UserTest.TestTimeInfo.UVTime_Begin).TotalSeconds;
            return step;

        }
        public StationStep StepEndRun(VisionControl visionControl = null, bool bmanual = false)
        {
         
            retry_open:
                IOMgr.GetInstace().WriteIoBit("平行光管升降气缸", false);
            if (ParamSetMgr.GetInstance().GetBoolParam("是否用平行光管或中继镜气缸"))
            {
                WaranResult waranResult = CheckIobyName("平行光管上升到位", true, "平行光管上升到位失败", bmanual, 3000);
                if (waranResult == WaranResult.Retry)
                    goto retry_open;
            }
            //获取当前夹取位置是A工位还是B工位
            int StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
            string stationLensName = StationIndex == 0 ? "A" : "B";
            if (stationLensName == "A")
            {
                GlobelManualResetEvent.ContinueShowA.Reset();
            }
            else
            {
                GlobelManualResetEvent.ContinueShowB.Reset();
            }
            StationStep step = StationStep.Step_Stop;
            EndSafe(bmanual);//走到安全位置 上料位置
            Info($"tilt调整前x：{UserTest.TestResultAB[StationIndex].AA_1_TiltX}y:{ UserTest.TestResultAB[StationIndex].AA_1_TiltY}，调整后x：{ UserTest.TestResultAB[StationIndex].AA_2_TiltX}y:{UserTest.TestResultAB[StationIndex].AA_2_TiltY}");
            Info("AA成功 结束,回准备位置");
            step = StationStep.Step_GetCmdFromTable;
            TableData.GetInstance().SetStationResult("A_AA", true);
            TableData.GetInstance().SetStationResult("B_AA", true);
            TableData.GetInstance().SetStationResult("A_Pick", true);
            TableData.GetInstance().SetStationResult("B_Pick", true);
            if (!bmanual)
            {
                if (!(UserTest.FailResultAB.Tilt && UserTest.FailResultAB.OC && UserTest.FailResultAB.SFR && UserTest.FailResultAB.Play))
                {
                    UserTest.TestResultAB[StationIndex].Result = false;
                }
                //拷贝图片
                ImageHelper.Instance.CopyImageToFail(UserTest.TestResultAB[StationIndex].SerialNumber, UserTest.TestResultAB[StationIndex].Result);
                //删除图片
                double passTime = ParamSetMgr.GetInstance().GetDoubleParam("保存OK图片天数");
                double failTime = ParamSetMgr.GetInstance().GetDoubleParam("保存NG图片天数");
                ImageHelper.Instance.DeleImage(passTime, failTime);
                if (UserTest.TestResultAB[StationIndex].Result)
                {
                    SocketMgr.GetInstance().socketArr[StationIndex].socketState = SocketState.HaveOK;
                }
                else
                {

                    SocketMgr.GetInstance().socketArr[StationIndex].socketState = SocketState.HaveNG;
                }
                UserTest.CTTestAB[StationIndex].Star = false;
                UserTest.CTTestAB[StationIndex].End = true;
                UserTest.CTTestAB[StationIndex].Show = false;
            }
            else
            {
                UserTest.FailResultAB.Tilt = true;
                UserTest.FailResultAB.OC = true;
                UserTest.FailResultAB.SFR = true;
                UserTest.FailResultAB.Play = true;
                UserTest.TestResultAB[StationIndex].Result = true;
            }
            UserTest.TestTimeInfo.AAend = DateTime.Now;
            UserTest.TestTimeInfo.AATime = (DateTime.Now - UserTest.TestTimeInfo.AAbegin).TotalSeconds;
            CSVHelper.Instance.SaveToCSVPath(PathHelper.TestTimeCsvPath, UserTest.TestTimeInfo);
            Task.Run(() =>
            {
                ModuleMgr.Instance.Stop(StationIndex);
                IOMgr.GetInstace().WriteIoBit($"{stationLensName}模组上电", false);
                if (UserTest.TestResultAB[StationIndex].Result)
                {
                    IOMgr.GetInstace().WriteIoBit($"{stationLensName}模组上电", true);
                    Thread.Sleep(200);
                    ModuleMgr.Instance.Play(StationIndex);
                    if (ParamSetMgr.GetInstance().GetBoolParam("是否写入OC值") && ParamSetMgr.GetInstance().GetStringParam("工位点亮类型").Contains("DT"))
                    {
                        int SlaveID = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("SlaveID"), 16);
                        int addr = Convert.ToInt32(ParamSetMgr.GetInstance().GetStringParam("OC烧录地址"), 16);
                        byte valueX = 0; byte valueY = 0;
                        int oc_x = (int)(1000 * UserTest.TestResultAB[StationIndex].UVBeforeOC_X / ParamSetMgr.GetInstance().GetDoubleParam("[Sensor] dPixelSize"));
                        int oc_y = (int)(1000 * UserTest.TestResultAB[StationIndex].UVBeforeOC_Y / ParamSetMgr.GetInstance().GetDoubleParam("[Sensor] dPixelSize"));
                        if (ParamSetMgr.GetInstance().GetBoolParam("夹爪松开Check"))
                        {
                            oc_x = (int)(UserTest.TestResultAB[StationIndex].GripOpenOC_X / ParamSetMgr.GetInstance().GetDoubleParam("[Sensor] dPixelSize"));
                            oc_y = (int)(UserTest.TestResultAB[StationIndex].GripOpenOC_Y / ParamSetMgr.GetInstance().GetDoubleParam("[Sensor] dPixelSize"));
                        }
                        if (oc_x < 0)
                            valueX = (byte)(oc_x + 256);
                        else
                            valueX = (byte)(oc_x);
                        if (oc_y < 0)
                            valueY = (byte)(oc_y + 256);
                        else
                            valueY = (byte)(oc_y);
                        int s = valueY << 8;
                        s = s + valueX;

                        int value = Convert.ToInt32(s.ToString("X4"), 16);
                        UserTest.RunLog.Write($"addr:{addr},value:{value}", LogType.Info, PathHelper.LogPathAuto);

                        ModuleMgr.Instance.WriteI2C(StationIndex, Convert.ToByte(SlaveID), 0, addr, value);
                    }
                    ModuleMgr.Instance.Stop(StationIndex);
                    IOMgr.GetInstace().WriteIoBit($"{stationLensName}模组上电", false);
                }


                IOMgr.GetInstace().WriteIoBit($"{stationLensName}Lens升降气缸", true);

            });
            if (UserTest.TestResultAB[StationIndex].Result)
                UserTest.RunLog.Write($"测试结果：结果{UserTest.TestResultAB[StationIndex].Result}", LogType.Wran, PathHelper.LogPathAuto);
            else
                UserTest.RunLog.Write($"测试结果：结果{UserTest.TestResultAB[StationIndex].Result}", LogType.Err, PathHelper.LogPathAuto);
            return step;
        }
        #endregion

        #region 方法
        public bool GoAAHome(bool bmanual = false)
        {
            ParamSetMgr.GetInstance().SetBoolParam("AA回零完成", false);
            bool bRtn = true;
            WaranResult waranResult = WaranResult.Run;
        retry_openclmp:
            IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", false);
            waranResult = CheckIobyName("夹爪气缸原位", true, "夹爪气缸打开失败", bmanual);
            if (waranResult == WaranResult.Retry)
                goto retry_openclmp;
            
            retry_UpColl:
                IOMgr.GetInstace().WriteIoBit("平行光管升降气缸", false);
            if (ParamSetMgr.GetInstance().GetBoolParam("是否用平行光管或中继镜气缸"))
            {
                waranResult = CheckIobyName("平行光管上升到位", true, "平行光管上升失败", bmanual);
                if (waranResult == WaranResult.Retry)
                    goto retry_UpColl;
            }
            IOMgr.GetInstace().WriteIoBit("启动按钮灯", false);
            IOMgr.GetInstace().WriteIoBit("OK指示绿灯", false);
            IOMgr.GetInstace().WriteIoBit("NG指示红灯", false);
            //if (!IOMgr.GetInstace().ReadIoInBit("A治具盖上检测"))
            if (!SysFunConfig.LodUnloadPatten.IsSafeWhenURun("A"))
            {
                Err("检查A治具盖上检测失败，不允许复位！");
                return false;
            }

            //if (!IOMgr.GetInstace().ReadIoInBit("B治具盖上检测"))
            if (!SysFunConfig.LodUnloadPatten.IsSafeWhenURun("B"))
            {
                Err("检查B治具盖上检测失败，不允许复位！");
                return false;
            }

            bool pathRslt = ParamSetMgr.GetInstance().GetBoolParam("屏蔽Chart轴运动");
            if (!pathRslt)
            {
                StationTable stationTable = (StationTable)StationMgr.GetInstance().GetStation("转盘站");
                double chartZ = stationTable.GetStationPointDic()["Chart工作位"].pointZ;
                Task.Run(() =>
                {
                    if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(stationTable.AxisZ) != AxisHomeFinishFlag.Homed)
                    {
                        stationTable.HomeSigleAxisPosWaitInpos(stationTable.AxisZ, this, 120000, bmanual);
                    }
                    stationTable.MoveSigleAxisPosWaitInpos(stationTable.AxisZ, chartZ, (double)SpeedType.High, 0.05, bmanual);
                });
            }

            Info("AA站 Z轴回原点");
            Task[] tasks = new Task[6];

            tasks[0] = Task.Run(() =>
            {
                if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisZ) != AxisHomeFinishFlag.Homed)
                {
                    HomeSigleAxisPosWaitInpos(AxisZ, this, 120000, bmanual);
                }
            });
            Thread.Sleep(2000);
            IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", true);
            Info("AA站 X轴回原点");
            tasks[1] = Task.Run(() =>
            {
                if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisY) != AxisHomeFinishFlag.Homed)
                {
                    HomeSigleAxisPosWaitInpos(AxisY, this, 120000, bmanual);
                }
            });
            Info("AA站 Y轴回原点");
            tasks[2] = Task.Run(() =>
            {
                if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisX) != AxisHomeFinishFlag.Homed)
                {
                    HomeSigleAxisPosWaitInpos(AxisX, this, 120000, bmanual);
                }
            });
            Info("AA站 Tz轴回原点");
            tasks[3] = Task.Run(() =>
            {
                if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisU) != AxisHomeFinishFlag.Homed)
                {
                    HomeSigleAxisPosWaitInpos(AxisU, this, 120000, bmanual);
                }
            });
            Info("AA站 Tx轴回原点");
            tasks[4] = Task.Run(() =>
            {
                if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisTx) != AxisHomeFinishFlag.Homed)
                {
                    HomeSigleAxisPosWaitInpos(AxisTx, this, 120000, bmanual);
                }
            });
            Info("AA站 Ty轴回原点");
            tasks[5] = Task.Run(() =>
            {
                if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisTy) != AxisHomeFinishFlag.Homed)
                {
                    HomeSigleAxisPosWaitInpos(AxisTy, this, 120000, bmanual);
                }
            });
            if (!Task.WaitAll(tasks, 120000))
            {
                Info("AA 6轴回原点失败");
                if (!bmanual)
                    return false;
            }
            if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisTy) != AxisHomeFinishFlag.Homed)
            {
                HomeSigleAxisPosWaitInpos(AxisTy, this, 120000, bmanual);
            }
            Info("AA 6轴回原点成功");
            IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", false);
            ParamSetMgr.GetInstance().SetBoolParam("AA回零完成", true);
            return true;
        }
        public void PickFromPlane(int indexpick, bool bmanual)
        {
            if (bmanual || PlaneMgr.GetInstance().PlaneArr[indexpick].planeState == PlaneState.Have)
            {

                string strPickPosName = indexpick == 0 ? "A工位夹取位" : "B工位夹取位";
                double x = GetStationPointDic()[strPickPosName].pointX;
                double y = GetStationPointDic()[strPickPosName].pointY;
                double z = GetStationPointDic()[strPickPosName].pointZ;
                Info("去" + strPickPosName + "位置,开始抓取物料");
                MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisZ }, new double[] { x, y, z },
                    new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, }, 0.05, bmanual, this, 30000);
                MoveSigleAxisPosWaitInpos(AxisX, z + 1, (double)SpeedType.High, 0.05, bmanual, this, 30000);
                Info("去" + strPickPosName + "位置,抓取物料成功");
            }

        }
        public void GoAAReadySafe(bool bmanual = false)
        {
            CheckLRuvCliyder(bmanual);
            IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", false);
            WaranResult waranResult = WaranResult.Run;
           
            retry_UpColl:
                IOMgr.GetInstace().WriteIoBit("平行光管升降气缸", false);
            if (ParamSetMgr.GetInstance().GetBoolParam("是否用平行光管或中继镜气缸"))
            {
                waranResult = CheckIobyName("平行光管上升到位", true, "平行光管上升失败", bmanual);
                if (waranResult == WaranResult.Retry)
                    goto retry_UpColl;
            }
        retry_downlensA:
            IOMgr.GetInstace().WriteIoBit($"ALens升降气缸", false);
            waranResult = CheckIobyName($"ALens下降到位", true, "Lens下降到位失败", bmanual);
            if (waranResult == WaranResult.Retry)
                goto retry_downlensA;
            retry_downlensB:
            IOMgr.GetInstace().WriteIoBit($"BLens升降气缸", false);
            waranResult = CheckIobyName($"BLens下降到位", true, "Lens下降到位失败", bmanual);
            if (waranResult == WaranResult.Retry)
                goto retry_downlensB;
            double x1 = GetStationPointDic()["安全位置"].pointX;
            double y1 = GetStationPointDic()["安全位置"].pointY;
            double z1 = GetStationPointDic()["安全位置"].pointZ;
            double tx = GetStationPointDic()["安全位置"].pointTx;
            double ty = GetStationPointDic()["安全位置"].pointTy;
            double tz = GetStationPointDic()["安全位置"].pointU;
            Info("AA站：移动到准备位置");
            MoveSigleAxisPosWaitInpos(AxisZ, z1, (double)SpeedType.High, 0.05, bmanual);//先往上和往后移动
            MoveSigleAxisPosWaitInpos(AxisY, y1, (double)SpeedType.High, 0.05, bmanual);
            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisZ, AxisU, AxisTx, AxisTy }, new double[] { x1, y1, z1, tz, tx, ty },
                new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High }, 0.05, bmanual);
        }
        public bool AAcheck(int StationIndex, CheckType checkType, bool bmanual)
        {
            double centerMin = ParamSetMgr.GetInstance().GetDoubleParam("中心阀值最小值");
            double cornerMin = ParamSetMgr.GetInstance().GetDoubleParam("四边阈值最小值");
            double centerDel = ParamSetMgr.GetInstance().GetDoubleParam("中心四边阈值差允许最大值");
            double cornerDel = ParamSetMgr.GetInstance().GetDoubleParam("四边阈值差允许最大值");
            double centerH = ParamSetMgr.GetInstance().GetDoubleParam("中心H最小值");
            double centerV = ParamSetMgr.GetInstance().GetDoubleParam("中心V最小值");
            double cornerH = ParamSetMgr.GetInstance().GetDoubleParam("四边H最小值");
            double cornerV = ParamSetMgr.GetInstance().GetDoubleParam("四边V最小值");
            double oc_x = ParamSetMgr.GetInstance().GetDoubleParam("第2次对心偏差X");
            double oc_y = ParamSetMgr.GetInstance().GetDoubleParam("第2次对心偏差Y");
            Info("AAcheck 开始");
            bool bResult = false;
            Bitmap bt = null;
            SFRValue sFRValue = new SFRValue();
            RectInfo rectInfo = new RectInfo();
            LightValue lightValue = new LightValue();
            List<double> Array = new List<double>();
            Rectangle[] rectangles = new Rectangle[13];
            double dx = 0; double dy = 0;
            List<double> re = new List<double>();
            double posZ = MotionMgr.GetInstace().GetAxisPos(AxisZ);
            UserTest.FailResultAB.Play = true;
            UserTest.FailResultAB.SFR = true;
            UserTest.FailResultAB.OC = true;
            Thread.Sleep(ParamSetMgr.GetInstance().GetIntParam("电机到位延时"));
            if (!ModuleMgr.Instance.CaptureToBmpRGB(StationIndex, 1, ref bt))
            {
                UserTest.FailResultAB.Play = false;
                UserTest.TestResultAB[StationIndex].FailStep = $"{checkType}AAcheck拍照异常";
                return false;
            }
            if (!AlgorithmMgr.Instance.GetSFRValue((Bitmap)bt.Clone(), ref sFRValue, ref rectInfo, rectangles, ref lightValue, true))
            {
                UserTest.FailResultAB.SFR = false;
                return false;
            }
            if (!AlgorithmMgr.Instance.Findcenter((Bitmap)bt.Clone(), ref dx, ref dy, ref Array, false))
            {
                UserTest.FailResultAB.OC = false;
                return false;
            }
            switch (checkType)
            {
                case CheckType.UVBefore:
                    ImageHelper.Instance.SaveImage($"{PathHelper.ImagePathCheck}UV前_Check_{UserTest.TestResultAB[StationIndex].SerialNumber}_{posZ}_{DateTime.Now.ToString("HHmmssfff")}.bmp", (Bitmap)bt.Clone());
                    Form_Auto.EvenShowImageDelegate(bt, PathHelper.ImagePathCheck + "UV前_SFR_" + posZ.ToString() + ".png", true, sFRValue, rectInfo, rectangles, lightValue);
                    break;
                case CheckType.UVAfter:
                    ImageHelper.Instance.SaveImage($"{PathHelper.ImagePathCheck}UV后_Check_{UserTest.TestResultAB[StationIndex].SerialNumber}_{posZ}_{DateTime.Now.ToString("HHmmssfff")}.bmp", (Bitmap)bt.Clone());
                    Form_Auto.EvenShowImageDelegate(bt, PathHelper.ImagePathCheck + "UV后_SFR_" + posZ.ToString() + ".png", true, sFRValue, rectInfo, rectangles, lightValue);
                    break;
                case CheckType.GripOpen:
                    ImageHelper.Instance.SaveImage($"{PathHelper.ImagePathCheck}夹爪打开_Check_{UserTest.TestResultAB[StationIndex].SerialNumber}_{posZ}_{DateTime.Now.ToString("HHmmssfff")}.bmp", (Bitmap)bt.Clone());
                    Form_Auto.EvenShowImageDelegate(bt, PathHelper.ImagePathCheck + "夹爪打开_SFR_" + posZ.ToString() + ".png", true, sFRValue, rectInfo, rectangles, lightValue);
                    break;
                case CheckType.Product:
                    ImageHelper.Instance.SaveImage($"{PathHelper.ImagePathProduct}成品_Check_{UserTest.TestResultAB[StationIndex].SerialNumber}_{posZ}_{DateTime.Now.ToString("HHmmssfff")}.bmp", (Bitmap)bt.Clone());
                    Form_Auto.EvenShowImageDelegate(bt, PathHelper.ImagePathProduct + "成品_SFR_" + posZ.ToString() + ".png", true, sFRValue, rectInfo, rectangles, lightValue);
                    break;
                default:
                    break;
            }

            Warn($"AACheck,{checkType}  z轴位置：{posZ}" + $"计算SFR数值：中间:{sFRValue.block[0].dValue},左上:{sFRValue.block[1].dValue},右上 :{sFRValue.block[2].dValue}," + $"左下:{sFRValue.block[3].dValue}, 右下:{sFRValue.block[4].dValue}");
            re.Add(sFRValue.block[1].dValue);
            re.Add(sFRValue.block[2].dValue);
            re.Add(sFRValue.block[3].dValue);
            re.Add(sFRValue.block[4].dValue);
            switch (checkType)
            {
                case CheckType.UVBefore:
                    UserTest.TestResultAB[StationIndex].UVBeforeSFR_CT_Value = sFRValue.block[0].dValue;
                    UserTest.TestResultAB[StationIndex].UVBeforeSFR_UL_Value = sFRValue.block[1].dValue;
                    UserTest.TestResultAB[StationIndex].UVBeforeSFR_UR_Value = sFRValue.block[2].dValue;
                    UserTest.TestResultAB[StationIndex].UVBeforeSFR_DR_Value = sFRValue.block[3].dValue;
                    UserTest.TestResultAB[StationIndex].UVBeforeSFR_DL_Value = sFRValue.block[4].dValue;
                    UserTest.TestResultAB[StationIndex].UVBeforeSFRCenterDel = Math.Abs(sFRValue.block[0].dValue - re.Average());
                    UserTest.TestResultAB[StationIndex].UVBeforeSFRCornerDel = re.Max() - re.Min();
                    UserTest.TestResultAB[StationIndex].UVBeforeOC_X = dx;
                    UserTest.TestResultAB[StationIndex].UVBeforeOC_Y = dy;
                    break;
                case CheckType.UVAfter:
                    UserTest.TestResultAB[StationIndex].UVAfterSFR_CT_Value = sFRValue.block[0].dValue;
                    UserTest.TestResultAB[StationIndex].UVAfterSFR_UL_Value = sFRValue.block[1].dValue;
                    UserTest.TestResultAB[StationIndex].UVAfterSFR_UR_Value = sFRValue.block[2].dValue;
                    UserTest.TestResultAB[StationIndex].UVAfterSFR_DR_Value = sFRValue.block[3].dValue;
                    UserTest.TestResultAB[StationIndex].UVAfterSFR_DL_Value = sFRValue.block[4].dValue;
                    UserTest.TestResultAB[StationIndex].UVAfterSFRCenterDel = Math.Abs(sFRValue.block[0].dValue - re.Average());
                    UserTest.TestResultAB[StationIndex].UVAfterSFRCornerDel = re.Max() - re.Min();
                    UserTest.TestResultAB[StationIndex].UVAfterOC_X = dx;
                    UserTest.TestResultAB[StationIndex].UVAfterOC_Y = dy;
                    break;
                case CheckType.GripOpen:
                    UserTest.TestResultAB[StationIndex].GripOpenSFR_CT_Value = sFRValue.block[0].dValue;
                    UserTest.TestResultAB[StationIndex].GripOpenSFR_UL_Value = sFRValue.block[1].dValue;
                    UserTest.TestResultAB[StationIndex].GripOpenSFR_UR_Value = sFRValue.block[2].dValue;
                    UserTest.TestResultAB[StationIndex].GripOpenSFR_DR_Value = sFRValue.block[3].dValue;
                    UserTest.TestResultAB[StationIndex].GripOpenSFR_DL_Value = sFRValue.block[4].dValue;
                    UserTest.TestResultAB[StationIndex].GripOpenSFRCenterDel = Math.Abs(sFRValue.block[0].dValue - re.Average());
                    UserTest.TestResultAB[StationIndex].GripOpenSFRCornerDel = re.Max() - re.Min();
                    UserTest.TestResultAB[StationIndex].GripOpenOC_X = dx;
                    UserTest.TestResultAB[StationIndex].GripOpenOC_Y = dy;
                    break;
                case CheckType.Product:
                    UserTest.ProductCheckResultAB[StationIndex].CheckSFR_CT_Value = sFRValue.block[0].dValue;
                    UserTest.ProductCheckResultAB[StationIndex].CheckSFR_UL_Value = sFRValue.block[1].dValue;
                    UserTest.ProductCheckResultAB[StationIndex].CheckSFR_UR_Value = sFRValue.block[2].dValue;
                    UserTest.ProductCheckResultAB[StationIndex].CheckSFR_DR_Value = sFRValue.block[3].dValue;
                    UserTest.ProductCheckResultAB[StationIndex].CheckSFR_DL_Value = sFRValue.block[4].dValue;
                    UserTest.ProductCheckResultAB[StationIndex].CheckSFRCenterDel = Math.Abs(sFRValue.block[0].dValue - re.Average());
                    UserTest.ProductCheckResultAB[StationIndex].CheckSFRCornerDel = re.Max() - re.Min();
                    UserTest.ProductCheckResultAB[StationIndex].CheckOC_X = dx;
                    UserTest.ProductCheckResultAB[StationIndex].CheckOC_Y = dy;
                    break;
            }
            if (checkType == CheckType.Product)//成品检测结果
            {
                bResult = (sFRValue.block[0].dValue > centerMin) && (re.Min() > cornerMin) && (Math.Abs(sFRValue.block[0].dValue - re.Average()) < centerDel) && (re.Max() - re.Min() < cornerDel);
                bResult &= (Math.Abs(dx) < Math.Abs(oc_x)) && (dy) < Math.Abs(oc_y);
                UserTest.ProductCheckResultAB[StationIndex].Result = bResult;
            }
            //门限判定
            UserTest.RunLog.Write($"oc:({dx.ToString("0.00000")},{dy.ToString("0.00000")}),sfr:({sFRValue.block[0].dValue.ToString("0.00")},{sFRValue.block[1].dValue.ToString("0.00")},{sFRValue.block[2].dValue.ToString("0.00")},{sFRValue.block[3].dValue.ToString("0.00")},{sFRValue.block[4].dValue.ToString("0.00")})", LogType.Info, PathHelper.LogPathAuto);
            bResult = (sFRValue.block[0].dValue > centerMin) && (re.Min() > cornerMin) && (Math.Abs(sFRValue.block[0].dValue - re.Average()) < centerDel) && (re.Max() - re.Min() < cornerDel);
            if (!bResult)
            {
                UserTest.FailResultAB.SFR = false;
                UserTest.TestResultAB[StationIndex].FailStep = $"{checkType} SFR计算超规格";
                string errInfo = $"{UserTest.TestResultAB[StationIndex].FailStep}，sfr值({sFRValue.block[0].dValue},{sFRValue.block[1].dValue},{sFRValue.block[2].dValue},{sFRValue.block[3].dValue},{sFRValue.block[4].dValue}),规格(中心min{centerMin},四角min{cornerMin},中心四角差{centerDel},四角差{cornerDel})";
                UserTest.RunLog.Write(errInfo, LogType.Err, PathHelper.LogPathAuto);
                Err(errInfo);
                return false;
            }
            else
            {
                bResult &= (Math.Abs(dx) < Math.Abs(oc_x)) && Math.Abs(dy) < Math.Abs(oc_y);
                if (!bResult)
                {
                    UserTest.FailResultAB.OC = false;
                    UserTest.TestResultAB[StationIndex].FailStep = $"{checkType} OC计算超规格";
                    string errInfo = $"{UserTest.TestResultAB[StationIndex].FailStep},oc测试值({dx},{dy}),规格({oc_x},{oc_y})";
                    UserTest.RunLog.Write(errInfo, LogType.Err, PathHelper.LogPathAuto);
                    Err(errInfo);
                    return false;
                }
                else
                {
                    bResult &= (sFRValue.block[0].aryValue[1] > centerH) && (sFRValue.block[0].aryValue[3] > centerH) && (sFRValue.block[0].aryValue[2] > centerV) && (sFRValue.block[0].aryValue[4] > centerV);
                    if (!bResult)
                    {
                        UserTest.FailResultAB.SFR = false;
                        UserTest.TestResultAB[StationIndex].FailStep = $"{checkType} 中心SFR HV方向计算超规格";
                        string errInfo = $"{checkType} 中心SFR HV方向计算超规格,请查看check结果图片";
                        UserTest.RunLog.Write(errInfo, LogType.Err, PathHelper.LogPathAuto);
                        Err(errInfo);
                        return false;
                    }
                    bResult &= (sFRValue.block[1].aryValue[1] >= cornerV) && (sFRValue.block[1].aryValue[3] >= cornerV) && (sFRValue.block[1].aryValue[2] >= cornerH) && (sFRValue.block[1].aryValue[4] >= cornerH);
                    bResult &= (sFRValue.block[2].aryValue[1] >= cornerV) && (sFRValue.block[2].aryValue[3] >= cornerV) && (sFRValue.block[2].aryValue[2] >= cornerH) && (sFRValue.block[2].aryValue[4] >= cornerH);
                    bResult &= (sFRValue.block[3].aryValue[1] >= cornerV) && (sFRValue.block[3].aryValue[3] >= cornerV) && (sFRValue.block[3].aryValue[2] >= cornerH) && (sFRValue.block[3].aryValue[4] >= cornerH);
                    bResult &= (sFRValue.block[4].aryValue[1] >= cornerV) && (sFRValue.block[4].aryValue[3] >= cornerV) && (sFRValue.block[4].aryValue[2] >= cornerH) && (sFRValue.block[4].aryValue[4] >= cornerH);
                    if (!bResult)
                    {
                        UserTest.FailResultAB.SFR = false;
                        UserTest.TestResultAB[StationIndex].FailStep = $"{checkType} 四周SFR HV方向计算超规格";
                        string errInfo = $"{checkType} 四周SFR HV方向计算超规格,请查看check结果图片";
                        UserTest.RunLog.Write(errInfo, LogType.Err, PathHelper.LogPathAuto);
                        Err(errInfo);
                        return false;
                    }
                }
            }
            return true;

        }
        public void EndSafe(bool bmanual)
        {

        retry_UpColl:
            IOMgr.GetInstace().WriteIoBit("平行光管升降气缸", false);
            if (ParamSetMgr.GetInstance().GetBoolParam("是否用平行光管或中继镜气缸"))
            {
                WaranResult waranResult1 = CheckIobyName("平行光管上升到位", true, "平行光管上升失败", bmanual);
                if (waranResult1 == WaranResult.Retry)
                    goto retry_UpColl;
            }
        retry_open:
            IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", false);
            WaranResult waranResult2 = CheckIobyName("夹爪气缸原位", true, "夹爪气缸原位 到位失败", bmanual, 3000);
            if (waranResult2 == WaranResult.Retry)
                goto retry_open;
            CheckLRuvCliyder(bmanual);
            Info("回准备位置");
            double safeX = GetStationPointDic()["安全位置"].pointX;
            double safeY = GetStationPointDic()["安全位置"].pointY;
            double safeZ = GetStationPointDic()["安全位置"].pointZ;
            double safeU = GetStationPointDic()["安全位置"].pointU;
            double safeTx = GetStationPointDic()["安全位置"].pointTx;
            double safeTy = GetStationPointDic()["安全位置"].pointTy;
            MoveSigleAxisPosWaitInpos(AxisZ, safeZ, (double)SpeedType.High, 0.005, bmanual, this, 30000);//Z轴上抬到等待位置
            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisU, AxisTx, AxisTy }, new double[] { safeX, safeY, safeU, safeTx, safeTy }, new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High }, 0.005, bmanual, this);

        }
        public void GoProductcheck(int StationIndex, CheckType checkType)
        {
        retry_openclmp:
            IOMgr.GetInstace().WriteIoBit("夹爪气缸电磁阀", false);
            WaranResult waranResult1 = CheckIobyName("夹爪气缸原位", true, "夹爪气缸打开失败", true);
            if (waranResult1 == WaranResult.Retry)
                goto retry_openclmp;
            string stationAAName = StationIndex == 0 ? "A" : "B";
            double safeX = GetStationPointDic()["安全位置"].pointX;
            double safeY = GetStationPointDic()["安全位置"].pointY;
            double safeZ = GetStationPointDic()["安全位置"].pointZ;
            double safeU = GetStationPointDic()["安全位置"].pointU;
            double safeTx = GetStationPointDic()["安全位置"].pointTx;
            double safeTy = GetStationPointDic()["安全位置"].pointTy;
            //6轴平台移动到夹取位置，先移动XY后再移动Z
            double AAX = GetStationPointDic()[$"{stationAAName}工位AA位"].pointX;
            double AAY = GetStationPointDic()[$"{stationAAName}工位AA位"].pointY;
            double AAZ = GetStationPointDic()[$"{stationAAName}工位AA位"].pointZ;
            double AAU = GetStationPointDic()[$"{stationAAName}工位AA位"].pointU;
            double AATx = GetStationPointDic()[$"{stationAAName}工位AA位"].pointTx;
            double AATy = GetStationPointDic()[$"{stationAAName}工位AA位"].pointTy;
            MoveSigleAxisPosWaitInpos(AxisZ, safeZ, (double)SpeedType.High, 0.005, true, this, 30000);//Z轴上抬到等待位置
            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisU, AxisTx, AxisTy }, new double[] { safeX, safeY, safeU, safeTx, safeTy }, new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High }, 0.005, true, this);
            //6轴平台移动到AA位置，先移动XY后再移动Z
            MoveMulitAxisPosWaitInpos(new int[] { AxisX, AxisY, AxisU, AxisTx, AxisTy }, new double[] { AAX, AAY, AAU, AATx, AATy }, new double[] { (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High, (double)SpeedType.High }, 0.005, true, this);
            MoveSigleAxisPosWaitInpos(AxisZ, AAZ, (double)SpeedType.High, 0.005, true, this, 30000);
            //平行光管下降
            if (ParamSetMgr.GetInstance().GetBoolParam("是否用平行光管或中继镜气缸"))
            {
            retry_open:
                IOMgr.GetInstace().WriteIoBit("平行光管升降气缸", true);
                WaranResult waranResult = CheckIobyName("平行光管下降到位", true, "平行光管下降到位失败", true, 3000);
                if (waranResult == WaranResult.Retry)
                    goto retry_open;
            }
            Thread.Sleep(5000);
            AAcheck(StationIndex, checkType, true);





        }
        public bool MFTest()
        {
            bool result = false;
            double PosZ = MotionMgr.GetInstace().GetAxisPos(AxisZ);
            int Step = ParamSetMgr.GetInstance().GetIntParam("MF_校准步数");
            double Distance = ParamSetMgr.GetInstance().GetDoubleParam("MF_校准范围");
            double MoveZ = PosZ - Distance / 2.0;
            double dstepdistance = Distance / (Step * 1.0);
            Bitmap bt = null; double dx = 0; double dy = 0;
            int StationIndex = TableData.GetInstance().GetSocketNum(2, 0.5) - 1;
            //保留最清晰的那张图片值
            Thread.Sleep(ParamSetMgr.GetInstance().GetIntParam("电机到位延时"));
            if (!ModuleMgr.Instance.CaptureToBmpRGB(StationIndex, 1, ref bt))
            {
                return false;
            }
            List<double> array = new List<double>();
            if (!AlgorithmMgr.Instance.Findcenter((Bitmap)bt.Clone(), ref dx, ref dy, ref array, true))
            {
                ImageHelper.Instance.SaveImage(PathHelper.ImagePathMFFail, (Bitmap)bt.Clone());
                return false;
            }
            if (array.Count < 5)
            {
                return false;
            }
            UserTest.mFHelpers.Clear();
            UserTest.mFHelpers.Add(new MFHelper { N0_Up_L_R = array[0], N1_Right_U_D = array[1], N2_Down_R_L = array[2], N3_Left_D_W = array[3], N4_UR_DL = array[4], N5_UL_DR = array[5], N6_CT = array[6], N7_UL = array[7], N8_UR = array[8], N9_DR = array[9], N10_DL = array[10] });
            CSVHelper.Instance.SaveToCSVPath(PathHelper.MFCsvPath, UserTest.mFHelpers[0]);
            MoveSigleAxisPosWaitInpos(AxisZ, MoveZ, (double)SpeedType.Mid, 0.005, true, this);
            Thread.Sleep(2000);
            string mfPath = $"{ParamSetMgr.GetInstance().CurrentWorkDir}\\{ParamSetMgr.GetInstance().CurrentProductFile}\\MF.xml";
            File.Delete(mfPath);
            int nTimeDelay = ParamSetMgr.GetInstance().GetIntParam("电机到位延时") * 3;
            for (int i = 0; i < Step; i++)
            {
                DateTime move = DateTime.Now;
                double pos = MoveZ + i * dstepdistance;
                MoveSigleAxisPosWaitInpos(AxisZ, pos, (double)SpeedType.Mid, 0.005, true, this);
                Thread.Sleep(nTimeDelay);
                if (!ModuleMgr.Instance.CaptureToBmpRGB(StationIndex, 1, ref bt))
                {
                    MoveSigleAxisPosWaitInpos(AxisZ, PosZ, (double)SpeedType.Mid, 0.005, true, this);//回到初始位置
                    return false;
                }
                array = new List<double>();
                if (!AlgorithmMgr.Instance.Findcenter((Bitmap)bt.Clone(), ref dx, ref dy, ref array, ParamSetMgr.GetInstance().GetBoolParam("选择MF快速调焦算法")))
                {
                    MoveSigleAxisPosWaitInpos(AxisZ, PosZ, (double)SpeedType.Mid, 0.005, true, this);//回到初始位置
                    return false;
                }
                if (array.Count < 5)
                {
                    return false;
                }
                UserTest.mFHelpers.Add(new MFHelper { N0_Up_L_R = array[0], N1_Right_U_D = array[1], N2_Down_R_L = array[2], N3_Left_D_W = array[3], N4_UR_DL = array[4], N5_UL_DR = array[5], N6_CT = array[6], N7_UL = array[6], N8_UR = array[6], N9_DR = array[6], N10_DL = array[6] });
                CSVHelper.Instance.SaveToCSVPath(PathHelper.MFCsvPath, UserTest.mFHelpers[UserTest.mFHelpers.Count - 1]);
            }
            MFInfoFile.Save(mfPath);
            result = true;
            return result;
        }
        #endregion
    }

}