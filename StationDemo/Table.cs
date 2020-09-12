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
using UserData;
using OtherDevice;

namespace StationDemo
{
    public class StationTable : CommonTools.Stationbase
    {
        #region 变量
        int nTableRuns = 0;//转盘旋转次数
        #endregion

        #region 主流程
        public StationTable(string strStationName, int[] arrAxis, string[] axisname, params string[] CameraName) : base(strStationName, arrAxis, axisname, CameraName)
        {

        }
        public StationTable(CommonTools.Stationbase pb) : base(pb)
        {
            m_listIoInput.Add("急停");
            m_listIoInput.Add("气源检测");
            m_listIoInput.Add("安全门");
            m_listIoInput.Add("安全光栅");

            m_listIoOutput.Add("蜂鸣");

        }
        public enum StationStep
        {
            [Description("0.初始化步骤")]
            Step_Init = 300,//回零 设置初始工位
            [Description("1.等待所有工位完成")]
            Step_WaitAllStationFinsh,//等待所有工位执行完毕
            [Description("2.旋转转盘")]
            Step_StepTableRun,//旋转 每次旋转后位置加一
            [Description("3.NG停止流程")]
            Step_Stop,//异常流程，等待重新初始化
        }
        protected override bool InitStation()
        {
            ParamSetMgr.GetInstance().SetBoolParam("转盘站初始化完成", false);
            ParamSetMgr.GetInstance().SetIntParam("工位选择", -1);
            //ParamSetMgr.GetInstance().SetBoolParam
            ClearAllStep();
            PushMultStep((int)StationStep.Step_Init);
            return true;
        }
        protected override void StationWork(int step)
        {
            switch (step)
            {
                case (int)StationStep.Step_Init:
                    nTableRuns = 0;
                    Info("转盘站开始初始化");
                    DelCurrentStep();
                    PushMultStep((int)StepInitRun());
                    break;
                case (int)StationStep.Step_WaitAllStationFinsh:
                    DelCurrentStep();
                    PushMultStep((int)StepWaitAllStationFinsh());
                    Thread.Sleep(10);
                    break;
                case (int)StationStep.Step_StepTableRun:
                    DelCurrentStep();
                    PushMultStep((int)StepTableRun());
                    break;
                case (int)StationStep.Step_Stop:
                    DelCurrentStep();
                    Thread.Sleep(10);
                    break;
            }

        }
        public StationStep StepInitRun(bool bmanual = false)
        {
            StationStep step = StationStep.Step_Stop;
            ParamSetMgr.GetInstance().SetBoolParam("转盘站初始化完成", false);
            Info("转盘站 等待其他工位回零");
            WaranResult waranResult = doWhileWaitInit.doSomething(this, doWhileWaitInit, false, null);
            if (waranResult == WaranResult.TimeOut)
            {
                AlarmMgr.GetIntance().WarnWithDlg("转盘站回原点前，点胶工站或者AA工站复位时间过长，请检查，程序将会停止", this, CommonDlg.DlgWaranType.WaranOK, doWhileWaitInit, false);
                ClearAllStep();
                GlobalVariable.g_StationState = StationState.StationStateStop;
                return step;
            }
            Info("转盘站 开始回零");
            if (!GoTableHome(bmanual))
            {
                Err("转盘站 回零失败");
                throw new Exception("转盘轴回零失败！");
            }
            GoTableReadySafe(bmanual);
            ParamSetMgr.GetInstance().SetBoolParam("转盘站初始化完成", true);
            step = StationStep.Step_WaitAllStationFinsh;
            TableData.GetInstance().SetAllSationResultFalse();
            TableData.GetInstance().SetALLStartCmd();//到位启动
            return step;

        }
        public StationStep StepWaitAllStationFinsh(bool bmanual = false)
        {
            StationStep step = StationStep.Step_WaitAllStationFinsh;
            if (TableData.GetInstance().GetAllStationResults())
                step = StationStep.Step_StepTableRun;
            return step;

        }
        public StationStep StepTableRun(bool bmanual = false)
        {
            StationStep step = StationStep.Step_Stop;
            ParamSetMgr.GetInstance().SetBoolParam("AA完成", false);
            ParamSetMgr.GetInstance().SetBoolParam("点胶完成", false);
            ParamSetMgr.GetInstance().SetBoolParam("启动AA", false);
            ParamSetMgr.GetInstance().SetBoolParam("启动点胶", false);
            if (!checkSafe())
            {
                Err("安全检测失败,转盘禁止转动！");
                return step;
            }
            MoveSigleAxisPosWaitInpos(AxisU, TableData.GetInstance().listPoss[nTableRuns % TableData.GetInstance().listPoss.Count], (double)SpeedType.High, 0.1, false, this, 60000);
            nTableRuns++;
            if (nTableRuns == TableData.GetInstance().listPoss.Count)
                nTableRuns = 0;
            TableData.GetInstance().SetAllSationResultFalse();
            TableData.GetInstance().SetALLStartCmd();
            // ParamSetMgr.GetInstance().SetBoolParam("到位启动", true);
            ;
            step = StationStep.Step_WaitAllStationFinsh;
            return step;

        }
        #endregion

