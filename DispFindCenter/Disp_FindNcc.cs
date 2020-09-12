using BaseDll;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispFindCenter
{
    public class Disp_FindNcc : DispBase
    {
        public override bool FindMark(HObject Image, HTuple Mold, ref double row, ref double col)
        {
            HTuple AngleStar = ParamSetMgr.GetInstance().GetDoubleParam("点胶匹配开始角度");
            HTuple AngleExtent = ParamSetMgr.GetInstance().GetDoubleParam("点胶匹配结束角度");
            HTuple Score = ParamSetMgr.GetInstance().GetDoubleParam("点胶匹配分数");
            HTuple NumLevels = ParamSetMgr.GetInstance().GetDoubleParam("点胶匹配等级");
            HTuple CenterRow = new HTuple();
            HTuple CenterColumn = new HTuple();
            HTuple CenterAngle = new HTuple();
            HTuple CenterScore = new HTuple();
            try
            {
                HOperatorSet.FindNccModel(Image, Mold, AngleStar, AngleExtent, Score, 1, 0.5, "true", NumLevels, out CenterRow, out CenterColumn, out CenterAngle, out CenterScore);
                if (CenterScore != null && CenterScore.D != 0)
                {
                    row = CenterRow.D;
                    col = CenterColumn.D;
                    return true;
                }
                else
                {
                    row = 0;
                    col = 0;
                    return false;
                }
            }
            catch
            {
                row = 0;
                col = 0;
                return false;
            }
        }
        public override bool SetMold(HObject ImageMold, ref HTuple MoldID)
        {
            HTuple NumLevels = ParamSetMgr.GetInstance().GetDoubleParam("点胶匹配等级");
            HTuple AngleStar = ParamSetMgr.GetInstance().GetDoubleParam("点胶匹配开始角度");
            HTuple AngleExtent = ParamSetMgr.GetInstance().GetDoubleParam("点胶匹配结束角度");
            try
            {
                HOperatorSet.CreateNccModel(ImageMold, NumLevels, AngleStar, AngleExtent, 0.05, "use_polarity", out MoldID);
                if (MoldID != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
