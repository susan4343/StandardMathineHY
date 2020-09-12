using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using System.Threading;
using System.Drawing.Imaging;

using AForge.Video.DirectShow;
using AForge.Video;
using System.Runtime.InteropServices;
using BaseDll;

namespace ModuleCapture
{
    public class CameraUsb : IDisposable
    {
        private int nWidth = 0;
        private int nHeight = 0;
        private FilterInfoCollection USE_Webcams = null;
        private VideoCaptureDevice cam = null;
        private FilterInfoCollection videoDevices;

        public Bitmap bm1 = null;
        public Bitmap bt1 = null;
        private IntPtr ptr = IntPtr.Zero;
        private byte[] bytesImg = null;
        private string m_strSn = "";

        public int nRoiWidth = 0;
        public int nRoiHeight = 0;
        public int nStartX = 0;
        public int nStartY = 0;
        public int nResolutionX = 0;
        public int nResolutionY = 0;

        Bitmap[] bmArry = new Bitmap[8];

        private object o1 = new object();

        public bool Enumerate(ref int nNum, List<String> strSN)
        {
            try
            {
                // 枚举所有视频输入设备
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                Thread.Sleep(100);
                if (videoDevices.Count == 0)
                    return false;
                for (int i = 0; i < videoDevices.Count; i++)
                    strSN?.Add(videoDevices[i].MonikerString);


                return true;
            }
            catch (ApplicationException)
            {

                videoDevices = null;
            }
            return false;

        }
        public Bitmap Bitmap1
        {
            set
            {
                lock (o1)
                {
                    bt1?.Dispose();
                    bt1 = value;
                }
            }
            get
            {
                lock (o1)
                {
                    return (Bitmap)bt1;
                }
            }
        }
        private void Cam_NewFrame1(object obj, NewFrameEventArgs eventArgs)
        {
            lock (o1)
            {
                Bitmap1?.Dispose();
                Bitmap1 = (Bitmap)eventArgs.Frame.Clone();
                nWidth = Bitmap1.Width;
                nHeight = Bitmap1.Height;
                //Rectangle rect = new Rectangle(0, 0, nWidth, nHeight);
                //System.Drawing.Imaging.BitmapData bmpData = Bitmap1.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, Bitmap1.PixelFormat);
                //if (bytesImg == null)
                //{
                //    if (Bitmap1.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
                //    {
                //        bytesImg = new byte[nWidth * nHeight * 4];
                //    }
                //    else if (Bitmap1.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                //    {
                //        bytesImg = new byte[nWidth * nHeight * 3];
                //    }
                //    else if (Bitmap1.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                //    {
                //        bytesImg = new byte[nWidth * nHeight * 1];
                //    }
                //}
                //ptr = bmpData.Scan0;
                //Bitmap1.UnlockBits(bmpData);
                return;
            }
        }
        public bool SetSN(string strSN)
        {
            m_strSn = strSN;
            if (m_strSn == null || m_strSn == "")
                return false;
            return true;
        }
        /// <summary>
        /// strBoot为分辨率X,Y,    起始ROI X,Y,    ROI 长宽Width,Height  6个参数逗号隔开
        /// </summary>
        /// <param name="strBoot"></param>
        /// <returns></returns>
        public bool Init(string strBoot)
        {
            Stop();
            nResolutionX = ParamSetMgr.GetInstance().GetIntParam("产品分辨率X");
            nResolutionY = ParamSetMgr.GetInstance().GetIntParam("产品分辨率Y");
            nStartX = ParamSetMgr.GetInstance().GetIntParam("产品起始坐标X");
            nStartY = ParamSetMgr.GetInstance().GetIntParam("产品起始坐标Y");
            nRoiWidth = ParamSetMgr.GetInstance().GetIntParam("产品ROI宽");
            nRoiHeight = ParamSetMgr.GetInstance().GetIntParam("产品ROI高");
            if (strBoot == "")
                strBoot = m_strSn;
            cam = new VideoCaptureDevice(strBoot);
            if (cam != null)
                cam.NewFrame += new NewFrameEventHandler(Cam_NewFrame1);
            foreach (VideoCapabilities capab in cam.VideoCapabilities)
            {
                if (capab.FrameSize.Width == nResolutionX && capab.FrameSize.Height == nResolutionY)
                {
                    cam.VideoResolution = capab;
                    Play();
                    break;
                }
            }



            return true;
        }
        public bool Play()
        {
            if (cam == null) return false;
            if (cam != null && !cam.IsRunning)
            {
                try
                {
                    cam?.Start();
                }
                catch
                {
                    Init("");
                    cam?.Start();
                }
            }


            return true;
        }
        public bool Stop()
        {

            if (cam != null && cam.IsRunning)
                cam?.Stop();
            return true;
        }
        public int GetWidth()
        {
            if (nRoiWidth != 0)
                return nRoiWidth;
            return nWidth;
        }
        public int GetHeight()
        {
            if (nRoiHeight != 0)
                return nRoiHeight;
            return nHeight;
        }
        public int GetBayerType()
        {
            int n = (int)Bitmap1?.PixelFormat;
            return n;
        }
        public Bitmap ToColorBitmap2(byte[] rawValues, int width, int height)
        {
            if (width < 1 || height < 1)
                return null;
            PixelFormat pixelFormat = (PixelFormat)GetBayerType();
            Bitmap bmp = new Bitmap(width, height, pixelFormat);
            try
            {
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, pixelFormat);   //// 获取图像参数  

                IntPtr iptr = bmpData.Scan0;

                int nDeep = pixelFormat == PixelFormat.Format8bppIndexed ? 1 : pixelFormat == PixelFormat.Format32bppRgb ? 4 : 3;
                System.Runtime.InteropServices.Marshal.Copy(rawValues, 0, iptr, width * height * nDeep);
                bmp.UnlockBits(bmpData);

                return bmp;
            }
            catch (System.Exception e)
            {

                return null;
            }
        }





