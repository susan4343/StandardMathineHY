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
    public enum PlaneState
    {
        None,
        Have,
        HaveOK,
        HaveNG,
        HaveUnKnow,
    }
    public enum PlaneType
    {
       A,
       B,
     
    }

   

    public struct PlaneData
    {
        public string strBarCode2d;
        public string strBarCode1d;
        public PlaneState planeState;
    }

    public class PlaneMgr
    {
        private PlaneMgr()
        {

        }
        private static object obj = new object();
        private static PlaneMgr planeMgr;

        public static PlaneMgr GetInstance()
        {
            if (planeMgr == null)
            {
                lock (obj)
                {
                    if (planeMgr == null)
                    {
                        planeMgr = new PlaneMgr();
                    }
                }
            }
            return planeMgr;
        }
        public PlaneData[] PlaneArr = new PlaneData[2] { new PlaneData(), new PlaneData()};
                                                            
        public void SetPlaneState(int index, PlaneState planeState)
        {
            if (index <= PlaneArr.Length - 1 && index >= 1)
                PlaneArr[index - 1].planeState = planeState;
        }
        public PlaneState GetPlaneState(int index)
        {
            return PlaneArr[index - 1].planeState;
        }
    
    }

}