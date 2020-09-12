using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
namespace DispFindCenter
{
    public abstract class DispBase
    {
        public abstract bool FindMark(HObject Image, HTuple Mold, ref double row, ref double col);
        public abstract bool SetMold(HObject ImageMold, ref HTuple Mold);
    }
}
