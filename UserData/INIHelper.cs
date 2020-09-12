using System.Runtime.InteropServices;

namespace UserData
{
    using System.Collections.Concurrent;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System;

    public class INIHelper
    {

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static bool WriteToIni(string section, string key, string val, string filePath)
        {
            WritePrivateProfileString(section, key, val, filePath);
            return true;

        }
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">要读取的段落名</param>
        /// <param name="key">要读取的键</param>
        /// <param name="def">读取异常的情况下的缺省值</param>
        /// <param name="filePath">INI文件的完整路径和文件名</param>
        /// <returns></returns>
        public static string ReadFromIni(string section, string key, string def, string filePath)
        {

            StringBuilder retVal = new StringBuilder();
            GetPrivateProfileString(section, key, def, retVal, 500, filePath);
            return retVal.ToString().Trim();

        }

       
    }


}
