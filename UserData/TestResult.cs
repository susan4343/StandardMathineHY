using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using BaseDll;
namespace UserData
{
    public class UserTest
    {
        public static TestResult[] TestResultAB = new TestResult[] { new TestResult(), new TestResult() };
        public static ProductCheckResult[] ProductCheckResultAB = new ProductCheckResult[] { new ProductCheckResult(), new ProductCheckResult() };
        public static ProductInfo ProductCount = new ProductInfo();
        public static TestTime TestTimeInfo = new TestTime();
        public static FailResult FailResultAB = new FailResult();
        public static CTTest []CTTestAB = new CTTest[] {new CTTest(),new CTTest () };
        public static string algType = "";//算法类型，chart 和coll 电控运动初始化会不一样
        public static bool ContiuneWhlie = true;//用于关闭软件中的while循环
        public static LogHelper RunLog = new LogHelper();
        public static List<MFHelper> mFHelpers = new List<MFHelper>();
        public static List<DispCalibration> [] DispCalibrationAB = new List<DispCalibration>[] { new List<DispCalibration>(), new List<DispCalibration>() };
        public static string Model = "机型1";
        public static string Batch = "批次1";
    }
    public class ProductInfo
    {
        public int CompeteA { get; set; }
        public int OKA { get; set; }
        public int NGA { get; set; }
        public double PencentA { get; set; }

        public int CompeteB { get; set; }
        public int OKB { get; set; }
        public int NGB { get; set; }
        public double PencentB { get; set; }

        public double UPH { get; set; }
        public int PlayFailA { get; set; }
        public int OCFailA { get; set; }
        public int SFRFailA { get; set; }
        public int TiltFailA { get; set; }
        public int OtherFailA { get; set; }
        public int PlayFailB { get; set; }
        public int OCFailB { get; set; }
        public int SFRFailB { get; set; }
        public int TiltFailB { get; set; }
        public int OtherFailB { get; set; }
        [XmlIgnore]
        public double PencentCTAll { get; set; }
        [XmlIgnore]
        public DateTime StarCTTime { get; set; }
        [XmlIgnore]
        public DateTime EndCTTime { get; set; }
        [XmlIgnore]
        public double CountCTAll { get; set; } = 0;
        [XmlIgnore]
        public double CountCTTime { get; set; } = 0;
    }
    public class ProductInfoFile
    {
        public static bool ReadCT(string path)
        {
            UserTest.ProductCount = (ProductInfo)AccessXmlSerializer.XmlToObject(path, typeof(ProductInfo));
            if (UserTest.ProductCount == null)
            {
                UserTest.ProductCount = new ProductInfo();
                SaveCT(path);
                return false;
            }
            return true;
        }
        public static void SaveCT(string path)
        {
            AccessXmlSerializer.ObjectToXml(path, UserTest.ProductCount);
        }
    }
    //csv
    public class TestResult
    {
        #region 私有变量
        private double _testTime = 0;
        private double _find_1_BeforeX = 0;
        private double _find_1_BeforeY = 0;
        private double _find_1_AfterX = 0;
        private double _find_1_AfterY = 0;
        private double _find_2_BeforeX = 0;
        private double _find_2_BeforeY = 0;
        private double _find_2_AfterX = 0;
        private double _find_2_AfterY = 0;
        private double _aA_1_TiltX = 0;
        private double _aA_1_TiltY = 0;
        private double _aA_2_TiltX = 0;
        private double _aA_2_TiltY = 0;
        private double _aA_1_BeforePosZ = 0;
        private double _aA_1_AfterPosZ = 0;
        private double _aA_2_BeforePosZ = 0;
        private double _aA_2_AfterPosZ = 0;
        private double _uVBeforeSFR_CT_Value = 0;
        private double _uVBeforeSFR_UL_Value = 0;
        private double _uVBeforeSFR_UR_Value = 0;
        private double _uVBeforeSFR_DL_Value = 0;
        private double _uVBeforeSFR_DR_Value = 0;
        private double _uVBeforeSFRCenterDel = 0;
        private double _uVBeforeSFRCornerDel = 0;
        private double _uVBeforeOC_X = 0;
        private double _uVBeforeOC_Y = 0;
        private double _uVAfterSFR_CT_Value = 0;
        private double _uVAfterSFR_UL_Value = 0;
        private double _uVAfterSFR_UR_Value = 0;
        private double _uVAfterSFR_DL_Value = 0;
        private double _uVAfterSFR_DR_Value = 0;
        private double _uVAfterSFRCenterDel = 0;
        private double _uVAfterSFRCornerDel = 0;
        private double _uVAfterOC_X = 0;
        private double _uVAfterOC_Y = 0;
        private double _gripOpenSFR_CT_Value = 0;
        private double _gripOpenSFR_UL_Value = 0;
        private double _gripOpenSFR_UR_Value = 0;
        private double _gripOpenSFR_DL_Value = 0;
        private double _gripOpenSFR_DR_Value = 0;
        private double _gripOpenSFRCenterDel = 0;
        private double _gripOpenSFRCornerDel = 0;
        private double _gripOpenOC_X = 0;
        private double _gripOpenOC_Y = 0;
        #endregion
        /// <summary>
        /// 机型
        /// </summary>
        public string Model { get { return UserTest.Model; } } 
        /// <summary>
        /// 批次
        /// </summary>
        public string Batch { get {return  UserTest.Batch; } }
        /// <summary>
        /// SN
        /// </summary>
        public string SerialNumber { get; set; } = $"Time_{DateTime.Now.ToString("yyyyMMdd_HHmmssfff")}";
        public string SocketerNumber { get; set; }
        public string AAModel { get; set; }
        /// <summary>
        /// 测试开始时间
        /// </summary>
        public DateTime StarTime { get; set; }
        /// <summary>
        /// 测试结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 测试时间
        /// </summary>      
        public double TestTime { get { return Math.Round(_testTime, 2); } set { _testTime = value; } }
        /// <summary>
        /// 测试结果
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// AA次数
        /// </summary>
        public int AACount { get; set; }
        /// <summary>
        /// 失败步骤
        /// </summary>
        public string FailStep { get; set; }

