using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools;
using MotionIoLib;
using Communicate;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using UserCtrl;
using EpsonRobot;
using BaseDll;
using UserData;

using HalconDotNet;
using System.IO;
using LightControler;
using CameraLib;
using OtherDevice;

using System.Threading.Tasks;

namespace StationDemo
{
    public static class UserConfig
    {
        public static UserRight userRight
        {
            get;
            set;
        }
        public static void CloseHardWork()
        {
            //for (int i = 0; i < 6; i++)
            //    LightControl.GetInstance().CloseLight(i);
            ResetIO();
            AlarmMgr.GetIntance().StopAlarmBeet();
            CameraMgr.GetInstance().Close();
            TcpMgr.GetInstance().CloseAllEth();
            MotionMgr.GetInstace().Close();
            IOMgr.GetInstace().Close();
            if (ParamSetMgr.GetInstance().GetBoolParam("是否选择程控电源"))
            {
                OtherDevices.ckPower.SetVoltage(1, 0);
                OtherDevices.ckPower.SetVoltage(2, 0);
            }
            //  LightControl.GetInstance().Close();
            //  Weighing.GetInstance().Close();

       

        }
        /// <summary>
        /// 报警提示 
        /// </summary>
        /// <param name="i"></param>
        public static void AlarmTips(int i)
        {
            if (i % 2 == 0)
            {
                if (IOMgr.GetInstace().GetOutputDic().ContainsKey("红灯"))
                    IOMgr.GetInstace().WriteIoBit("红灯", true);
                if (IOMgr.GetInstace().GetOutputDic().ContainsKey("蜂鸣"))
                    IOMgr.GetInstace().WriteIoBit("蜂鸣", true);
            }
            else
            {
                if (IOMgr.GetInstace().GetOutputDic().ContainsKey("红灯"))
                    IOMgr.GetInstace().WriteIoBit("红灯", false);
                if (IOMgr.GetInstace().GetOutputDic().ContainsKey("蜂鸣"))
                    IOMgr.GetInstace().WriteIoBit("蜂鸣", false);
            }
        }
        /// <summary>
        /// 报警提示后 处理
        /// </summary>
        public static void AlarmAfterTips()
        {
            try
            {
                if (IOMgr.GetInstace().GetOutputDic().ContainsKey("蜂鸣"))
                {
                    IOMgr.GetInstace().WriteIoBit("蜂鸣", false);
                }
                if (IOMgr.GetInstace().GetOutputDic().ContainsKey("红灯"))
                {
                    IOMgr.GetInstace().WriteIoBit("红灯", true);
                }

            }
            catch (Exception E)
            { }

        }

