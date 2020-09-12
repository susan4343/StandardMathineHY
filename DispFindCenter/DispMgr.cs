using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UserData;

namespace DispFindCenter
{
    public class DispMgr
    {
        private DispMgr()
        {

        }
        private static object obj = new object();
        private static DispMgr dispMgr;
        public static DispMgr Instance
        {
            get
            {
                lock (obj)
                {
                    if (dispMgr == null)
                    {

                        if (dispMgr == null)
                        {
                            dispMgr = new DispMgr();
                        }
                    }

                    return dispMgr;
                }
            }
        }
        private Dictionary<int, DispBase> m_lisDevice  = new Dictionary<int, DispBase>();
        public void AddDisp(int nID, string strName)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(DispBase));
            string name = "DispFindCenter.Disp_" + strName;
            //Motion_Advantech
            Type type = assembly.GetType(name);
            bool flag = type == null;
            if (flag)
            {
                throw new Exception($"模组点亮类型{strName}找不到可用的封装类，请确认该类型DLL或配置错误");
            }
            object[] args = new object[] { };
            if (!m_lisDevice .ContainsKey(nID))
            {
                m_lisDevice .Add(nID, Activator.CreateInstance(type, args) as DispBase);
            }
            else
            {
                m_lisDevice [nID] = Activator.CreateInstance(type, args) as DispBase;
            }
        }
        public DispBase GetDispByIndexID(int nID)
        {
            DispBase temp = m_lisDevice [nID];
            return temp;
        }
        public bool FindMark(int nID, HObject Image, HTuple Mold, ref double Row, ref double Col)
        {
            DispBase tempModuleBase = GetDispByIndexID(nID);
            if (tempModuleBase != null)
            {
                return tempModuleBase.FindMark(Image, Mold, ref Row, ref Col);
            }
            else
                return false;
        }
        public bool SetMold(int nID, HObject ImageMold, ref HTuple MoldID)
        {
            DispBase tempModuleBase = GetDispByIndexID(nID);
            if (tempModuleBase != null)
            {
                return tempModuleBase.SetMold(ImageMold, ref MoldID);
            }
            else
                return false;
        }

        public bool Calibration(List<DispCalibration> dispCalibrations, ref HTuple mat2D)
        {
            bool flag = false;
            double[] PosX = new double[9];
            double[] PosY = new double[9];
            try
            {
                int Count = dispCalibrations.Count;
                double[] X = new double[Count];
                double[] Y = new double[Count];
                double[] Row = new double[Count];
                double[] Col = new double[Count];
                for (int i = 0; i < Count; i++)
                {
                    X[i] = dispCalibrations[Count].MotionX;
                    Y[i] = dispCalibrations[Count].MotionY;
                    Row[i] = dispCalibrations[Count].Row;
                    Col[i] = dispCalibrations[Count].Col;
                }
                HOperatorSet.VectorToHomMat2d(Row, Col, X, Y, out mat2D);//9点标定数组
                flag = true;
            }
            catch (Exception e)
            {
                flag = false;
            }
            return flag;
        }

    }
}