        //对心数据
        public double Find_1_BeforeX { get { return Math.Round(_find_1_BeforeX, 5); } set { _find_1_BeforeX = value; } }
        public double Find_1_BeforeY { get { return Math.Round(_find_1_BeforeY, 5); } set { _find_1_BeforeY = value; } }
        public double Find_1_AfterX { get { return Math.Round(_find_1_AfterX, 5); } set { _find_1_AfterX = value; } }
        public double Find_1_AfterY { get { return Math.Round(_find_1_AfterY, 5); } set { _find_1_AfterY = value; } }
        public double Find_2_BeforeX { get { return Math.Round(_find_2_BeforeX, 5); } set { _find_2_BeforeX = value; } }
        public double Find_2_BeforeY { get { return Math.Round(_find_2_BeforeY, 5); } set { _find_2_BeforeY = value; } }
        public double Find_2_AfterX { get { return Math.Round(_find_2_AfterX, 5); } set { _find_2_AfterX = value; } }
        public double Find_2_AfterY { get { return Math.Round(_find_2_AfterY, 5); } set { _find_2_AfterY = value; } }
        //AA数据
        public double AA_1_TiltX { get { return Math.Round(_aA_1_TiltX, 4); } set { _aA_1_TiltX = value; } }
        public double AA_1_TiltY { get { return Math.Round(_aA_1_TiltY, 4); } set { _aA_1_TiltY = value; } }
        public double AA_2_TiltX { get { return Math.Round(_aA_2_TiltX, 4); } set { _aA_2_TiltX = value; } }
        public double AA_2_TiltY { get { return Math.Round(_aA_2_TiltY, 4); } set { _aA_2_TiltY = value; } }
        public double AA_1_BeforePosZ { get { return Math.Round(_aA_1_BeforePosZ, 4); } set { _aA_1_BeforePosZ = value; } }
        public double AA_1_AfterPosZ { get { return Math.Round(_aA_1_AfterPosZ, 4); } set { _aA_1_AfterPosZ = value; } }
        public double AA_2_BeforePosZ { get { return Math.Round(_aA_2_BeforePosZ, 4); } set { _aA_2_BeforePosZ = value; } }
        public double AA_2_AfterPosZ { get { return Math.Round(_aA_2_AfterPosZ, 4); } set { _aA_2_AfterPosZ = value; } }
        //sfr 打印UV前四周加中心
        public double UVBeforeSFR_CT_Value { get { return Math.Round(_uVBeforeSFR_CT_Value, 2); } set { _uVBeforeSFR_CT_Value = value; } }
        public double UVBeforeSFR_UL_Value { get { return Math.Round(_uVBeforeSFR_UL_Value, 2); } set { _uVBeforeSFR_UL_Value = value; } }
        public double UVBeforeSFR_UR_Value { get { return Math.Round(_uVBeforeSFR_UR_Value, 2); } set { _uVBeforeSFR_UR_Value = value; } }
        public double UVBeforeSFR_DL_Value { get { return Math.Round(_uVBeforeSFR_DL_Value, 2); } set { _uVBeforeSFR_DL_Value = value; } }
        public double UVBeforeSFR_DR_Value { get { return Math.Round(_uVBeforeSFR_DR_Value, 2); } set { _uVBeforeSFR_DR_Value = value; } }
        public double UVBeforeSFRCenterDel { get { return Math.Round(_uVBeforeSFRCenterDel, 2); } set { _uVBeforeSFRCenterDel = value; } }
        public double UVBeforeSFRCornerDel { get { return Math.Round(_uVBeforeSFRCornerDel, 2); } set { _uVBeforeSFRCornerDel = value; } }
        public double UVBeforeOC_X { get { return Math.Round(_uVBeforeOC_X, 5); } set { _uVBeforeOC_X = value; } }
        public double UVBeforeOC_Y { get { return Math.Round(_uVBeforeOC_Y, 5); } set { _uVBeforeOC_Y = value; } }
        //sfr 打印UV后四周加中心
        public double UVAfterSFR_CT_Value { get { return Math.Round(_uVAfterSFR_CT_Value, 2); } set { _uVAfterSFR_CT_Value = value; } }
        public double UVAfterSFR_UL_Value { get { return Math.Round(_uVAfterSFR_UL_Value, 2); } set { _uVAfterSFR_UL_Value = value; } }
        public double UVAfterSFR_UR_Value { get { return Math.Round(_uVAfterSFR_UR_Value, 2); } set { _uVAfterSFR_UR_Value = value; } }
        public double UVAfterSFR_DL_Value { get { return Math.Round(_uVAfterSFR_DL_Value, 2); } set { _uVAfterSFR_DL_Value = value; } }
        public double UVAfterSFR_DR_Value { get { return Math.Round(_uVAfterSFR_DR_Value, 2); } set { _uVAfterSFR_DR_Value = value; } }
        public double UVAfterSFRCenterDel { get { return Math.Round(_uVAfterSFRCenterDel, 2); } set { _uVAfterSFRCenterDel = value; } }
        public double UVAfterSFRCornerDel { get { return Math.Round(_uVAfterSFRCornerDel, 2); } set { _uVAfterSFRCornerDel = value; } }
        public double UVAfterOC_X { get { return Math.Round(_uVAfterOC_X, 5); } set { _uVAfterOC_X = value; } }
        public double UVAfterOC_Y { get { return Math.Round(_uVAfterOC_Y, 5); } set { _uVAfterOC_Y = value; } }
        //sfr 打印抓手放开四周加中心
        public double GripOpenSFR_CT_Value { get { return Math.Round(_gripOpenSFR_CT_Value, 2); } set { _gripOpenSFR_CT_Value = value; } }
        public double GripOpenSFR_UL_Value { get { return Math.Round(_gripOpenSFR_UL_Value, 2); } set { _gripOpenSFR_UL_Value = value; } }
        public double GripOpenSFR_UR_Value { get { return Math.Round(_gripOpenSFR_UR_Value, 2); } set { _gripOpenSFR_UR_Value = value; } }
        public double GripOpenSFR_DL_Value { get { return Math.Round(_gripOpenSFR_DL_Value, 2); } set { _gripOpenSFR_DL_Value = value; } }
        public double GripOpenSFR_DR_Value { get { return Math.Round(_gripOpenSFR_DR_Value, 2); } set { _gripOpenSFR_DR_Value = value; } }
        public double GripOpenSFRCenterDel { get { return Math.Round(_gripOpenSFRCenterDel, 2); } set { _gripOpenSFRCenterDel = value; } }
        public double GripOpenSFRCornerDel { get { return Math.Round(_gripOpenSFRCornerDel, 2); } set { _gripOpenSFRCornerDel = value; } }
        public double GripOpenOC_X { get { return Math.Round(_gripOpenOC_X, 5); } set { _gripOpenOC_X = value; } }
        public double GripOpenOC_Y { get { return Math.Round(_gripOpenOC_Y, 5); } set { _gripOpenOC_Y = value; } }

    }
    public class ProductCheckResult
    {
        /// <summary>
        /// SN
        /// </summary>
        public string SerialNumber { get; set; } = "NOSN";
        /// <summary>
        /// 产品
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// 测试开始时间
        /// </summary>
        public DateTime StarTime { get; set; }
        /// <summary>
        /// 测试结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 测试时间
        /// </summary>
        public double TestTime { get; set; }
        /// <summary>
        /// 测试结果
        /// </summary>
        public bool Result { get; set; }

