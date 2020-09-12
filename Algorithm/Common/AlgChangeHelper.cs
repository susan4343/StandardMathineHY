
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
    /// <summary>
    /// 转换关系，把算法参数统一
    /// </summary>
    public class AlgChangeHelper
    {
        //chart 类型
        public static SFRValue Chart_Info2Value(SFRInfo info)
        {

            if (info == null || info.block == null || info.block.Length == 0)
                return null;
            int nCnt = 0;
            SFRValue result = new SFRValue();
            result.block = new BlockValue[13];
            for (int i = 0; i < 13; i++)
            {
                result.block[i] = new BlockValue();
            }
            result.dZ = info.dZ;
            for (int i = 0; i < info.block.Length; i++)
            {
                // result.block[i].aryValue = new double[info.block[i].aryValue.Length];
                result.block[i].aryValue = info.block[i].aryValue;
                //  result.block[i].aryValue[1] = result.block[i].aryValue[2];
                // result.block[i].aryValue[3] = result.block[i].aryValue[4];

                result.block[i].dValue = info.block[i].dValue;
                result.block[i].dX = info.block[i].dX;
                result.block[i].dY = info.block[i].dY;
            }
            return result;
        }
        public static RectInfo Chart_Info2Value(ObjectInfo info)
        {
            int x = info.SFR.Count();

            RectInfo result = new RectInfo();
            result.Points = new Point[x][];
            int i = 0;
            for (i = 0; i < info.SFR.Count(); i++)
            {
                result.Points[i] = info.SFR[i];
            }
            //result.Points
            //for (int i = 0; i < info.SFR.Length; i++)
            //{
            //    // result.block[i].aryValue = new double[info.block[i].aryValue.Length];
            //    result.block[i].aryValue = info.block[i].aryValue;
            //    result.block[i].dValue = info.block[i].dValue;
            //    result.block[i].dX = info.block[i].dX;
            //    result.block[i].dY = info.block[i].dY;
            //}
            return result;
        }
        public static SFRInfo Chart_Value2Info(SFRValue value)
        {
            SFRInfo result = new SFRInfo();
            result.dZ = value.dZ;
            for (int i = 0; i < value.block.Length; i++)
            {
                result.block[i].aryValue = value.block[i].aryValue;
                result.block[i].dValue = value.block[i].dValue;
                result.block[i].dX = value.block[i].dX;
                result.block[i].dY = value.block[i].dY;
            }
            return result;
        }
        public static SFRValue[] Chart_Info2Values(SFRInfo[] infos)
        {
            SFRValue[] result = new SFRValue[infos.Length];
            for (int j = 0; j < infos.Length; j++)
            {
                result[j].dZ = infos[j].dZ;
                for (int i = 0; i < infos[j].block.Length; i++)
                {
                    result[j].block[i].aryValue = infos[j].block[i].aryValue;
                    result[j].block[i].dValue = infos[j].block[i].dValue;
                    result[j].block[i].dX = infos[j].block[i].dX;
                    result[j].block[i].dY = infos[j].block[i].dY;
                }
            }
            return result;
        }
        public static SFRInfo[] Chart_Value2Infos(SFRValue[] value)
        {
            SFRInfo[] result = new SFRInfo[value.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new SFRInfo();
            }
            for (int j = 0; j < value.Length; j++)
            {
                result[j].dZ = value[j].dZ;
                for (int i = 0; i < value[j].block.Length; i++)
                {
                    result[j].block[i].aryValue = value[j].block[i].aryValue;
                    result[j].block[i].dValue = value[j].block[i].dValue;
                    result[j].block[i].dX = value[j].block[i].dX;
                    result[j].block[i].dY = value[j].block[i].dY;
                }
            }
            return result;
        }
        //Collimator
        public static SFRValue Collimator_Info2Value(SFR_C_Info info)
        {

            if (info == null || info.block == null || info.block.Length == 0)
                return null;
            int nCnt = 0;
            SFRValue result = new SFRValue();
            result.block = new BlockValue[13];
            for (int i = 0; i < 13; i++)
            {
                result.block[i] = new BlockValue();
            }
            result.dZ = info.dZ;

            result.block[0].aryValue = new double[5] { info.block[4].dValue, info.block[4].dValue, info.block[4].dValue, info.block[4].dValue, info.block[4].dValue };
            result.block[0].dValue = info.block[4].dValue;
            result.block[0].dX = info.block[4].dX;
            result.block[0].dY = info.block[4].dY;
            result.block[1].aryValue = new double[5] { info.block[0].dValue, info.block[0].dValue, info.block[0].dValue, info.block[0].dValue, info.block[0].dValue };
            result.block[1].dValue = info.block[0].dValue;
            result.block[1].dX = info.block[0].dX;
            result.block[1].dY = info.block[0].dY;
            result.block[2].aryValue = new double[5] { info.block[2].dValue, info.block[2].dValue, info.block[2].dValue, info.block[2].dValue, info.block[2].dValue };
            result.block[2].dValue = info.block[2].dValue;
            result.block[2].dX = info.block[2].dX;
            result.block[2].dY = info.block[2].dY;
            result.block[3].aryValue = new double[5] { info.block[6].dValue, info.block[6].dValue, info.block[6].dValue, info.block[6].dValue, info.block[6].dValue };
            result.block[3].dValue = info.block[6].dValue;
            result.block[3].dX = info.block[6].dX;
            result.block[3].dY = info.block[6].dY;
            result.block[4].aryValue = new double[5] { info.block[8].dValue, info.block[8].dValue, info.block[8].dValue, info.block[8].dValue, info.block[8].dValue };
            result.block[4].dValue = info.block[8].dValue;
            result.block[4].dX = info.block[8].dX;
            result.block[4].dY = info.block[8].dY;
            result.block[9].aryValue = new double[5] { info.block[1].dValue, info.block[1].dValue, info.block[1].dValue, info.block[1].dValue, info.block[1].dValue };
            result.block[9].dValue = info.block[1].dValue;
            result.block[9].dX = info.block[1].dX;
            result.block[9].dY = info.block[1].dY;
            result.block[10].aryValue = new double[5] { info.block[3].dValue, info.block[3].dValue, info.block[3].dValue, info.block[3].dValue, info.block[3].dValue };
            result.block[10].dValue = info.block[3].dValue;
            result.block[10].dX = info.block[3].dX;
            result.block[10].dY = info.block[3].dY;
            result.block[11].aryValue = new double[5] { info.block[5].dValue, info.block[5].dValue, info.block[5].dValue, info.block[5].dValue, info.block[5].dValue };
            result.block[11].dValue = info.block[5].dValue;
            result.block[11].dX = info.block[5].dX;
            result.block[11].dY = info.block[5].dY;
            result.block[12].aryValue = new double[5] { info.block[7].dValue, info.block[7].dValue, info.block[7].dValue, info.block[7].dValue, info.block[7].dValue };
            result.block[12].dValue = info.block[7].dValue;
            result.block[12].dX = info.block[7].dX;
            result.block[12].dY = info.block[7].dY;
            return result;
        }
        public static RectInfo Collimator_Info2Rect(SFR_C_Info info)
        {
            int x = info.block.Count();
            RectInfo result = new RectInfo();
            result.Points = new Point[x][];
        

            result.Points[0] = new Point[1] { new Point { X = (int)info.block[4].dX, Y = (int)info.block[4].dY } };
            result.Points[1] = new Point[1] { new Point { X = (int)info.block[0].dX, Y = (int)info.block[0].dY } };
            result.Points[2] = new Point[1] { new Point { X = (int)info.block[2].dX, Y = (int)info.block[2].dY } };
            result.Points[3] = new Point[1] { new Point { X = (int)info.block[6].dX, Y = (int)info.block[6].dY } };
            result.Points[4] = new Point[1] { new Point { X = (int)info.block[8].dX, Y = (int)info.block[8].dY } };
            result.Points[5] = new Point[1] { new Point { X = (int)info.block[1].dX, Y = (int)info.block[1].dY } };
            result.Points[6] = new Point[1] { new Point { X = (int)info.block[3].dX, Y = (int)info.block[3].dY } };
            result.Points[7] = new Point[1] { new Point { X = (int)info.block[5].dX, Y = (int)info.block[5].dY } };
            result.Points[8] = new Point[1] { new Point { X = (int)info.block[7].dX, Y = (int)info.block[7].dY } };
       
            return result;
        }
        public static SFR_C_Info Collimator_Value2Info(SFRValue value)
        {
            SFR_C_Info result = new SFR_C_Info();

            return result;
        }
        public static SFRValue[] Collimator_Info2Values(SFR_C_Info[] infos)
        {
            SFRValue[] result = new SFRValue[infos.Length];

            return result;
        }
        public static SFR_C_Info[] Collimator_Value2Infos(SFRValue[] value)
        {
            SFR_C_Info[] result = new SFR_C_Info[value.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new SFR_C_Info();
            }
            for (int j = 0; j < value.Length; j++)
            {
                result[j].dZ = value[j].dZ;
                //(0,1)(1,9)(2,2)(3,10)(4,0)(5,11)(6,3)(7,12)(8,4)
                result[j].block[0].aryValue = value[j].block[1].aryValue;
                result[j].block[0].dValue = value[j].block[1].dValue;
                result[j].block[0].dX = value[j].block[1].dX;
                result[j].block[0].dY = value[j].block[1].dY;
                result[j].block[1].aryValue = value[j].block[9].aryValue;
                result[j].block[1].dValue = value[j].block[9].dValue;
                result[j].block[1].dX = value[j].block[9].dX;
                result[j].block[1].dY = value[j].block[9].dY;
                result[j].block[2].aryValue = value[j].block[2].aryValue;
                result[j].block[2].dValue = value[j].block[2].dValue;
                result[j].block[2].dX = value[j].block[2].dX;
                result[j].block[2].dY = value[j].block[2].dY;
                result[j].block[3].aryValue = value[j].block[10].aryValue;
                result[j].block[3].dValue = value[j].block[10].dValue;
                result[j].block[3].dX = value[j].block[10].dX;
                result[j].block[3].dY = value[j].block[10].dY;
                result[j].block[4].aryValue = value[j].block[0].aryValue;
                result[j].block[4].dValue = value[j].block[0].dValue;
                result[j].block[4].dX = value[j].block[0].dX;
                result[j].block[4].dY = value[j].block[0].dY;
                result[j].block[5].aryValue = value[j].block[11].aryValue;
                result[j].block[5].dValue = value[j].block[11].dValue;
                result[j].block[5].dX = value[j].block[11].dX;
                result[j].block[5].dY = value[j].block[11].dY;
                result[j].block[6].aryValue = value[j].block[3].aryValue;
                result[j].block[6].dValue = value[j].block[3].dValue;
                result[j].block[6].dX = value[j].block[3].dX;
                result[j].block[6].dY = value[j].block[3].dY;
                result[j].block[7].aryValue = value[j].block[12].aryValue;
                result[j].block[7].dValue = value[j].block[12].dValue;
                result[j].block[7].dX = value[j].block[12].dX;
                result[j].block[7].dY = value[j].block[12].dY;
                result[j].block[8].aryValue = value[j].block[4].aryValue;
                result[j].block[8].dValue = value[j].block[4].dValue;
                result[j].block[8].dX = value[j].block[4].dX;
                result[j].block[8].dY = value[j].block[4].dY;
            }
            return result;
        }

        //Sector 类型
        public static SFRValue Sector_Info2Value(SFRInfo info)
        {

            if (info == null || info.block == null || info.block.Length == 0)
                return null;
            int nCnt = 0;
            SFRValue result = new SFRValue();
            result.block = new BlockValue[info.block.Length];
            for (int i = 0; i < info.block.Length; i++)
            {
                result.block[i] = new BlockValue();
            }
            result.dZ = info.dZ;
            for (int i = 0; i < info.block.Length; i++)
            {
                // result.block[i].aryValue = new double[info.block[i].aryValue.Length];
                result.block[i].aryValue = info.block[i].aryValue;
                //  result.block[i].aryValue[1] = result.block[i].aryValue[2];
                // result.block[i].aryValue[3] = result.block[i].aryValue[4];

                result.block[i].dValue = info.block[i].dValue;
                result.block[i].dX = info.block[i].dX;
                result.block[i].dY = info.block[i].dY;
            }
            return result;
        }
        public static RectInfo Sector_Info2Value(ObjectInfo info)
        {
            int x = info.SFR.Count();

            RectInfo result = new RectInfo();
            result.Points = new Point[x][];
            int i = 0;
            for (i = 0; i < info.SFR.Count(); i++)
            {
                result.Points[i] = info.SFR[i];
            }
            //result.Points
            //for (int i = 0; i < info.SFR.Length; i++)
            //{
            //    // result.block[i].aryValue = new double[info.block[i].aryValue.Length];
            //    result.block[i].aryValue = info.block[i].aryValue;
            //    result.block[i].dValue = info.block[i].dValue;
            //    result.block[i].dX = info.block[i].dX;
            //    result.block[i].dY = info.block[i].dY;
            //}
            return result;
        }
        public static SFRInfo Sector_Value2Info(SFRValue value)
        {
            SFRInfo result = new SFRInfo();
            result.dZ = value.dZ;
            for (int i = 0; i < value.block.Length; i++)
            {
                result.block[i].aryValue = value.block[i].aryValue;
                result.block[i].dValue = value.block[i].dValue;
                result.block[i].dX = value.block[i].dX;
                result.block[i].dY = value.block[i].dY;
            }
            return result;
        }
        public static SFRValue[] Sector_Info2Values(SFRInfo[] infos)
        {
            SFRValue[] result = new SFRValue[infos.Length];
            for (int j = 0; j < infos.Length; j++)
            {
                result[j].dZ = infos[j].dZ;
                for (int i = 0; i < infos[j].block.Length; i++)
                {
                    result[j].block[i].aryValue = infos[j].block[i].aryValue;
                    result[j].block[i].dValue = infos[j].block[i].dValue;
                    result[j].block[i].dX = infos[j].block[i].dX;
                    result[j].block[i].dY = infos[j].block[i].dY;
                }
            }
            return result;
        }
        public static SFRInfo[] Sector_Value2Infos(SFRValue[] value)
        {
            SFRInfo[] result = new SFRInfo[value.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new SFRInfo();
            }
            for (int j = 0; j < value.Length; j++)
            {
                result[j].dZ = value[j].dZ;
                for (int i = 0; i < value[j].block.Length; i++)
                {
                    result[j].block[i].aryValue = value[j].block[i].aryValue;
                    result[j].block[i].dValue = value[j].block[i].dValue;
                    result[j].block[i].dX = value[j].block[i].dX;
                    result[j].block[i].dY = value[j].block[i].dY;
                }
            }
            return result;
        }
    }

}
