﻿using System;
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
using Newtonsoft.Json;

namespace StationDemo
{

    public interface LoadPatten
    {
        bool IsLoadOK(string SoketName, bool bManual);
        void Load(string SoketName, bool bManual);
        void ULoad(string SoketName, bool bManual);
        bool IsSafeWhenURun(string SoketName);
        bool IsOpenSocket(string SoketName);
        bool IsUload(string SoketName);
    }
    public class ManualLoad : LoadPatten
    {
        public bool IsLoadOK(string SoketName, bool bManual)
        {
            string Soket = SoketName;
            if (
                 IOMgr.GetInstace().ReadIoInBit($"{Soket}治具盖上检测") &&
                 IOMgr.GetInstace().ReadIoInBit($"{Soket}治具LENS检测")
              )
                return true;
            else
                return false;
        }

        public bool IsOpenSocket(string SoketName)
        {
            string Soket = SoketName;
            return !IOMgr.GetInstace().ReadIoInBit($"{Soket}治具盖上检测");
        }

        public bool IsSafeWhenURun(string SoketName)
        {
            string Soket = SoketName;
            if (IOMgr.GetInstace().ReadIoInBit($"{Soket}治具盖上检测"))
                return true;
            else
                return false;
        }

        public bool IsUload(string SoketName)
        {
            string Soket = SoketName;
            return !IOMgr.GetInstace().ReadIoInBit($"{Soket}治具盖上检测");
        }

        public void Load(string SoketName, bool bManual)
        {
            return;
        }
        public void ULoad(string SoketName, bool bManual)
        {
            return;
        }
    }

    public class CliyderLoad : LoadPatten
    {
        StationAA stationAAT = (StationAA)StationMgr.GetInstance().GetStation("AA站");
        public bool IsLoadOK(string SoketName, bool bManual)
        {
            string Soket = SoketName;
            if (
                IOMgr.GetInstace().ReadIoInBit($"{Soket}治具夹紧检测") &&
                (
                 sys.g_AppMode == AppMode.Run && IOMgr.GetInstace().ReadIoInBit($"{Soket}治具真空检测") ||
                 sys.g_AppMode == AppMode.AirRun) &&
                 IOMgr.GetInstace().ReadIoInBit($"{Soket}治具LENS检测")
               )
                return true;
            else
                return false;
        }

        public bool IsOpenSocket(string SoketName)
        {
            string Soket = SoketName;
            return IOMgr.GetInstace().ReadIoInBit($"{Soket}治具松开检测");
        }

        public bool IsSafeWhenURun(string SoketName)
        {
            return true;
        }

        public bool IsUload(string SoketName)
        {

            return IOMgr.GetInstace().ReadIoInBit($"{SoketName}治具松开检测");
        }

        public void Load(string SoketName, bool bManual)
        {
            string Soket = SoketName;
            WaranResult waranResult;
            retry_close_checkVac:
            IOMgr.GetInstace().WriteIoBit($"{Soket}真空吸", true);
            waranResult = stationAAT.CheckIobyName($"{Soket}治具真空检测", true, $"{Soket}治具真空检测 失败，请检查 产品是否正常放置，气压", bManual);
            if (waranResult == WaranResult.Retry)
                goto retry_close_checkVac;
            retry_close_cliyder:
            IOMgr.GetInstace().WriteIoBit($"{Soket}工位夹紧", true);
            waranResult = stationAAT.CheckIobyName($"{Soket}治具夹紧检测", true, $"{Soket}治具夹紧检测 失败，请检查气缸及感应器", bManual);
            if (waranResult == WaranResult.Retry)
                goto retry_close_cliyder;

            return;
        }
        public void ULoad(string SoketName, bool bManual)
        {
            string Soket = SoketName;
            retry_open_cliyder:
            IOMgr.GetInstace().WriteIoBit($"{Soket}工位夹紧", false);
            WaranResult waranResult = stationAAT.CheckIobyName($"{Soket}治具松开检测", true, $"{Soket}治具松开检测 失败，请检查气缸及感应器", bManual);
            if (waranResult == WaranResult.Retry)
                goto retry_open_cliyder;
            retry_close_checkVac:
            IOMgr.GetInstace().WriteIoBit($"{Soket}真空吸", false);

        }
    }

    public   class  SysFunParam
    {
        public string LoadPatten= "气缸上料";
        public bool bIsLRUV;
    }
    /// <summary>
    /// 系统功能 （本类中所有成员全是static）
    /// </summary>
    [Serializable]
    public  class SysFunConfig
    {
        [JsonIgnore]
        public static bool IsLRUV
        {
            set
            {
                sysFunParam.bIsLRUV = value;
                ParamSetMgr.GetInstance().SetBoolParam("是否侧向UV", value);
            }
            get
            {
                return sysFunParam.bIsLRUV;
            }
        }
        [JsonIgnore]
        public static LoadPatten LodUnloadPatten = null;


        public   static SysFunParam sysFunParam = new SysFunParam(); 


        public static string LoadPatten
        {
            set
            {
                
                sysFunParam.LoadPatten = value;
            }
            get
            {
                return sysFunParam.LoadPatten;
            }

        }

        public  static  void Save()
        {
          AccessJosnSerializer.ObjectToJson(AppDomain.CurrentDomain.BaseDirectory + "SysFunConfig.json", sysFunParam);
            
        }
    
        public static SysFunParam Read()
        {
            object obj = new object();
            obj = AccessJosnSerializer.JsonToObject(AppDomain.CurrentDomain.BaseDirectory + "SysFunConfig.json", typeof(SysFunParam));
            if(obj!=null)
            {
                sysFunParam = (SysFunParam) obj; Config();
                return sysFunParam;
            }
            return null;
        }
     
        public static void  Config()
        {
            switch (LoadPatten)
            {
                case "气缸上料":
                    LodUnloadPatten = new CliyderLoad();
                    break;
                case "手动上料":
                    LodUnloadPatten = new ManualLoad();
                    break;


            }

            SysFunConfig.IsLRUV = sysFunParam.bIsLRUV;
        }

    }


}