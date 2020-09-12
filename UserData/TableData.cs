using BaseDll;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionIoLib;

namespace UserData
{
    public enum TableStationState
    {

        Wait,//等待到位
        Processing,//处理中
        Processed,//处理完成


    }

    public class TableData
    {
        private TableData()
        {

        }
        private static TableData pTableData = null;
        private static object lockobj = new object();
        public static TableData GetInstance()
        {
            if (pTableData == null)
            {
                lock (lockobj)
                {
                    pTableData = new TableData();
                }

            }
            return pTableData;

        }
        public string TableName = "Table";
        public int NumStaionsBroundTable = 2;
        public int AxisNo = 0;
        TableStationState[] tableStationStates = new TableStationState[20];
        public object locksocketandpos = new object();
        //位置和夹具号
        public Dictionary<double, int> dicTableSocketAndPos = new Dictionary<double, int>();
        public object lockstationandpos = new object();
        //位置和工位名
        public Dictionary<double, string> dicTableStationAndPos = new Dictionary<double, string>();
        public object lockstationandresult = new object();
        //各工位结果
        public Dictionary<string, bool> dicTableRuslut = new Dictionary<string, bool>();
        //各工位启动
        public object lockstationandCmd = new object();
        public Dictionary<string, bool> dicTableCmdStart = new Dictionary<string, bool>();

        public List<double> listPoss = new List<double>();

        /// <summary>
        /// 添加夹具号和对位的位置
        /// </summary>
        /// <param name="currentpos"></param>
        /// <param name="SocketNo"></param>
        public void Add(double currentpos, int SocketNo)
        {
            lock (locksocketandpos)
            {
                dicTableSocketAndPos.Add(currentpos, SocketNo);
            }
        }
        public void Clear()
        {
            lock (locksocketandpos)
            {
                dicTableSocketAndPos.Clear();
            }
        }
        public void AddPosStationName(double currentpos, string SocketName)
        {
            lock (lockstationandpos)
            {
                dicTableStationAndPos.Add(currentpos, SocketName);
            }
        }
        public void AddStationResult(string stationname, bool bResult)
        {
            lock (lockstationandresult)
            {
                dicTableRuslut.Add(stationname, bResult);
            }

        }
        public void SetStationResult(string stationname, bool bResult)
        {
            lock (lockstationandresult)
            {
                dicTableRuslut[stationname] = bResult;
            }
        }
        public void SetAllSationResultFalse()
        {
            List<string> keystr = new List<string>();
            lock (lockstationandresult)
            {
                foreach (var temp in dicTableRuslut)
                    keystr.Add(temp.Key);
                foreach (var s in keystr)
                    dicTableRuslut[s] = false;
            }
        }

        public bool GetAllStationResults()
        {
            bool bresult = true;
            lock (lockstationandresult)
            {
                foreach (var temp in dicTableRuslut)
                    bresult &= temp.Value;
            }
            return bresult;
        }

        public void SetALLStartCmd()
        {
            lock (lockstationandCmd)
            {
                List<string> keystr = new List<string>();
                foreach (var temp in dicTableCmdStart)
                    keystr.Add(temp.Key);
                foreach (var s in keystr)
                    dicTableCmdStart[s] = true;

            }
        }
        public void ResetStartCmd(string strStationName)
        {
            lock (lockstationandCmd)
            {
                dicTableCmdStart[strStationName] = false;
            }
        }
        public bool GetStationStartCmd(string strStationName)
        {
            lock (lockstationandCmd)
            {
                return dicTableCmdStart[strStationName];
            }
        }
        /// <summary>
        /// 获取当工站的夹具号 nCurrentNo当前工站号(站号从1开始）nCurrentNo=工位号，获取的结果是夹具号
        /// </summary>
        /// <param name="nCurrentNo"></param>
        /// <returns></returns>
        public int GetSocketNum(int nCurrentNo, double Fine = 0.1)
        {
            if (MotionMgr.GetInstace().GetHomeFinishFlag(AxisNo) != AxisHomeFinishFlag.Homed)
                return -1;
            lock (locksocketandpos)
            {
                double val = MotionMgr.GetInstace().GetAxisPos(AxisNo);
                foreach (var temp in dicTableSocketAndPos)
                {
                    if (val < temp.Key + Fine && val > temp.Key - Fine)
                    {
                        int No = (temp.Value + nCurrentNo - 1) % NumStaionsBroundTable;
                        if (No == 0)
                            return NumStaionsBroundTable;
                        else
                            return No;
                    }
                }
                return -1;
            }
        }

        public string GetStationName(double Fine = 0.1)
        {
            if (MotionMgr.GetInstace().GetHomeFinishFlag(AxisNo) != AxisHomeFinishFlag.Homed)
                return "None";
            lock (lockstationandpos)
            {
                double val = MotionMgr.GetInstace().GetAxisPos(AxisNo);
                foreach (var temp in dicTableStationAndPos)
                {
                    if (val < temp.Key + Fine && val > temp.Key - Fine)
                    {
                        return temp.Value;
                    }
                }
            }
            return "None";
        }
    }



}