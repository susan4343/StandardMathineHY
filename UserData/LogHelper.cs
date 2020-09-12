using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Concurrent;

namespace UserData
{
    public partial class LogHelper : UserControl
    {
        //旧版本
        //private delegate void ShowChartDelegate(string SaveChartImagePath = "", LogType type = LogType.Info);
        //private static ShowChartDelegate EvenShowChartDelegate;
        //public LogHelper()
        //{
        //    EvenShowChartDelegate += show;
        //    InitializeComponent();
        //}
        //private readonly static object o = new object();
        //public string Path = "";
        //public async void Write(string message, LogType type = LogType.Info)
        //{
        //    await Task.Run(() =>
        //    {
        //        string value = "";
        //        //日志内容
        //        switch (type)
        //        {
        //            case LogType.Info:
        //                value = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ssfff")}] Info:{message}";
        //                break;
        //            case LogType.Wran:
        //                value = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ssfff")}] Warn:{message}";
        //                break;
        //            case LogType.Err:
        //                value = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ssfff")}] Err :{message}";
        //                break;
        //        }
        //        if (Path == "")
        //        {
        //            EvenShowChartDelegate(value, type);
        //        }
        //        else
        //        {
        //            string p = "";
        //            if (Path.Contains(".txt"))
        //            {
        //                p = Directory.GetParent(Path).FullName;
        //            }
        //            else
        //            {
        //                p = Path;
        //                Path = $"{p}\\{DateTime.Now.ToString("yyyy-MM-dd")}.txt";
        //            }

        //            if (!Directory.Exists(p))
        //            {
        //                Directory.CreateDirectory(p);
        //            }
        //            lock (o)
        //            {
        //                try
        //                {
        //                    EvenShowChartDelegate(value, type);
        //                    FileStream fs;
        //                    StreamWriter sw;
        //                    fs = new FileStream(Path, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);
        //                    sw = new StreamWriter(fs);

        //                    sw.WriteLine(value);
        //                    sw.Close();
        //                    fs.Close();

        //                }
        //                catch
        //                {

        //                }
        //            }
        //        }


        //    });


        //}
        //private void show(string mesg, LogType type = LogType.Info)
        //{
        //    if (InvokeRequired)
        //    {
        //        this.BeginInvoke(new Action(() => show(mesg, type)));
        //    }
        //    else
        //    {
        //        Color color = Color.Black;
        //        switch (type)
        //        {
        //            case LogType.Info:
        //                color = Color.Black;
        //                break;
        //            case LogType.Wran:
        //                color = Color.Blue;
        //                break;
        //            case LogType.Err:
        //                color = Color.Red;
        //                break;
        //        }
        //        mesg += Environment.NewLine;
        //        richTextBox1.SelectionStart = richTextBox1.TextLength;
        //        richTextBox1.SelectionLength = 0;
        //        richTextBox1.SelectionColor = color;
        //        richTextBox1.AppendText(mesg);
        //        richTextBox1.SelectionColor = richTextBox1.ForeColor;


        //    }
        //}

        //private void richTextBox1_TextChanged(object sender, EventArgs e)
        //{
        //    richTextBox1.SelectionStart = richTextBox1.Text.Length;
        //    richTextBox1.ScrollToCaret();

        //}

        BlockingCollection<LogClass> blockingCollection = new BlockingCollection<LogClass>();
        //FileStream f;//
        //StreamWriter writer;
        private delegate void ShowChartDelegate(string SaveChartImagePath = "", LogType type = LogType.Info);
        private static ShowChartDelegate EvenShowChartDelegate;
        public void Write(string Item, LogType logType = LogType.Info, string path = "")
        {
            blockingCollection.Add(new LogClass { Value = Item, LogType = logType, LogPath = path });
        }
        public LogHelper()
        {
            InitializeComponent();
            EvenShowChartDelegate += show;
            //f = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            //writer = new StreamWriter(f);
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.RunWorkerAsync();
        }
        private readonly static object o = new object();
        void backgroundWorker_DoWork(object sensor, DoWorkEventArgs e)
        {
            foreach (LogClass a in blockingCollection.GetConsumingEnumerable())
            {
                //Task.Run(() =>
                {
                    string value = "";
                    //日志内容
                    switch (a.LogType)
                    {
                        case LogType.Info:
                            value = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ssfff")}] Info:{a.Value}";
                            break;
                        case LogType.Wran:
                            value = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ssfff")}] Warn:{a.Value}";
                            break;
                        case LogType.Err:
                            value = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ssfff")}] Err :{a.Value}";
                            break;
                    }
                    if (a.LogPath == "")
                    {
                        EvenShowChartDelegate(value, a.LogType);
                    }
                    else
                    {
                        string p = "";
                        if (a.LogPath.Contains(".txt"))
                        {
                            p = Directory.GetParent(a.LogPath).FullName;
                        }
                        else
                        {
                            p = a.LogPath;
                            a.LogPath = $"{p}\\{DateTime.Now.ToString("yyyy-MM-dd")}.txt";
                        }

                        if (!Directory.Exists(p))
                        {
                            Directory.CreateDirectory(p);
                        }
                        EvenShowChartDelegate(value, a.LogType);
                        lock (o)
                        {
                            try
                            {
                        
                                FileStream fs;
                                StreamWriter sw;
                                fs = new FileStream(a.LogPath, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);
                                sw = new StreamWriter(fs);

                                sw.WriteLine(value);
                                sw.Close();
                                fs.Close();

                            }
                            catch(Exception ex)
                            {

                            }
                        }
                    }


                };

            }

        }
        private void show(string mesg, LogType type = LogType.Info)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => show(mesg, type)));
            }
            else
            {
                try {
                    Color color = Color.Black;
                    switch (type)
                    {
                        case LogType.Info:
                            color = Color.Black;
                            break;
                        case LogType.Wran:
                            color = Color.Blue;
                            break;
                        case LogType.Err:
                            color = Color.Red;
                            break;
                    }

                    //if (richTextBox1.TextLength>50*100)
                    //{
                    //    richTextBox1.Clear();
                    //}
                    mesg += Environment.NewLine;
                    richTextBox1.SelectionStart = richTextBox1.TextLength;
                    richTextBox1.SelectionLength = 0;
                    richTextBox1.SelectionColor = color;
                    richTextBox1.AppendText(mesg);
                    //richTextBox1.SelectionColor = richTextBox1.ForeColor;

                }
                catch { }
                

            }
        }

    }
    public enum LogType
    {
        Info,
        Wran,
        Err,
    }
    public class LogClass
    {
        public string LogPath;
        public LogType LogType;
        public string Value;
    }

}
