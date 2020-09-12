using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
namespace UserData
{
    public class ImageHelper
    {
        private static ImageHelper instance = null;
        private static object o = new object();
        private ImageHelper()
        {
        }
        public static ImageHelper Instance
        {
            get
            {
                lock (o)
                {
                    if (instance == null)
                    {
                        instance = new ImageHelper();
                    }
                    return instance;
                }

            }
        }

        public void SaveImage(string fullName, Bitmap image)
        {
            Task.Run(() =>
            {
                image.Save(fullName, ImageFormat.Bmp);
            });
        }
        public void SaveImage(string fullName, string type, HObject image)
        {
            Task.Run(() =>
            {
                HOperatorSet.WriteImage(image, type, 0, fullName);
            });
        }
        public void SaveBitmapToFile(Bitmap bmp, string strImgPath = "", string strImgName = "", bool bIsCheckLastModifiedDate = false)
        {
            try
            {
                if (string.Compare(strImgName, "") == 0)
                {
                    strImgName = string.Format("{0}.bmp", DateTime.Now.ToString("HHmmssff"));
                }
                FileInfo fileInfo = new FileInfo(strImgPath);
                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }
                if (strImgPath != "" && bIsCheckLastModifiedDate)
                {
                    foreach (string item in from s in Directory.EnumerateFiles(strImgPath, "*.*", SearchOption.AllDirectories)
                                            where s.EndsWith(".bmp")
                                            select s)
                    {
                        FileInfo fileInfo2 = new FileInfo(item);
                        if (fileInfo2.LastWriteTime < DateTime.Now.AddDays(-1.0))
                        {
                            fileInfo2.Delete();
                        }
                    }
                }
                bmp.Save(strImgPath + strImgName, ImageFormat.Bmp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void CopyImageToFail(string FullDirName, bool Result)
        {
            //Result==false当前图片拷贝到NG目录
            //Result==true当前图片拷贝到OK目录
            Task.Run(() =>
            {
                if (Result)
                    CopyDir($"{PathHelper.ImageRunPath}{FullDirName}\\", $"{PathHelper.ImageOKPath}{FullDirName}\\");
                else
                    CopyDir($"{PathHelper.ImageRunPath}{FullDirName}\\", $"{PathHelper.ImageNGPath}{FullDirName}\\" );
                DeleteDir($"{PathHelper.ImageRunPath}{FullDirName}\\");
            });
        }
        public void DeleImage(double PassTimeDay, double FailTimeDay)
        {
            //删除OK图片
            //删除NG图片
            Task.Run(() =>
            {
                DeleteImage(PathHelper.ImagePathDelete, PassTimeDay, FailTimeDay);
            });

        }


        private static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                if (aimPath[aimPath.Length - 1] != System.IO.Path.AltDirectorySeparatorChar)
                {
                    aimPath += System.IO.Path.DirectorySeparatorChar;
                }
                if (!System.IO.Directory.Exists(aimPath))
                {
                    Directory.CreateDirectory(aimPath);
                }
                string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
                foreach (string file in fileList)
                {
                    if (System.IO.Directory.Exists(file))
                    {
                        CopyDir(file, aimPath + Path.GetFileName(file));
                    }
                    else
                    {
                        try
                        {
                            string fileFullName = aimPath + Path.GetFileName(file);
                            if (fileFullName.Contains(".bmp") || fileFullName.Contains(".png") || fileFullName.Contains(".jpg"))
                            {
                                File.Copy(file, fileFullName, true);
                                File.Delete(file);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch
            {
            }
        }
        public static void DeleteDir(string srcPath)
        {
            try
            {
                string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
                foreach (string file in fileList)
                {
                    if (Directory.GetDirectories(file).Count() == 0)
                    {
                        Directory.Delete(file);
                    }
                    else
                    {
                        DeleteDir(file);
                    }
                }
            }
            catch
            {
            }
        }
        private static void DeleteImage(string imgPath, double PassTimeDay, double FailTimeDay)
        {
            try
            {
                string[] fileList = System.IO.Directory.GetFileSystemEntries(imgPath);
                foreach (string file in fileList)
                {
                    if (System.IO.Directory.Exists(file))
                    {
                        DeleteImage(file, PassTimeDay, FailTimeDay);
                    }
                    else
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if (file.Contains(".bmp") || file.Contains(".png") || file.Contains(".jpg"))
                        {
                            if (file.Contains("\\OK\\"))
                            {
                                if (fileInfo.CreationTime < DateTime.Now.AddDays(-PassTimeDay))
                                {
                                    try
                                    {
                                        fileInfo.Delete();
                                    }
                                    catch { }
                                }
                            }
                            if (file.Contains("\\NG\\"))
                            {
                                if (fileInfo.CreationTime < DateTime.Now.AddDays(-FailTimeDay))
                                {
                                    try
                                    {
                                        fileInfo.Delete();
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
