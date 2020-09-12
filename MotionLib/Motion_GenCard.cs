using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTN;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using static GTN.mc;
using log4net;
/*
* 固高卡号从0 开始
* 轴号从1开始
* Home 原点规则 ，不管信号是常开常闭，一律将其配置成
* 挡片挡住原点时候，固高软件的原点指示灯亮
* 挡片离开原点， 固高软件的原点指示灯灭
* 配置完成后 配置文件di原点 
* 信号是正常 则 m_ProPrietarySignal[轴号].oriSignal = 0;
* 信号是取反 则 m_ProPrietarySignal[轴号].oriSignal = 1;
*/

namespace MotionIoLib
{
    public class InfoOnly
    {
        private object lockobj = new object();
        protected ILog _logger = null;
        string strOldMsg = "";
        public InfoOnly(ILog logger)
        {
            _logger = logger;
        }
        public void Info(string strMsg)
        {
            if (strOldMsg != strMsg)
            {
                _logger.Info(strMsg);

            }
            strOldMsg = strMsg;
        }
    }
    /// <summary>
    /// Z 向信号对应探针1   原点信号对应探针2
    /// </summary>
    public enum CaptureBit
    {
        CaptureNone = 0,
        RiseCapture1 = 1,
        FallCapture1 = 2,
        RiseCapture2 = 9,
        FallCapture2 = 10,
    }

    public enum HomeStep
    {
        JudePos,//判断位置
        JudeSignl,// 判断信号
        JudeInPos,//判断规划停止    
        JogVH,
        JogVL,
        CheckHome,
        JogVLToHome,
        JogVLLeaveHome,
        JogPVHFindHomeRise,// 
        JogNVHFindHomeRise,// 
        JogPVHFindHomeFall,// 
        JogNVHFindHomeFall,// 
        JogPVLFindHomeRise,// 
        JogNVLFindHomeRise,// 
        JogPVLFindHomeFall,// 
        JogNVLFindHomeFall,// 
        SetProbeHomeRise,//设置探针上升沿
        SetProbeHomeFall,//设置探针上升沿
        ClrProbe,//清除探针
        GoCatputeHomeRisePos, //去捕获位置
        GoCatputeHomeFallPos,
        EndHome,
    }
    public interface EtherCardFun
    {
        void SetEcatAxisMode(int nAxisZ, MotorMode eMode);
        double GetEcatAxisAtlTorque(int nAxis);
        void SetEcateAxisTorque(int nAxis, double Torque);
        MotorMode GetEcatAxisMode(int nAxisNo);
        void SynchAxisPos(int nAxisNo);
    }

    public class Motion_Gen : MotionCardBase, EtherCardFun
    {

        // HoingSignalState[][] FindHomeSignalStates = null;
        Dictionary<int, List<HoingSignalState>> FindHomeSignalStates = new Dictionary<int, List<HoingSignalState>>();
        Queue<HomeStep>[] homeSteps = null;
        public Motion_Gen(ulong indexCard, string strName, int nMinAxisNo, int nMaxAxisNo)
            : base(indexCard, strName, nMinAxisNo, nMaxAxisNo)
        {
            MotionMgr.GetInstace().m_eventChangeMotionIoOrPos += (nAxisNo, bChangeBitArr, state, axisPos) =>
            {
                if (nAxisNo > AxisNumOnCard)
                    return;
                long MotionIo = GetMotionIoState(nAxisNo);
                if (bChangeBitArr[(int)AxisSignlNo.正极限])
                {
                    if (state._bLimtP)
                        return;
                    else
                        ClearAlarm((short)nAxisNo);
                }
                if (bChangeBitArr[(int)AxisSignlNo.负极限])
                {
                    if (state._bLimtN)
                        return;
                    else
                        ClearAlarm((short)nAxisNo);
                }
            };

            bJogHomeDir = new bool[m_nMaxAxisNo - m_nMinAxisNo + 1];
            for (int i = 0; i < m_nMaxAxisNo - m_nMinAxisNo + 1; i++)
                FindHomeSignalStates.Add(i, new List<HoingSignalState>());
            //FindHomeSignalStates = new HoingSignalState[m_nMinAxisNo - m_nMinAxisNo + 1][];
            //for (int i = 0; i < m_nMinAxisNo - m_nMinAxisNo + 1; i++)
            //    FindHomeSignalStates[i] = new HoingSignalState[2] {
            //        new HoingSignalState (){
            //        findHomeSignalState =FindHomeSignalState.None,
            //        bCurrentMoveIsPositive=false,},

            //        new HoingSignalState (){
            //        findHomeSignalState =FindHomeSignalState.None,
            //        bCurrentMoveIsPositive=false,
            //    }};
            homeSteps = new Queue<HomeStep>[m_nMaxAxisNo - m_nMinAxisNo + 1];
            for (int i = 0; i < m_nMaxAxisNo - m_nMinAxisNo + 1; i++)
                homeSteps[i] = new Queue<HomeStep>();
        }
        int AxisNumOnCard = 0;


        public override bool Open()
        {
            short nCore = 1;
            short rtn = 0;
            short sEcatSts;
            rtn |= mc.GTN_Reset(1);
            rtn |= mc.GTN_Reset(2);
            rtn |= mc.GTN_Open(5, 1);
            if (rtn != 0)
            {
                MessageBox.Show("EtherCat 打开卡失败（GTN_Open） 初始化失败", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("EtherCAT GTN_Open error!\n");
                return false;
            }
            rtn |= mc.GTN_InitEcatComm(nCore);
            if (rtn != 0)
            {
                string strErrMsg = rtn == -8 ? "配置错误，请检查 eni 配置文件和实际连接从站是否匹配" :
                    (rtn == -9 ? "未找到 eni 配置文件，请将 eni 文件放到可执行程序的相同目录" :
                    (rtn == -10 ? "未找到从站，请检查从站接线是否稳固" : $"其他错误{rtn.ToString()}"));

                MessageBox.Show($"EtherCat 初始化通讯失败（GTN_InitEcatComm）:{strErrMsg}", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("EtherCAT GTN_Open error!\n");
                logger.Warn("EtherCAT GTN_Open error!\n");
                return false;
            }
            // 等待EtherCAT总线初始化成功
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            do
            {
                // 读取EtherCAT总线状态
                rtn |= mc.GTN_IsEcatReady(nCore, out sEcatSts);
                Thread.Sleep(200);
                Console.WriteLine("EtherCAT communication is not ready!\n");

                if (stopwatch.ElapsedMilliseconds > 20000)
                {
                    MessageBox.Show("GTN_IsEcatReady  等待超时", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return false;
                }
            } while (sEcatSts != 1);

            // 启动EtherCAT通讯
            rtn |= mc.GTN_StartEcatComm(nCore);
            if (rtn != 0)
            {
                Console.WriteLine("EtherCAT communication error!\n");
                return false;
            }
            //rtn |= mc.GTN_Reset(1);
            //rtn |= mc.GTN_Reset(2);
            stopwatch.Restart();
            short nSlaveMotionNum = 0;
            short nSlaveIONum = 0;
            int pAxisNum = 0;

            rtn |= mc.GTN_GetEcatSlaves(1, out nSlaveMotionNum, out nSlaveIONum);
            for (short i = 0; i < nSlaveMotionNum + nSlaveIONum; i++)
            {
                mc.TSlaveInfo slaveInfo = new mc.TSlaveInfo();
                rtn |= mc.GTN_GetEcatSlaveInfo(1, i, out slaveInfo);
                if (slaveInfo.slave_type == (int)mc.SlaveType.Motion)
                    pAxisNum += slaveInfo.motion_cnt;

            }

            int[] axisSts = new int[6];
            uint pClock;
            m_bOpen = (rtn == 0);
            AxisNumOnCard = pAxisNum;
            if (pAxisNum > 0)
            {
                rtn |= mc.GTN_LoadConfig((short)1, AppDomain.CurrentDomain.BaseDirectory + "gtn_core" + (1).ToString() + ".cfg");
                //for (short i=1;i<=6;i++)
                //{
                //    rtn = mc.GTN_SetSense(1, mc.MC_LIMIT_POSITIVE, i, 1);
                //    rtn = mc.GTN_SetSense(1, mc.MC_LIMIT_NEGATIVE, i, 1);
                //}
                rtn |= mc.GTN_ClrSts(1, 1, 32);
                rtn = mc.GTN_GetSts(1, 1, out axisSts[0], 6, out pClock);
            }
            if (pAxisNum > 32)
            {
                rtn |= mc.GTN_LoadConfig((short)2, AppDomain.CurrentDomain.BaseDirectory + "gtn_core" + (2).ToString() + ".cfg");
                rtn |= mc.GTN_ClrSts(2, 1, 32);
            }
            //  for (short i = 1; i <= 8; i++)
            short ii = 1;
            {
                rtn |= mc.GTN_RelateEcatSlaveToMcGpoBit(1, ii, 0, 1, (short)(ii - 1), 0);
            }

            return rtn == 0;
        }
        public override bool IsOpen()
        {
            return m_bOpen;
        }
        public override bool Close()
        {
            short rtn = 0;
            rtn |= mc.GTN_TerminateEcatComm(1);
            rtn |= mc.GTN_Close();
            return 0 == rtn;
        }
        public override bool ServoOn(short nAxisNo)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            short rtn = 0;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            rtn |= mc.GTN_AxisOn((short)nCore, (short)nAxisNo);
            return 0 == rtn;
        }
        public override bool ServoOff(short nAxisNo)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            short rtn = 0;
            rtn |= mc.GTN_AxisOff((short)nCore, (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo));
            return 0 == rtn;
        }
        public bool ClearAlarm(short nAxisNo)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            short rtn = 0;
            rtn |= mc.GTN_ClrSts((short)nCore, (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo), 1);
            return 0 == rtn;

        }
        public override bool AbsMove(int nAxisNo, double nPos, double nSpeed)
        {
            short rtn = 0;
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            uint axisMark = (uint)1 << (nAxisNo - 1);

            mc.TTrapPrm trap = new mc.TTrapPrm();
            double speed = nSpeed;
            double Acc = m_MovePrm[nAxisNo - 1].AccH;
            double Dcc = m_MovePrm[nAxisNo - 1].AccL;
            switch (nSpeed)
            {
                case 0:
                    Acc = m_MovePrm[nAxisNo - 1].AccH;
                    Dcc = m_MovePrm[nAxisNo - 1].DccH;
                    speed = m_MovePrm[nAxisNo - 1].VelH;
                    break;
                case 1:
                    Acc = m_MovePrm[nAxisNo - 1].AccM;
                    Dcc = m_MovePrm[nAxisNo - 1].DccM;
                    speed = m_MovePrm[nAxisNo - 1].VelM;
                    break;
                case 2:
                    Acc = m_MovePrm[nAxisNo - 1].AccL;
                    Dcc = m_MovePrm[nAxisNo - 1].DccL;
                    speed = m_MovePrm[nAxisNo - 1].VelL;
                    break;

            }
            trap.acc = Acc;//3;
            trap.dec = Dcc;//3;
            trap.velStart = 0;
            trap.smoothTime = 20 /*(short)m_nSmoothTimes[nAxisNo - 1]*/;//20;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            rtn |= mc.GTN_SetAxisBand((short)nCore, (short)nAxisNo, 20, 10);// 5);
            rtn |= mc.GTN_PrfTrap((short)nCore, (short)nAxisNo);
            rtn |= mc.GTN_SetTrapPrm((short)nCore, (short)nAxisNo, ref trap);
            rtn |= mc.GTN_SetVel((short)nCore, (short)nAxisNo, (double)speed);
            rtn |= mc.GTN_SetPos((short)nCore, (short)nAxisNo, (int)nPos);
            rtn |= mc.GTN_Update((short)nCore, 1 << (nAxisNo - 1));
            if (rtn == 0)
                return true;
            else
                return false;

        }
        public override bool RelativeMove(int nAxisNo, double nPos, double nSpeed)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            uint axisMark = (uint)1 << (nAxisNo - 1);
            short nRtn = 0;
            uint clk = 0;
            double dAxisCurPos = 0;


