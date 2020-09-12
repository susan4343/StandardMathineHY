
using BaseDll;
using CameraDevice;
using HalconDotNet;
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
    public class Alg_Sector : AlgorithmBase
    {
        public override bool Findcenter(Bitmap bmp, ref double dx, ref double dy, ref List<double> Array, bool MF)
        {
            if (ParamSetMgr.GetInstance().GetBoolParam("更换对心算法"))
            {
                HObject ho_Image = null, ho_GrayImage = null; HTuple hv_Area0 = new HTuple();
                try
                {
                    ImageChangeHelper.Instance.Bitmap2HObject((Bitmap)bmp.Clone(), ref ho_Image);
                    if (ho_Image == null)
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
                try
                {
                    HObject ho_SelectedRegions0 = null;
                    HTuple hv_CenterROIR = null, hv_CenterROIC = null, hv_CenterMinArea = null;
                    HTuple hv_CenterSFRThreshold = null;
                    HTuple hv_CenterClosing = null, hv_Width = new HTuple(), hv_Height = new HTuple();
                    HTuple hv_Row0 = new HTuple(), hv_Column0 = new HTuple();
                    hv_CenterROIR = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCenterROIW");
                    hv_CenterROIC = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCenterROIH");
                    hv_CenterMinArea = ParamSetMgr.GetInstance().GetDoubleParam("MF中心对心最小面积");
                    hv_CenterSFRThreshold = ParamSetMgr.GetInstance().GetDoubleParam("MF中心对心阈值设置");
                    hv_CenterClosing = ParamSetMgr.GetInstance().GetDoubleParam("MF中心对心膨胀系数");
                    double pixelSize = ParamSetMgr.GetInstance().GetDoubleParam("[Sensor] dPixelSize");
                    HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
                    HOperatorSet.GetImageSize(ho_GrayImage, out hv_Width, out hv_Height);
                    bool result = FindCenterRegions(ho_Image, hv_CenterROIR, hv_CenterROIC, 0, 0, hv_CenterMinArea, hv_CenterSFRThreshold, hv_CenterClosing, ref ho_SelectedRegions0);
                    if (!result)
                        return false;
                    HOperatorSet.AreaCenter(ho_SelectedRegions0, out hv_Area0, out hv_Row0, out hv_Column0);
                    dx = (hv_Column0.D - hv_Width / 2) * pixelSize / 1000.0;
                    dy = (hv_Row0.D - hv_Height / 2) * pixelSize / 1000.0;

                    ////对心
                    //HObject ho_Rectangle0 = null;
                    //HObject ho_ImageReduced0 = null;
                    //HObject ho_Region0 = null;
                    //HObject ho_RegionClosing0 = null;
                    //HObject ho_ConnectedRegion0 = null;
                    //HObject ho_SelectedRegions0 = null;
                    //HObject ho_Circle0 = null;

                    //// Local control variables 

                    //HTuple hv_XField = null, hv_YField = null;
                    //HTuple hv_CenterROIR = null, hv_CenterROIC = null, hv_CenterMinArea = null;
                    //HTuple hv_CenterSFRThreshold = null;
                    //HTuple hv_CenterClosing = null;
                    //HTuple hv_Width = new HTuple();
                    //HTuple hv_Height = new HTuple(), hv_del = new HTuple();
                    //HTuple hv_delX = new HTuple(), hv_delY = new HTuple();
                    //HTuple hv_Area0 = new HTuple(), hv_Row0 = new HTuple(), hv_Row4 = new HTuple(), hv_Column0 = new HTuple();
                    //// Initialize local and output iconic variables 
                    //HOperatorSet.GenEmptyObj(out ho_GrayImage);
                    //HOperatorSet.GenEmptyObj(out ho_Rectangle0);
                    //HOperatorSet.GenEmptyObj(out ho_ImageReduced0);
                    //HOperatorSet.GenEmptyObj(out ho_Region0);
                    //HOperatorSet.GenEmptyObj(out ho_RegionClosing0);
                    //HOperatorSet.GenEmptyObj(out ho_ConnectedRegion0);
                    //HOperatorSet.GenEmptyObj(out ho_SelectedRegions0);
                    //HOperatorSet.GenEmptyObj(out ho_Circle0);

                    //hv_CenterROIR = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCenterROIW");
                    //hv_CenterROIC = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCenterROIH");
                    //hv_CenterMinArea = ParamSetMgr.GetInstance().GetDoubleParam("MF中心对心最小面积");
                    //hv_CenterSFRThreshold = ParamSetMgr.GetInstance().GetDoubleParam("MF中心对心阈值设置");
                    //hv_CenterClosing = ParamSetMgr.GetInstance().GetDoubleParam("MF中心对心膨胀系数");
                    //double pixelSize = ParamSetMgr.GetInstance().GetDoubleParam("[Sensor] dPixelSize");
                    //ho_GrayImage.Dispose();
                    //HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
                    //HOperatorSet.GetImageSize(ho_GrayImage, out hv_Width, out hv_Height);
                    //ho_Rectangle0.Dispose();
                    //HOperatorSet.GenRectangle2(out ho_Rectangle0, hv_Height / 2, hv_Width / 2, 0, hv_CenterROIR, hv_CenterROIC);
                    //ho_ImageReduced0.Dispose();
                    //HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle0, out ho_ImageReduced0);
                    //ho_Region0.Dispose();
                    //HOperatorSet.Threshold(ho_ImageReduced0, out ho_Region0, hv_CenterSFRThreshold, 255);
                    //ho_RegionClosing0.Dispose();
                    //HOperatorSet.ClosingCircle(ho_Region0, out ho_RegionClosing0, hv_CenterClosing);
                    //ho_ConnectedRegion0.Dispose();
                    //HOperatorSet.Connection(ho_RegionClosing0, out ho_ConnectedRegion0);
                    //HOperatorSet.AreaCenter(ho_ConnectedRegion0, out hv_Area0, out hv_Row0, out hv_Column0);
                    //ho_SelectedRegions0.Dispose();
                    //HOperatorSet.SelectShape(ho_ConnectedRegion0, out ho_SelectedRegions0, "area", "and", hv_Area0.TupleMax(), 9999999);
                    //HOperatorSet.AreaCenter(ho_SelectedRegions0, out hv_Area0, out hv_Row0, out hv_Column0);
                    //dx = (hv_Column0.D - hv_Width / 2) * pixelSize / 1000.0;
                    //dy = (hv_Row0.D - hv_Height / 2) * pixelSize / 1000.0;
                }
                catch (Exception e)
                {
                    return false;
                }
                if (!MF)
                {
                    return true;//不选择计算MF ，直接返回
                }
                try
                {
                    HObject ho_SelectedRegions1 = null;
                    HObject ho_SelectedRegions2 = null;
                    HObject ho_SelectedRegions3 = null;
                    HObject ho_SelectedRegions4 = null;
                    HTuple hv_XField = null, hv_YField = null;
                    HTuple hv_Corner1ROIR = null, hv_Corner1ROIC = null, hv_Corner1MinArea = null, hv_Corner1SFRThreshold = null, hv_Corner1Closing = null;
                    HTuple hv_Area1 = new HTuple(), hv_Area2 = new HTuple(), hv_Area3 = new HTuple(), hv_Area4 = new HTuple();
                    HTuple hv_Row1 = new HTuple(), hv_Row2 = new HTuple(), hv_Row3 = new HTuple(), hv_Row4 = new HTuple();
                    HTuple hv_Column1 = new HTuple(), hv_Column2 = new HTuple(), hv_Column3 = new HTuple(), hv_Column4 = new HTuple();
                    hv_XField = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCorner1XField");
                    hv_YField = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCorner1YField");
                    hv_Corner1ROIR = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCorner1ROIW");
                    hv_Corner1ROIC = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCorner1ROIH");
                    hv_Corner1MinArea = ParamSetMgr.GetInstance().GetDoubleParam("MF_Corner1最小面积");
                    hv_Corner1SFRThreshold = ParamSetMgr.GetInstance().GetDoubleParam("MF_Corner1阈值设置");
                    hv_Corner1Closing = ParamSetMgr.GetInstance().GetDoubleParam("MF_Corner1膨胀系数");

                    bool result = true;
                    result &= FindCenterRegions(ho_Image, hv_Corner1ROIR, hv_Corner1ROIC, -hv_XField, -hv_YField, hv_Corner1MinArea, hv_Corner1SFRThreshold, hv_Corner1Closing, ref ho_SelectedRegions1);
                    result &= FindCenterRegions(ho_Image, hv_Corner1ROIR, hv_Corner1ROIC, hv_XField, -hv_YField, hv_Corner1MinArea, hv_Corner1SFRThreshold, hv_Corner1Closing, ref ho_SelectedRegions2);
                    result &= FindCenterRegions(ho_Image, hv_Corner1ROIR, hv_Corner1ROIC, -hv_XField, hv_YField, hv_Corner1MinArea, hv_Corner1SFRThreshold, hv_Corner1Closing, ref ho_SelectedRegions3);
                    result &= FindCenterRegions(ho_Image, hv_Corner1ROIR, hv_Corner1ROIC, hv_XField, hv_YField, hv_Corner1MinArea, hv_Corner1SFRThreshold, hv_Corner1Closing, ref ho_SelectedRegions4);
                    if (!result)
                        return false;
                    HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_Area1, out hv_Row1, out hv_Column1);
                    HOperatorSet.AreaCenter(ho_SelectedRegions2, out hv_Area2, out hv_Row2, out hv_Column2);
                    HOperatorSet.AreaCenter(ho_SelectedRegions3, out hv_Area3, out hv_Row3, out hv_Column3);
                    HOperatorSet.AreaCenter(ho_SelectedRegions4, out hv_Area4, out hv_Row4, out hv_Column4);

                    Array.Add(Math.Sqrt((hv_Row2.D - hv_Row1.D) * (hv_Row2.D - hv_Row1.D) + (hv_Column2.D - hv_Column1.D) * (hv_Column2.D - hv_Column1.D)));
                    Array.Add(Math.Sqrt((hv_Row4.D - hv_Row2.D) * (hv_Row4.D - hv_Row2.D) + (hv_Column4.D - hv_Column2.D) * (hv_Column4.D - hv_Column2.D)));
                    Array.Add(Math.Sqrt((hv_Row3.D - hv_Row4.D) * (hv_Row3.D - hv_Row4.D) + (hv_Column3.D - hv_Column4.D) * (hv_Column3.D - hv_Column4.D)));
                    Array.Add(Math.Sqrt((hv_Row1.D - hv_Row3.D) * (hv_Row1.D - hv_Row3.D) + (hv_Column1.D - hv_Column3.D) * (hv_Column1.D - hv_Column3.D)));
                    Array.Add(Math.Sqrt((hv_Row4.D - hv_Row1.D) * (hv_Row4.D - hv_Row1.D) + (hv_Column4.D - hv_Column1.D) * (hv_Column4.D - hv_Column1.D)));
                    Array.Add(Math.Sqrt((hv_Row2.D - hv_Row3.D) * (hv_Row2.D - hv_Row3.D) + (hv_Column2.D - hv_Column3.D) * (hv_Column2.D - hv_Column3.D)));
                    Array.Add(hv_Area0.D / 100);
                    Array.Add(hv_Area1.D / 100);
                    Array.Add(hv_Area2.D / 100);
                    Array.Add(hv_Area3.D / 100);
                    Array.Add(hv_Area4.D / 100);


                    ////对心
                    //HObject ho_Rectangle1 = null, ho_Rectangle2 = null, ho_Rectangle3 = null, ho_Rectangle4 = null;
                    //HObject ho_ImageReduced1 = null, ho_ImageReduced2 = null, ho_ImageReduced3 = null, ho_ImageReduced4 = null;
                    //HObject ho_Region1 = null, ho_Region2 = null, ho_Region3 = null, ho_Region4 = null;
                    //HObject ho_RegionClosing1 = null, ho_RegionClosing2 = null, ho_RegionClosing3 = null, ho_RegionClosing4 = null;
                    //HObject ho_ConnectedRegion1 = null, ho_ConnectedRegion2 = null, ho_ConnectedRegion3 = null, ho_ConnectedRegion4 = null;
                    //HObject ho_SelectedRegions1 = null, ho_SelectedRegions2 = null, ho_SelectedRegions3 = null, ho_SelectedRegions4 = null;
                    //HObject ho_Circle1 = null, ho_Circle2 = null, ho_Circle3 = null, ho_Circle4 = null;

                    //// Local control variables 

                    //HTuple hv_XField = null, hv_YField = null;
                    //HTuple hv_Corner1ROIR = null;
                    //HTuple hv_Corner1ROIC = null, hv_Corner1MinArea = null;
                    //HTuple hv_Corner1SFRThreshold = null;
                    //HTuple hv_Row0 = new HTuple(), hv_Column0 = new HTuple();
                    //HTuple hv_Corner1Closing = null, hv_Width = new HTuple();
                    //HTuple hv_Height = new HTuple(), hv_del = new HTuple();
                    //HTuple hv_delX = new HTuple(), hv_delY = new HTuple();
                    //HTuple hv_Area1 = new HTuple(), hv_Area2 = new HTuple(), hv_Area3 = new HTuple(), hv_Area4 = new HTuple();
                    //HTuple hv_Row1 = new HTuple(), hv_Row2 = new HTuple(), hv_Row3 = new HTuple(), hv_Row4 = new HTuple();
                    //HTuple hv_Column1 = new HTuple(), hv_Column2 = new HTuple(), hv_Column3 = new HTuple(), hv_Column4 = new HTuple();
                    //// Initialize local and output iconic variables 
                    //HOperatorSet.GenEmptyObj(out ho_GrayImage);
                    //HOperatorSet.GenEmptyObj(out ho_Rectangle1);
                    //HOperatorSet.GenEmptyObj(out ho_Rectangle2);
                    //HOperatorSet.GenEmptyObj(out ho_Rectangle3);
                    //HOperatorSet.GenEmptyObj(out ho_Rectangle4);
                    //HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
                    //HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
                    //HOperatorSet.GenEmptyObj(out ho_ImageReduced3);
                    //HOperatorSet.GenEmptyObj(out ho_ImageReduced4);
                    //HOperatorSet.GenEmptyObj(out ho_Region1);
                    //HOperatorSet.GenEmptyObj(out ho_Region2);
                    //HOperatorSet.GenEmptyObj(out ho_Region3);
                    //HOperatorSet.GenEmptyObj(out ho_Region4);
                    //HOperatorSet.GenEmptyObj(out ho_RegionClosing1);
                    //HOperatorSet.GenEmptyObj(out ho_RegionClosing2);
                    //HOperatorSet.GenEmptyObj(out ho_RegionClosing3);
                    //HOperatorSet.GenEmptyObj(out ho_RegionClosing4);
                    //HOperatorSet.GenEmptyObj(out ho_ConnectedRegion1);
                    //HOperatorSet.GenEmptyObj(out ho_ConnectedRegion2);
                    //HOperatorSet.GenEmptyObj(out ho_ConnectedRegion3);
                    //HOperatorSet.GenEmptyObj(out ho_ConnectedRegion4);
                    //HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
                    //HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
                    //HOperatorSet.GenEmptyObj(out ho_SelectedRegions3);
                    //HOperatorSet.GenEmptyObj(out ho_SelectedRegions4);
                    //HOperatorSet.GenEmptyObj(out ho_Circle1);
                    //HOperatorSet.GenEmptyObj(out ho_Circle2);
                    //HOperatorSet.GenEmptyObj(out ho_Circle3);
                    //HOperatorSet.GenEmptyObj(out ho_Circle4);
                    //hv_XField = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCorner1XField");
                    //hv_YField = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCorner1YField");
                    //hv_Corner1ROIR = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCorner1ROIW");
                    //hv_Corner1ROIC = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCorner1ROIH");
                    //hv_Corner1MinArea = ParamSetMgr.GetInstance().GetDoubleParam("MF_Corner1最小面积");
                    //hv_Corner1SFRThreshold = ParamSetMgr.GetInstance().GetDoubleParam("MF_Corner1阈值设置");
                    //hv_Corner1Closing = ParamSetMgr.GetInstance().GetDoubleParam("MF_Corner1膨胀系数");

                    //ho_GrayImage.Dispose();
                    //HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
                    ////初步对心

                    //HOperatorSet.GetImageSize(ho_GrayImage, out hv_Width, out hv_Height);
                    //hv_delX = hv_Width * hv_XField;
                    //hv_delY = hv_Height * hv_YField;

                    //ho_Rectangle1.Dispose();
                    //HOperatorSet.GenRectangle2(out ho_Rectangle1, (hv_Height / 2) - (hv_delY / 2), (hv_Width / 2) - (hv_delX / 2), 0, hv_Corner1ROIR, hv_Corner1ROIC);
                    //ho_Rectangle2.Dispose();
                    //HOperatorSet.GenRectangle2(out ho_Rectangle2, (hv_Height / 2) - (hv_delY / 2), (hv_Width / 2) + (hv_delX / 2), 0, hv_Corner1ROIR, hv_Corner1ROIC);
                    //ho_Rectangle3.Dispose();
                    //HOperatorSet.GenRectangle2(out ho_Rectangle3, (hv_Height / 2) + (hv_delY / 2), (hv_Width / 2) - (hv_delX / 2), 0, hv_Corner1ROIR, hv_Corner1ROIC);
                    //ho_Rectangle4.Dispose();
                    //HOperatorSet.GenRectangle2(out ho_Rectangle4, (hv_Height / 2) + (hv_delY / 2), (hv_Width / 2) + (hv_delX / 2), 0, hv_Corner1ROIR, hv_Corner1ROIC);

                    //ho_ImageReduced1.Dispose();
                    //HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle1, out ho_ImageReduced1);
                    //ho_ImageReduced2.Dispose();
                    //HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle2, out ho_ImageReduced2);
                    //ho_ImageReduced3.Dispose();
                    //HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle3, out ho_ImageReduced3);
                    //ho_ImageReduced4.Dispose();
                    //HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle4, out ho_ImageReduced4);
                    //ho_Region1.Dispose();
                    //HOperatorSet.Threshold(ho_ImageReduced1, out ho_Region1, hv_Corner1SFRThreshold, 255);
                    //ho_Region2.Dispose();
                    //HOperatorSet.Threshold(ho_ImageReduced2, out ho_Region2, hv_Corner1SFRThreshold, 255);
                    //ho_Region3.Dispose();
                    //HOperatorSet.Threshold(ho_ImageReduced3, out ho_Region3, hv_Corner1SFRThreshold, 255);
                    //ho_Region4.Dispose();
                    //HOperatorSet.Threshold(ho_ImageReduced4, out ho_Region4, hv_Corner1SFRThreshold, 255);
                    //ho_RegionClosing1.Dispose();
                    //HOperatorSet.ClosingCircle(ho_Region1, out ho_RegionClosing1, hv_Corner1Closing);
                    //ho_RegionClosing2.Dispose();
                    //HOperatorSet.ClosingCircle(ho_Region2, out ho_RegionClosing2, hv_Corner1Closing);
                    //ho_RegionClosing3.Dispose();
                    //HOperatorSet.ClosingCircle(ho_Region3, out ho_RegionClosing3, hv_Corner1Closing);
                    //ho_RegionClosing4.Dispose();
                    //HOperatorSet.ClosingCircle(ho_Region4, out ho_RegionClosing4, hv_Corner1Closing);
                    //ho_ConnectedRegion1.Dispose();
                    //HOperatorSet.Connection(ho_RegionClosing1, out ho_ConnectedRegion1);
                    //ho_ConnectedRegion2.Dispose();
                    //HOperatorSet.Connection(ho_RegionClosing2, out ho_ConnectedRegion2);
                    //ho_ConnectedRegion3.Dispose();
                    //HOperatorSet.Connection(ho_RegionClosing3, out ho_ConnectedRegion3);
                    //ho_ConnectedRegion4.Dispose();
                    //HOperatorSet.Connection(ho_RegionClosing4, out ho_ConnectedRegion4);
                    //HOperatorSet.AreaCenter(ho_ConnectedRegion1, out hv_Area1, out hv_Row0, out hv_Column0);
                    //HOperatorSet.AreaCenter(ho_ConnectedRegion2, out hv_Area2, out hv_Row0, out hv_Column0);
                    //HOperatorSet.AreaCenter(ho_ConnectedRegion3, out hv_Area3, out hv_Row0, out hv_Column0);
                    //HOperatorSet.AreaCenter(ho_ConnectedRegion4, out hv_Area4, out hv_Row0, out hv_Column0);
                    //ho_SelectedRegions1.Dispose();
                    //HOperatorSet.SelectShape(ho_ConnectedRegion1, out ho_SelectedRegions1, "area", "and", hv_Area1.TupleMax(), 9999999);
                    //ho_SelectedRegions2.Dispose();
                    //HOperatorSet.SelectShape(ho_ConnectedRegion2, out ho_SelectedRegions2, "area", "and", hv_Area2.TupleMax(), 9999999);
                    //ho_SelectedRegions3.Dispose();
                    //HOperatorSet.SelectShape(ho_ConnectedRegion3, out ho_SelectedRegions3, "area", "and", hv_Area3.TupleMax(), 9999999);
                    //ho_SelectedRegions4.Dispose();
                    //HOperatorSet.SelectShape(ho_ConnectedRegion4, out ho_SelectedRegions4, "area", "and", hv_Area4.TupleMax(), 9999999);

                }
                catch (Exception e)
                {
                    return false;
                }
                return true;
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
                    unsafe
                    {
                        fixed (byte* pbuffer = byBuffer)
                        {
                            bRet = ActiveAlignment.GetAlignmentValue_Sector(pbuffer, nWidth, nHeight, ref info);
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
            ObjectInfo objectInfo = new ObjectInfo();
            //   Rectangle[] rectangles = new Rectangle[13];

            if (bmp == null)
                return false;
            Bitmap temp = (Bitmap)bmp.Clone();
            bool bRet = false;
            int nWidth = temp.Width;
            int nHeight = temp.Height;
            byte[] byBuffer = ImageChangeHelper.Instance.Rgb2Gray(bmp);
            unsafe
            {
                fixed (byte* byBufferptr = byBuffer)
                {
                    bRet = ActiveAlignment.GetSFRValue_Sector(byBufferptr, nWidth, nHeight, ref info, ref objectInfo, rectangles);
                }
            }
            temp.Dispose();
            //sfr 转换
            if (TestLight)
            {
                double[] value = new double[5];
                HObject ho_Image = null, ho_GrayImage = null;
                try
                {
                    HObject ho_SelectedRegions0 = null;
                    HObject ho_SelectedRegions1 = null;
                    HObject ho_SelectedRegions2 = null;
                    HObject ho_SelectedRegions3 = null;
                    HObject ho_SelectedRegions4 = null;
                    HTuple hv_XField = null, hv_YField = null;
                    HTuple hv_CenterROIR = null, hv_CenterROIC = null, hv_CenterMinArea = null, hv_CenterSFRThreshold = null, hv_CenterClosing = null;
                    HTuple hv_Corner1ROIR = null, hv_Corner1ROIC = null, hv_Corner1MinArea = null, hv_Corner1SFRThreshold = null, hv_Corner1Closing = null;
                    HTuple CenterMean = null, CenterDeviation = null;
                    HTuple Corner1Mean = null, Corner1Deviation = null;
                    HTuple Corner2Mean = null, Corner2Deviation = null;
                    HTuple Corner3Mean = null, Corner3Deviation = null;
                    HTuple Corner4Mean = null, Corner4Deviation = null;
                    hv_CenterROIR = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCenterROIW");
                    hv_CenterROIC = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCenterROIH");
                    hv_CenterMinArea = ParamSetMgr.GetInstance().GetDoubleParam("MF中心对心最小面积");
                    hv_CenterSFRThreshold = ParamSetMgr.GetInstance().GetDoubleParam("MF中心对心阈值设置");
                    hv_CenterClosing = ParamSetMgr.GetInstance().GetDoubleParam("MF中心对心膨胀系数");
                    hv_XField = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCorner1XField");
                    hv_YField = ParamSetMgr.GetInstance().GetDoubleParam("[SFR] dCorner1YField");
                    hv_Corner1ROIR = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCorner1ROIW");
                    hv_Corner1ROIC = ParamSetMgr.GetInstance().GetIntParam("[SFR] nCorner1ROIH");
                    hv_Corner1MinArea = ParamSetMgr.GetInstance().GetDoubleParam("MF_Corner1最小面积");
                    hv_Corner1SFRThreshold = ParamSetMgr.GetInstance().GetDoubleParam("MF_Corner1阈值设置");
                    hv_Corner1Closing = ParamSetMgr.GetInstance().GetDoubleParam("MF_Corner1膨胀系数");
                    ImageChangeHelper.Instance.Bitmap2HObject((Bitmap)bmp.Clone(), ref ho_Image);
                    FindCenterRegions(ho_Image, hv_CenterROIR, hv_CenterROIC, 0, 0, hv_CenterMinArea, hv_CenterSFRThreshold, hv_CenterClosing, ref ho_SelectedRegions0);
                    FindCenterRegions(ho_Image, hv_Corner1ROIR, hv_Corner1ROIC, -hv_XField, -hv_YField, hv_Corner1MinArea, hv_Corner1SFRThreshold, hv_Corner1Closing, ref ho_SelectedRegions1);
                    FindCenterRegions(ho_Image, hv_Corner1ROIR, hv_Corner1ROIC, hv_XField, -hv_YField, hv_Corner1MinArea, hv_Corner1SFRThreshold, hv_Corner1Closing, ref ho_SelectedRegions2);
                    FindCenterRegions(ho_Image, hv_Corner1ROIR, hv_Corner1ROIC, -hv_XField, hv_YField, hv_Corner1MinArea, hv_Corner1SFRThreshold, hv_Corner1Closing, ref ho_SelectedRegions3);
                    FindCenterRegions(ho_Image, hv_Corner1ROIR, hv_Corner1ROIC, hv_XField, hv_YField, hv_Corner1MinArea, hv_Corner1SFRThreshold, hv_Corner1Closing, ref ho_SelectedRegions4);
                    HOperatorSet.Intensity(ho_SelectedRegions0, ho_Image, out CenterMean, out CenterDeviation);
                    HOperatorSet.Intensity(ho_SelectedRegions1, ho_Image, out Corner1Mean, out Corner1Deviation);
                    HOperatorSet.Intensity(ho_SelectedRegions2, ho_Image, out Corner2Mean, out Corner2Deviation);
                    HOperatorSet.Intensity(ho_SelectedRegions3, ho_Image, out Corner3Mean, out Corner3Deviation);
                    HOperatorSet.Intensity(ho_SelectedRegions4, ho_Image, out Corner4Mean, out Corner4Deviation);

                    value[0] = CenterMean.D;
                    value[1] = Corner1Mean.D;
                    value[2] = Corner2Mean.D;
                    value[3] = Corner3Mean.D;
                    value[4] = Corner4Mean.D;

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
            }
            SFRValue = AlgChangeHelper.Sector_Info2Value(info);
            rectInfo = AlgChangeHelper.Sector_Info2Value(objectInfo);
            return bRet;
        }

        public override bool GetTiltValue(SFRValue[] SFRValues, ref double dPeakZ, ref double dTx, ref double dTy)
        {
            TiltInfo tiltInfo = new TiltInfo();
            SFRInfo[] SFRInfo = new SFRInfo[SFRValues.Length];
            SFRInfo = AlgChangeHelper.Sector_Value2Infos(SFRValues);
            bool bRet = false;
            bRet = ActiveAlignment.GetTiltValue(SFRInfo.Length, SFRInfo, ref tiltInfo);
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
        private static object obj = new object();
        private bool FindCenterRegions(HObject ho_Image, HTuple hv_ROIR, HTuple hv_ROIC, HTuple hv_XField, HTuple hv_YField, HTuple hv_MinArea, HTuple hv_SFRThreshold, HTuple hv_Closing, ref HObject ho_SelectedRegions)
        {
            lock (obj)
            {
                //对心
                try
                {
                    HObject ho_Rectangle = null;
                    HObject ho_ImageReduced = null;
                    HObject ho_Region = null;
                    HObject ho_RegionClosing = null;
                    HObject ho_ConnectedRegion = null;
                    HObject ho_GrayImage = null;
                    // Local control variables 
                    HTuple hv_Area = new HTuple();
                    HTuple hv_Row0 = new HTuple(), hv_Column0 = new HTuple();
                    HTuple hv_Width = new HTuple();
                    HTuple hv_Height = new HTuple(), hv_del = new HTuple();
                    HTuple hv_delX = new HTuple(), hv_delY = new HTuple();
                    HOperatorSet.GenEmptyObj(out ho_Rectangle);
                    HOperatorSet.GenEmptyObj(out ho_ImageReduced);
                    HOperatorSet.GenEmptyObj(out ho_Region);
                    HOperatorSet.GenEmptyObj(out ho_RegionClosing);
                    HOperatorSet.GenEmptyObj(out ho_ConnectedRegion);
                    HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
                    HOperatorSet.GenEmptyObj(out ho_GrayImage);
                    ho_GrayImage.Dispose();
                    HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
                    HOperatorSet.GetImageSize(ho_GrayImage, out hv_Width, out hv_Height);
                    hv_delX = hv_Width * hv_XField;
                    hv_delY = hv_Height * hv_YField;

                    //ho_Rectangle1.Dispose();
                    //HOperatorSet.GenRectangle2(out ho_Rectangle1, (hv_Height / 2) - (hv_delY / 2), (hv_Width / 2) - (hv_delX / 2), 0, hv_Corner1ROIR, hv_Corner1ROIC);
                    //ho_Rectangle2.Dispose();
                    //HOperatorSet.GenRectangle2(out ho_Rectangle2, (hv_Height / 2) - (hv_delY / 2), (hv_Width / 2) + (hv_delX / 2), 0, hv_Corner1ROIR, hv_Corner1ROIC);
                    //ho_Rectangle3.Dispose();
                    //HOperatorSet.GenRectangle2(out ho_Rectangle3, (hv_Height / 2) + (hv_delY / 2), (hv_Width / 2) - (hv_delX / 2), 0, hv_Corner1ROIR, hv_Corner1ROIC);
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, (hv_Height / 2) + (hv_delY / 2), (hv_Width / 2) + (hv_delX / 2), 0, hv_ROIR / 2, hv_ROIC / 2);

                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle, out ho_ImageReduced);
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, hv_SFRThreshold, 255);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Region, out ho_RegionClosing, hv_Closing);
                    ho_ConnectedRegion.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegion);
                    HOperatorSet.AreaCenter(ho_ConnectedRegion, out hv_Area, out hv_Row0, out hv_Column0);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegion, out ho_SelectedRegions, "area", "and", hv_Area.TupleMax(), 9999999);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }

    }



}
