
using UserData;
using CameraDevice;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCapture
{

    public class Module_DT_Dynamic : MCameraDeviceBase
    {

        // CamDevice CamDevice = new CamDevice ();
        private static Assembly ass;
        private static Type t;
        private static List<object> o = new List<object>() { new object(), new object(), new object(), new object() };
        private static readonly object lockRun = new object();
        public override bool LoadDll(string FullPath)
        {
            try
            {
                ass = Assembly.LoadFrom(FullPath); //加载DLL
                t = ass.GetType("CliDll.CamDevice");//获得类型
                o[0] = System.Activator.CreateInstance(t);//创建实例
                o[1] = System.Activator.CreateInstance(t);//创建实例
                o[2] = System.Activator.CreateInstance(t);//创建实例
                o[3] = System.Activator.CreateInstance(t);//创建实例
                return true;
            }
            catch
            {
                return false;
            }

        }
        private object Run(int nID, string MethodName, object[] value = null)
        {
            lock (lockRun)
            {
                MethodInfo mi = t.GetMethod(MethodName);//获得方法
                object a = mi.Invoke(o[nID], value);//调用方法
                return a;
            }
        }
        public override bool Capture(int nID, byte[] _FrameBuffer)
        {
            object[] k = new object[1];
            k[0] = _FrameBuffer;
            bool result = (bool)Run(nID, "Capture", k);
            if (result)
            {
                _FrameBuffer = (byte[])k[0];
            }
            return result;
            // return CamDevice.Capture(_FrameBuffer);
        }
        public override bool WriteI2C(int nID, byte _DevceID, int _i2Cmode, int _wAddr, int _wData)
        {
            object[] k = new object[4];
            k[0] = _DevceID;
            k[1] = _i2Cmode;
            k[2] = _wAddr;
            k[3] = _wData;
            bool result = (bool)Run(nID, "WriteI2C", k);

            return result;
        }
        public override bool Enumerate(int nID, ref int nNum, List<string> strSN)
        {
            object[] k = new object[2];
            k[0] = nNum;
            k[1] = strSN;
            bool result = (bool)Run(nID, "Enumerate", k);
            if (result)
            {
                nNum = (int)k[0];
                strSN = (List<string>)k[1];
            }
            return result;
            //     return CamDevice.Enumerate(ref nNum, strSN);
        }

        public override int GetBayerType(int nID)
        {
            int result = (int)Run(nID, "GetBayerType", null);
            return result;
            // return CamDevice.GetBayerType();
        }

        public override int GetHeight(int nID)
        {
            int result = (int)Run(nID, "GetHeight");
            return result;
        }

        public override int GetWidth(int nID)
        {
            int result = (int)Run(nID, "GetWidth");
            return result;
        }

        public override bool Init(int nID, string strBoot)
        {
            object[] k = new object[1];
            k[0] = strBoot;
            bool result = (bool)Run(nID, "Init", k);
            return result;
            //  return CamDevice.Init(strBoot);
        }

        public override bool Play(int nID)
        {

            bool result = (bool)Run(nID, "Play", null);
            return result;
            //  return CamDevice.Play();
        }

        public override bool SetSN(int nID, string strSN)
        {
            object[] k = new object[1];
            k[0] = strSN;
            bool result = (bool)Run(nID, "SetSN", k);
            if (result)
            {
                strSN = (string)k[0];
            }
            return result;
            //    return CamDevice.SetSN(strSN);
        }

        public override bool Stop(int nID)
        {
            bool result = (bool)Run(nID, "Stop", null);
            return result;
            //   return CamDevice.Stop();
        }
        public override int GetBufLenght(int nID)
        {
            return (GetHeight(nID) * GetWidth(nID) * 4 + 1024);
        }
        public override bool BufToByteGray(int nID, byte[] _FrameBuffer, byte[] buf)
        {
            ImageConvert ToImageConverRTV24 = new ImageConvert();
            bool result = ToImageConverRTV24.BGR_To_Y(_FrameBuffer, buf, (UInt32)GetWidth(nID), (UInt32)GetHeight(nID));
            return result;
        }

        public override bool BufToBmpGray(int nID, byte[] _FrameBuffer, ref Bitmap bitmap)
        {
            byte[] buf = new byte[GetHeight(nID) * GetWidth(nID)];
            if (!BufToByteGray(nID, _FrameBuffer, buf))
            {
                return false;
            }
            bitmap = ImageChangeHelper.Instance.ConvertBinaryToBitmap(buf, GetWidth(nID), GetHeight(nID));
            if (bitmap == null)
                return false;
            return true;
        }

        public override bool BufToBmpRGB(int nID, byte[] _FrameBuffer, ref Bitmap bitmap)
        {
            int width = GetWidth(nID);
            int height = GetHeight(nID);
            if (width < 1 || height < 1)
                return false;
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            try
            {
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);   //// 获取图像参数  
                IntPtr iptr = bmpData.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(_FrameBuffer, 0, iptr, width * height * 3);
                bmp.UnlockBits(bmpData);
                bitmap = (Bitmap)bmp.Clone();
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }


    }
}