            double speed = nSpeed;
            double Acc = m_MovePrm[nAxisNo - 1].AccH;
            double Dcc = m_MovePrm[nAxisNo - 1].AccL;
            switch (nSpeed)
            {
                case 0:
                    Acc = m_MovePrm[nAxisNo - 1].AccH;
                    Dcc = m_MovePrm[nAxisNo - 1].DccH;
                    speed = m_MovePrm[nAxisNo - 1].VelH;
                    break;
                case 1:
                    Acc = m_MovePrm[nAxisNo - 1].AccM;
                    Dcc = m_MovePrm[nAxisNo - 1].DccM;
                    speed = m_MovePrm[nAxisNo - 1].VelM;
                    break;
                case 2:
                    Acc = m_MovePrm[nAxisNo - 1].AccL;
                    Dcc = m_MovePrm[nAxisNo - 1].DccL;
                    speed = m_MovePrm[nAxisNo - 1].VelL;
                    break;

            }
            mc.TTrapPrm trap = new mc.TTrapPrm();
            trap.acc = Acc;//3;
            trap.dec = Dcc;//3;
            trap.velStart = 0;
            trap.smoothTime = 20;//(short)m_nSmoothTimes[nAxisNo - 1];//20;
            int extPos = 0;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            mc.GTN_SetAxisBand((short)nCore, (short)nAxisNo, 20, 10);// 5);
            if (0 == mc.GTN_ClrSts((short)nCore, (short)nAxisNo, 8)
                && 0 == mc.GTN_PrfTrap((short)nCore, (short)nAxisNo)
                && 0 == mc.GTN_SetTrapPrm((short)nCore, (short)nAxisNo, ref trap)
                && 0 == mc.GTN_SetVel((short)nCore, (short)nAxisNo, (double)speed)
                && 0 == mc.GTN_GetPos((short)nCore, (short)nAxisNo, out extPos)
                && 0 == mc.GTN_SetPos((short)nCore, (short)nAxisNo, extPos + (int)nPos)
                && 0 == mc.GTN_Update((short)nCore, 1 << (nAxisNo - 1))
               )
            {

                return true;
            }

