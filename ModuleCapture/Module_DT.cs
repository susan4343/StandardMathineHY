
using UserData;
using CameraDevice;
using CliDll;
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

    public class Module_DT : MCameraDeviceBase
    {

        List<CamDevice> CamDeviceList = new List<CamDevice>() { new CamDevice(), new CamDevice(), new CamDevice(), new CamDevice() };
        public override bool LoadDll( string FullPath)
        {
            return true;
        }
        public override bool Capture(int nID, byte[] _FrameBuffer)
        {
            return CamDeviceList[nID].Capture(_FrameBuffer);
        }

        public override bool Enumerate(int nID, ref int nNum, List<string> strSN)
        {
            return CamDeviceList[nID].Enumerate(ref nNum, strSN);
        }

        public override int GetBayerType(int nID)
        {
            return CamDeviceList[nID].GetBayerType();
        }

        public override int GetHeight(int nID)
        {
            return CamDeviceList[nID].GetHeight();
        }

        public override int GetWidth(int nID)
        {
            return CamDeviceList[nID].GetWidth();
        }

        public override bool Init(int nID, string strBoot)
        {
            return CamDeviceList[nID].Init(strBoot);
        }

        public override bool Play(int nID)
        {
            return CamDeviceList[nID].Play();
        }

        public override bool SetSN(int nID, string strSN)
        {
            return CamDeviceList[nID].SetSN(strSN);
        }

        public override bool Stop(int nID)
        {
            return CamDeviceList[nID].Stop();
        }
        public override int GetBufLenght(int nID)
        {
            return (CamDeviceList[nID].GetHeight() * CamDeviceList[nID].GetWidth() * 4 + 1024);
        }

        public override bool BufToByteGray(int nID, byte[] _FrameBuffer, byte[] buf)
        {
            ImageConvert ToImageConverRTV24 = new ImageConvert();
            bool result = ToImageConverRTV24.BGR_To_Y(_FrameBuffer, buf, (UInt32)CamDeviceList[nID].GetWidth(), (UInt32)CamDeviceList[nID].GetHeight());
            return result;
        }

        public override bool BufToBmpGray(int nID, byte[] _FrameBuffer, ref Bitmap bitmap)
        {
            byte[] buf = new byte[CamDeviceList[nID].GetHeight() * CamDeviceList[nID].GetWidth()];
            if (!BufToByteGray(nID, _FrameBuffer, buf))
            {
                return false;
            }
            bitmap = ImageChangeHelper.Instance.ConvertBinaryToBitmap(buf, CamDeviceList[nID].GetWidth(), CamDeviceList[nID].GetHeight());
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

        public override bool WriteI2C(int nID, byte _DevceID, int _i2Cmode, int _wAddr, int _wData)
        {
            return (CamDeviceList[nID].WriteI2C(_DevceID, _i2Cmode, _wAddr, _wData));
        }
    }
}
