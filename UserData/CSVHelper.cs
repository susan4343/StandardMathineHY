using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using BaseDll;

namespace UserData
{
    public class CSVHelper
    {
        private static CSVHelper instance = null;
        private static object o = new object();
        private CSVHelper()
        {
        }
        public static CSVHelper Instance
        {
            get
            {
                lock (o)
                {
                    if (instance == null)
                    {
                        instance = new CSVHelper();
                    }
                    return instance;
                }

            }
        }
        /// <summary>
        /// 带路径CSV保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="Result"></param>
        public string SaveToCSVPath<T>(string path, T Result)
        {
            string err = "";
            string p = "";
            if (path.Contains(".csv"))
            {
                p = Directory.GetParent(path).FullName;
            }
            else
            {
                p = path;
                //if (p.EndsWith("\\\\"))
                //{
                //    path =$"{p}{DateTime.Now.ToString("yyyyMMdd")}.csv";
                //}
                //else
                //{
                path = $"{p}\\{DateTime.Now.ToString("yyyy-MM-dd")}.csv";
                //}                              
            }

            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);
            }

            if (Result == null)
            {
                return err = "数据为空";
            }
            try
            {
                //多写入一笔表头
                if (!File.Exists(path))
                {
                    FileStream fsHead = new FileStream(path, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);

                    if (fsHead != null)
                    {
                        StreamWriter swHead = new StreamWriter(fsHead, System.Text.Encoding.UTF8);
                        var properties = Result.GetType().GetProperties();
                        string strRowValue = "";
                        // 寫入新一筆資料
                        foreach (var item in properties)
                        {
                            strRowValue += $"{item.Name}, ";
                        }
                        strRowValue = strRowValue.Substring(0, strRowValue.Length - 2);
                        swHead.WriteLine(strRowValue);
                        swHead.Close();
                        fsHead.Close();
                    }
                }
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);

