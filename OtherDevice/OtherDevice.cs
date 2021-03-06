﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaseDll;
using log4net;
using SerialDict;

namespace OtherDevice
{

    public static class OtherDevices
    {

       // public static COM_KEYENCE_Scanner cOM_KEYENCE_Scanner = new COM_KEYENCE_Scanner();

        public static ElecCmp elecCmp = new ElecCmp();

        public static CKPower ckPower = new CKPower();

    }

    /// <summary>
    /// 胶水个数设置（用于传递数据）
    /// </summary>
    public static class GlobalParaSet
    {
        public static int GuleCountPoint = 0;
        public static string GetModelNumber(string SN)
        {
            string content = "";
            try
            {
                HttpWebRequest httpWebRequest = WebRequest.Create($"http://huaweb11/sfcs/Service.asmx/GetModelNumber?snNumber={SN}") as HttpWebRequest;
                httpWebRequest.Host = "huaweb11";
                httpWebRequest.Method = "GET";

                HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse; // 获取响应
                if (httpWebResponse != null)
                {
                    using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        content = sr.ReadToEnd();
                    }

                    httpWebResponse.Close();
                }
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();//新建对象
                doc.LoadXml(content);//符合xml格式的字符串
                content = doc.InnerText;

            }
            catch(Exception ex)
            {
            }
            return content;
        }
    }


}