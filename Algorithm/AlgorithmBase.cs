
using CameraDevice;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UserData;

namespace AlgorithmNamespace
{
    public abstract class AlgorithmBase
    {
        public abstract bool LoadConfig(string path);
        public abstract bool Findcenter(Bitmap bmp, ref double dx, ref double dy,ref List<double> Array,bool MF);
        public abstract bool GetSFRValue(Bitmap bmp, ref SFRValue SFRValue, ref RectInfo rectInfo, Rectangle[] rectangles, ref LightValue lightValue, bool TestLight = false);
        public abstract bool GetTiltValue(SFRValue[] SFRValues, ref double dPeakZ, ref double dTx, ref double dTy);
    }



}