        //方法定义

        private Bitmap crop(Bitmap src, int nStartX, int nStartY, int nWidth, int nHeight)
        {
            Rectangle cropRect = new Rectangle(nStartX, nStartY, nWidth, nHeight);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height, src.PixelFormat);
            using (Graphics g = Graphics.FromImage(target))
            {

                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                     cropRect,
                      GraphicsUnit.Pixel);
            }
            return target;
        }

        public bool Capture(Byte[] _FrameBuffer)
        {
            lock (o1)
            {
                if (Bitmap1 == null)
                    return false;
                if (nRoiWidth != 0)
                {
                    Bitmap bitcrop = crop(Bitmap1, nStartX, nStartY, nRoiWidth, nRoiHeight);
                    Rectangle rect = new Rectangle(0, 0, bitcrop.Width, bitcrop.Height);
                    PixelFormat oldpiexel = Bitmap1.PixelFormat;
                    System.Drawing.Imaging.BitmapData bmpData = bitcrop.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitcrop.PixelFormat);
                    ptr = bmpData.Scan0;
                    int nDeep = bitcrop.PixelFormat == PixelFormat.Format8bppIndexed ? 1 : (bitcrop.PixelFormat == PixelFormat.Format32bppRgb || bitcrop.PixelFormat == PixelFormat.Format32bppArgb
                        || bitcrop.PixelFormat == PixelFormat.Format32bppPArgb) ? 4 : 3;
                    Marshal.Copy(ptr, _FrameBuffer, 0, nRoiWidth * nRoiHeight * nDeep);
                    bitcrop.UnlockBits(bmpData);
                }
                else
                {
                    Rectangle rect = new Rectangle(0, 0, nWidth, nHeight);
                    System.Drawing.Imaging.BitmapData bmpData = Bitmap1.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, Bitmap1.PixelFormat);
                    ptr = bmpData.Scan0;
                    int nDeep = Bitmap1.PixelFormat == PixelFormat.Format8bppIndexed ? 1 : (Bitmap1.PixelFormat == PixelFormat.Format32bppRgb || Bitmap1.PixelFormat == PixelFormat.Format32bppArgb
                       || Bitmap1.PixelFormat == PixelFormat.Format32bppPArgb) ? 4 : 3;
                    Marshal.Copy(ptr, _FrameBuffer, 0, nWidth * nHeight * nDeep);
                    Bitmap1.UnlockBits(bmpData);
                }




            }
            return true;
        }

        public void Dispose()
        {
            try
            {
                if (cam != null)
                    cam.NewFrame -= new NewFrameEventHandler(Cam_NewFrame1);
            }
            catch (Exception ex)
            {

            }
            try
            {
                cam?.Stop();
                if (cam != null)
                    cam = null;
            }
            catch (Exception ex)
            {

            }

        }
    }

}
