using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gts;
using System.Diagnostics;
using Advantech.Motion;
using System.Windows.Forms;
using log4net;
namespace MotionIoLib
{
    public enum SpeedType
    {
        High,
        Mid,
        Low,

    }

    public class Motion_Advantech : MotionCardBase
    {
        IntPtr[] m_Axishand = null;
        IntPtr m_devPtr = new IntPtr();

        public Motion_Advantech(ulong indexCard, string strName, int nMinAxisNo, int nMaxAxisNo)
            : base(indexCard, strName, nMinAxisNo, nMaxAxisNo)
        {

        }
        //public override int GetAxisNo(int IndexAxis)
        //{
        //    return AxisInRang(IndexAxis) ? (IndexAxis - GetMinAxisNo() ) : int.MaxValue;
        //}
        public override bool Open()
        {
            m_bOpen = false;
            uint Result = Motion.mAcm_DevOpen((uint)m_nCardIndex, ref m_devPtr);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            uint AxesPerDev = 0;
            Result = Motion.mAcm_GetU32Property(m_devPtr, (uint)PropertyID.FT_DevAxesCount, ref AxesPerDev);
            if (Result != (uint)ErrorCode.SUCCESS || AxesPerDev <= 0)
            { throw new Exception(string.Format("运动卡{0}轴打开失败", m_strCardName)); return false; }
            else
            {
                m_Axishand = new IntPtr[AxesPerDev];
                for (int i = 0; i < AxesPerDev; i++)
                {
                    Result |= Motion.mAcm_AxOpen(m_devPtr, (UInt16)i, ref m_Axishand[i]);
                    Result |= Motion.mAcm_AxResetError(m_Axishand[i]);
                    if (Result != (uint)ErrorCode.SUCCESS)
                    {
                        throw new Exception(string.Format("运动卡{0},{1}轴打开失败", m_strCardName, i));
                        return false;
                    }
                }
                m_bOpen = true;
                string str = System.AppDomain.CurrentDomain.BaseDirectory;
                string strConfigPath = "Motion_" + m_nCardIndex.ToString("x") + ".cfg";
                strConfigPath = str + strConfigPath;
                Result = Motion.mAcm_DevLoadConfig(m_devPtr, strConfigPath);
                if (Result != (uint)ErrorCode.SUCCESS)
                {
                    string strErr = string.Format("运动卡{0}读配置文件异常", m_strCardName);

                    MessageBox.Show(strErr, "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception(string.Format("运动卡{0}读配置文件异常", m_strCardName));
                }
                return true;
            }
        }
        public override bool IsOpen()
        {
            return m_bOpen;
        }
        public override bool Close()
        {

            short rtn = 0;
            if (m_bOpen)
            {
                uint Result = Motion.mAcm_DevClose(ref m_devPtr);
                if (Result != (uint)ErrorCode.SUCCESS)
                    return false;

            }
            return (uint)ErrorCode.SUCCESS == rtn;
        }
        public override bool ServoOn(short nAxisNo)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            uint rtn = 0;
            uint svOn = 1;
            rtn = Motion.mAcm_AxSetSvOn(m_Axishand[nAxisNo], svOn);
            return (uint)ErrorCode.SUCCESS == rtn;
        }
        public override bool ServoOff(short nAxisNo)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            uint rtn = 0;
            uint svOn = 0;
            rtn = Motion.mAcm_AxSetSvOn(m_Axishand[nAxisNo], svOn);
            return (uint)ErrorCode.SUCCESS == rtn;
        }
        public bool ClearAlarm(short nAxisNo)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            uint rtn = 0;
            uint rsAlarm = 1;
            rtn = Motion.mAcm_AxResetAlm(m_Axishand[nAxisNo], rsAlarm);
            return (uint)ErrorCode.SUCCESS == rtn;
        }
        public override bool AbsMove(int nAxisNo, double nPos, double nSpeed)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;

            uint Result = 0;
            double speed = nSpeed;
            double Acc = m_MovePrm[nAxisNo].AccH;
            double Dcc = m_MovePrm[nAxisNo].DccH;

            switch (nSpeed)
            {
                case (double)0:
                    Acc = m_MovePrm[nAxisNo].AccH;
                    Dcc = m_MovePrm[nAxisNo].DccH;
                    speed = m_MovePrm[nAxisNo].VelH;
                    break;
                case (double)SpeedType.Mid:
                    Acc = m_MovePrm[nAxisNo].AccM;
                    Dcc = m_MovePrm[nAxisNo].DccM;
                    speed = m_MovePrm[nAxisNo].VelM;
                    break;
                case (double)SpeedType.Low:
                    Acc = m_MovePrm[nAxisNo].AccL;
                    Dcc = m_MovePrm[nAxisNo].DccL;
                    speed = m_MovePrm[nAxisNo].VelL;
                    break;

            }
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxVelLow, 0.0 * speed);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                return false;
            }

            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxVelHigh, speed);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                return false;
            }
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxAcc, Acc);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                return false;
            }

            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxDec, Dcc);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                return false;
            }
            double AxJerk = 0;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxJerk, AxJerk);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                return false;
            }
            Result |= Motion.mAcm_AxResetError(m_Axishand[nAxisNo]);
            Result |= Motion.mAcm_AxMoveAbs(m_Axishand[nAxisNo], nPos);

            return (Result == (uint)ErrorCode.SUCCESS);

        }
        public override bool RelativeMove(int nAxisNo, double nPos, double nSpeed)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;

            uint Result = 0;
            double speed = nSpeed;
            double Acc = m_MovePrm[nAxisNo].AccH;
            double Dcc = m_MovePrm[nAxisNo].AccL;
            switch (nSpeed)
            {
                case (double)0:
                    Acc = m_MovePrm[nAxisNo].AccH;
                    Dcc = m_MovePrm[nAxisNo].DccH;
                    speed = m_MovePrm[nAxisNo].VelH;
                    break;
                case (double)SpeedType.Mid:
                    Acc = m_MovePrm[nAxisNo].AccM;
                    Dcc = m_MovePrm[nAxisNo].DccM;
                    speed = m_MovePrm[nAxisNo].VelM;
                    break;
                case (double)SpeedType.Low:
                    Acc = m_MovePrm[nAxisNo].AccL;
                    Dcc = m_MovePrm[nAxisNo].DccL;
                    speed = m_MovePrm[nAxisNo].VelL;
                    break;
            }
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxVelLow, 0.0 * speed);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxVelHigh, speed);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxAcc, Acc);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;

            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxDec, Dcc);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            double AxJerk = 0;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxJerk, AxJerk);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            Result |= Motion.mAcm_AxResetError(m_Axishand[nAxisNo]);
            Result |= Motion.mAcm_AxMoveRel(m_Axishand[nAxisNo], nPos);
            return Result == (uint)ErrorCode.SUCCESS;
        }
        public override bool JogMove(int nAxisNo, bool bPositive, int bStart, double nSpeed)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            uint rtn = 0;

            double speed = nSpeed;
            double Acc = m_MovePrm[nAxisNo].AccH;
            double Dcc = m_MovePrm[nAxisNo].AccL;
            switch (nSpeed)
            {
                case (int)0:
                    Acc = m_MovePrm[nAxisNo].AccH;
                    Dcc = m_MovePrm[nAxisNo].DccH;
                    speed = m_MovePrm[nAxisNo].VelH;
                    break;
                case (int)SpeedType.Mid:
                    Acc = m_MovePrm[nAxisNo].AccM;
                    Dcc = m_MovePrm[nAxisNo].DccM;
                    speed = m_MovePrm[nAxisNo].VelM;
                    break;
                case (int)SpeedType.Low:
                    Acc = m_MovePrm[nAxisNo].AccL;
                    Dcc = m_MovePrm[nAxisNo].DccL;
                    speed = m_MovePrm[nAxisNo].VelL;
                    break;
            }
            uint Result = 0;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxVelLow, 0.3 * speed);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxVelHigh, speed);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxAcc, Acc);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;

            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxDec, Dcc);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            double AxJerk = 0;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxJerk, AxJerk);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            rtn |= Motion.mAcm_AxResetError(m_Axishand[nAxisNo]);
            rtn |= Motion.mAcm_AxMoveVel(m_Axishand[nAxisNo], bPositive ? (ushort)0 : (ushort)1);
            return rtn == 0;
        }
        public override bool StopAxis(int nAxisNo)
        {

            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            return (uint)ErrorCode.SUCCESS == Motion.mAcm_AxStopDec(m_Axishand[nAxisNo]);
        }
        public override bool StopEmg(int nAxisNo)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            return (uint)ErrorCode.SUCCESS == Motion.mAcm_AxStopEmg(m_Axishand[nAxisNo]);
        }
        public override bool ReasetAxis(int nAxisNo)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            return (uint)ErrorCode.SUCCESS == Motion.mAcm_AxResetError(m_Axishand[nAxisNo]);
        }
        public override long GetMotionIoState(int nAxisNo)
        {

            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return -1;
            long lStandardIo = 0;
            uint Reslut = 0; uint IOStatus = 0;
            Reslut |= Motion.mAcm_AxGetMotionIO(m_Axishand[nAxisNo], ref IOStatus);

            if (0 == Reslut)
            {
                if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_ALM) > 0)//ALM
                    lStandardIo |= (0x01 << 0); //驱动器报警存在第1位
                if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_LMTP) > 0)//+EL
                    lStandardIo |= (0x01 << 1); //正限位报警存在第2位
                if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_LMTN) > 0)//-EL
                    lStandardIo |= (0x01 << 2); //负限位报警存在第3位
                if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_EMG) > 0)//EMG
                    lStandardIo |= (0x01 << 4); //急停触发存在第5位
                if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_INP) > 0)//INP
                    lStandardIo |= (0x01 << 6); //电机到位标志存在第7位
                if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_SVON) > 0)//SVON
                    lStandardIo |= (0x01 << 7); //电机使能标志存在第8位
                if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_ORG) > 0)//ORG
                    lStandardIo |= (0x01 << 3); //原点到位存在第4位
                return lStandardIo;
            }
            else
            {
                return -1;
            }



        }
        public override bool GetServoState(int nAxisNo)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            uint Reslut = 0; uint IOStatus = 0;
            Reslut |= Motion.mAcm_AxGetMotionIO(m_Axishand[nAxisNo], ref IOStatus);

            if (0 == Reslut)
            {
                if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_SVON) > 0)//SVON
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public override AxisState IsAxisNormalStop(int nAxisNo)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return 0;
            ushort state = 0;
            if ((uint)ErrorCode.SUCCESS == Motion.mAcm_AxGetState(m_Axishand[nAxisNo], ref state))
            {
                // logger.Info(string.Format("{0}卡{1}轴当前状态为{2}", m_nCardIndex, nAxisNo, (Advantech.Motion.AxisState)Enum.Parse(typeof(Advantech.Motion.AxisState), state.ToString())));
                if ((ushort)Advantech.Motion.AxisState.STA_AX_DISABLE == state)
                {
                    return AxisState.DriveAlarm;   //驱动器异常报警
                }
                else if ((ushort)Advantech.Motion.AxisState.STA_AX_ERROR_STOP == state)
                {
                    return AxisState.ErrAlarm;   //轴错误停止
                }
                if ((ushort)Advantech.Motion.AxisState.STA_AX_READY == state)
                {
                    return AxisState.NormalStop;   //正常停止
                }
                if ((ushort)Advantech.Motion.AxisState.STA_AX_HOMING == state)
                {

                    return AxisState.Homeing;  //正在运动中
                }

                return AxisState.Moving;  //正在运动中
            }
            else
                return AxisState.ErrAlarm;
        }

        public override bool Home(int nAxisNo, int nParam)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            logger.Info(string.Format("{0}卡{1}轴回原点", m_nCardIndex, nAxisNo));
            if (m_MotorType[nAxisNo] == MotorType.SEVER)
                Motion.mAcm_AxSetSvOn(m_Axishand[nAxisNo], 1);
            else
                Motion.mAcm_AxSetSvOn(m_Axishand[nAxisNo], 0);
            uint Result = 0;
            Result |= Motion.mAcm_AxResetError(m_Axishand[nAxisNo]);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxVelLow, m_HomePrm[nAxisNo].VelL);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxVelHigh, m_HomePrm[nAxisNo].VelH);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxAcc, m_HomePrm[nAxisNo].AccH);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;

            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxDec, m_HomePrm[nAxisNo].DccH);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            double AxJerk = 0;
            Result |= Motion.mAcm_SetF64Property(m_Axishand[nAxisNo], (uint)PropertyID.PAR_AxJerk, AxJerk);
            if (Result != (uint)ErrorCode.SUCCESS)
                return false;
            uint nDir = 0;
            if (m_HomePrm[nAxisNo]._bHomeDir)
                nDir = 0;
            else
                nDir = 1;
            Result |= Motion.mAcm_AxHome(m_Axishand[nAxisNo], (uint)m_HomePrm[nAxisNo]._nHomeMode, nDir);

            return Result == (uint)ErrorCode.SUCCESS;
        }

        public override bool SetActutalPos(int nAxisNo, double pos)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            logger.Info(string.Format("{0}卡{1}轴设置实际位置", m_nCardIndex, nAxisNo));
            uint Result = 0;
            Result |= Motion.mAcm_AxSetActualPosition(m_Axishand[nAxisNo], pos);
            return Result == (uint)ErrorCode.SUCCESS;

        }

        public override bool SetCmdPos(int nAxisNo, double pos)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return false;
            logger.Info(string.Format("{0}卡{1}轴设置命令位置", m_nCardIndex, nAxisNo));
            uint Result = 0;
            Result |= Motion.mAcm_AxSetCmdPosition(m_Axishand[nAxisNo], pos);
            return Result == (uint)ErrorCode.SUCCESS;
        }
        public override AxisState IsHomeNormalStop(int nAxisNo)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return AxisState.ErrAlarm;
            ushort state = 0;
            if ((uint)ErrorCode.SUCCESS == Motion.mAcm_AxGetState(m_Axishand[nAxisNo], ref state))
            {
                if ((ushort)Advantech.Motion.AxisState.STA_AX_DISABLE == state)
                {
                    return AxisState.DriveAlarm;   //驱动器异常报警
                }
                else if ((ushort)Advantech.Motion.AxisState.STA_AX_ERROR_STOP == state)
                {
                    return AxisState.ErrAlarm;   //轴错误停止
                }
                else if ((ushort)Advantech.Motion.AxisState.STA_AX_READY == state)
                {
                    return AxisState.NormalStop;   //正常停止
                }
                else if ((ushort)Advantech.Motion.AxisState.STA_AX_HOMING == state)
                    return AxisState.Homeing;  //正在运动中
                else
                    return AxisState.ErrAlarm;   //轴错误停止
            }
            else
                return AxisState.ErrAlarm;   //轴错误停止
        }
        public override int GetAxisActPos(int nAxisNo)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return 0;
            double pos = 0;
            uint sRtn = 0;
            sRtn |= Motion.mAcm_AxGetActualPosition(m_Axishand[nAxisNo], ref pos);
            return sRtn == 0 ? (int)pos : int.MaxValue;
        }
        public override int GetAxisCmdPos(int nAxisNo)
        {
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return 0;
            double pos = 0;
            uint sRtn = 0;
            sRtn |= Motion.mAcm_AxGetCmdPosition(m_Axishand[nAxisNo], ref pos);
            return sRtn == 0 ? (int)pos : int.MaxValue;
        }
        public override int GetAxisPos(int nAxisNo)
        {
            int pos = 0;
            if (m_Axishand == null || m_Axishand[nAxisNo] == null)
                return 0;
            if (m_MotorType[nAxisNo] == MotorType.SEVER)
                pos = GetAxisActPos(nAxisNo);
            else
                pos = GetAxisCmdPos(nAxisNo);
            return pos;
        }


        public override bool isOrgTrig(int nAxisNo)
        {
            //  throw new NotImplementedException();
            return true;
        }
        public override bool AddAxisToGroup(int[] nAxisArr, ref dynamic group)
        {
            if (nAxisArr == null || group == null || nAxisArr.Length <= 0)
                return false;
            uint reslut = 0;
            IntPtr gouppId =  (IntPtr)group;
            for (int i = 0; i < nAxisArr.Length; i++)
                reslut |= Motion.mAcm_GpAddAxis(ref gouppId, m_Axishand[nAxisArr[i]]);
            group = gouppId;
            return reslut == (uint)ErrorCode.SUCCESS;
        }
        public override bool CloseAxisGroup(ref IntPtr group)
        {
            uint reslut = 0;
            reslut |= Motion.mAcm_GpClose(ref group);
            return reslut == (uint)ErrorCode.SUCCESS;

        }
        public override GpState GetGpState(IntPtr group)
        {
            uint reslut = 0;

            ushort groupstate = 0;
            try
            {

                reslut |= Motion.mAcm_GpGetState(group, ref groupstate);
                if (reslut != (uint)ErrorCode.SUCCESS)
                {
                    logger.Info("获取群组状态异常（GetGpState）");
                    return GpState.GpErrStop;
                }
                switch (groupstate)
                {
                    case (ushort)GroupState.STA_Gp_Ready:
                        return GpState.GpReady;
                    case (ushort)GroupState.STA_Gp_Motion:
                        return GpState.GpMotion;
                    case (ushort)GroupState.STA_Gp_Stopping:
                        return GpState.GpStoping;
                    case (ushort)GroupState.STA_Gp_Disable:
                        return GpState.GpDisable;
                    case (ushort)GroupState.STA_Gp_ErrorStop:
                        return GpState.GpErrStop;
                    case (ushort)GroupState.STA_GP_PAUSE:
                        return GpState.GpPause;
                    case (ushort)GroupState.STA_GP_AX_MOTION:
                        return GpState.Gp_AX_Motion;
                }
                return GpState.GpReady;
            }
            catch (Exception e)
            {
                logger.Info("获取群组状态异常（GetGpState）");
                return GpState.GpErrStop;
            }
        }
        public override bool StopGp(IntPtr group)
        {
            uint reslut = 0;
            reslut |= Motion.mAcm_GpStopDec(group);
            return reslut == (uint)ErrorCode.SUCCESS;
        }
        public override bool ResetGpErr(IntPtr group)
        {
            uint reslut = 0;
            reslut |= Motion.mAcm_GpResetError(group);
            return reslut == (uint)ErrorCode.SUCCESS;
        }
        public override bool Line2Axisabs(IntPtr group, int xAxis, int yAxis, double xpos, double ypos, double acc, double dec, double velrun, double velori = 0)
        {

            try
            {
                uint reslut = 0;
                reslut |= Motion.mAcm_SetF64Property(group, (uint)PropertyID.PAR_GpVelLow, velori);
                reslut |= Motion.mAcm_SetF64Property(group, (uint)PropertyID.PAR_GpVelHigh, velrun);
                reslut |= Motion.mAcm_SetF64Property(group, (uint)PropertyID.PAR_GpAcc, acc);
                reslut |= Motion.mAcm_SetF64Property(group, (uint)PropertyID.PAR_GpDec, dec);
                double AxJerk = 0;
                reslut |= Motion.mAcm_SetF64Property(group, (uint)PropertyID.PAR_GpJerk, AxJerk);
                uint ememtcount = 2;
                Motion.mAcm_GpResetError(group);
                reslut |= Motion.mAcm_GpMoveLinearAbs(group, new double[] { xpos, ypos }, ref ememtcount);
                if (reslut != (uint)ErrorCode.SUCCESS)
                {
                    return false;
                }

                return reslut == (uint)ErrorCode.SUCCESS;
            }
            catch (Exception e)
            {
                logger.Info("Line2Axisabs运动异常：" + e.Message);

                return false;
            }
            return false;

        }
        public override bool AddBufMove(dynamic objGroup, BufMotionType type, int mode, int nAxisNum, double velHigh, double velLow, double[] Point1, double[] Point2)
        {

            ushort Cmd = 0;
            switch (type)
            {
                case BufMotionType.buf_Line3dAbs:
                    Cmd = (ushort)PathCmd.Abs3DLine;
                    break;

                    break;
                case BufMotionType.buf_Line2dAbs:
                    Cmd = (ushort)PathCmd.Abs2DLine;
                    break;
                case BufMotionType.buf_Arc2dAbsCCW:
                    Cmd = (ushort)PathCmd.Abs2DArcCCW;
                    break;
                case BufMotionType.buf_Arc2dAbsCW:
                    Cmd = (ushort)PathCmd.Abs2DArcCW;
                    break;
                case BufMotionType.buf_end:
                    Cmd = (ushort)PathCmd.EndPath;
                    break;
                default:
                    MessageBox.Show("path运动类型不对", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
            }
            uint nAxisCount = (uint)nAxisNum;

            IntPtr GpHand = (IntPtr)objGroup;
            uint reslut = 0;
            reslut = Motion.mAcm_GpAddPath(GpHand, Cmd, 0,
              velHigh, velLow,
              Point1,
              Point2 //  new double[] { 0,0}
            , ref nAxisCount);

            return reslut == (uint)ErrorCode.SUCCESS;
        }
        public override bool AddBufIo(dynamic objGroup, string strIoName, bool bVal, int nAxisIndexInGroup)
        {
            bool bFindIo = false;
            int nAxisIndex = 0, nIoIndex = 0;
            if (IOMgr.GetInstace().GetOutputDic() != null)
            {
                if (IOMgr.GetInstace().GetOutputDic().ContainsKey(strIoName))
                {
                    bFindIo = true;
                    nAxisIndex = IOMgr.GetInstace().GetOutputDic()[strIoName]._AxisIndex;
                    nIoIndex = IOMgr.GetInstace().GetOutputDic()[strIoName]._IoIndex;
                }
            }
            if (!bFindIo)
            {
                MessageBox.Show("群组中添加Io,Io没有在配置文件中找到", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (nAxisIndexInGroup >= 3 || nAxisIndexInGroup < 0)
            {
                MessageBox.Show("轴号在群组的索引超出", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            double[] Point1 = new double[3] { 0, 0, 0 };
            if (bVal)
                Point1[nAxisIndexInGroup] = ((0x1 << nIoIndex)) << 16 | (0x1 << nIoIndex);
            else
                Point1[nAxisIndexInGroup] = ((0x1 << nIoIndex)) << 16 | (0x0 << nIoIndex);

            uint ArraryElements = 3;
            IntPtr GpHand = (IntPtr)objGroup;
            ushort Cmd = (ushort)PathCmd.DOControl;
            uint reslut = 0;
            reslut = Motion.mAcm_GpAddPath(GpHand, Cmd, 0,
              0, 0,
              Point1,
              null //  new double[] { 0,0}
            , ref ArraryElements);
            return true;
        }

        public override bool AddBufDelay(dynamic objGroup, int nTime)
        {
            IntPtr GpHand = (IntPtr)objGroup;
            uint ArraryElements = 3;
            ushort Cmd = (ushort)PathCmd.GPDELAY;
            uint reslut = 0;
            reslut = Motion.mAcm_GpAddPath(GpHand, Cmd, 0, nTime, 2000, null, null, ref ArraryElements);

            return reslut == (uint)ErrorCode.SUCCESS;
        }
        public override bool ClearBufMove(dynamic objGroup)
        {
            IntPtr GpHand = (IntPtr)objGroup;
            // Motion.mAcm_GpResetPath(ref GpHand);
            Motion.mAcm_GpResetError(GpHand);
            uint result = Motion.mAcm_GpResetPath(ref GpHand);
            return result == (uint)ErrorCode.SUCCESS;
        }

        public override bool StartBufMove(dynamic objGroup)
        {         
            IntPtr GpHand = (IntPtr)objGroup;
            uint reslut = 0;
            reslut = Motion.mAcm_GpMovePath(GpHand, IntPtr.Zero);
            return reslut == (uint)ErrorCode.SUCCESS;


        }
        public override bool SetBufMoveParam(dynamic objGroup, double velhigh, double vellow, double acc, double dec)
        {
            IntPtr GpHand = (IntPtr)objGroup;
            uint reslut = 0;

            reslut |= Motion.mAcm_SetF64Property(GpHand, (uint)PropertyID.PAR_GpVelHigh, velhigh);
            reslut |= Motion.mAcm_SetF64Property(GpHand, (uint)PropertyID.PAR_GpVelLow, vellow);
            reslut |= Motion.mAcm_SetF64Property(GpHand, (uint)PropertyID.PAR_GpAcc, acc);
            reslut |= Motion.mAcm_SetF64Property(GpHand, (uint)PropertyID.PAR_GpDec, dec);
            reslut |= Motion.mAcm_SetI32Property(GpHand, (uint)PropertyID.CFG_GpSFEnable, (int)SFEnable.SF_DIS);
            reslut |= Motion.mAcm_SetI32Property(GpHand, (uint)PropertyID.CFG_GpBldTime, 0);
            //objGroup.SFEnable = SFEnable.SF_DIS;
            //objGroup._group.BldTime = 0;
            return reslut == 0;
        }

        public override bool IsInpos(int nAxisNo)
        {
            return IsAxisNormalStop(nAxisNo) == AxisState.NormalStop;

        }
    }
}