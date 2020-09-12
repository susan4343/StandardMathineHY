using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCapture
{

    public class Module_Pylon : MCameraDeviceBase
    {
        public override bool BufToBmpGray(int nID, byte[] _FrameBuffer, ref Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public override bool BufToBmpRGB(int nID, byte[] _FrameBuffer, ref Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public override bool BufToByteGray(int nID, byte[] _FrameBuffer, byte[] buf)
        {
            throw new NotImplementedException();
        }

        public override bool Capture(int nID, byte[] _FrameBuffer)
        {
            throw new NotImplementedException();
        }

        public override bool Enumerate(int nID, ref int nNum, List<string> strSN)
        {
            throw new NotImplementedException();
        }

        public override int GetBayerType(int nID)
        {
            throw new NotImplementedException();
        }

        public override int GetBufLenght(int nID)
        {
            throw new NotImplementedException();
        }

        public override int GetHeight(int nID)
        {
            throw new NotImplementedException();
        }

        public override int GetWidth(int nID)
        {
            throw new NotImplementedException();
        }

        public override bool Init(int nID, string strBoot)
        {
            throw new NotImplementedException();
        }

        public override bool LoadDll(string FullPath)
        {
            throw new NotImplementedException();
        }

        public override bool Play(int nID)
        {
            throw new NotImplementedException();
        }

        public override bool SetSN(int nID, string strSN)
        {
            throw new NotImplementedException();
        }

        public override bool Stop(int nID)
        {
            throw new NotImplementedException();
        }

        public override bool WriteI2C(int nID, byte _DevceID, int _i2Cmode, int _wAddr, int _wData)
        {
            throw new NotImplementedException();
        }
    }
}
