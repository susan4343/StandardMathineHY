using BaseDll;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserData
{
 
    public enum NozzleState
    {
        None,
        Have,
        HaveSnaped1,// 第一次拍照已经拍过
        HaveSnapOK1,//拍照蜂鸣器成功
        HaveSnapNG1,//拍照蜂鸣器失败
        HaveSnaped2,// 第一次拍照已经拍过
        HaveSnapOK2,//拍照黑帽成功
        HaveSnapNG2,//拍照黑帽失败
        HaveOK,
        HaveNg,
    }

    public class Nozzle
    {
        public Nozzle()
        {
            nozzleState = NozzleState.None;
            VacuumTime = 100;
            BreakVacuumTime = 100;

        }
        public void Reset()
        {
            strBarCode = "";
            nozzleState = NozzleState.None;
            indexPickFromSocket = -1;
        }
        public NozzleState nozzleState
        {
            set
            {
                _nozzleState = value;
                if(_nozzleState== NozzleState.None)
                {
                    indexPickFromSocket = -1;
                    strBarCode = "";
                }
            }
            get => _nozzleState;
        }
        
        public XYUPoint DstMachinePos
        {
            set;
            get;
        }
        public XYUPoint ObjMachinePos
        {
            set;
            get;
        }
        public XYUPoint ObjSnapMachinePos
        {
            set;
            get;

        }
        public XYUPoint DstSnapMachinePos
        {
            set;
            get;

        }
        public XYUPoint xYUOffset = new XYUPoint(0, 0, 0);
        
        public NozzleState nozzleState2
        {
            set
            {
                _nozzleState2 = value;
                if (_nozzleState2 == NozzleState.None)
                {
                    indexPickFromSocket = -1;
                    strBarCode = "";
                }
            }
            get => _nozzleState2;
        }
        NozzleState _nozzleState;
        NozzleState _nozzleState2;
        public string NozzleVacuumIoName
        {
            set;
            get;
        }
        public string NozzleBreakVacuumIoName
        {
            set;
            get;
        }
        public string NozzleVacuumCheckIoName
        {
            set;
            get;
        }
        public int BreakVacuumTime
        {
            set;
            get;
        }
        public int VacuumTime
        {
            set;
            get;
        }
        public string strNozzleName
        {
            set;
            get;
        }
        public string strBarCode
        {
            set; get;
        }
        public string strBarCode1d
        {
            set;
            get;
        }

        public int indexPickFromSocket
        {
            set; get;
        }
     



    }
    public enum NozzleType
    {
        BuzzerNozzle_L1=0,
        BuzzerNozzle_L2,
        BuzzerNozzle_L3,
        BuzzerNozzle_L4,

        BuzzerNozzle_R1=4,
        BuzzerNozzle_R2,
        BuzzerNozzle_R3,
        BuzzerNozzle_R4,
        LoadNozzle=8,
        UnLoadNozzle,
        LeftStripNozzle,
        RightStripNozzle,
    
    }
    public class NozzleMgr
    {
        public NozzleMgr()
        {

        }
        private static object obj = new object();
        private static NozzleMgr nozzleMgr;
        public static NozzleMgr GetInstance()
        {
            if (nozzleMgr == null)
            {
                lock (obj)
                {
                    if (nozzleMgr == null)
                    {
                        nozzleMgr = new NozzleMgr();
                    }
                }
            }
            return nozzleMgr;
        }
        public Nozzle[] nozzleArr = new Nozzle[12] {
            new Nozzle(), new Nozzle(), new Nozzle(), new Nozzle(),
            new Nozzle(), new Nozzle(), new Nozzle(), new Nozzle(),
            new Nozzle(), new Nozzle(), new Nozzle(), new Nozzle()
        };
        public void Save()
        {

            string currentNozzleFile = ParamSetMgr.GetInstance().CurrentWorkDir + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + ("\\") + "nozzleArr" + (".xml");
            AccessXmlSerializer.ObjectToXml(currentNozzleFile, nozzleArr);
        }
    
       public void  Read()
        {
            string currentNozzleFile = ParamSetMgr.GetInstance().CurrentWorkDir + ("\\") + ParamSetMgr.GetInstance().CurrentProductFile + ("\\") + "nozzleArr" + (".xml");
            if( !File.Exists(currentNozzleFile))
            {
                Save();
            }
            Object obs=  AccessXmlSerializer.XmlToObject(currentNozzleFile, typeof(Nozzle[]));
            if(obs!=null)
            {
                nozzleArr =(Nozzle[])obs;
            }
        }
        public void SetNozzleState(NozzleType nozzleType, NozzleState nozzleState)
        {
            nozzleArr[(int)nozzleType].nozzleState = nozzleState;
        } 
      
    }

}
