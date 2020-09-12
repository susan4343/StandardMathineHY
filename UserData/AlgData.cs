using System;
using System.Collections.Generic;
using System.Drawing;

namespace UserData
{
    public class SFRValue
    {
        public double dZ;
        public BlockValue[] block;
        //对应值,自定义值(Chart 为准)，颢天算法对应值、
        //Chart                      Collimator        Sector
        //block[0]=中心CT   ---------block[4] ---------block[0]
        //block[1]=边角1左上---------block[0] ---------block[1]
        //block[2]=边角1右上---------block[2] ---------block[2]
        //block[3]=边角1左下---------block[6] ---------block[3]
        //block[4]=边角1右下---------block[8] ---------block[4]
        //block[5]=边角2左上---------         ---------
        //block[6]=边角2右上---------         ---------
        //block[7]=边角2左下---------         ---------
        //block[8]=边角2右下---------         ---------
        //block[9]=十字上   ---------block[1] ---------
        //block[10]=十字左  ---------block[3] ---------
        //block[11]=十字右  ---------block[5] ---------
        //block[12]=十字下  ---------block[7] ---------
    }
    public class RectInfo
    {
        public Point[][] Points;
    }
    public class BlockValue
    {

        public double dX;
        public double dY;
        public double dValue;
        public double[] aryValue;
    }
    public class LightValue
    {
        public double[] blockValue;
    }
}


