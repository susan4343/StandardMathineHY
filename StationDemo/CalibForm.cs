using ModuleCapture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StationDemo
{
    public partial class CalibForm : Form
    {
        public CalibForm()
        {
            InitializeComponent();
        }

        private void BtnSnap_Click(object sender, EventArgs e)
        {
            Bitmap bt = null;
            ModuleMgr.Instance.CaptureToBmpRGB(5, 1, ref bt);
            if (bt != null)
                PictureShow.Image = bt;
        }

        private void CalibForm_Load(object sender, EventArgs e)
        {
            ModuleMgr.Instance.AddModule("USBDircect",1);
            ModuleMgr.Instance.Init(5, @"@device:pnp:\\?\usb#vid_05a3&pid_0317&mi_00#6&2519296e&0&0000#{65e8773d-8f56-11d0-a3b9-00a0c9223196}\global",1);

        }
    }
}
