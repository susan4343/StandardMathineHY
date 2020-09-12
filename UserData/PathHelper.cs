using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserData
{
    public static class PathHelper
    {
        public static int AA_ID = 0;
        public static int Disp_ID = 0;
        private static string BasePath = "D:\\AA System\\";
        //public static string PlayPath = $"{BasePath}ConfigSensor\\";
        public static string MotorCsvPath
        {
            get
            {
                string path = $"{BasePath}Result\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{DateTime.Now.ToString("yyyy-MM-dd")}_Motor.csv";
                if (!Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                return path;
            }
        }
        public static string TestResultCsvPath
        {
            get
            {
                string path = $"{BasePath}\\Result\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{DateTime.Now.ToString("yyyy-MM-dd")}_Result.csv";
                if (!Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                return path;
            }
        }
        public static string TestTimeCsvPath
        {
            get
            {
                string path = $"{BasePath}\\Result\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{DateTime.Now.ToString("yyyy-MM-dd")}_Time.csv";
                if (!Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                return path;
            }
        }
        public static string ProductCsvPath
        {
            get
            {
                string path = $"{BasePath}Result\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{DateTime.Now.ToString("yyyy-MM-dd")}_Product.csv";
                if (!Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                return path;
            }
        }
        public static string MFCsvPath
        {
            get
            {
                string path = $"{BasePath}Result\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{DateTime.Now.ToString("yyyy-MM-dd")}_MF.csv";
                if (!Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                return path;
            }
        }
        public static string SFRResultCsvPath
        {
            get
            {
                string path = $"{BasePath}Result\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{DateTime.Now.ToString("yyyy-MM-dd")}_SFR.csv";
                if (!Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                return path;
            }
        }
        public static string FindCenterCsvPath
        {
            get
            {
                string path = $"{BasePath}Result\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{DateTime.Now.ToString("yyyy-MM-dd")}_FindCenter.csv";
                if (!Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                return path;
            }
        }
        public static string TFCsvPath
        {
            get
            {
                string path = $"{BasePath}Result\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{DateTime.Now.ToString("yyyy-MM-dd")}_TF.csv";
                if (!Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                return path;
            }
        }
        public static string LogPathAuto
        {
            get
            {
                string path = $"{BasePath}Log\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{DateTime.Now.Hour}_Auto.txt";
                if (!Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                return path;
            }
        }
        public static string LogPathManual
        {
            get
            {
                string path = $"{BasePath}ManualLog\\{DateTime.Now.ToString("yyyy-MM-dd")}_Manual.txt";
                if (!Directory.Exists(Directory.GetParent(path).FullName))
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                return path;
            }
        }
        public static string ImagePathDelete
        {
            get
            {
                string path = $"{BasePath}Image\\";
                return path;
            }
        }
        public static string ImageRunPath
        {
            get
            {
                string path = $"{ImagePathDelete}\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\Run\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImageNGPath
        {

            get
            {
                string path = $"{ImagePathDelete}\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\NG\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImageOKPath
        {

            get
            {
                string path = $"{ImagePathDelete}\\{UserTest.Model}\\{UserTest.Batch}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\OK\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImagePathFindcenter
        {
            get
            {
                string path = $"{ImageRunPath}\\{UserTest.TestResultAB[AA_ID].SerialNumber}\\Findcenter\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImagePathThroughFocus
        {
            get
            {
                string path = $"{ImageRunPath}\\{UserTest.TestResultAB[AA_ID].SerialNumber}\\ThroughFocus\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImagePathSFRResult
        {
            get
            {
                string path = $"{ImageRunPath}\\{UserTest.TestResultAB[AA_ID].SerialNumber}\\ThroughFocusSFR\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImagePathWhiteText
        {
            get
            {
                string path = $"{ImageRunPath}\\{UserTest.TestResultAB[AA_ID].SerialNumber}\\白场\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImagePathCheck
        {
            get
            {
                string path = $"{ImageRunPath}\\{UserTest.TestResultAB[AA_ID].SerialNumber}\\Check\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImagePathChart
        {
            get
            {
                string path = $"{ImageRunPath}\\{UserTest.TestResultAB[AA_ID].SerialNumber}\\Chart\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImagePathProduct
        {
            get
            {
                string path = $"{ImageRunPath}{UserTest.TestResultAB[AA_ID].SerialNumber}\\成品\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImagePathDisp
        {
            get
            {
                string path = $"{ImageRunPath}\\{UserTest.TestResultAB[Disp_ID].SerialNumber}\\点胶\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string ImagePathMFFail
        {
            get
            {
                string path = $"{ImageRunPath}\\{UserTest.TestResultAB[Disp_ID].SerialNumber}\\MFFail\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

    }
}
