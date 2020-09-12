using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UserData
{
    public class GlobelManualResetEvent
    {
        
        public static ManualResetEvent ContinueShowA=new ManualResetEvent(false);
        public static ManualResetEvent ContinueShowB=new ManualResetEvent(false);
        public static ManualResetEvent ContinueShowC=new ManualResetEvent(false);
        public static ManualResetEvent AutoPlay = new ManualResetEvent(false);
    }
}