                if (fs != null)
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    var properties = Result.GetType().GetProperties();
                    string strRowValue = "";
                    // 寫入新一筆資料
                    foreach (var item in properties)
                    {
                        strRowValue += $"{item.GetValue(Result)}, ";
                    }
                    strRowValue = strRowValue.Substring(0, strRowValue.Length - 2);
                    sw.WriteLine(strRowValue);
                    sw.Close();
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"csv,{path}文件保存异常，{e.ToString()}。", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            return err;
        }
        /// <summary>
        /// 选择需要保存的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="Result"></param>
        /// <param name="EnableValue">属性名称列表</param>
        public string SaveToCSVPath<T>(string path, T Result, List<string> EnableValue)
        {
            string err = "";
            string p = "";
            if (path.Contains(".csv"))
            {
                p = Directory.GetParent(path).FullName;
            }
            else
            {
                p = path;
                //if (p.EndsWith("\\\\"))
                //{
                //    path = $"{p}EnableValue{DateTime.Now.ToString("yyyyMMdd")}.csv";
                //}
                //else
                //{
                path = $"{p}\\EnableValue{DateTime.Now.ToString("yyyyMMdd")}.csv";
                //}


            }
            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);
            }
            if (Result == null)
            {
                return err = "数据为空";
            }
            try
            {              //多写入一笔表头
                if (!File.Exists(path))
                {
                    FileStream fsHead = new FileStream(path, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);

                    if (fsHead != null)
                    {
                        StreamWriter swHead = new StreamWriter(fsHead, System.Text.Encoding.UTF8);
                        var properties = Result.GetType().GetProperties();
                        string strRowValue = "";
                        // 寫入新一筆資料
                        foreach (var item in properties)
                        {
                            if (EnableValue.Contains(item.Name))
                            {
                                strRowValue += $"{item.Name}, ";
                            }
                        }
                        strRowValue = strRowValue.Substring(0, strRowValue.Length - 2);
                        swHead.WriteLine(strRowValue);
                        swHead.Close();
                        fsHead.Close();
                    }
                }
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);
                if (fs != null)
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    var properties = Result.GetType().GetProperties();
                    // 寫入新一筆資料
                    string strRowValue = "";
                    foreach (var item in properties)
                    {
                        if (EnableValue.Contains(item.Name))
                        {
                            strRowValue += $"{item.GetValue(Result)}, ";
                        }
                    }
                    strRowValue = strRowValue.Substring(0, strRowValue.Length - 2);
                    sw.WriteLine(strRowValue);
                    sw.Close();
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"csv,{path}文件保存异常，{e.ToString()}。", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            return err;
        }
        public string SaveSFRInfoData(string sn, dynamic sFRInfos, string times)
        {
            string err = "";
            try
            {
                FileStream fs = new FileStream(PathHelper.SFRResultCsvPath, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);
                if (fs != null)
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    sw.WriteLine(string.Format("SN,{0},Time,{1}", sn, DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString()));
                    bool bCenterTop = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterTop");
                    bool bCenterLeft = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterLeft");
                    bool bCenterBottom = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterBottom");
                    bool bCenterRight = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCenterRight");

                    bool bCorner1Top = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Top");
                    bool bCorner1Left = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Left");
                    bool bCorner1Bottom = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Bottom");
                    bool bCorner1Right = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner1Right");

                    bool bCorner2Top = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Top");
                    bool bCorner2Left = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Left");
                    bool bCorner2Bottom = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Bottom");
                    bool bCorner2Right = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCorner2Right");

                    bool bCrossTop = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossTop");
                    bool bCrossLeft = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossLeft");
                    bool bCrossBottom = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossBottom");
                    bool bCrossRight = ParamSetMgr.GetInstance().GetBoolParam("[SFR] bCrossRight");


                    string[] data = new string[58];
                    string name = times == "1" ? "Before" : "After";
                    {
                        data[0] = $"{sn}_{name}_POS";
                        data[1] = $"{sn}_{name}_CT_Ave";
                        data[2] = $"{sn}_{name}_UL_Ave";
                        data[3] = $"{sn}_{name}_UR_Ave";
                        data[4] = $"{sn}_{name}_DL_Ave";
                        data[5] = $"{sn}_{name}_DR_Ave";
                        data[6] = $"{sn}_{name}_Center_U";
                        data[7] = $"{sn}_{name}_Center_R";
                        data[8] = $"{sn}_{name}_Center_D";
                        data[9] = $"{sn}_{name}_Center_L";//中心和平均的无论怎么选都打印
                        data[10] = bCorner1Top ? $"{sn}_{name}_Bevel_U_U" : "";
                        data[11] = bCorner1Top ? $"{sn}_{name}_Bevel_U_R" : "";
                        data[12] = bCorner1Top ? $"{sn}_{name}_Bevel_U_D" : "";
                        data[13] = bCorner1Top ? $"{sn}_{name}_Bevel_U_L" : "";
                        data[14] = bCorner1Right ? $"{sn}_{name}_Bevel_R_U" : "";
                        data[15] = bCorner1Right ? $"{sn}_{name}_Bevel_R_R" : "";
                        data[16] = bCorner1Right ? $"{sn}_{name}_Bevel_R_D" : "";
                        data[17] = bCorner1Right ? $"{sn}_{name}_Bevel_R_L" : "";
                        data[18] = bCorner1Bottom ? $"{sn}_{name}_Bevel_D_U" : "";
                        data[19] = bCorner1Bottom ? $"{sn}_{name}_Bevel_D_R" : "";
                        data[20] = bCorner1Bottom ? $"{sn}_{name}_Bevel_D_D" : "";
                        data[21] = bCorner1Bottom ? $"{sn}_{name}_Bevel_D_L" : "";
                        data[22] = bCorner1Left ? $"{sn}_{name}_Bevel_L_U" : "";
                        data[23] = bCorner1Left ? $"{sn}_{name}_Bevel_L_R" : "";
                        data[24] = bCorner1Left ? $"{sn}_{name}_Bevel_L_D" : "";
                        data[25] = bCorner1Left ? $"{sn}_{name}_Bevel_L_L" : "";
                        data[26] = bCorner2Top ? $"{sn}_{name}_Bevel2_U_U" : "";
                        data[27] = bCorner2Top ? $"{sn}_{name}_Bevel2_U_R" : "";
                        data[28] = bCorner2Top ? $"{sn}_{name}_Bevel2_U_D" : "";
                        data[29] = bCorner2Top ? $"{sn}_{name}_Bevel2_U_L" : "";
                        data[30] = bCorner2Right ? $"{sn}_{name}_Bevel2_R_U" : "";
                        data[31] = bCorner2Right ? $"{sn}_{name}_Bevel2_R_R" : "";
                        data[32] = bCorner2Right ? $"{sn}_{name}_Bevel2_R_D" : "";
                        data[33] = bCorner2Right ? $"{sn}_{name}_Bevel2_R_L" : "";
                        data[34] = bCorner2Bottom ? $"{sn}_{name}_Bevel2_D_U" : "";
                        data[35] = bCorner2Bottom ? $"{sn}_{name}_Bevel2_D_R" : "";
                        data[36] = bCorner2Bottom ? $"{sn}_{name}_Bevel2_D_D" : "";
                        data[37] = bCorner2Bottom ? $"{sn}_{name}_Bevel2_D_L" : "";
                        data[38] = bCorner2Left ? $"{sn}_{name}_Bevel2_L_U" : "";
                        data[39] = bCorner2Left ? $"{sn}_{name}_Bevel2_L_R" : "";
                        data[40] = bCorner2Left ? $"{sn}_{name}_Bevel2_L_D" : "";
                        data[41] = bCorner2Left ? $"{sn}_{name}_Bevel2_L_L" : "";
                        data[42] = bCrossTop ? $"{sn}_{name}_Cross_U_U" : "";
                        data[43] = bCrossTop ? $"{sn}_{name}_Cross_U_R" : "";
                        data[44] = bCrossTop ? $"{sn}_{name}_Cross_U_D" : "";
                        data[45] = bCrossTop ? $"{sn}_{name}_Cross_U_L" : "";
                        data[46] = bCrossRight ? $"{sn}_{name}_Cross_R_U" : "";
                        data[47] = bCrossRight ? $"{sn}_{name}_Cross_R_R" : "";
                        data[48] = bCrossRight ? $"{sn}_{name}_Cross_R_D" : "";
                        data[49] = bCrossRight ? $"{sn}_{name}_Cross_R_L" : "";
                        data[50] = bCrossBottom ? $"{sn}_{name}_Cross_D_U" : "";
                        data[51] = bCrossBottom ? $"{sn}_{name}_Cross_D_R" : "";
                        data[52] = bCrossBottom ? $"{sn}_{name}_Cross_D_D" : "";
                        data[53] = bCrossBottom ? $"{sn}_{name}_Cross_D_L" : "";
                        data[54] = bCrossLeft ? $"{sn}_{name}_Cross_L_U" : "";
                        data[55] = bCrossLeft ? $"{sn}_{name}_Cross_L_R" : "";
                        data[56] = bCrossLeft ? $"{sn}_{name}_Cross_L_D" : "";
                        data[57] = bCrossLeft ? $"{sn}_{name}_Cross_L_L" : "";
                    }
                    for (int j = 0; j < data.Count(); j++)
                    {

                    }

                    foreach (var a in sFRInfos)
                    {
                        data[0] += "," + a.dZ;

                        data[1] += "," + a.block[0].dValue.ToString();
                        data[2] += "," + a.block[1].dValue.ToString();
                        data[3] += "," + a.block[2].dValue.ToString();
                        data[4] += "," + a.block[3].dValue.ToString();
                        data[5] += "," + a.block[4].dValue.ToString();
                        for (int i = 0; i < 13; i++)
                        {
                            if (data[i * 4 + 6] != "")
                            {
                                data[i * 4 + 6] += "," + a.block[i].aryValue[1].ToString();
                                data[i * 4 + 7] += "," + a.block[i].aryValue[2].ToString();
                                data[i * 4 + 8] += "," + a.block[i].aryValue[3].ToString();
                                data[i * 4 + 9] += "," + a.block[i].aryValue[4].ToString();
                            }
                        }
                    }
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] != "")
                        {
                            sw.WriteLine(data[i]);
                        }
                    }

                    sw.WriteLine("");
                    sw.Close();
                    fs.Close();
                }
            }
            catch
            {
                // CSV Result 無法寫入
                err = $"Can't wrtie result into {PathHelper.SFRResultCsvPath}, please check the file isn't opened.";
            }
            return err;
        }

    }

}