        //sfr 打印UV前四周加中心
        public double CheckSFR_CT_Value { get; set; }
        public double CheckSFR_UL_Value { get; set; }
        public double CheckSFR_UR_Value { get; set; }
        public double CheckSFR_DL_Value { get; set; }
        public double CheckSFR_DR_Value { get; set; }
        public double CheckSFRCenterDel { get; set; }
        public double CheckSFRCornerDel { get; set; }
        public double CheckOC_X { get; set; }
        public double CheckOC_Y { get; set; }
    }
    public class FindCenterResult
    {
        public string Name { get; set; }
        public string DX { get; set; }
        public string DY { get; set; }
    }
    public class TFResult
    {
        public string Z { get; set; }
        public string TiltX { get; set; }
        public string TiltY { get; set; }
    }
    public class TestTime
    {
        public DateTime AAbegin { get; set; }
        public DateTime AAend { get; set; }
        public double AATime { get; set; }
        public double DispTime { get; set; }
        public double Center_1 { get; set; }
        public double Center_2 { get; set; }
        public double Center_3 { get; set; }
        public double UVTime { get; set; }
        public double TF_1 { get; set; }
        public double TF_2 { get; set; }
        public double Tilt_1 { get; set; }
        public double Tilt_2 { get; set; }
        public DateTime Center_1_Begin { get; set; }
        public DateTime Center_2_Begin { get; set; }
        public DateTime Center_3_Begin { get; set; }
        public DateTime UVTime_Begin { get; set; }
        public DateTime TF_1_Begin { get; set; }
        public DateTime TF_2_Begin { get; set; }
        public DateTime Tilt_1_Begin { get; set; }
        public DateTime Tilt_2_Begin { get; set; }
        public DateTime DispBegin { get; set; }
        public DateTime DispEnd { get; set; }
    }
    public class FailResult
    {
        public bool Play { get; set; } = true;
        public bool OC { get; set; } = true;
        public bool SFR { get; set; } = true;
        public bool Tilt { get; set; } = true;

    }
    public class CTTest
    {
        public bool Star { get; set; } = false;
        public bool End { get; set; } = false;
        public bool Show { get; set; } = false;
    }
    public enum CheckType
    {
        UVBefore,
        UVAfter,
        GripOpen,
        Product,
    }
    public class MFHelper
    {
        public double N0_Up_L_R { get; set; }
        public double N1_Right_U_D { get; set; }
        public double N2_Down_R_L { get; set; }
        public double N3_Left_D_W { get; set; }
        public double N4_UR_DL { get; set; }
        public double N5_UL_DR { get; set; }
        public double N6_CT { get; set; }
        public double N7_UL { get; set; }
        public double N8_UR { get; set; }
        public double N9_DR { get; set; }
        public double N10_DL { get; set; }
    }
    public class MFInfoFile
    {
        public static bool Read(string path)
        {
            try
            {
                // UserTest.mFHelpers = (List<MFHelper>)AccessXmlSerializer.XmlToObject(path, typeof(List<MFHelper>));
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<MFHelper>));
                UserTest.mFHelpers = (List<MFHelper>)xmlSerializer.Deserialize(fs);
                fs.Close();

                if (UserTest.mFHelpers == null)
                {
                    UserTest.mFHelpers = new List<MFHelper>();
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
  
        }
        public static void Save(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<MFHelper>));
                xmlSerializer.Serialize(fs, UserTest.mFHelpers);
                fs.Close();

            }
            catch { };
           // AccessXmlSerializer.ObjectToXml(path, UserTest.mFHelpers);
        }
    }





}


