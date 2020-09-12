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
    public enum SocketState
    {
        None,
        Have,
        Picked,
        //  HaveHaftSnapOK,
        HaveHaftOK,//socket 治具左部分贴装完成
        HaveOK,
        HaveNG,
        HaveUnKnow,
    }
    public enum SocketType
    {
      A,
      B
    }

    public enum SokcetLineState
    {
        None,
        Finish,
    }
    public enum SocketCellState
    {
        CellStateNone,
        CellStateOK,
        CellStateNG,
        
    }

    public struct SocketCell
    {
        public SocketCellState Cellstate;
        public SocketCellState Cellstate2;
        public string strBarCode;
        public XYUZPoint pos;
        public SocketCell(SocketCellState cellstate= SocketCellState.CellStateNone)
        {
            strBarCode = "";
            Cellstate = cellstate;
            Cellstate2 = cellstate;
            pos = new XYUZPoint(0, 0, 0, 0);
        }


    }

    public struct SocketData
    {
        public bool bEable;
        public string strBarCode2d;
        public string strBarCode1d;
        public SocketState socketStateRecord;
        public SocketState socketState
        {
            set
            {
                socketStateRecord = value;
              
            }
            get
            {
                return socketStateRecord;
            }
        }

        public SocketCell[] socketcells;
      
        public SocketData(SocketState socketState2= SocketState.None)
        {
            bEable = true;
            strBarCode2d = "";
            strBarCode1d = "";
            socketStateRecord = socketState2;
           
            socketcells = new SocketCell[8];
          
      }
    }

    public class SocketMgr
    {
        private SocketMgr()
        {

        }
        private static object obj = new object();
        private static SocketMgr socketMgr;

        public delegate void SocketStateChanged();

        public event SocketStateChanged SocketStateChangedEvent = null;


        public static SocketMgr GetInstance()
        {
            if (socketMgr == null)
            {
                lock (obj)
                {
                    if (socketMgr == null)
                    {
                        socketMgr = new SocketMgr();
                    }
                }
            }
            return socketMgr;
        }
        public SocketData[] socketArr = new SocketData[2] {  new SocketData(SocketState.None),new SocketData(SocketState.None)}; 
        public void SetSocketState(int index, SocketState socketState)
        {
            if (index <= socketArr.Length && index >= 0)
            {
                socketArr[index].socketState = socketState;

            }
              
        }
        public SocketState GetSocketState(int index)
        {
            return socketArr[index].socketState;
        }
     
        public void ResetAllSocket()
        {
            for (int i = socketArr.Length - 1; i > 0; i--)
            {
                socketArr[i].socketState = SocketState.None;
                socketArr[i].socketcells = new SocketCell[8] {
                new SocketCell(SocketCellState.CellStateNone), new SocketCell(SocketCellState.CellStateNone),new SocketCell(SocketCellState.CellStateNone),
                new SocketCell(SocketCellState.CellStateNone),new SocketCell(SocketCellState.CellStateNone),new SocketCell(SocketCellState.CellStateNone),
                new SocketCell(SocketCellState.CellStateNone),new SocketCell(SocketCellState.CellStateNone)};

            }
               
        }
        public void MoveNext()
        {
            SocketData socketData = socketArr[socketArr.Length - 1];
            socketData.socketcells = socketArr[socketArr.Length - 1].socketcells;
            for (int i = socketArr.Length - 1; i >0; i--)
            {
                socketArr[i] = socketArr[i - 1];
                socketArr[i].socketcells = socketArr[i - 1].socketcells;
            }
               
            socketArr[0] = socketData;
            socketArr[0].socketcells = socketData.socketcells;
        }
    }

}