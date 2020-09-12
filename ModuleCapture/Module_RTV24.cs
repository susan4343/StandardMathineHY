
using BaseDll;
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

    public class Module_RTV24 : MCameraDeviceBase
    {
        List<CameraRTV24> CamRTV24 = new List<CameraRTV24>() { new CameraRTV24(), new CameraRTV24(), new CameraRTV24(), new CameraRTV24() };
        public override bool LoadDll(string FullPath)
        {
            return true;
        }
        public override bool Capture(int nID, byte[] _FrameBuffer)
        {
            return CamRTV24[nID].Capture(_FrameBuffer);
        }

        public override bool Enumerate(int nID, ref int nNum, List<string> strSN)
        {
            strSN.Add("0");
            strSN.Add("1");
            strSN.Add("2");
            strSN.Add("3");
            return CamRTV24[nID].Enumerate(ref nNum, strSN);




        }

        public override int GetBayerType(int nID)
        {
            return 0;
        }

        public override int GetHeight(int nID)
        {
            return CamRTV24[nID].GetHeight();
        }

        public override int GetWidth(int nID)
        {
            return CamRTV24[nID].GetWidth();
        }

        public override bool Init(int nID, string strBoot)
        {
            return CamRTV24[nID].Init("");
        }

        public override bool Play(int nID)
        {
            bool result = true;
            result &= CamRTV24[nID].Play();
            if (ParamSetMgr.GetInstance().GetBoolParam("是否关闭曝光"))
            {
                MSerialPort.GetInstance().CloseExposure(nID);
            }
            return result;
        }

        public override bool SetSN(int nID, string strSN)
        {
            return CamRTV24[nID].SetSN(strSN);
        }

        public override bool Stop(int nID)
        {
            return CamRTV24[nID].Stop();
        }
        public override int GetBufLenght(int nID)
        {
            return (CamRTV24[nID].GetHeight() * CamRTV24[nID].GetWidth() * 3);
        }

        public override bool BufToByteGray(int nID, byte[] _FrameBuffer, byte[] buf)
        {
            ImageConvert ToImageConverRTV24 = new ImageConvert();
            bool result = ToImageConverRTV24.BGR_To_Y(_FrameBuffer, buf, (UInt32)CamRTV24[nID].GetWidth(), (UInt32)CamRTV24[nID].GetHeight());
            return result;

        }

        public override bool BufToBmpGray(int nID, byte[] _FrameBuffer, ref Bitmap bitmap)
        {
            byte[] buf = new byte[CamRTV24[nID].GetHeight() * CamRTV24[nID].GetWidth()];
            if (!BufToByteGray(nID, _FrameBuffer, buf))
            {
                return false;
            }
            bitmap = ImageChangeHelper.Instance.ConvertBinaryToBitmap(buf, CamRTV24[nID].GetWidth(), CamRTV24[nID].GetHeight());
            if (bitmap == null)
                return false;
            return true;
        }

        public override bool BufToBmpRGB(int nID, byte[] _FrameBuffer, ref Bitmap bitmap)
        {
            byte[] buf = new byte[CamRTV24[nID].GetHeight() * CamRTV24[nID].GetWidth()];
            if (!BufToByteGray(nID, _FrameBuffer, buf))
            {
                return false;
            }
            bitmap = ImageChangeHelper.Instance.ConvertBinaryToBitmap(buf, CamRTV24[nID].GetWidth(), CamRTV24[nID].GetHeight());
            if (bitmap == null)
                return false;
            return true;
            // return false;

        }
        public override bool WriteI2C(int nID, byte _DevceID, int _i2Cmode, int _wAddr, int _wData)
        {
            return true;
        }
    }
}
