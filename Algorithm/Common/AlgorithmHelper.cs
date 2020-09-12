
using CameraDevice;
using UserData;
using HDMISpcae;
using ImageAlgorithm;
using NST_ActiveAlignment;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlgorithmNamespace
{
    public class AlgorithmHelper
    {
        //private static AlgorithmHelper instance = null;
        //private static object o = new object();
        //private AlgorithmHelper()
        //{
        //}
        //public static AlgorithmHelper Instance
        //{
        //    get
        //    {
        //        lock (o)
        //        {
        //            if (instance == null)
        //            {
        //                instance = new AlgorithmHelper();
        //            }
        //            return instance;
        //        }

        //    }
        //}

        ///// <summary>
        ///// 计算SFR值
        ///// </summary>
        ///// <param name="bmp"></param>
        ///// <param name="info"></param>
        ///// <param name="objectInfo"></param>
        ///// <param name="rectangles"></param>
        ///// <returns></returns>
        //public bool AA_GetSFRValue(Bitmap bmp, ref SFRValue SFRValue, ref RectInfo rectInfo, Rectangle[] rectangles)
        //{
        //    SFRInfo info = new SFRInfo();
        //    ObjectInfo objectInfo = new ObjectInfo();
        //    //   Rectangle[] rectangles = new Rectangle[13];

        //    if (bmp == null)
        //        return false;
        //    Bitmap temp = (Bitmap)bmp.Clone();
        //    bool bRet = false;
        //    int nWidth = temp.Width;
        //    int nHeight = temp.Height;
        //    byte[] byBuffer = ImageChangeHelper.Rgb2Gray(bmp);
        //    unsafe
        //    {
        //        fixed (byte* byBufferptr = byBuffer)
        //        {
        //            bRet = ActiveAlignment.GetSFRValue(byBufferptr, nWidth, nHeight, ref info, ref objectInfo, rectangles);
        //        }
        //    }
        //    temp.Dispose();
        //    //sfr 转换
        //    SFRValue = Info2Value(info);
        //    rectInfo = Info2Value(objectInfo);
        //    return bRet;
        //}
       
        ///// <summary>
        ///// 计算对心偏差
        ///// </summary>
        ///// <param name="bmp"></param>
        ///// <param name="info"></param>
        ///// <returns></returns>       
        //public bool AA_Findcenter(Bitmap bmp, ref double dx, ref double dy)
        //{
        //    AlignmentInfo info = new AlignmentInfo();
        //    try
        //    {
        //        if (bmp == null)
        //            return false;
        //        Bitmap temp = (Bitmap)bmp.Clone();
        //        bool bRet = false;
        //        int nWidth = temp.Width;
        //        int nHeight = temp.Height;
        //        byte[] byBuffer = ImageChangeHelper.Instance.Rgb2Gray(bmp);
        //        unsafe
        //        {
        //            fixed (byte* pbuffer = byBuffer)
        //            {
        //                bRet = ActiveAlignment.GetAlignmentValue(pbuffer, nWidth, nHeight, ref info);
        //            }
        //        }
        //        temp.Dispose();
        //        dx = info.dSx;
        //        dy = info.dSy;
        //        return bRet;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        ///// <summary>
        ///// 计算Tilt
        ///// </summary>
        ///// <param name="nSFRStepNum"></param>
        ///// <param name="SFRInfo"></param>
        ///// <param name="tiltInfo"></param>
        ///// <returns></returns>
        //public bool AA_GetTiltValue(SFRValue[] SFRValues, ref double dPeakZ, ref double dTx, ref double dTy)
        //{
        //    TiltInfo tiltInfo = new TiltInfo();
        //    SFRInfo[] SFRInfo = new SFRInfo[SFRValues.Length];
        //    SFRInfo = Value2Infos(SFRValues);
        //    bool bRet = false;
        //    bRet = ActiveAlignment.GetTiltValue(SFRInfo.Length, SFRInfo, ref tiltInfo);
        //    dPeakZ = tiltInfo.dPeakZ;
        //    dTx = tiltInfo.dTx;
        //    dTy = tiltInfo.dTy;
        //    return bRet;
        //}
        ///// <summary>
        ///// 初始化，输入ini文件路径
        ///// </summary>
        ///// <param name="strPath"></param>
        ///// <returns></returns>
        //public bool AOI_SetParam(string strPath)
        //{
        //    bool bRet = false;
        //    bRet = ActiveAlignment.SetParam(strPath);
        //    return bRet;
        //}
    }

}
