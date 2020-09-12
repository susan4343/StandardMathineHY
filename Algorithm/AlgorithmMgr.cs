
using BaseDll;
using CameraDevice;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UserData;

namespace AlgorithmNamespace
{
    public class AlgorithmMgr
    {
        private AlgorithmMgr()
        {

        }
        private static object obj = new object();
        private static AlgorithmMgr moduleMgr;
        private Dictionary<int, AlgorithmBase> m_lisDevice= new Dictionary<int, AlgorithmBase>();
        private bool bLoad = false;
        public static AlgorithmMgr Instance
        {
            get
            {
                lock (obj)
                {
                    if (moduleMgr == null)
                    {

                        if (moduleMgr == null)
                        {
                            moduleMgr = new AlgorithmMgr();
                        }

                    }
                    return moduleMgr;
                }
            }
        }
        public void AddAlgorithm(int nID, string strName)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(AlgorithmBase));
            string name = "AlgorithmNamespace.Alg_" + strName;
            //Motion_Advantech
            Type type = assembly.GetType(name);
            bool flag = type == null;
            if (flag)
            {
                throw new Exception($"算法类型{strName}找不到可用的封装类，请确认该类型DLL或配置错误");
            }
            object[] args = new object[] { };
            if (!m_lisDevice .ContainsKey(nID))
            {
                m_lisDevice .Add(nID, Activator.CreateInstance(type, args) as AlgorithmBase);
            }
            else
            {
                m_lisDevice [nID] = Activator.CreateInstance(type, args) as AlgorithmBase;
            }
        }
        public AlgorithmBase GetAlgByIndexID()
        {
            AlgorithmBase temp = m_lisDevice [0];
            return temp;
        }
        public bool LoadConfig(string path)
        {
            AlgorithmBase tempModuleBase = GetAlgByIndexID();
            if (tempModuleBase == null)
            {
                return false;
            }
            if (!tempModuleBase.LoadConfig(path))
            {
                return false;
            }
            bLoad = true;
            return true;
        }
        public bool Findcenter(Bitmap bmp, ref double dx, ref double dy, ref List<double> Array, bool MF)
        {
            if (!bLoad)
            {
                return false;
            }
            AlgorithmBase tempModuleBase = GetAlgByIndexID();
            if (tempModuleBase != null)
            {
                return tempModuleBase.Findcenter(bmp, ref dx, ref dy, ref Array, MF);
            }
            else
                return false;
        }

        public bool GetSFRValue(Bitmap bmp, ref SFRValue SFRValue, ref RectInfo rectInfo, Rectangle[] rectangles, ref LightValue lightValue, bool TestLight = false)
        {
            if (!bLoad)
            {
                return false;
            }
            AlgorithmBase tempModuleBase = GetAlgByIndexID();
            if (tempModuleBase != null)
            {
                bool result = tempModuleBase.GetSFRValue(bmp, ref SFRValue, ref rectInfo, rectangles, ref lightValue, TestLight);
                if (SFRValue != null)
                    for (int i = 0; i < SFRValue.block.Length; i++)
                    {             
                        if (SFRValue.block[i].dValue > ParamSetMgr.GetInstance().GetDoubleParam("SFR最大值"))
                        {
                            SFRValue.block[i].dValue = -1;             
                        }
                    }

                return result;
            }
            else
                return false;
        }
        public bool GetTiltValue(SFRValue[] SFRValues, ref double dPeakZ, ref double dTx, ref double dTy)
        {
            if (!bLoad)
            {
                return false;
            }
            AlgorithmBase tempModuleBase = GetAlgByIndexID();
            if (tempModuleBase != null)
            {
                return tempModuleBase.GetTiltValue(SFRValues, ref dPeakZ, ref dTx, ref dTy);
            }
            else
                return false;
        }
    }



}
