
using CameraDevice;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UserData;

namespace ModuleCapture
{
    /// <summary>
    /// Usb 直连电脑 主要用作机台校准
    /// </summary>
    public class Module_USBDircect : MCameraDeviceBase
    {
        CameraUsbDirect CameUsb = new CameraUsbDirect();
        public override bool LoadDll(string FullPath)
        {
            return true;
        }
        public override bool Capture(int nID, byte[] _FrameBuffer)
        {
            return CameUsb.Capture(_FrameBuffer);
        }

        public override bool Enumerate(int nID, ref int nNum, List<string> strSN)
        {
            return CameUsb.Enumerate(ref nNum, strSN);
        }



        public override int GetHeight(int nID)
        {
            return CameUsb.GetHeight();
        }

        public override int GetWidth(int nID)
        {
            return CameUsb.GetWidth();
        }

        public override bool Init(int nID, string strBoot)
        {
            return CameUsb.Init(strBoot);
        }

        public override bool Play(int nID)
        {
            return CameUsb.Play();
        }

        public override bool SetSN(int nID, string strSN)
        {
            return CameUsb.SetSN(strSN);
        }

        public override bool Stop(int nID)
        {
            return CameUsb.Stop();
        }
        public override int GetBufLenght(int nID)
        {
            return (CameUsb.GetHeight() * CameUsb.GetWidth() * 3);
        }
        public override int GetBayerType(int nID)
        {
            return CameUsb.GetBayerType();
        }
        public override bool BufToByteGray(int nID, byte[] _FrameBuffer, byte[] buf)
        {
            ImageConvert ToImageConverRTV24 = new ImageConvert();
            bool result = ToImageConverRTV24.BGR_To_Y(_FrameBuffer, buf, (UInt32)CameUsb.GetWidth(), (UInt32)CameUsb.GetHeight());
            return result;
        }

        public override bool BufToBmpGray(int nID, byte[] _FrameBuffer, ref Bitmap bitmap)
        {
            byte[] buf = new byte[CameUsb.GetHeight() * CameUsb.GetWidth()];
            if (!BufToByteGray( nID, _FrameBuffer, buf))
            {
                return false;
            }
            bitmap = ImageChangeHelper.Instance.ConvertBinaryToBitmap(buf, CameUsb.GetWidth(), CameUsb.GetHeight());
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
            return true;
        }
    }
}