        public static void AddStation()
        {
            //添加工站
            StationMgr.GetInstance().AddStation(new StationForm(), "AA站", new StationAA(StationMgr.GetInstance().GetStation("AA站")));
            StationMgr.GetInstance().AddStation(new StationForm(), "转盘站", new StationTable(StationMgr.GetInstance().GetStation("转盘站")));
            StationMgr.GetInstance().AddStation(new StationForm(), "点胶站", new StationDisp(StationMgr.GetInstance().GetStation("点胶站")));
            Dictionary<Form, Stationbase> keyValuePairs = StationMgr.GetInstance().GetDicStaion();

        }
        public static void BandStationWithCtrl(Form_Auto formauto)
        {
            UserConfig.bandStationAndVisionCtrl("点胶站", formauto.visionControl1, 1);
        }
        public static SysFunConfig sysFunConfig = null;
        public static void InitSysFun()
        {
         
            if(SysFunConfig.Read() != null)
            {

              
            }
            else
            {
                SysFunConfig.Save();
                SysFunConfig.Config();
                MessageBox.Show("系统功能参数读取失败", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        public static void bandStationAndVisionCtrl(string stationname, VisionControl visionControl, int index = 1)
        {
            Stationbase pb = StationMgr.GetInstance().GetStation(stationname);
            switch (index)
            {

                case 1:
                    pb.VisionControl = visionControl;
                    break;
                case 2:
                    pb.VisionControl2 = visionControl;
                    break;
                case 3:
                    pb.VisionControl3 = visionControl;
                    break;
                case 4:
                    pb.VisionControl4 = visionControl;
                    break;
            }

        }
        public static void InitHandWareWithUI()
        {
           // IOMgr.GetInstace().WriteIoBit("点胶机", false);
            IOMgr.GetInstace().WriteIoBit("相机光源", false);
            IOMgr.GetInstace().WriteIoBit("平行光管升降气缸", false);
            IOMgr.GetInstace().WriteIoBit("OK指示绿灯", false);
            IOMgr.GetInstace().WriteIoBit("NG指示红灯", false);
            IOMgr.GetInstace().WriteIoBit("蜂鸣", false);
            IOMgr.GetInstace().WriteIoBit("红灯", false);
            IOMgr.GetInstace().WriteIoBit("黄灯", false);
            IOMgr.GetInstace().WriteIoBit("绿灯", false);
            IOMgr.GetInstace().WriteIoBit("黄灯", true);
            IOMgr.GetInstace().WriteIoBit("UV固化", false);

        }
        /// <summary>
        /// 增加自动界面 标志位
        /// </summary>
        /// <param name="formauto"></param>
        public static void AddFlag(Form_Auto formauto)
        {

            formauto.AddFlag("可以上下料", false);
            formauto.AddFlag("启动点胶", false);
            formauto.AddFlag("点胶完成", false);
            formauto.AddFlag("抓取启动", false);
            formauto.AddFlag("抓取完成", false);
            formauto.AddFlag("启动AA", false);
            formauto.AddFlag("AA完成", false);
            formauto.AddFlag("点胶工站初始化完成", false);
            formauto.AddFlag("AA工站初始化完成", false);
            formauto.AddFlag("转盘站初始化完成", false);
            formauto.AddFlag("点胶回零完成", false);
            formauto.AddFlag("AA回零完成", false);
            formauto.AddFlag("转盘回零完成", false);
            //  formauto.AddFlag("系统空跑", sys.g_AppMode == AppMode.AirRun);


        }
        public static void FlushDatatoAutoScreen(Form_Auto formauto)
        {


        }

        /// <summary>
        /// 初始其他硬件状态硬件状态
        /// </summary>
        public static void InitHardWare()
        {
            try
            {
                ResetIO();
                AlarmMgr.GetIntance().StopAlarmBeet();
                string err = "";        
            }
            catch (Exception e)
            {

                return;
            }



        }
        /// <summary>
        /// 增加IO处理前安全判断函数 
        /// </summary>
        public static void AddIoSafeOperate()
        {
            IOMgr.GetInstace().m_eventIsSafeWhenOutIo += Safe.IsSafeWhenLeftRightUVOut;
            //    IOMgr.GetInstace().m_eventIsSafeWhenOutIo += Safe.IsSafeYAxisCliyder;
        }


        /// <summary>
        /// 添加运动处理前的安全判断函数
        /// </summary>
        public static void AddAxisSafeOperate()
        {
            // MotionMgr.GetInstace().m_eventIsSafeWhenAxisMove += Safe.IsSafeWhenUnloadXYAxisMoveing;
            MotionMgr.GetInstace().m_eventIsSafeWhenAxisMove += Safe.IsSafeWhenTableAxisMoveHandler;
        }
        public static bool bHaveVissionProcess = false;
        /// <summary>
        /// 初始化产品数据参数
        /// </summary>
        public static void InitProductNum()
        {
            NozzleMgr.GetInstance().Read();
            try
            {
                StationTable stationTable = (StationTable)StationMgr.GetInstance().GetStation("转盘站");

                TableData.GetInstance().Add(stationTable.GetStationPointDic()["A工位AA位"].pointU, 2);
                TableData.GetInstance().Add(stationTable.GetStationPointDic()["A工位夹取位"].pointU, 2);
                TableData.GetInstance().Add(stationTable.GetStationPointDic()["B工位AA位"].pointU, 1);
                TableData.GetInstance().Add(stationTable.GetStationPointDic()["B工位夹取位"].pointU, 1);

                TableData.GetInstance().AddPosStationName(stationTable.GetStationPointDic()["A工位AA位"].pointU, "A_AA");
                TableData.GetInstance().AddPosStationName(stationTable.GetStationPointDic()["A工位夹取位"].pointU, "A_Pick");
                TableData.GetInstance().AddPosStationName(stationTable.GetStationPointDic()["B工位AA位"].pointU, "B_AA");
                TableData.GetInstance().AddPosStationName(stationTable.GetStationPointDic()["B工位夹取位"].pointU, "B_Pick");

                TableData.GetInstance().AddStationResult("A_AA", false);
                TableData.GetInstance().AddStationResult("A_Pick", false);
                TableData.GetInstance().AddStationResult("B_AA", false);
                TableData.GetInstance().AddStationResult("B_Pick", false);
                TableData.GetInstance().AddStationResult("A_UnLoadLoad", false);
                TableData.GetInstance().AddStationResult("B_UnLoadLoad", false);

                TableData.GetInstance().dicTableCmdStart.Add("A_AA", false);
                TableData.GetInstance().dicTableCmdStart.Add("B_AA", false);
                TableData.GetInstance().dicTableCmdStart.Add("A_Pick", false);
                TableData.GetInstance().dicTableCmdStart.Add("B_Pick", false);
                TableData.GetInstance().dicTableCmdStart.Add("A_UnLoadLoad", false);
                TableData.GetInstance().dicTableCmdStart.Add("B_UnLoadLoad", false);


                TableData.GetInstance().AxisNo = stationTable.AxisU;
                TableData.GetInstance().listPoss.Clear();
                TableData.GetInstance().listPoss.Add(stationTable.GetStationPointDic()["A工位夹取位"].pointU);
                TableData.GetInstance().listPoss.Add(stationTable.GetStationPointDic()["A工位AA位"].pointU);
                TableData.GetInstance().listPoss.Add(stationTable.GetStationPointDic()["B工位夹取位"].pointU);
                TableData.GetInstance().listPoss.Add(stationTable.GetStationPointDic()["B工位AA位"].pointU);
            }
            catch (Exception e)
            {

            }

          
            //   
        }
        public static void InitLoadSnapCoor(string strStationName, string posName)
        {
            if (posName == "拍照左上" || posName == "拍照右下")
            {

            }


        }
        public static void InitLoadData(string strStationName, string posName)
        {
            if ("上料站" == strStationName && (posName == "上料左上" || posName == "上料右下"))
            {

            }
        }
        public static void InitCalibPosData(string strStationName, string posName)
        {
            if (strStationName == "右贴装站" && posName == "上下相机标定位")
            {
                //double x = StationMgr.GetInstance().GetStation("右贴装站").GetStationPointDic()["上下相机标定位"].pointX;
                //double y = StationMgr.GetInstance().GetStation("右贴装站").GetStationPointDic()["上下相机标定位"].pointY;
                //VisionAddtion.xyrightCalib.CalibPoint = new XYUPoint(x, y, 0);
            }
            if (strStationName == "左贴装站" && posName == "上下相机标定位")
            {
                //double x = StationMgr.GetInstance().GetStation("左贴装站").GetStationPointDic()["上下相机标定位"].pointX;
                //double y = StationMgr.GetInstance().GetStation("左贴装站").GetStationPointDic()["上下相机标定位"].pointY;
                //VisionAddtion.xyleftCalib.CalibPoint = new XYUPoint(x, y, 0);

            }
        }
        public static void InitUnLoadData(string strStationName, string posName)
        {
            if ("下料站" == strStationName && (posName == "下料左上" || posName == "下料右下"))
            {
                //TrayMgr.GetInstance().trayDataLoadArr[(int)TrayType.unLoad].RowCount = 4;
                //TrayMgr.GetInstance().trayDataLoadArr[(int)TrayType.unLoad].ColCount = 2;
                //TrayMgr.GetInstance().trayDataLoadArr[(int)TrayType.unLoad].PlaceLeftTopcoordinate.X = StationMgr.GetInstance().GetStation("下料站").GetStationPointDic()["下料左上"].pointX;
                //TrayMgr.GetInstance().trayDataLoadArr[(int)TrayType.unLoad].PlaceLeftTopcoordinate.Y = StationMgr.GetInstance().GetStation("下料站").GetStationPointDic()["下料左上"].pointY;
                //TrayMgr.GetInstance().trayDataLoadArr[(int)TrayType.unLoad].PlaceRightBottomcoordinate.X = StationMgr.GetInstance().GetStation("下料站").GetStationPointDic()["下料右下"].pointX;
                //TrayMgr.GetInstance().trayDataLoadArr[(int)TrayType.unLoad].PlaceRightBottomcoordinate.Y = StationMgr.GetInstance().GetStation("下料站").GetStationPointDic()["下料右下"].pointY;
                //TrayMgr.GetInstance().trayDataLoadArr[(int)TrayType.unLoad].Init2();
            }
        }

        public static void InitPBSafeData(string strStationName, string posName)
        {
            try
            {
                if ("左贴装站" == strStationName && (posName == "安全左上" || posName == "安全右下"))
                {
                    //double top, bottom, left, right;
                    //Safe.SafeRegionLeft.rectangle1.Left = StationMgr.GetInstance().GetStation("左贴装站").GetStationPointDic()["安全左上"].pointX;
                    //Safe.SafeRegionLeft.rectangle1.Top = StationMgr.GetInstance().GetStation("左贴装站").GetStationPointDic()["安全左上"].pointY;
                    //Safe.SafeRegionLeft.rectangle1.Right = StationMgr.GetInstance().GetStation("左贴装站").GetStationPointDic()["安全右下"].pointX;
                    //Safe.SafeRegionLeft.rectangle1.Bottom = StationMgr.GetInstance().GetStation("左贴装站").GetStationPointDic()["安全右下"].pointY;

                }
                if ("右贴装站" == strStationName && (posName == "安全左上" || posName == "安全右下"))
                {
                    //double top, bottom, left, right;
                    //Safe.SafeRegionRight.rectangle1.Left = StationMgr.GetInstance().GetStation("左贴装站").GetStationPointDic()["安全左上"].pointX;
                    //Safe.SafeRegionRight.rectangle1.Top = StationMgr.GetInstance().GetStation("左贴装站").GetStationPointDic()["安全左上"].pointY;
                    //Safe.SafeRegionRight.rectangle1.Right = StationMgr.GetInstance().GetStation("左贴装站").GetStationPointDic()["安全右下"].pointX;
                    //Safe.SafeRegionRight.rectangle1.Bottom = StationMgr.GetInstance().GetStation("左贴装站").GetStationPointDic()["安全右下"].pointY;

                }
            }
            catch (Exception e)
            {

            }

        }
        public static void ResetIO()
        {
            IOMgr.GetInstace().WriteIoBit("蜂鸣", false);
        }


    }
}