            return false;
        }
        public override bool JogMove(int nAxisNo, bool bPositive, int bStart, double nSpeed)
        {
            nAxisNo += 1;

            int nCore = nAxisNo <= 32 ? 1 : 2;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            double speed = nSpeed;
            double Acc = m_MovePrm[nAxisNo - 1].AccH;
            double Dcc = m_MovePrm[nAxisNo - 1].AccL;
            switch (nSpeed)
            {
                case 0:
                    Acc = m_MovePrm[nAxisNo - 1].AccH;
                    Dcc = m_MovePrm[nAxisNo - 1].DccH;
                    speed = m_MovePrm[nAxisNo - 1].VelH;
                    break;
                case 1:
                    Acc = m_MovePrm[nAxisNo - 1].AccM;
                    Dcc = m_MovePrm[nAxisNo - 1].DccM;
                    speed = m_MovePrm[nAxisNo - 1].VelM;
                    break;
                case 2:
                    Acc = m_MovePrm[nAxisNo - 1].AccL;
                    Dcc = m_MovePrm[nAxisNo - 1].DccL;
                    speed = m_MovePrm[nAxisNo - 1].VelL;
                    break;

            }
            short rtn = 0;
            mc.TJogPrm trap = new mc.TJogPrm();
            rtn |= mc.GTN_ClrSts((short)nCore, (short)nAxisNo, 8);
            rtn |= mc.GTN_PrfJog((short)nCore, (short)nAxisNo);
            rtn |= mc.GTN_GetJogPrm((short)nCore, (short)nAxisNo, out trap);
            if (rtn == 0)
            {
                trap.acc = Acc;
                trap.dec = Dcc;
                trap.smooth = 0.2;
                rtn |= mc.GTN_SetJogPrm((short)nCore, (short)nAxisNo, ref trap);
                rtn |= mc.GTN_SetVel((short)nCore, (short)nAxisNo, bPositive ? (double)speed : (double)-speed);
                rtn |= mc.GTN_Update((short)nCore, 1 << (nAxisNo - 1));
                if (rtn == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public override bool StopAxis(int nAxisNo)
        {
            int nSystemAxisno = GetSystemAxisNo(nAxisNo);
            nAxisNo += 1;

            int nCore = nAxisNo <= 32 ? 1 : 2;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            if (MotorType.EcatAxis == MotionMgr.GetInstace().GetMotorType(nSystemAxisno))
            {

                mc.GTN_GetEcatAxisMode((short)nCore, (short)nAxisNo, out ushort drvMode);
                if ((ushort)MotorMode.HomeIng == drvMode)
                {
                    mc.GTN_StopEcatHoming((short)nCore, (short)nAxisNo);
                }
            }
            if (m_AxisStates[nAxisNo - 1] == AxisState.Homeing)
            {
                m_AxisStates[nAxisNo - 1] = AxisState.NormalStop;
                m_ManualEventHomeingStop[nAxisNo - 1].Set();
            }
            short rtn = mc.GTN_SetStopDec((short)nCore, (short)nAxisNo, 100, 1000);
            rtn |= mc.GTN_Stop((short)nCore, 1 << (nAxisNo - 1), 0);
            return 0 == rtn;
        }
        public bool StopAxisOnly(int nAxisNo)
        {
            int nSystemAxisno = GetSystemAxisNo(nAxisNo);
            nAxisNo += 1;

            int nCore = nAxisNo <= 32 ? 1 : 2;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);

            short rtn = mc.GTN_SetStopDec((short)nCore, (short)nAxisNo, 100, 1000);
            rtn |= mc.GTN_Stop((short)nCore, 1 << (nAxisNo - 1), 0);
            return 0 == rtn;
        }
        public override bool StopEmg(int nAxisNo)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            return 0 == mc.GTN_Stop((short)nCore, 1 << (nAxisNo - 1), 1 << (nAxisNo - 1));

        }
        public override bool ReasetAxis(int nAxisNo)
        {
            int nSystemAxisNo = GetSystemAxisNo(nAxisNo);
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            m_AxisStates[nAxisNo - 1] = AxisState.NormalStop;
            mc.GTN_ClrSts((short)nCore, (short)nAxisNo, 8);
            IsAxisNormalStop(nSystemAxisNo);
            return 0 == mc.GTN_ClrSts((short)nCore, (short)nAxisNo, 8);
        }
        public override bool ResetAllAxis()
        {
            if (AxisNumOnCard > 0)
                mc.GTN_ClrSts((short)1, (short)1, 32);
            if (AxisNumOnCard > 32)
                mc.GTN_ClrSts((short)2, (short)1, 32);
            return true;
        }
        /// <summary>
        ///  轴卡的规划器运动完成
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        private bool GetPrfStop(int nAxisNo)
        {
            int nSystemAxisno = GetSystemAxisNo(nAxisNo);
            nAxisNo += 1;

            uint clk = 0;
            int sts;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            short rtn = 0;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            if (0 == mc.GTN_GetSts((short)nCore, (short)nAxisNo, out sts, 1, out clk))
            {
                //if (0 != (sts & (1 << 1)))
                //{
                //    mc.GTN_ClrSts((short)nCore, (short)nAxisNo, 1);
                //    return true;   //驱动器异常报警
                //}

                //if (m_AxisStates[nAxisNo - 1] == AxisState.ErrAlarm)
                //{
                //    mc.GTN_ClrSts((short)nCore, (short)nAxisNo, 1);
                //    return true;
                //}
                //if (0 != (sts & (1 << 5)))
                //{
                //    mc.GTN_ClrSts((short)nCore, (short)nAxisNo, 1);
                //    return true;
                //}
                //if (0 != (sts & (1 << 6)))
                //{
                //    mc.GTN_ClrSts((short)nCore, (short)nAxisNo, 1);
                //    return true;
                //}
                if ((sts & (0x01 << 10)) == 0)
                    return true;
            }
            return false;
        }
        public override long GetMotionIoState(int nAxisNo)
        {
            int nSystemAxisno = GetSystemAxisNo(nAxisNo);
            nAxisNo += 1;
            //0:保留
            //1:alarm
            //2:保留
            //3:保留
            //4:跟随误差
            //5：正限位触发
            //6：负限位触发
            //7：IO平滑停止触发
            //8：IO急停触发
            //9：电机使能标志
            //10：规划运动标志
            //11：电机到位标志
            uint clk = 0;
            int sts;
            long lStandardIo = 0;
            int nCore = nAxisNo <= 32 ? 1 : 2;

            short rtn = 0;

            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            uint pDI = 0;
            MotorType motorType = MotionMgr.GetInstace().GetMotorType(nSystemAxisno);
            if (MotorType.EcatAxis == MotionMgr.GetInstace().GetMotorType(nSystemAxisno)
                 || MotorType.EcatAxisModleSevro == MotionMgr.GetInstace().GetMotorType(nSystemAxisno)
                 || MotorType.EcatAxisModleStep == MotionMgr.GetInstace().GetMotorType(nSystemAxisno)
                )
            {
                if (MotorType.EcatAxis == MotionMgr.GetInstace().GetMotorType(nSystemAxisno))
                {
                    mc.GTN_GetEcatAxisDI((short)nCore, (short)nAxisNo, out pDI);
                    if ((pDI & (0x01 << 2)) != 0) //原点到位存在第4位
                        lStandardIo |= (0x01 << 3);
                }


                if (MotorType.EcatAxisModleSevro == MotionMgr.GetInstace().GetMotorType(nSystemAxisno)
                || MotorType.EcatAxisModleStep == MotionMgr.GetInstace().GetMotorType(nSystemAxisno)
               )
                {
                    if (0 == mc.GTN_GetDi((short)nCore, mc.MC_HOME, out sts))
                    {
                        if ((sts & (0x01 << (nAxisNo - 1))) == 0)
                        {
                            lStandardIo |= (0x01 << 3); //原点到位存在第4位  
                        }
                    }
                }

            }
            //if (0 == mc.GTN_GetDi((short)nCore, mc.MC_HOME, out sts))
            //{
            //    if ((sts & (0x01 << (nAxisNo - 1))) == 0)
            //    {
            //        lStandardIo |= (0x01 << 3); //原点到位存在第4位
            //        lStandardIo |= (0x01 << 5); //零位到位存在第6位
            //    }
            //}
            if (0 == mc.GTN_GetSts((short)nCore, (short)nAxisNo, out sts, 1, out clk))
            {
                if ((sts & (0x01 << 1)) != 0)
                {
                    lStandardIo |= (0x01 << 0); //驱动器报警存在第1位
                    m_AxisStates[nAxisNo - 1] = AxisState.DriveAlarm;
                }

                //if ((sts & (0x01 << 5)) != 0)
                //    lStandardIo |= (0x01 << 1); //正限位报警存在第2位
                //if ((sts & (0x01 << 6)) != 0)
                //    lStandardIo |= (0x01 << 2); //负限位报警存在第3位
                if ((sts & (0x01 << 8)) != 0)
                    lStandardIo |= (0x01 << 4); //急停触发存在第5位
                if ((sts & (0x01 << 11)) != 0)
                    lStandardIo |= (0x01 << 6); //电机到位标志存在第7位
                if ((sts & (0x01 << 9)) != 0)
                    lStandardIo |= (0x01 << 7); //电机使能标志存在第8位
                if (MotorType.EcatAxis == MotionMgr.GetInstace().GetMotorType(nSystemAxisno)
                     || MotorType.EcatAxisModleSevro == MotionMgr.GetInstace().GetMotorType(nSystemAxisno)
                     || MotorType.EcatAxisModleStep == MotionMgr.GetInstace().GetMotorType(nSystemAxisno)
                    )
                {
                    mc.GTN_GetEcatAxisDI((short)nCore, (short)nAxisNo, out pDI);
                    if ((pDI & (0x01 << 0)) != 0)
                    {
                        lStandardIo |= (0x01 << 2); //负限位报警存在第3位
                        if (m_AxisStates[nAxisNo - 1] != AxisState.Homeing)
                            m_AxisStates[nAxisNo - 1] = AxisState.LimtNStop;
                    }

                    if ((pDI & (0x01 << 1)) != 0)
                    {
                        lStandardIo |= (0x01 << 1); //正限位报警存在第2位
                        if (m_AxisStates[nAxisNo - 1] != AxisState.Homeing)
                            m_AxisStates[nAxisNo - 1] = AxisState.LimtPStop;
                    }


                }
                return lStandardIo;
            }
            else
            {
                return -1;
            }

        }
        public override bool GetServoState(int nAxisNo)
        {
            nAxisNo += 1;
            int sts = 0;
            uint clk = 0;

            int nCore = nAxisNo <= 32 ? 1 : 2;

            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            if (0 == mc.GTN_GetSts((short)nCore, (short)nAxisNo, out sts, 1, out clk))
            {
                return 0 != (sts & (1 << 9));
            }
            return true;
        }
        public override AxisState IsAxisNormalStop(int nAxisNo)
        {
            int nSystemAxis = GetSystemAxisNo(nAxisNo);

            nAxisNo += 1;
            int sts = 0;
            uint clk = 0;

            int nCore = nAxisNo <= 32 ? 1 : 2;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            if (0 == mc.GTN_GetSts((short)nCore, (short)nAxisNo, out sts, 1, out clk))
            {
                if (0 != (sts & (1 << 1)))
                {
                    m_AxisStates[nAxisNo - 1] = AxisState.DriveAlarm;
                    return AxisState.DriveAlarm;   //驱动器异常报警
                }
                if (m_AxisStates[nAxisNo - 1] == AxisState.Homeing)
                {
                    return AxisState.Homeing;
                }
                if (m_AxisStates[nAxisNo - 1] == AxisState.ErrAlarm)
                {
                    return AxisState.ErrAlarm;
                }
                if (0 != (sts & (1 << 5)))
                {
                    m_AxisStates[nAxisNo - 1] = AxisState.LimtPStop;
                    return AxisState.LimtPStop;   //正限位触发
                }
                if (0 != (sts & (1 << 6)))
                {
                    m_AxisStates[nAxisNo - 1] = AxisState.LimtNStop;
                    return AxisState.LimtNStop;   //负限位触发
                }

                if (0 == (sts & (1 << 10)))
                {
                    m_AxisStates[nAxisNo - 1] = AxisState.NormalStop;
                    return AxisState.NormalStop;   //正常停止
                }
                m_AxisStates[nAxisNo - 1] = AxisState.Moving;
                return AxisState.Moving;  //正在运动中
            }
            else
                return AxisState.ErrAlarm;
        }
        public override bool isOrgTrig(int nAxisNo)
        {
            // 信号取反 m_ProPrietarySignal[nAxisNo - 1].oriSignal===1    
            //0 信号为正常m_ProPrietarySignal[nAxisNo - 1].oriSignal===0
            nAxisNo += 1;
            short sRtn = 0;
            int orgLimit = 0;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            uint pDI = 0;
            long lStandardIo = 0;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            mc.GTN_GetEcatAxisDI((short)nCore, (short)nAxisNo, out pDI);
            lStandardIo |= (pDI & (0x01 << 2)); //原点到位存在第4位
            return 0 != lStandardIo;
            // sRtn |= mc.GTN_GetDi((short)nCore, mc.MC_HOME, out orgLimit);
            //  int signal = (orgLimit & (1 << (nAxisNo - 1)));
        }
        public bool isOrgTrigAllClose(int nAxisNo)
        {
            // 信号取反 m_ProPrietarySignal[nAxisNo - 1].oriSignal===1    
            //0 信号为正常m_ProPrietarySignal[nAxisNo - 1].oriSignal===0
            //isNegLimit(nAxisNo);
            //isPosLimit(nAxisNo);
            nAxisNo += 1;
            short sRtn = 0;
            int orgLimit = 0;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            uint pDI = 0;
            long lStandardIo = 0;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            mc.GTN_GetEcatAxisDI((short)nCore, (short)nAxisNo, out pDI);
            if ((pDI & (0x01 << 2)) != 0)
                return false; //原点到位存在第4位    
            else
                return true;

            // sRtn |= mc.GTN_GetDi((short)nCore, mc.MC_HOME, out orgLimit);
            //  int signal = (orgLimit & (1 << (nAxisNo - 1)));

        }
        public bool isNegLimit(int nAxisNo)
        {
            nAxisNo += 1;
            short sRtn = 0;
            int negLimit = 0;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            uint pDI = 0;
            long lStandardIo = 0;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            mc.GTN_GetEcatAxisDI((short)nCore, (short)nAxisNo, out pDI);
            lStandardIo |= pDI & (0x01 << 0); //原点到位存在第4位 
            return 0 != lStandardIo;
            //sRtn |= mc.GTN_GetDi((short)nCore, mc.MC_LIMIT_NEGATIVE, out negLimit);
            //return 0 != (negLimit & (1 << (nAxisNo - 1)));
        }
        public bool isPosLimit(int nAxisNo)
        {

            nAxisNo += 1;
            short sRtn = 0;
            int posLimit = 0;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            uint pDI = 0;
            long lStandardIo = 0;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            mc.GTN_GetEcatAxisDI((short)nCore, (short)nAxisNo, out pDI);
            lStandardIo |= pDI & (0x01 << 1); //原点到位存在第4位 
            return 0 != lStandardIo;
            //sRtn |= mc.GTN_GetDi((short)nCore, mc.MC_LIMIT_POSITIVE, out posLimit);
            //return 0 != (posLimit & (1 << (nAxisNo - 1)));
        }

        public bool isCapture(int nAxisNo, out CaptureBit captureBit, out int nPro1PPos, out int nPro1NPos, out int nPro2PPos, out int nPro2NPos)
        {
            nAxisNo += 1;
            ushort capture = 0;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            captureBit = CaptureBit.CaptureNone;
            nPro1PPos = nPro1NPos = nPro2PPos = nPro2NPos = 0;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            mc.GTN_GetTouchProbeStatus((short)nCore, (short)nAxisNo, out capture,
                out nPro1PPos, out nPro1NPos, out nPro2PPos, out nPro2NPos);//读取捕获状态
                                                                            //if (0 != (capture & (0x01 << (int)CaptureBit.RiseCapture1)))
                                                                            //{
                                                                            //    logger.Info($"{capture}触发 并清除");
                                                                            //   ClearProb(nAxisNo-1,1);
                                                                            //    captureBit = CaptureBit.RiseCapture1; return true;
                                                                            //}
            if (0 != (capture & (0x01 << (int)CaptureBit.FallCapture1)))
            {
                logger.Info($"{capture}触发 并清除");
                ClearProb(nAxisNo - 1, 1);
                captureBit = CaptureBit.FallCapture1; return true;
            }
            if (0 != (capture & (0x01 << (int)CaptureBit.RiseCapture2)))
            {
                ClearProb(nAxisNo - 1, 1);
                captureBit = CaptureBit.RiseCapture2; return true;
            }
            if (0 != (capture & (0x01 << (int)CaptureBit.FallCapture2)))
            {
                ClearProb(nAxisNo - 1, 1);
                captureBit = CaptureBit.FallCapture2; return true;
            }
            else
                return false;
        }

        delegate void CheckAxisSignal(int nAxisNo, bool bDir);

        public bool JogMoveHome(int nAxisNo, bool bPositive, int bStart, double nSpeed)
        {
            nAxisNo += 1;

            int nCore = nAxisNo <= 32 ? 1 : 2;
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            double speed = nSpeed;
            double Acc = m_MovePrm[nAxisNo - 1].AccH;
            double Dcc = m_MovePrm[nAxisNo - 1].AccL;
            speed = nSpeed;
            Acc = speed == m_HomePrm[nAxisNo - 1].VelH ? m_HomePrm[nAxisNo - 1].AccH : m_HomePrm[nAxisNo - 1].AccL;
            Dcc = speed == m_HomePrm[nAxisNo - 1].VelH ? m_HomePrm[nAxisNo - 1].DccH : m_HomePrm[nAxisNo - 1].DccL;
            short rtn = 0;
            mc.TJogPrm trap = new mc.TJogPrm();
            rtn |= mc.GTN_ClrSts((short)nCore, (short)nAxisNo, 8);
            rtn |= mc.GTN_PrfJog((short)nCore, (short)nAxisNo);
            rtn |= mc.GTN_GetJogPrm((short)nCore, (short)nAxisNo, out trap);
            if (rtn == 0)
            {
                trap.acc = Acc;
                trap.dec = Dcc;
                trap.smooth = 0.2;
                rtn |= mc.GTN_SetJogPrm((short)nCore, (short)nAxisNo, ref trap);
                rtn |= mc.GTN_SetVel((short)nCore, (short)nAxisNo, bPositive ? (double)speed : (double)-speed);
                rtn |= mc.GTN_Update((short)nCore, 1 << (nAxisNo - 1));
                if (rtn == 0)
                {
                    return true;
                }
            }
            return false;
        }
        private short JogHomeHV(int nAxisNo, double vel, CheckAxisSignal checkAxisSignal = null)
        {
            short sRtn = 0;
            bool bPositive = bJogHomeDir[nAxisNo];
            if (isOrgTrig(nAxisNo))
            {
                StopAxisOnly(nAxisNo);
                if (checkAxisSignal != null)
                    checkAxisSignal(nAxisNo, bPositive);
                return sRtn;
            }
            if (GetPrfStop(nAxisNo))
                JogMoveHome(nAxisNo, bPositive, 0, vel);
            // JogMove(nAxisNo, bPositive, 0, (int)vel);
            if (checkAxisSignal != null)
                checkAxisSignal(nAxisNo - 1, bPositive);
            return sRtn;
        }
        private short JogHomeHV2(int nAxisNo, double vel, CheckAxisSignal checkAxisSignal = null)
        {
            short sRtn = 0;
            bool bPositive = bJogHomeDir[nAxisNo];
            if (isOrgTrigAllClose(nAxisNo))
            {
                StopAxisOnly(nAxisNo);
                if (checkAxisSignal != null)
                    checkAxisSignal(nAxisNo, bPositive);
                return sRtn;
            }
            if (GetPrfStop(nAxisNo))
                JogMoveHome(nAxisNo, bPositive, 0, vel);
            //   JogMove(nAxisNo, bPositive, 0, (int)vel);
            if (checkAxisSignal != null)
                checkAxisSignal(nAxisNo - 1, bPositive);
            return sRtn;
        }
        private void JogVlToHomeSignal(int nAxisNo)
        {

            int CountTrigger = FindHomeSignalStates[nAxisNo].Count;
            if (CountTrigger <= 0)
                return;
            bool bDir = false;
            if (FindHomeSignalStates[nAxisNo][CountTrigger - 1].findHomeSignalState == FindHomeSignalState.FiristHome
                 || FindHomeSignalStates[nAxisNo][CountTrigger - 1].findHomeSignalState == FindHomeSignalState.FiristHomeFall
                 )
            {
                bDir = !FindHomeSignalStates[nAxisNo][CountTrigger - 1].bCurrentMoveIsPositive;
            }
            else
            {
                bDir = FindHomeSignalStates[nAxisNo][CountTrigger - 1].bCurrentMoveIsPositive;
            }

            if (GetPrfStop(nAxisNo))
                JogMoveHome(nAxisNo, bDir, 0, m_HomePrm[nAxisNo].VelL);
            // JogMove(nAxisNo, bDir, 0, (int)m_HomePrm[nAxisNo].VelL);
            if (isOrgTrig(nAxisNo))
            {
                StopAxisOnly(nAxisNo);
                homeSteps[nAxisNo].Dequeue();
            }
        }

        private void JogVlToHomeSignal2(int nAxisNo)
        {

            int CountTrigger = FindHomeSignalStates[nAxisNo].Count;
            if (CountTrigger <= 0)
                return;
            bool bDir = false;
            if (FindHomeSignalStates[nAxisNo][CountTrigger - 1].findHomeSignalState == FindHomeSignalState.FiristHome
                 || FindHomeSignalStates[nAxisNo][CountTrigger - 1].findHomeSignalState == FindHomeSignalState.FiristHomeRise
                 )
            {
                bDir = !FindHomeSignalStates[nAxisNo][CountTrigger - 1].bCurrentMoveIsPositive;
            }
            else
            {
                bDir = FindHomeSignalStates[nAxisNo][CountTrigger - 1].bCurrentMoveIsPositive;
            }
            string strDir = bDir ? "正向" : "负向";
            infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{"JogVlToHomeSignal2"}，低速{strDir}寻复归原点，原点信号{isOrgTrigAllClose(nAxisNo)}");
            if (GetPrfStop(nAxisNo))
                JogMoveHome(nAxisNo, bDir, 0, m_HomePrm[nAxisNo].VelL);
            if (isOrgTrigAllClose(nAxisNo))
            {
                infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{"JogVlToHomeSignal2"}，低速{strDir}寻复归原点，找到原点。原点信号{isOrgTrigAllClose(nAxisNo)}");
                StopAxisOnly(nAxisNo);
                homeSteps[nAxisNo].Dequeue();
            }

        }

        private void JogVlLeaveHomeSignal(int nAxisNo)
        {
            if (!isOrgTrig(nAxisNo))
            {
                Thread.Sleep(100);
                StopAxisOnly(nAxisNo);
                Thread.Sleep(100);
                ClearProb(nAxisNo, 1);
                SetProb(nAxisNo, 1);
                homeSteps[nAxisNo].Dequeue();
                return;
            }
            if (GetPrfStop(nAxisNo))
                JogMoveHome(nAxisNo, !m_HomePrm[nAxisNo]._bHomeDir, 0, m_HomePrm[nAxisNo].VelL);

        }
        private void JogVlLeaveHomeSignal2(int nAxisNo)
        {
            if (!isOrgTrigAllClose(nAxisNo))
            {
                Thread.Sleep(200);
                StopAxisOnly(nAxisNo);
                double distance = 0.5;
                if (!m_HomePrm[nAxisNo]._bHomeDir)
                    distance = 10;
                else
                    distance = -10;
                MotionMgr.GetInstace().RelativeMove(nAxisNo, distance, m_HomePrm[nAxisNo].VelL);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                while (true)
                {
                    if (GetPrfStop(nAxisNo))
                        break;
                    if (stopwatch.ElapsedMilliseconds > 1000)
                        break;
                    Thread.Sleep(10);
                }

                if (!isOrgTrigAllClose(nAxisNo))
                {
                    infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{"JogVlLeaveHomeSignal2"}， 离开原点停止 原点信号{isOrgTrigAllClose(nAxisNo)}");

                    StopAxisOnly(nAxisNo);
                    Thread.Sleep(100);
                    ClearProb(nAxisNo, 1);
                    SetProb(nAxisNo, 1);
                    homeSteps[nAxisNo].Dequeue();
                    return;
                }
            }
            if (GetPrfStop(nAxisNo))
            {
                string strDir = m_HomePrm[nAxisNo]._bHomeDir ? "负向" : "正向";
                infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{"JogVlLeaveHomeSignal2"}，低速{strDir} 离开原点 原点信号{isOrgTrigAllClose(nAxisNo)}");
                JogMoveHome(nAxisNo, !m_HomePrm[nAxisNo]._bHomeDir, 0, m_HomePrm[nAxisNo].VelL);
            }


        }
        private void JogVlFindHomeRising(int nAxisNo)
        {
            if (GetPrfStop(nAxisNo))
                JogMoveHome(nAxisNo, m_HomePrm[nAxisNo]._bHomeDir, 0, m_HomePrm[nAxisNo].VelL);
            CaptureBit captureBit = CaptureBit.CaptureNone;
            isCapture(nAxisNo, out captureBit, out int Pro1ppos, out int Pro1npos, out int Pro2ppos, out int Pro2npos);
            if (captureBit == CaptureBit.RiseCapture1)
            {
                StopAxisOnly(nAxisNo);
                ClearProb(nAxisNo, 1);
                homeSteps[nAxisNo].Clear();
                PushMulitStep(nAxisNo, HomeStep.JudeInPos, HomeStep.EndHome);
                AbsMove(nAxisNo, Pro1ppos, m_HomePrm[nAxisNo].VelH);
            }
        }

        private void JogVlFindHomeFailing(int nAxisNo)
        {
            CaptureBit captureBit = CaptureBit.CaptureNone;
            isCapture(nAxisNo, out captureBit, out int Pro1ppos, out int Pro1npos, out int Pro2ppos, out int Pro2npos);
            if (captureBit == CaptureBit.FallCapture1)
            {
                infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{"JogVlFindHomeFailing"}，寻找到下降沿 原点信号{isOrgTrigAllClose(nAxisNo)}");

                StopAxisOnly(nAxisNo);
                ClearProb(nAxisNo, 1);
                homeSteps[nAxisNo].Clear();
                PushMulitStep(nAxisNo, HomeStep.JudeInPos, HomeStep.EndHome);
                AbsMove(nAxisNo, Pro1npos, m_HomePrm[nAxisNo].VelH);
            }
        }
        //nParam==1  回0 的上升沿触发 
        public override bool Home(int nAxisNo, int nParam)
        {
            m_AxisStates[nAxisNo] = AxisState.Homeing;
            m_ManualEventHomeingStop[nAxisNo].Reset();
            int nSystemAxisno = GetSystemAxisNo(nAxisNo);
            if (MotorType.EcatAxis == MotionMgr.GetInstace().GetMotorType(nSystemAxisno))
            {
                EcatAxisHome(nAxisNo, m_HomePrm[nAxisNo]._nHomeMode);
                return true;
            }
            else
            {
                AxisOnModle2(nAxisNo);
            }
            return true;
        }
        public void SetAxisMode(int nAxisNo, byte mode)
        {
            short AXIS = (short)(nAxisNo + 1);
            int nSystemAxisno = GetSystemAxisNo(nAxisNo);
            short sRtn = 0;
            short nCore = 0;
            nCore = (short)(AXIS <= 32 ? 1 : 2);
            //sRtn = mc.GTN_SetHomingMode(nCore, AXIS, (short)MotorMode.CyclicSyncPosition);

            sRtn = mc.GTN_SetHomingMode(1, 1, 8);
            uint abortCode;
            if (sRtn != 0)
                sRtn = mc.GTN_EcatSDODownload(1, 0, 0x6060, 0x00, ref mode, 1, out abortCode);
        }
        private void EcatAxisHome(int nAxisNo, int nParam)
        {
            short AXIS = (short)(nAxisNo + 1);
            int nSystemAxisno = GetSystemAxisNo(nAxisNo);
            short sRtn = 0;
            short nCore = 0;
            Task.Run(() =>
            {
                ushort sHomeSts = 0; // 回零状态
                try
                {

                    nCore = (short)(AXIS <= 32 ? 1 : 2);
                    int nDir = 0;
                    int lAxisStatus = 0; // 轴状态
                    sRtn = mc.GTN_ClrSts(nCore, AXIS, 1);
                    sRtn = mc.GTN_AxisOn(nCore, AXIS);
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Restart();
                    do
                    {
                        if (GetServoState(nSystemAxisno))
                            break;
                        if (stopwatch.ElapsedMilliseconds > 1000)
                        {
                            m_AxisStates[nAxisNo] = AxisState.ErrAlarm;
                            return;
                        }
                        Thread.Sleep(10);
                    } while (GetServoState(nSystemAxisno));
                    // 必须处于伺服使能状态，切换到回零模式
                    sRtn = mc.GTN_SetHomingMode(nCore, AXIS, (short)MotorMode.HomeIng);
                    // 设置回零参数
                    sRtn = mc.GTN_SetEcatHomingPrm(nCore, AXIS, (short)nParam, m_HomePrm[nAxisNo].VelH, m_HomePrm[nAxisNo].VelL, m_HomePrm[nAxisNo].AccH, 0, 0);
                    // 启动回零
                    sRtn = mc.GTN_StartEcatHoming(nCore, AXIS);
                    stopwatch.Restart();
                    sRtn = mc.GTN_GetEcatHomingStatus(nCore, AXIS, out sHomeSts);
                    do
                    {
                        if (AxisState.DriveAlarm == MotionMgr.GetInstace().IsAxisNormalStop(nSystemAxisno))
                            mc.GTN_ClrSts((short)nCore, (short)AXIS, 8);
                        if (stopwatch.ElapsedMilliseconds > 160000)
                        {
                            m_AxisStates[nAxisNo] = AxisState.ErrAlarm;
                            //超时停止
                            sRtn = mc.GTN_StopEcatHoming(nCore, AXIS);
                            logger.Warn($"{nSystemAxisno}号轴 回0 过程中超时");
                            return;
                        }
                        if (!m_ManualEventHomeingStop[nAxisNo].WaitOne(5))
                        {
                            //获取回原点状态
                            sRtn = mc.GTN_GetEcatHomingStatus(nCore, AXIS, out sHomeSts);
                        }
                        else
                        {
                            sRtn = mc.GTN_StopEcatHoming(nCore, AXIS);
                            return;
                        }

                    } while (3 != sHomeSts); // 等待搜索原点完成

                }
                catch (Exception ex)
                {
                    sRtn = mc.GTN_StopEcatHoming(nCore, AXIS);
                    m_AxisStates[nAxisNo] = AxisState.ErrAlarm;
                    logger.Warn($"{nSystemAxisno}号轴 回0 过程中异常");
                }
                finally
                {

                    m_ManualEventHomeingStop[nAxisNo].Reset();
                    string strInfo = m_AxisStates[nAxisNo] == AxisState.NormalStop ? "成功" : "失败";

                    short sRtn1 = 0;
                    do
                    {
                        sRtn1 = mc.GTN_GetEcatHomingStatus(nCore, AXIS, out sHomeSts);
                    } while ((sHomeSts & 0x01) == 0);

                    sRtn1 |= mc.GTN_SetHomingMode(nCore, AXIS, (short)8); // 切换到位置控制模式

                    ushort drmove = 0;
                    //for (int i = 0; i < 3; i++)
                    //{
                    //    Thread.Sleep(10);
                    //    byte pval = 8;
                    //    //mc.GTN_SetEcatRawData(nCore, 22, 1, ref pval);
                    //    sRtn1 |= mc.GTN_SetHomingMode(nCore, AXIS, (short)8); // 切换到位置控制模式
                    //}
                    mc.GTN_GetEcatAxisMode(nCore, AXIS, out drmove);
                    logger.Warn($"{nSystemAxisno}号轴 回0 过程结束{strInfo}，切换到控制模式{drmove} ");
                    //StopAxis(nAxisNo);
                    Thread.Sleep(30);
                    //sRtn1 |= mc.GTN_ZeroPos(nCore, AXIS, 0);
                    SynchAxisPos(nAxisNo);
                    if (m_AxisStates[nAxisNo] != AxisState.ErrAlarm || m_AxisStates[nAxisNo] != AxisState.DriveAlarm)
                        m_AxisStates[nAxisNo] = AxisState.NormalStop;
                }
            });
        }
        public override GpState GetGpState(IntPtr group)
        {
            int pSegment = 0;
            short GpHand = (short)(int)group;
            short run = 0; short Prun = 0;
            try
            {
                short nCore = 1;
                if (GroupDic.ContainsKey(GpHand))
                {
                    nCore = (short)(GroupDic[GpHand].profile1 <= 32 ? 1 : 2);
                }
                run= mc.GTN_CrdStatus(nCore, GpHand, out Prun, out pSegment, 0);
                if (run != 0)
                {
                    return GpState.GpErrStop;
                }
                if (Prun == 1)
                {
                    return GpState.GpMotion;
                }
                else
                {
                    return GpState.GpReady;
                }
          
            }
            catch (Exception e)
            {
                logger.Info("获取群组状态异常（GetGpState）");
                return GpState.GpErrStop;
            }
        }


        private void PushMulitStep(int nAxisNo, params HomeStep[] Steps)
        {
            if (Steps == null)
                return;
            for (int i = 0; i < Steps.Length; i++)
                homeSteps[nAxisNo].Enqueue(Steps[i]);
        }

        bool[] bJogHomeDir = null;
        public enum FindHomeSignalState
        {
            None,
            FiristHome,
            FiristHomeRise,
            FiristHomeFall,
            FiristLimt,
            FiristLimtP,
            FiristLimtN,
        }
        struct HoingSignalState
        {
            public FindHomeSignalState findHomeSignalState;
            public bool bCurrentMoveIsPositive;
        }


        int TriggleNum = 0;


        public void FindSignal(int nAxisNo, bool bDir)
        {
            CaptureBit captureBit = CaptureBit.CaptureNone;
            isCapture(nAxisNo, out captureBit, out int Pro1ppos, out int Pro1npos, out int Pro2ppos, out int Pro2npos);
            FindHomeSignalState sFindHomeSignalState = FindHomeSignalState.None;
            if (isOrgTrig(nAxisNo))
            {
                sFindHomeSignalState = FindHomeSignalState.FiristHome;
                StopAxis(nAxisNo);
            }
            else if (isNegLimit(nAxisNo) || isPosLimit(nAxisNo))
            {
                if (isNegLimit(nAxisNo))
                    sFindHomeSignalState = FindHomeSignalState.FiristLimtN;
                if (isPosLimit(nAxisNo))
                    sFindHomeSignalState = FindHomeSignalState.FiristLimtP;
                bJogHomeDir[nAxisNo] = !bJogHomeDir[nAxisNo];
            }
            else
                sFindHomeSignalState = captureBit == CaptureBit.FallCapture2 ? FindHomeSignalState.FiristHomeFall : (captureBit == CaptureBit.RiseCapture2 ? FindHomeSignalState.FiristHomeRise : FindHomeSignalState.None);

            if (sFindHomeSignalState == FindHomeSignalState.FiristHome || sFindHomeSignalState == FindHomeSignalState.FiristHomeFall || sFindHomeSignalState == FindHomeSignalState.FiristHomeRise)
            {
                StopAxis(nAxisNo);
                homeSteps[nAxisNo].Dequeue();
            }
            if (sFindHomeSignalState == FindHomeSignalState.FiristHome || sFindHomeSignalState == FindHomeSignalState.FiristLimtP || sFindHomeSignalState == FindHomeSignalState.FiristLimtN)
                ClearProb(nAxisNo, 2);
            if (sFindHomeSignalState != FindHomeSignalState.None)
            {
                //FindHomeSignalStates[nAxisNo][TriggleNum] = sFindHomeSignalState;
                //FindHomeSignalStates[nAxisNo][TriggleNum].bCurrentMoveIsPositive = bDir;
                FindHomeSignalStates[nAxisNo].Add(new HoingSignalState()
                {
                    findHomeSignalState = sFindHomeSignalState,
                    bCurrentMoveIsPositive = bDir
                });
                //TriggleNum = (TriggleNum + 1) % 2;
            }
        }
        public void ClearProb(int nAxisNo, int nIndex)
        {
            nAxisNo += 1;
            short nCore = (short)(nAxisNo <= 32 ? 1 : 2);
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            short sRtn = 0;
            int param = 0;
            infoOnly.Info($"系统轴：{nAxisNo} 清除探针{nIndex}");
            mc.GTN_SetTouchProbeFunction(nCore, (short)nAxisNo, (short)param);
        }
        public void SetProb(int nAxisNo, int nIndex)
        {
            nAxisNo += 1;
            short nCore = (short)(nAxisNo <= 32 ? 1 : 2);
            nAxisNo = (short)(nAxisNo > 32 ? nAxisNo - 32 : nAxisNo);
            short sRtn = 0;
            int param = 0;
            param |= param | (0x01 << 8 * (nIndex - 1));
            param |= param | (0x01 << (8 * (nIndex - 1) + 4));
            param |= param | (0x01 << (8 * (nIndex - 1) + 5));
            infoOnly.Info($"系统轴：{nAxisNo} 设置探针{nIndex}");
            sRtn = mc.GTN_SetTouchProbeFunction(nCore, (short)nAxisNo, (short)param);
        }

        public void SynchAxisPos(int nAxisNo)
        {
            nAxisNo += 1;
            nAxisNo = nAxisNo > 32 ? nAxisNo - 32 : nAxisNo;
            short nCore = (short)(nAxisNo <= 32 ? 1 : 2);
            short rtn = GTN.mc.GTN_GetEcatEncPos(nCore, (short)nAxisNo, out int encpos);
            rtn = GTN.mc.GTN_SetPrfPos(nCore, (short)nAxisNo, encpos);
            rtn = GTN.mc.GTN_SetEncPos(nCore, (short)nAxisNo, encpos);
            rtn = GTN.mc.GTN_SynchAxisPos(nCore, 0x1 << (nAxisNo - 1));
        }


        private bool AxisOnModle(int nAxisNo)
        {
            ClearProb(nAxisNo, 1);
            SetProb(nAxisNo, 1);
            bJogHomeDir[nAxisNo] = m_HomePrm[nAxisNo]._bHomeDir;
            try
            {
                SynchAxisPos(nAxisNo);
                HomeStep homeStep = HomeStep.JudeSignl;
                homeSteps[nAxisNo].Clear();
                string strDir = m_HomePrm[nAxisNo]._bHomeDir ? "正向" : "负向";
                PushMulitStep(nAxisNo, HomeStep.JogVH, HomeStep.CheckHome);
                CaptureBit captureBit = CaptureBit.CaptureNone;
                int nProb1RisePos = 0, nProb1FallPos = 0, nProb2RisePos = 0, nProb2FallPos = 0;
                while (true)
                {
                    homeStep = homeSteps[nAxisNo].Peek();
                    switch (homeStep)
                    {
                        case HomeStep.JogVH:
                            infoOnly.Info($"系统轴：{ nAxisNo}轴高速速{strDir}接近原点");
                            JogHomeHV(nAxisNo, m_HomePrm[nAxisNo].VelH);
                            homeSteps[nAxisNo].Dequeue();
                            SetProb(nAxisNo, 1);
                            break;
                        case HomeStep.CheckHome:

                            if (isOrgTrig(nAxisNo))
                            {
                                StopAxisOnly(nAxisNo);
                                homeSteps[nAxisNo].Dequeue();
                                PushMulitStep(nAxisNo, HomeStep.JudePos);

                                FindHomeSignalStates[nAxisNo].Add(new HoingSignalState()
                                {
                                    findHomeSignalState = FindHomeSignalState.FiristHome,
                                    bCurrentMoveIsPositive = bJogHomeDir[nAxisNo],
                                });
                            }
                            else if (isCapture(nAxisNo, out captureBit, out nProb1RisePos, out nProb1FallPos,
                               out nProb2RisePos, out nProb2FallPos))
                            {
                                if (captureBit != CaptureBit.CaptureNone)
                                {
                                    homeSteps[nAxisNo].Dequeue();
                                    StopAxisOnly(nAxisNo);
                                    int npos = captureBit == CaptureBit.RiseCapture1 ? nProb1RisePos : (captureBit == CaptureBit.FallCapture1 ? nProb1FallPos : int.MaxValue);
                                    FindHomeSignalState findHomeSignalState = captureBit == CaptureBit.RiseCapture1 ? FindHomeSignalState.FiristHomeRise : (captureBit == CaptureBit.FallCapture1 ? FindHomeSignalState.FiristHomeFall : FindHomeSignalState.None);
                                    if (findHomeSignalState != FindHomeSignalState.None)
                                    {
                                        FindHomeSignalStates[nAxisNo].Add(new HoingSignalState()
                                        {
                                            findHomeSignalState = findHomeSignalState,
                                            bCurrentMoveIsPositive = bJogHomeDir[nAxisNo],
                                        });
                                        if (GetPrfStop(nAxisNo))
                                            AbsMove(nAxisNo, npos, m_HomePrm[nAxisNo].VelH);
                                        ClearProb(nAxisNo, 1);
                                        PushMulitStep(nAxisNo, HomeStep.JudeInPos, HomeStep.JudePos);
                                    }
                                }
                            }
                            if (isPosLimit(nAxisNo) || isNegLimit(nAxisNo))
                            {
                                homeSteps[nAxisNo].Dequeue();
                                FindHomeSignalStates[nAxisNo].Add(new HoingSignalState()
                                {
                                    findHomeSignalState = FindHomeSignalState.FiristLimt,
                                    bCurrentMoveIsPositive = bJogHomeDir[nAxisNo],
                                });
                                bJogHomeDir[nAxisNo] = !bJogHomeDir[nAxisNo];
                                StopAxisOnly(nAxisNo);
                                PushMulitStep(nAxisNo, HomeStep.JogVH, HomeStep.CheckHome);

                            }
                            break;
                        case HomeStep.JudeInPos:
                            if (GetPrfStop(nAxisNo))
                            {
                                homeSteps[nAxisNo].Dequeue();
                            }
                            break;
                        case HomeStep.JudePos:
                            homeSteps[nAxisNo].Dequeue();
                            if (isOrgTrig(nAxisNo))
                            {
                                PushMulitStep(nAxisNo, HomeStep.JogVLLeaveHome, HomeStep.JogPVLFindHomeRise, HomeStep.EndHome);
                            }
                            else
                            {
                                PushMulitStep(nAxisNo, HomeStep.JogVLToHome, HomeStep.JudePos);
                            }
                            break;
                        case HomeStep.JogVLToHome:
                            JogVlToHomeSignal(nAxisNo);
                            break;
                        case HomeStep.JogVLLeaveHome:
                            strDir = m_HomePrm[nAxisNo]._bHomeDir ? "负向" : "正向";
                            infoOnly.Info($"系统轴：{ nAxisNo}轴低速{strDir}离开原点");
                            JogVlLeaveHomeSignal(nAxisNo);
                            break;
                        case HomeStep.JogPVLFindHomeRise:
                            strDir = m_HomePrm[nAxisNo]._bHomeDir ? "正向" : "负向";
                            infoOnly.Info($"系统轴：{ nAxisNo}轴低速{strDir}离开原点");
                            JogVlFindHomeRising(nAxisNo);
                            break;
                        case HomeStep.EndHome:
                            SetCmdPos(nAxisNo, 0);
                            SetActutalPos(nAxisNo, 0);
                            homeSteps[nAxisNo].Clear();
                            m_AxisStates[nAxisNo] = AxisState.NormalStop;
                            return true;
                    }

                    if (!m_ManualEventHomeingStop[nAxisNo].WaitOne(5))
                    {
                        continue;
                    }
                    else
                    {
                        StopAxisOnly(nAxisNo);
                        return false;
                    }

                }
            }
            catch (Exception e)
            {
                logger.Info($"系统轴：{ nAxisNo}轴低速停止");
                m_AxisStates[nAxisNo] = AxisState.DriveAlarm;
                return false;
            }
            finally
            {
                m_ManualEventHomeingStop[nAxisNo].Reset();
                if (m_AxisStates[nAxisNo] != AxisState.ErrAlarm || m_AxisStates[nAxisNo] != AxisState.DriveAlarm)
                    m_AxisStates[nAxisNo] = AxisState.NormalStop;
                IsAxisNormalStop(nAxisNo);
                StopAxisOnly(nAxisNo);
            }
            return true;
        }

        private bool AxisOnModle2(int nAxisNo)
        {
            ClearProb(nAxisNo, 1);
            SetProb(nAxisNo, 1);
            bJogHomeDir[nAxisNo] = m_HomePrm[nAxisNo]._bHomeDir;
            try
            {
                SynchAxisPos(nAxisNo);
                HomeStep homeStep = HomeStep.JudeSignl;
                homeSteps[nAxisNo].Clear();
                string strDir = m_HomePrm[nAxisNo]._bHomeDir ? "正向" : "负向";
                PushMulitStep(nAxisNo, HomeStep.JogVH, HomeStep.CheckHome);
                CaptureBit captureBit = CaptureBit.CaptureNone;
                int nProb1RisePos = 0, nProb1FallPos = 0, nProb2RisePos = 0, nProb2FallPos = 0;


                while (true)
                {


                    homeStep = homeSteps[nAxisNo].Peek();
                    switch (homeStep)
                    {
                        case HomeStep.JogVH:

                            infoOnly.Info($"系统轴：{ nAxisNo}轴高速速{strDir}接近原点");
                            JogHomeHV2(nAxisNo, m_HomePrm[nAxisNo].VelH);
                            homeSteps[nAxisNo].Dequeue();
                            ClearProb(nAxisNo, 1);
                            SetProb(nAxisNo, 1);
                            break;
                        case HomeStep.CheckHome:

                            if (isOrgTrigAllClose(nAxisNo))
                            {
                                infoOnly.Info($"系统轴：{nAxisNo} 步骤：{homeStep}，原点到达");
                                StopAxisOnly(nAxisNo);
                                homeSteps[nAxisNo].Dequeue();
                                PushMulitStep(nAxisNo, HomeStep.JudePos);

                                FindHomeSignalStates[nAxisNo].Add(new HoingSignalState()
                                {
                                    findHomeSignalState = FindHomeSignalState.FiristHome,
                                    bCurrentMoveIsPositive = bJogHomeDir[nAxisNo],
                                });
                            }
                            else if (isCapture(nAxisNo, out captureBit, out nProb1RisePos, out nProb1FallPos,
                               out nProb2RisePos, out nProb2FallPos))
                            {
                                if (captureBit != CaptureBit.CaptureNone)
                                {
                                    infoOnly.Info($"系统轴：{nAxisNo} 步骤：{homeStep}，原点上下边沿触发");
                                    homeSteps[nAxisNo].Dequeue();
                                    StopAxisOnly(nAxisNo);
                                    int npos = captureBit == CaptureBit.RiseCapture1 ? nProb1RisePos : (captureBit == CaptureBit.FallCapture1 ? nProb1FallPos : int.MaxValue);
                                    FindHomeSignalState findHomeSignalState = captureBit == CaptureBit.RiseCapture1 ? FindHomeSignalState.FiristHomeRise : (captureBit == CaptureBit.FallCapture1 ? FindHomeSignalState.FiristHomeFall : FindHomeSignalState.None);
                                    if (findHomeSignalState != FindHomeSignalState.None)
                                    {
                                        FindHomeSignalStates[nAxisNo].Add(new HoingSignalState()
                                        {
                                            findHomeSignalState = findHomeSignalState,
                                            bCurrentMoveIsPositive = bJogHomeDir[nAxisNo],
                                        });
                                        if (GetPrfStop(nAxisNo))
                                            AbsMove(nAxisNo, npos, m_HomePrm[nAxisNo].VelH);
                                        PushMulitStep(nAxisNo, HomeStep.JudeInPos, HomeStep.JudePos);
                                        ClearProb(nAxisNo, 1);
                                    }
                                }
                            }
                            if (isPosLimit(nAxisNo) || isNegLimit(nAxisNo))
                            {
                                infoOnly.Info($"系统轴：{nAxisNo} 步骤：{homeStep}，限位触发");
                                homeSteps[nAxisNo].Dequeue();
                                FindHomeSignalStates[nAxisNo].Add(new HoingSignalState()
                                {
                                    findHomeSignalState = FindHomeSignalState.FiristLimt,
                                    bCurrentMoveIsPositive = bJogHomeDir[nAxisNo],
                                });
                                if (isPosLimit(nAxisNo) && bJogHomeDir[nAxisNo] != false)
                                {
                                    bJogHomeDir[nAxisNo] = false;
                                    StopAxisOnly(nAxisNo);
                                }


                                if (isNegLimit(nAxisNo) && bJogHomeDir[nAxisNo] != true)
                                {
                                    bJogHomeDir[nAxisNo] = true;
                                    StopAxisOnly(nAxisNo);
                                }
                                PushMulitStep(nAxisNo, HomeStep.JogVH, HomeStep.CheckHome);
                            }
                            break;
                        case HomeStep.JudeInPos:
                            if (GetPrfStop(nAxisNo))
                            {
                                homeSteps[nAxisNo].Dequeue();
                            }
                            break;
                        case HomeStep.JudePos:
                            homeSteps[nAxisNo].Dequeue();
                            if (isOrgTrigAllClose(nAxisNo))
                            {
                                infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{homeStep}，在原点中");
                                PushMulitStep(nAxisNo, HomeStep.JogVLLeaveHome, HomeStep.JudeInPos, HomeStep.SetProbeHomeRise, HomeStep.JogPVLFindHomeRise, HomeStep.EndHome);
                            }
                            else
                            {
                                infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{homeStep}，不在原点中");
                                PushMulitStep(nAxisNo, HomeStep.JogVLToHome, HomeStep.JudePos);
                            }
                            break;
                        case HomeStep.JogVLToHome:

                            JogVlToHomeSignal2(nAxisNo);
                            break;
                        case HomeStep.JogVLLeaveHome:
                            strDir = m_HomePrm[nAxisNo]._bHomeDir ? "负向" : "正向";
                            infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{homeStep}，低速{strDir}离开原点");
                            JogVlLeaveHomeSignal2(nAxisNo);
                            break;
                        case HomeStep.SetProbeHomeRise:
                            if (GetPrfStop(nAxisNo))
                            {
                                ClearProb(nAxisNo, 1);
                                SetProb(nAxisNo, 1);
                                Thread.Sleep(300);
                                homeSteps[nAxisNo].Dequeue();
                                infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{"SetProbeHomeRise"}， 原点信号{isOrgTrigAllClose(nAxisNo)}，设置Probe");

                                JogMoveHome(nAxisNo, m_HomePrm[nAxisNo]._bHomeDir, 0, m_HomePrm[nAxisNo].VelL);

                            }
                            break;
                        case HomeStep.JogPVLFindHomeRise:
                            strDir = m_HomePrm[nAxisNo]._bHomeDir ? "正向" : "负向";
                            infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{homeStep}，低速{strDir}寻找原点下降沿");
                            JogVlFindHomeFailing(nAxisNo);
                            break;
                        case HomeStep.EndHome:
                            SetCmdPos(nAxisNo, 0);
                            SetActutalPos(nAxisNo, 0);
                            homeSteps[nAxisNo].Clear();
                            infoOnly.Info($"系统轴：{ nAxisNo}轴 步骤：{homeStep}，回原点完成");
                            m_AxisStates[nAxisNo] = AxisState.NormalStop;
                            return true;
                    }

                    if (!m_ManualEventHomeingStop[nAxisNo].WaitOne(5))
                    {
                        continue;
                    }
                    else
                    {
                        StopAxisOnly(nAxisNo);
                        return false;
                    }

                }
            }
            catch (Exception e)
            {
                logger.Info($"系统轴：{ nAxisNo}轴低速停止");
                m_AxisStates[nAxisNo] = AxisState.DriveAlarm;
                return false;
            }
            finally
            {
                m_ManualEventHomeingStop[nAxisNo].Reset();
                if (m_AxisStates[nAxisNo] != AxisState.ErrAlarm || m_AxisStates[nAxisNo] != AxisState.DriveAlarm)
                    m_AxisStates[nAxisNo] = AxisState.NormalStop;

                StopAxisOnly(nAxisNo);
            }
            return true;
        }

        public override AxisState IsHomeNormalStop(int nAxisNo)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            return m_AxisStates[nAxisNo - 1];
        }
        public override int GetAxisPos(int nAxisNo)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            double pos;
            uint pclock;
            short sRtn = 0;
            if (m_MotorType[nAxisNo - 1] == MotorType.SEVER
                || m_MotorType[nAxisNo - 1] == MotorType.EcatAxisModleSevro
                || m_MotorType[nAxisNo - 1] == MotorType.EcatAxis)
                pos = GetAxisActPos(nAxisNo - 1);
            else
                pos = GetAxisCmdPos(nAxisNo - 1);

