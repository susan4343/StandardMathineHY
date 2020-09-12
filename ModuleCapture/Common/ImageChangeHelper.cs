
using CameraDevice;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ModuleCapture
{
    public class ImageChangeHelper
    {
        private static ImageChangeHelper instance = null;
        private static object o = new object();
        private ImageChangeHelper()
        {
        }
        public static ImageChangeHelper Instance
        {
            get
            {
                lock (o)
                {
                    if (instance == null)
                    {
                        instance = new ImageChangeHelper();
                    }
                    return instance;
                }

            }
        }
     
        public  byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            BitmapData bitmapData = null;
            try
            {
                bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int num = bitmapData.Stride * bitmap.Height;
                byte[] array = new byte[num];
                Marshal.Copy(bitmapData.Scan0, array, 0, num);
                if (bitmapData.Stride == bitmap.Width * 3)
                {
                    return array;
                }
                if (bitmapData.Stride == bitmap.Width)
                {
                    return array;
                }
                byte[] array2 = new byte[bitmap.Width * 3 * bitmap.Height];
                for (int i = 0; i < bitmapData.Height; i++)
                {
                    Buffer.BlockCopy(array, i * bitmapData.Stride, array2, i * bitmap.Width * 3, bitmap.Width * 3);
                }
                return array2;
            }
            finally
            {
                if (bitmapData != null)
                {
                    bitmap.UnlockBits(bitmapData);
                }
            }
        }

        public Bitmap MakeGrayscale(Bitmap original)
        {
            Bitmap bitmap = new Bitmap(original.Width, original.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            ColorMatrix colorMatrix = new ColorMatrix(new float[5][]
            {
            new float[5]
            {
                0.3f,
                0.3f,
                0.3f,
                0f,
                0f
            },
            new float[5]
            {
                0.59f,
                0.59f,
                0.59f,
                0f,
                0f
            },
            new float[5]
            {
                0.11f,
                0.11f,
                0.11f,
                0f,
                0f
            },
            new float[5]
            {
                0f,
                0f,
                0f,
                1f,
                0f
            },
            new float[5]
            {
                0f,
                0f,
                0f,
                0f,
                1f
            }
            });
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);
            graphics.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, imageAttributes);
            graphics.Dispose();
            return bitmap;
        }

        public  Bitmap Convert24T8(Bitmap SImage)
        {
            Bitmap bitmap = new Bitmap(SImage.Width, SImage.Height, PixelFormat.Format8bppIndexed);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = SImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData bitmapData2 = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            IntPtr scan = bitmapData.Scan0;
            IntPtr scan2 = bitmapData2.Scan0;
            int num = bitmapData.Stride * bitmapData.Height;
            int num2 = bitmapData2.Stride * bitmapData2.Height;
            int num3 = bitmapData2.Stride - bitmap.Width;
            byte[] array = new byte[num];
            byte[] array2 = new byte[num2];
            Marshal.Copy(scan, array, 0, num);
            Marshal.Copy(scan2, array2, 0, num2);
            int num4 = 0;
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width * 3; j += 3)
                {
                    int num5 = bitmapData.Stride * i + j;
                    double num6 = (double)(int)array[num5 + 2] * 0.299 + (double)(int)array[num5 + 1] * 0.587 + (double)(int)array[num5] * 0.114;
                    array2[num4] = (byte)num6;
                    num4++;
                }
                num4 += num3;
            }
            Marshal.Copy(array, 0, scan, num);
            Marshal.Copy(array2, 0, scan2, num2);
            ColorPalette palette;
            using (Bitmap bitmap2 = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
            {
                palette = bitmap2.Palette;
            }
            for (int k = 0; k < 256; k++)
            {
                palette.Entries[k] = Color.FromArgb(k, k, k);
            }
            bitmap.Palette = palette;
            SImage.UnlockBits(bitmapData);
            bitmap.UnlockBits(bitmapData2);
            return bitmap;
        }
        public  byte[] Rgb2Gray(Bitmap temp)
        {
            bool bRet = false;
            int nWidth = temp.Width;
            int nHeight = temp.Height;
            int nDeep = temp.PixelFormat == PixelFormat.Format8bppIndexed ? 1 : (temp.PixelFormat == PixelFormat.Format32bppRgb || temp.PixelFormat == PixelFormat.Format32bppArgb
                      || temp.PixelFormat == PixelFormat.Format32bppPArgb) ? 4 : 3;

            byte[] byBuffer = new byte[(uint)nWidth * (uint)nHeight * nDeep];
            byte[] byBufferY = new byte[(uint)nWidth * (uint)nHeight * 1];
            if (temp.PixelFormat == PixelFormat.Format24bppRgb
                || temp.PixelFormat == PixelFormat.Format32bppRgb
                 || temp.PixelFormat == PixelFormat.Format32bppArgb
                 || temp.PixelFormat == PixelFormat.Format32bppPArgb
                )
            {
                byBuffer = ImageChangeHelper.Instance.ConvertBitmapToByteArray(temp);
                ImageConvert ic = new ImageConvert();
                ic.BGR_To_Y(byBuffer, byBufferY, (uint)nWidth, (uint)nHeight);
            }
            else
            {
                byBufferY = ImageChangeHelper.Instance.ConvertBitmapToByteArray(temp);
            }
            return byBufferY;
        }
        public  Bitmap ConvertBinaryToBitmap(byte[] imageData, int nWidth, int nHeight)
        {
            Bitmap bitmap = new Bitmap(nWidth, nHeight, PixelFormat.Format8bppIndexed);
            try
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, nWidth, nHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                IntPtr scan = bitmapData.Scan0;
                int length = bitmapData.Stride * nHeight;
                Marshal.Copy(imageData, 0, scan, length);
                bitmap.UnlockBits(bitmapData);
                ColorPalette palette = bitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                bitmap.Palette = palette;
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return bitmap;
            }
        }
        public  void Bitmap2HObject(Bitmap bmp, ref HObject image)
        {

            try
            {
                Bitmap bitmap = (Bitmap)bmp.Clone();
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    HOperatorSet.GenImageInterleaved(out image, srcBmpData.Scan0, "bgr", bmp.Width, bmp.Height, 0, "byte", 0, 0, 0, 0, -1, 0);
                }
                else
                {
                    HOperatorSet.GenImage1(out image, "byte", bmp.Width, bmp.Height, srcBmpData.Scan0);
                }
                bitmap.UnlockBits(srcBmpData);
                bitmap?.Dispose();

            }
            catch (Exception ex)
            {
                image?.Dispose();
            }
        }






    }
}
