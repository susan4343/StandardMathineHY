using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCapture
{

    public abstract class MCameraDeviceBase
    {
        public abstract bool LoadDll(string FullPath);
        public abstract bool Enumerate(int nID, ref int nNum, List<string> strSN);
        public abstract bool Init(int nID, string strBoot);
        public abstract bool Play(int nID);
        public abstract bool Stop(int nID);
        public abstract int GetWidth(int nID);
        public abstract int GetHeight(int nID);
        public abstract bool WriteI2C(int nID, byte _DevceID, int _i2Cmode, int _wAddr, int _wData);
        public abstract bool Capture(int nID, byte[] _FrameBuffer);
        public abstract bool SetSN(int nID, string strSN);
        public abstract int GetBayerType(int nID);
        public abstract int GetBufLenght(int nID);
        public abstract bool BufToByteGray(int nID, byte[] _FrameBuffer, byte[] buf);
        public abstract bool BufToBmpGray(int nID, byte[] _FrameBuffer, ref Bitmap bitmap);
        public abstract bool BufToBmpRGB(int nID, byte[] _FrameBuffer, ref Bitmap bitmap);

  
    }
}
