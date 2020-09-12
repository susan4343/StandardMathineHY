
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleCapture
{
    public class ModuleMgr
    {
        private ModuleMgr()
        {

        }
        private static object obj = new object();
        private static ModuleMgr moduleMgr;
        public static ModuleMgr Instance
        {
            get
            {
                lock (obj)
                {
                    if (moduleMgr == null)
                    {

                        if (moduleMgr == null)
                        {
                            moduleMgr = new ModuleMgr();
                        }
                    }

                    return moduleMgr;
                }
            }
        }
        private MCameraDeviceBase[] m_lisDevice= new MCameraDeviceBase[2];
        public void AddModule(string strName, int ModuleType=0)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(MCameraDeviceBase));
            string name = "ModuleCapture.Module_" + strName;
            //Motion_Advantech
            Type type = assembly.GetType(name);
            bool flag = type == null;
            if (flag)
            {
                throw new Exception($"模组点亮类型{strName}找不到可用的封装类，请确认该类型DLL或配置错误");
            }
            object[] args = new object[] { };
            m_lisDevice[ModuleType] = Activator.CreateInstance(type, args) as MCameraDeviceBase;
        }
        public bool Capture(int nID, int Frame, byte[] _FrameBuffer, int ModuleType = 0)
        {

            int cnt = 0;
            bool result = false;
            for (int i = 0; i < Frame; i++)
            {
                result = m_lisDevice[ModuleType].Capture(nID, _FrameBuffer);
            }
            do
            {
                result = m_lisDevice[ModuleType].Capture(nID, _FrameBuffer);
                if (result)
                {
                    break;
                }
                cnt++;
            } while (cnt < 5);
            return result;
        }
        public bool CaptureToBmpRGB(int nID, int Frame, ref Bitmap Image, int ModuleType = 0)
        {
            byte[] _FrameBuffer = new byte[m_lisDevice[ModuleType].GetBufLenght(nID)];
            if (!Capture(nID, Frame, _FrameBuffer, ModuleType))
            {
                return false;
            }
            if (!m_lisDevice[ModuleType].BufToBmpRGB(nID, _FrameBuffer, ref Image))
            {
                return false;
            }
            if (Image == null)
            {
                return false;
            }
            return true;

        }
        public bool CaptureToBmpGray(int nID, int Frame, ref Bitmap Image, int ModuleType = 0)
        {
            byte[] _FrameBuffer = new byte[m_lisDevice[ModuleType].GetBufLenght(nID)];
            if (!Capture(nID, Frame, _FrameBuffer, ModuleType))
            {
                return false;
            }
            if (!m_lisDevice[ModuleType].BufToBmpGray(nID, _FrameBuffer, ref Image))
            {
                return false;
            }
            if (Image == null)
            {
                return false;
            }
            return true;

        }
        public bool CaptureToByteGray(int nID, int Frame, byte[] buf, int ModuleType = 0)
        {
            byte[] _FrameBuffer = new byte[m_lisDevice[ModuleType].GetBufLenght(nID)];
            if (!Capture(nID, Frame, _FrameBuffer, ModuleType))
            {
                return false;
            }
            if (!m_lisDevice[ModuleType].BufToByteGray(nID, _FrameBuffer, buf))
            {
                return false;
            }
            return true;

        }

        public bool Init(int nID, string strBoot, int ModuleType = 0)
        {

            bool result = m_lisDevice[ModuleType].Init(nID, strBoot);
            return result;
        }
        public bool Play(int nID, int ModuleType = 0)
        {
            m_lisDevice[ModuleType].Stop(nID);
            bool result = m_lisDevice[ModuleType].Play(nID);
            if (!result)
            {
                m_lisDevice[ModuleType].Stop(nID);
                Thread.Sleep(200);
                result = m_lisDevice[ModuleType].Play(nID);
            }
            return result;
        }
        public bool Stop(int nID, int ModuleType = 0)
        {
            bool result = m_lisDevice[ModuleType].Stop(nID);
            return result;
        }
        private ConcurrentDictionary<int, List<string>> _sn = new ConcurrentDictionary<int, List<string>>();
        public bool Enumerate(int nID, ref int nNum, List<string> strSN, int ModuleType = 0)
        {

            bool result = m_lisDevice[ModuleType].Enumerate(nID, ref nNum, strSN);
            if (!_sn.ContainsKey(nID))
            {
                _sn.TryAdd(nID, strSN);
            }
            else
            {
                _sn[nID] = strSN;
            }

            return result;
        }
        public bool LoadDll(string FullPath, int ModuleType = 0)
        {
            bool result = m_lisDevice[ModuleType].LoadDll(FullPath);
            return result;
        }
        public bool WriteI2C(int nID, byte _DevceID, int _i2Cmode, int _wAddr, int _wData, int ModuleType = 0)
        {
            bool result = m_lisDevice[ModuleType].WriteI2C(nID, _DevceID, _i2Cmode, _wAddr, _wData);
            return result;
        }
        public bool SetSN(int nID, int nSN, int ModuleType = 0)
        {
            string strSN = _sn[0][nSN];
            bool result = m_lisDevice[ModuleType].SetSN(nID, strSN);
            return result;
        }
        public int GetBayerType(int nID, int ModuleType = 0)
        {
            int result = m_lisDevice[ModuleType].GetBayerType(nID);
            return result;
        }
        public int GetHeight(int nID, int ModuleType = 0)
        {
            int result = m_lisDevice[ModuleType].GetHeight(nID);
            return result;
        }
        public int GetWidth(int nID, int ModuleType = 0)
        {
            int result = m_lisDevice[ModuleType].GetWidth(nID);
            return result;
        }
    }
}
