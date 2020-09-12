using BaseDll;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleCapture
{
    class MSerialPort
    {
        private static readonly MSerialPort instance = new MSerialPort();

        List<SerialPortOperation> Para_Port = new List<SerialPortOperation>() { new SerialPortOperation(), new SerialPortOperation() };
        private MSerialPort()
        {
            string COMA = ParamSetMgr.GetInstance().GetStringParam("A工位关闭曝光COM");
            string COMB = ParamSetMgr.GetInstance().GetStringParam("B工位关闭曝光COM");
            Para_Port[0] = new SerialPortOperation(COMA, 38400, Parity.None, 8, StopBits.One);
            Para_Port[1] = new SerialPortOperation(COMB, 38400, Parity.None, 8, StopBits.One);
        }
        public static MSerialPort GetInstance()
        {
            return instance;
        }
        public void CloseExposure(int nID)
        {
            Para_Port[nID].CloseExposure();
        }
    }
}
