﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CommonTools;
using MotionIoLib;
using log4net;
using BaseDll;
using System.Drawing;
//运动 安全检测函数，安全返回true  不安全返回false
//public delegate bool IsSafeWhenAxisMoveHandler(int nAxisNo);
//public event IsSafeWhenAxisMoveHandler m_eventIsSafeWhenAxisMove = null;

//Io操作 安全检测函数，安全返回true  不安全返回false
///public delegate bool IsSafeWhenOutIoHandler(string ioName, bool bVal);
//public event IsSafeWhenOutIoHandler m_eventIsSafeWhenOutIo;
//public delegate void ChangeMotionIoOrPosHandler(int nAxisNo, bool[] bChangeBitArr, AxisIOState state, AxisPos axisPos);
//public event ChangeMotionIoOrPosHandler m_eventChangeMotionIoOrPos;
//public delegate bool IsSafeWhenAxisMoveHandler(int nAxisNo, double currentpos, double dsstpos, MoveType moveType);
//public event IsSafeWhenAxisMoveHandler m_eventIsSafeWhenAxisMove = null;

namespace StationDemo
{
    public static class Safe
    {
        static ILog _logger = LogManager.GetLogger("Safe");

        public static bool IsSafeWhenTableAxisMoveHandler(int nAxisNo, double currentpos, double dsstpos, MoveType moveType)
         {
            StationAA stationAA = (StationAA)StationMgr.GetInstance().GetStation("AA站");
            StationDisp stationDisp = (StationDisp)StationMgr.GetInstance().GetStation("点胶站");
            double SafeHeightAA = stationAA.GetStationPointDic()["安全位置"].pointZ;
            double SafeHeightDisp = stationDisp.GetStationPointDic()["安全位置"].pointZ;
            //  double SafeHeightDisp = stationDisp.GetStationPointDic()["安全位置"].pointZ;
            if (MotionMgr.GetInstace().GetAxisName(nAxisNo)== "U")
            {
                if (MotionMgr.GetInstace().GetHomeFinishFlag(stationAA.AxisZ) != AxisHomeFinishFlag.Homed || MotionMgr.GetInstace().GetAxisPos(stationAA.AxisZ) < SafeHeightAA-0.5)
                {
                    if( GlobalVariable.g_StationState!= StationState.StationStateRun)
                    {
                        MessageBox.Show("转盘运动前，检查AA站Z轴没有回原点或者低于安全高度", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }
                   
                if (MotionMgr.GetInstace().GetHomeFinishFlag(stationDisp.AxisZ) != AxisHomeFinishFlag.Homed || MotionMgr.GetInstace().GetAxisPos(stationDisp.AxisZ) < SafeHeightDisp - 1)
                {
                    if (GlobalVariable.g_StationState != StationState.StationStateRun)
                    {
                        MessageBox.Show("转盘运动前，检查点胶站Z轴没有回原点或者低于吐胶点高度", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    _logger.Error("转盘运动前，检查点胶站Z轴没有回原点或者低于吐胶点高度");
                    return false;
                }

                if(ParamSetMgr.GetInstance().GetBoolParam("是否侧向UV") &&
                    ( !IOMgr.GetInstace().ReadIoInBit("左侧UV原位") || !IOMgr.GetInstace().ReadIoInBit("左侧UV原位")))
                {
                    if (GlobalVariable.g_StationState != StationState.StationStateRun)
                    {
                        MessageBox.Show("转盘运动前，左右UV是否在原位", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    _logger.Error("转盘运动前，左右UV是否在原位");
                    return false;
                }



            }
            return true;
        }
             
      public static bool IsSafeWhenLeftRightUVOut(string ioName, bool bVal)
        {
            if( ioName == "侧向UV气缸" && bVal)
            {
                int nAxisNo = 8;
                AxisState axisState = MotionMgr.GetInstace().IsAxisNormalStop(nAxisNo);
                if(axisState< AxisState.NormalStop)
                {
                    if (GlobalVariable.g_StationState != StationState.StationStateRun)
                    {
                        MessageBox.Show("侧向UV气缸推出时，转盘在转动，不能推出", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    _logger.Error("侧向UV气缸推出时，转盘在转动，不能推出");
                    return false;
                }
            }
            return true;
        }

        public   static   CrossZDRegion_Z SafeRegionLeft = new CrossZDRegion_Z();
        public   static   CrossZDRegion_Z SafeRegionRight = new CrossZDRegion_Z();
    }

    public class SafeCheck
    {
       // public virtual bool IsSafe( double[] currntPos, double)
    }

    public class UserRect
    {
        public UserRect()
        {

        }
        public UserRect(double x1, double y1, double x2, double y2)
        {
            Left = x1;
            Right = x2;
            Top = y1;
            Bottom = y2;
        }
        public double Top { set; get; }
        public double Left { set; get; }

        public double Bottom { set; get; }
        public double Right { set; get; }
        public bool Contains( double xpos, double ypos)
        {
            bool xcontains = XContains(xpos);//(xpos >= Left && xpos <= Right) || (xpos >= Right && xpos <= Left);
            bool ycontains = YContains(ypos); //(ypos >= Top && ypos <= Bottom) || (ypos >= Bottom && ypos <= Top);
            return xcontains & ycontains;
        }
        public bool XContains( double xpos)
        {
            bool xcontains = Math.Abs(xpos - Left) <= Math.Abs(Left - Right) && Math.Abs(xpos - Right) <= Math.Abs(Left - Right);
            return xcontains;

        }
        public bool YContains(double ypos)
        {
            bool ycontains = Math.Abs(ypos - Bottom) <= Math.Abs(Top - Bottom)  && Math.Abs(ypos - Top) <= Math.Abs(Top - Bottom);
            return ycontains;
        }
    }

    public  class CrossZDRegion_Z
    {
/*
 *       Y
        /|\
         |
         |
         |————》X

    */
        public UserRect rectangle1 = new UserRect();
        public double SafeZ;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strLine">"X"跨越X（y1-》y2）"Y"跨越Y（x1-》x2）</param> 以 x 还是y分界
        /// <param name="currentposX"></param>
        /// <param name="currentposY"></param>
        /// <param name="currentposZ"></param>
        /// <param name="dstposX"></param>
        /// <param name="dstposY"></param>
        /// <param name="dstposZ"></param>
        /// <returns></returns>
        public bool IsSafe( string strLine,double currentposX ,double currentposY,double currentposZ, 
            double dstposX, double dstposY, double dstposZ)
        {
            if (currentposZ > SafeZ && dstposZ > SafeZ)
                return true;
            if (rectangle1.Contains((int)currentposX, (int)currentposY))
                return false;
            switch (strLine.ToUpper())
            {
                case "X":
                    if ((currentposX <= rectangle1.Left && dstposX <= rectangle1.Left )
                        && !rectangle1.XContains(currentposX)
                        && !rectangle1.XContains(dstposX))
                        return true;
                    if ((currentposX <= rectangle1.Right && dstposX <= rectangle1.Right)
                       && !rectangle1.XContains(currentposX)
                       && !rectangle1.XContains(dstposX))
                        return true;
                    else if ((currentposX >= rectangle1.Left && dstposX >= rectangle1.Left) 
                        && !rectangle1.XContains(dstposX)
                        && !rectangle1.XContains(currentposX)
                        )
                        return true;
                    else if ((currentposX >= rectangle1.Right && dstposX >= rectangle1.Right)
                       && !rectangle1.XContains(dstposX)
                       && !rectangle1.XContains(currentposX)
                       )
                        return true;
                    else
                        return false;
                case "Y":
                     if ((currentposY >= rectangle1.Top && dstposY >= rectangle1.Top)
                         && !rectangle1.YContains(dstposY)
                         && !rectangle1.YContains(currentposY)
                        )
                        return true;
                    if ((currentposY <= rectangle1.Top && dstposY <= rectangle1.Top)
                       && !rectangle1.YContains(dstposY)
                       && !rectangle1.YContains(currentposY)
                      )
                        return true;
                    else if ((currentposY >= rectangle1.Bottom && dstposY >= rectangle1.Bottom)
                          && !rectangle1.YContains(dstposY)
                       && !rectangle1.YContains(currentposY)
                        )
                        return true;
                    else if ((currentposY <= rectangle1.Bottom && dstposY <= rectangle1.Bottom)
                                     && !rectangle1.YContains(dstposY)
                                    && !rectangle1.YContains(currentposY)
                         )
                        return true;
                    else
                        return false;
            }
            return true;
        }
       

    }


   
}