            return sRtn == 0 ? (int)pos : int.MaxValue;
        }
        public override int GetAxisActPos(int nAxisNo)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            double pos;
            uint pclock;
            short sRtn = 0;
            sRtn |= mc.GTN_GetEncPos((short)nCore, (short)nAxisNo, out pos, 1, out pclock);
            return sRtn == 0 ? (int)pos : int.MaxValue;
        }
        public override bool SetActutalPos(int nAxisNo, double pos)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            logger.Info(string.Format("{0}卡{1}轴设置实际位置", nCore, nAxisNo));
            short Result = 0;
            Result |= mc.GTN_SetEncPos((short)nCore, (short)nAxisNo, (int)pos);
            return Result == 0;
        }

        public override bool SetCmdPos(int nAxisNo, double pos)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            logger.Info(string.Format("{0}卡{1}轴设置命令位置", nCore, nAxisNo));
            short Result = 0;
            Result |= mc.GTN_SetPrfPos((short)nCore, (short)nAxisNo, (int)pos);
            return Result == 0;
        }
        public override int GetAxisCmdPos(int nAxisNo)
        {
            nAxisNo += 1;
            int nCore = nAxisNo <= 32 ? 1 : 2;
            double pos;
            uint pclock;
            short sRtn = 0;
            sRtn |= mc.GTN_GetPrfPos((short)nCore, (short)nAxisNo, out pos, 1, out pclock);
            return sRtn == 0 ? (int)pos : int.MaxValue;
        }
        public override bool AddAxisToGroup(int[] Axisarr, ref object group)
        {
            if (Axisarr == null)
                return false;
            TCrdPrm crdPrm = new TCrdPrm();
            crdPrm.dimension = (short)Axisarr.Length;
            crdPrm.synAccMax = 20;
            crdPrm.synVelMax = 1000;
            crdPrm.setOriginFlag = 1;
            crdPrm.evenTime = 50;

            for (int i = 0; i < Axisarr.Length; i++)
            {
                if (i == 0)
                {
                    crdPrm.profile1 = (short)(1);
                    crdPrm.originPos1 = 0;
                }
                if (i == 1)
                {
                    crdPrm.profile2 = (short)(2);
                    crdPrm.originPos2 = 0;
                }
                if (i == 3)
                {
                    crdPrm.profile3 = (short)(Axisarr[i] + 1);
                    crdPrm.originPos3 = 0;
                }
                if (i == 4)
                {
                    crdPrm.profile4 = (short)(Axisarr[i] + 1);
                    crdPrm.originPos4 = 0;
                }
            }
            short nCore = (short)((Axisarr[0] + 1) <= 32 ? 1 : 2);
            short crd = (short)(int)group;
            short u1 = -1;
            short u2 = -1;
            short ie= mc.GTN_SetCrdMapBase(nCore, crd, (short)(Axisarr[0]+1));
            short p = mc.GTN_GetCrdMapBase(nCore, 1, out u1);
            //p = mc.GTN_GetCrdMapBase(nCore, 2, out u2);
            short rtn = mc.GTN_SetCrdPrm(nCore, crd, ref crdPrm);
            TCrdPrm crdPrm1= new TCrdPrm();
            rtn = mc.GTN_GetCrdPrm(nCore, crd, out crdPrm1);
            if (!GroupDic.ContainsKey((int)group))
            {
                GroupDic.Add((int)group, crdPrm);
            }
            else
            {
                GroupDic[(int)group] = crdPrm;
            }
            return rtn == 0;

        }
        Dictionary<int, TCrdPrm> GroupDic = new Dictionary<int, TCrdPrm>();
        public override bool SetBufMoveParam(object objGroup, double velhigh, double vellow, double acc, double dec)
        {
            int GpHand = (int)objGroup;
            uint reslut = 0;
            TCrdPrm crdPrm =new TCrdPrm ();
            short nCore = 1;
            if (GroupDic.ContainsKey(GpHand))
            {
                nCore = (short)(GroupDic[GpHand].profile1 <= 32 ? 1 : 2);
            }

            short rtn = mc.GTN_GetCrdPrm(nCore, (short)GpHand, out crdPrm);
            crdPrm.synVelMax = velhigh;
            crdPrm.synAccMax = acc;
            rtn |= mc.GTN_SetCrdPrm(nCore, (short)GpHand, ref crdPrm);
            if (GroupDic.ContainsKey(GpHand))
            {
                GroupDic[GpHand] = crdPrm;
            }
            else
            {
                GroupDic.Add(GpHand, crdPrm);
            }
            return rtn == 0;
        }
        public override bool AddBufMove(object objGroup, BufMotionType type, int mode, int nAxisNum, double velHigh, double velLow, double[] Point1, double[] Point2)
        {
            ushort Cmd = 0;
            int GpHand = (int)objGroup;
            double dacc = GroupDic[GpHand].synAccMax;
            short nCore = (short)(GroupDic[GpHand].profile1 <= 32 ? 1 : 2);
            short rtn = 0;

            switch (type)
            {
                case BufMotionType.buf_Line3dAbs:
                    rtn |= mc.GTN_LnXYZ(nCore, (short)(int)objGroup, (int)Point1[0], (int)Point1[1], (int)Point1[2], velHigh, dacc, velHigh, (short)0);
                    break;
                case BufMotionType.buf_Line2dAbs:
                    rtn |= mc.GTN_LnXY(nCore, (short)(int)objGroup, (int)Point1[0], (int)Point1[1], velHigh, dacc, velHigh, (short)0);
                    break;
                case BufMotionType.buf_Arc2dAbsCCW:
                    rtn |= mc.GTN_ArcXYC(nCore, (short)(int)objGroup, (int)Point1[0], (int)Point1[1],(int)Point2[0], (int)Point2[1], 1, velHigh, dacc, 0, (short)0);
                    break;
                case BufMotionType.buf_Arc2dAbsCW:
                    rtn |= mc.GTN_ArcXYC(nCore, (short)(int)objGroup, (int)Point1[0], (int)Point1[1], (int)Point2[0], (int)Point2[1], 0, velHigh, dacc, 0, (short)0);

                    //{
                    //    rtn = 0;
                    //       rtn |= mc.GTN_SetCrdMapBase(1, 1, (short)7);
                    //    TCrdPrm crdPrm = new TCrdPrm();
                    //    int D = 0;
                    //    crdPrm.dimension = 2;
                    //    crdPrm.synAccMax = 1;
                    //    crdPrm.synVelMax = 5000;
                    //    crdPrm.setOriginFlag = 1;
                    //    crdPrm.evenTime = 50;
                    //    crdPrm.profile1 = 1;
                    //    crdPrm.profile2 = 2;
                    //    rtn |= mc.GTN_SetCrdPrm(1, 1, ref crdPrm);
                    //    rtn |= mc.GTN_CrdClear(1, 1, 0);
                    //    //rtn |= mc.GTN_LnXY(1, 1, 100000, -200000, velHigh, dacc, 0, (short)0);
                    //    //rtn |= mc.GTN_ArcXYR(nCore, (short)(int)objGroup, (int)Point1[0], (int)Point1[1], 5000, 0, velHigh, dacc, 0, (short)0);
                    //    rtn |= mc.GTN_ArcXYC(nCore, (short)(int)objGroup, 110000, -300000, 10000,-10000, 0, velHigh, dacc, 0, (short)0);

                    //    rtn |= mc.GTN_CrdSpace(1, 1, out D, 0);
                    //    rtn |= mc.GTN_CrdStart(1, 1, 0);

                    //    short S = 0; int posX = 0;

                    //    //rtn |= mc.GTN_GetEcatEncPos(1, 7, out posX);
                    //    rtn |= mc.GTN_CrdStatus(1, 1, out S, out D, 0);
                    //}

                    break;
                case BufMotionType.buf_end:

                    break;
                default:
                    MessageBox.Show("path运动类型不对", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
            }
            uint nAxisCount = (uint)nAxisNum;

            ;


            return true;
        }
        public override bool AddBufIo(object objGroup, string strIoName, bool bVal, int nAxisIndexInGroup)
        {
            int GpHand = (int)objGroup;

            short nCore = (short)(GroupDic[GpHand].profile1 <= 32 ? 1 : 2);
            bool bFindIo = false;
            int nAxisIndex = 0;
            int nIoIndex = 0;
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


            short rtn = 0;
            ushort mask = (ushort)(0x01 << nIoIndex);
            ushort dovalue = 0;
            if (bVal)
                dovalue = (ushort)(0x01 << nIoIndex);

            int ret=mc.GTN_BufIO(nCore, (short)GpHand, mc.MC_GPO, mask, dovalue, (short)0);
            return true;
        }

        public override bool AddBufDelay(object objGroup, int nTime)
        {
            short GpHand = (short)(int)objGroup;
            short nCore = (short)(GroupDic[GpHand].profile1 <= 32 ? 1 : 2);
            return 0 == mc.GTN_BufDelay(nCore, GpHand, (ushort)nTime, 0);
        }
        public override bool ClearBufMove(object objGroup)
        {
            short GpHand = (short)(int)objGroup;
            short nCore = (short)(int)(GroupDic[GpHand].profile1 <= 32 ? 1 : 2);
            short result= mc.GTN_CrdClear(nCore, GpHand, 0);
            return 0 == result;
        }

        public override bool StartBufMove(object objGroup)
        {
            short GpHand = (short)(int)objGroup;
            short nCore = (short)(GroupDic[GpHand].profile1 <= 32 ? 1 : 2);
            return 0 == mc.GTN_CrdStart(nCore, GpHand, 0);

        }
        public override bool IsInpos(int nAxisNo)
        {
            return IsAxisNormalStop(nAxisNo) == 0;
        }

        public void SetEcatAxisMode(int nAxisNo, MotorMode eMode)
        {
            short Axis = (short)(GetMinAxisNo() + nAxisNo);
            Axis += 1;
            int nCore = Axis <= 32 ? 1 : 2;
            mc.GTN_SetEcatAxisMode((short)nCore, (short)Axis, (short)eMode);
        }
        public MotorMode GetEcatAxisMode(int nAxisNo)
        {
            short Axis = (short)(GetMinAxisNo() + nAxisNo);
            Axis += 1;
            int nCore = Axis <= 32 ? 1 : 2;
            ushort eMode = 0;
            mc.GTN_GetEcatAxisMode((short)nCore, (short)Axis, out eMode);
            return (MotorMode)eMode;
        }

        public double GetEcatAxisAtlTorque(int nAxis)
        {
            short Axis = (short)(GetMinAxisNo() + nAxis);
            Axis += 1;
            int nCore = Axis <= 32 ? 1 : 2;
            short Torque = 0;
            mc.GTN_GetEcatAxisAtlTorque((short)nCore, (short)Axis, out Torque);
            return Torque;
        }
        public void SetEcateAxisTorque(int nAxis, double Torque)
        {
            short Axis = (short)(GetMinAxisNo() + nAxis);
            Axis += 1;
            int nCore = Axis <= 32 ? 1 : 2;

            mc.GTN_SetEcatAxisPT((short)nCore, (short)Axis, (short)Torque);
        }
    }
}