        #region 方法
        private bool checkSafe()
        {
            StationAA stationAA = (StationAA)StationMgr.GetInstance().GetStation("AA站");
            StationDisp stationDisp = (StationDisp)StationMgr.GetInstance().GetStation("点胶站");
            double SafeHeightAA = stationAA.GetStationPointDic()["安全位置"].pointZ;
            double SafeHeightDisp = stationDisp.GetStationPointDic()["安全位置"].pointZ;
            double getAAPos = MotionMgr.GetInstace().GetAxisPos(stationAA.AxisZ);
            double getDispPos = MotionMgr.GetInstace().GetAxisPos(stationDisp.AxisZ);
            if (MotionMgr.GetInstace().GetHomeFinishFlag(stationAA.AxisZ) != AxisHomeFinishFlag.Homed || getAAPos < SafeHeightAA - 0.5)
            {
                if (GlobalVariable.g_StationState != StationState.StationStateRun)
                {
                    MessageBox.Show("转盘运动前，检查AA站Z轴没有回原点或者低于安全高度", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                return false;
            }

            if (MotionMgr.GetInstace().GetHomeFinishFlag(stationDisp.AxisZ) != AxisHomeFinishFlag.Homed || getDispPos < SafeHeightDisp - 0.5)
            {
                if (GlobalVariable.g_StationState != StationState.StationStateRun)
                {
                    MessageBox.Show("转盘运动前，检查点胶站Z轴没有回原点或者低于吐胶点高度", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                return false;
            }

            //if (!IOMgr.GetInstace().ReadIoInBit("A治具盖上检测"))
            if (!SysFunConfig.LodUnloadPatten.IsSafeWhenURun("A"))
            {
                MessageBox.Show("转盘运动前，检查A治具盖上检测失败！", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }

            // if (!IOMgr.GetInstace().ReadIoInBit("B治具盖上检测"))
            if (!SysFunConfig.LodUnloadPatten.IsSafeWhenURun("B"))
            {
                MessageBox.Show("转盘运动前，检查B治具盖上检测失败！", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }

            Info($"安全位置检查成功,disp:{MotionMgr.GetInstace().GetAxisPos(stationDisp.AxisZ)}aa:{ MotionMgr.GetInstace().GetAxisPos(stationAA.AxisZ)}.");
            return true;
        }
        public bool GoTableHome(bool bmanual = false)
        {
            ParamSetMgr.GetInstance().SetBoolParam("转盘回零完成", false);
            if (!checkSafe())
            {
                return false;
            }
            Info("转盘开始回零");
            if (bmanual || MotionMgr.GetInstace().GetHomeFinishFlag(AxisU) != AxisHomeFinishFlag.Homed)
            {
                MotionMgr.GetInstace().ServoOn((short)AxisU);
                HomeSigleAxisPosWaitInpos(AxisU, this, 120000, bmanual);
            }
            ParamSetMgr.GetInstance().SetBoolParam("转盘回零完成", true);
            Info("转盘回零完成");
            return true;

        }
        public bool GoTableReadySafe(bool bmanual = false)
        {
            if (!checkSafe())
            {
                return false;
            }
            bool enableA = ParamSetMgr.GetInstance().GetBoolParam("屏蔽A工位");
            double AA_A = GetStationPointDic()["A工位AA位"].pointU;
            double AA_B = GetStationPointDic()["B工位AA位"].pointU;
            if (enableA)
            {
                nTableRuns = 2;
                MoveSigleAxisPosWaitInpos(AxisU, AA_A, (double)SpeedType.High, 0.1, bmanual, this, 60000);
            }
            else
            {
                MoveSigleAxisPosWaitInpos(AxisU, AA_B, (double)SpeedType.High, 0.1, bmanual, this, 60000);
            }
            return true;
        }
        DoWhile doWhileWaitInit = new DoWhile((time, dowhileobj, bmanual, paramobjs) =>
        {
            if (ParamSetMgr.GetInstance().GetBoolParam("AA工站初始化完成")
            && ParamSetMgr.GetInstance().GetBoolParam("AA工站初始化完成"))
            {
                return WaranResult.Run;
            }
            else if (time > 120000)
                return WaranResult.TimeOut;
            else return WaranResult.CheckAgain;
        }, int.MaxValue);
        public bool Urun(string posName, bool bManual = false)
        {
            if (!checkSafe())
            {
                return false;
            }
            WaranResult waranResult;
            if (GetStationPointDic().ContainsKey(posName))
            {
                double u = GetStationPointDic()[posName].pointU;
                waranResult = MoveSigleAxisPosWaitInpos(AxisU, u, (double)SpeedType.High, 0.03, bManual, this);
                if (waranResult == WaranResult.Run)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }


        #endregion



    }
}