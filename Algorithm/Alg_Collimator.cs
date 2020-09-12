
using BaseDll;
using CameraDevice;
using ImageAlgorithm;
using ModuleCapture;
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
using UserData;

namespace AlgorithmNamespace
{
    public class Alg_Collimator : AlgorithmBase
    {
        public override bool Findcenter(Bitmap bmp, ref double dx, ref double dy, ref List<double> Array, bool MF)
        {
            if (ParamSetMgr.GetInstance().GetBoolParam("更换对心算法"))
            {
                return false;
            }
            else
            {
                AlignmentInfo info = new AlignmentInfo();
                try
                {
                    if (bmp == null)
                        return false;
                    Bitmap temp = (Bitmap)bmp.Clone();
                    bool bRet = false;
                    int nWidth = temp.Width;
                    int nHeight = temp.Height;
                    byte[] byBuffer = ImageChangeHelper.Instance.Rgb2Gray(bmp);
                    int collimatorNum = ParamSetMgr.GetInstance().GetIntParam("[SFR] CollimatorNum");
                    bool bCheckMode = (collimatorNum == 2) ? true : false;
                    unsafe
                    {
                        fixed (byte* pbuffer = byBuffer)
                        {
                            bRet = ActiveAlignment.GetAlignmentValue_Collimators(pbuffer, nWidth, nHeight, bCheckMode, ref info);
                        }
                    }
                    temp.Dispose();
                    dx = info.dSx;
                    dy = info.dSy;
                    return bRet;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

        }

        public override bool GetSFRValue(Bitmap bmp, ref SFRValue SFRValue, ref RectInfo rectInfo, Rectangle[] rectangles, ref LightValue lightValue, bool TestLight = false)
        {
            SFRInfo info = new SFRInfo();
            //   Rectangle[] rectangles = new Rectangle[13];
            SFR_C_Info sFR_C_Info = new SFR_C_Info();
            SFR_C_Info YData = new SFR_C_Info();
            double frequency = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dSFR_Parameter");
            bool bRet = false;
            try
            {           
                if (bmp == null)
                    return false;
                Bitmap temp = (Bitmap)bmp.Clone();
                int nWidth = temp.Width;
                int nHeight = temp.Height;
                byte[] byBuffer = ImageChangeHelper.Instance.Rgb2Gray(bmp);
                unsafe
                {
                    fixed (byte* byBufferptr = byBuffer)
                    {
                        bRet = ActiveAlignment.CalibrationMode_Collimators(byBufferptr, nWidth, nHeight, ref sFR_C_Info, ref YData);
                    }
                }
                temp.Dispose();
                //sfr 转换
                SFRValue = AlgChangeHelper.Collimator_Info2Value(sFR_C_Info);
                rectInfo = AlgChangeHelper.Collimator_Info2Rect(sFR_C_Info);
            }
            catch { }
            double[] value = new double[5];
            try
            {
                value[0] = YData.block[0].dValue;
                value[1] = YData.block[1].dValue;
                value[2] = YData.block[2].dValue;
                value[3] = YData.block[3].dValue;
                value[4] = YData.block[4].dValue;

            }
            catch (Exception ex)
            {
                value[0] = -1;
                value[1] = -1;
                value[2] = -1;
                value[3] = -1;
                value[4] = -1;
                //return false;
            }
            lightValue.blockValue = value;

            return bRet;
        }

        public override bool GetTiltValue(SFRValue[] SFRValues, ref double dPeakZ, ref double dTx, ref double dTy)
        {
            TiltInfo tiltInfo = new TiltInfo();
            SFR_C_Info[] SFRInfo = new SFR_C_Info[SFRValues.Length];
            SFRInfo = AlgChangeHelper.Collimator_Value2Infos(SFRValues);
            bool bRet = false;
            bRet = ActiveAlignment.GetTiltValue_Collimators(SFRInfo.Length, SFRInfo, ref tiltInfo);
            dPeakZ = tiltInfo.dPeakZ;
            dTx = tiltInfo.dTx;
            dTy = tiltInfo.dTy;
            return bRet;
        }

        public override bool LoadConfig(string path)
        {
            bool bRet = false;
            bRet = ActiveAlignment.SetParam(path);
            return bRet;
        }
    }



}
