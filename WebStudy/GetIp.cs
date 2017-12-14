﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using FlyPig.HttpHelper;
using FlyPig.HttpHelper.Item;
using FlyPig.Utility;

namespace WebStudy
{
    public partial class GetIp : Form
    {
        public GetIp()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Dns.GetHostName());
            MessageBox.Show(Environment.MachineName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //IPHostEntry ipHostEntry;

            //ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());

            //if (ipHostEntry != null)
            //{
            //    for (int i = 0; i < ipHostEntry.AddressList.Length; i++)
            //    {
            //        if (ipHostEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
            //        {
            //            MessageBox.Show(ipHostEntry.AddressList[i].ToString());
            //        }
            //    }
            //}

            //IPHostEntry ipHostEntry = Dns.Resolve("www.baidu.com");
            
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ipAddress in ipHostEntry.AddressList)
            {
                MessageBox.Show(ipAddress.ToString());

            }

        }

        [DllImport("wininet")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);


        /// <summary>
        /// 检测本机是否联网
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedInternet()
        {
            int i = 0;
            if (InternetGetConnectedState(out i, 0))
            {
                //已联网
                return true;
            }
            else
            {
                //未联网
                return false;
            }

        }

        private void btnGetPicList_Click(object sender, EventArgs e)
        {
            string url = @"http://www.baidu.com";
            


            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream, Encoding.UTF8);
            string str = sr.ReadToEnd();

            //var matches = Regex.Matches(str, "<a href=\"([^\"]*?)\".*?>(.*?)</a>", RegexOptions.IgnoreCase);
            var matches = Regex.Matches(str, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                //listBox.Items.Add(match.Value.ToString());
                listBox.Items.Add(match.Groups[1].Value);
                
            }
        }

        private List<string> _picExtents = new List<string> {".jpg", ".gif", ".png"};

        private void button3_Click(object sender, EventArgs e)
        {
            string url = textBox.Text;
            if (url == string.Empty) return;
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);

            string content = sr.ReadToEnd();

            var matches = Regex.Matches(content, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            List<string> exts = new List<string>();
            int i = 0;

            foreach (Match match in matches)
            {
                string picAddress = match.Groups[1].Value;
                if (picAddress == string.Empty) continue;
                listBox.Items.Add(picAddress);

                if (!picAddress.StartsWith("http")) continue;
                bool isFind = false;
                string ext = string.Empty;
                foreach (string extent in _picExtents)
                {
                    if (picAddress.EndsWith(extent))
                    {
                        isFind = true;
                        ext = extent;
                        break;
                    }
                }

                if (isFind)
                {
                    string filePath = string.Format(@"D:\CSharp\eyeshot\WebStudy\WebStudy\bin\Debug\imgCollect\{0}{1}", i++, ext);
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(picAddress, filePath);
                    Thread.Sleep(10);

                    //using (WebClient wc = new WebClient())
                    //{
                    //    //wc.DownloadFile("http://dat0a11.book.hexun.com/chapter-1031-1-7.shtml", filePath);

                    //    string html = wc.DownloadString(picAddress);
                    //    using (StreamWriter writer = new StreamWriter(filePath, false, wc.Encoding))
                    //    {
                    //        writer.Write(html);
                    //        writer.Flush();
                    //    }^
                    //}


                }

            }
            MessageBox.Show("ok");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string url = @"http://sports.sina.com.cn/";
            MessageBox.Show(HttpHelper.GetUrlIp(url));
            //List<AItem> ls = HttpHelper.GetAList(url);

            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream, Encoding.UTF8);

            string content = sr.ReadToEnd();

            //var ls = HttpHelper.GetAList(content);

            //foreach (var s in ls)
            //{
            //    if (s.Img == null) continue;
            //    listBox.Items.Add(s.Img.Src);
            //    LogHelper.WriteLine("src : " + s.Img.Src);
            //    LogHelper.WriteLine("html : " + s.Img.Html);
                
            //}

            var imgs = HttpHelper.GetImgList(content);
            LogHelper.WriteLine("---------------------");
            foreach (var img in imgs)
            {
                LogHelper.WriteLine(img.Src);
            }

            LogHelper.WriteLine(HttpHelper.GetHtmlTitle(content));
            MessageBox.Show("ok");
            MessageBox.Show("Test1");
            
                
            
            
        }

        internal static string GetUrlIp(string url)
        {
            string result;
            try
            {
                IPHostEntry hostByName = Dns.GetHostEntry(GetUrlHost(url));
                result = hostByName.AddressList[0].ToString();
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        internal static string GetUrlHost(string url)
        {
            string result;
            try
            {
                result = new Uri(url).Host;
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Environment.CurrentDirectory);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var url = @"https://mjcdn.cc/2/118966443/Tm93IE9yIE5ldmVyIGZlYXQuIFBob2ViZSBSeWFuIChUcml0b25hbCBDbHViIE1peCkubXAz";
            HttpDownLoad.DownloadFileByAria2(url, Path.Combine(Environment.CurrentDirectory, @"3.mp3"));
            MessageBox.Show("ok");
        }

        public static void RedirectExcuteProcess(Process p, string exe, string arg, DataReceivedEventHandler output)
        {
            p.StartInfo.FileName = exe;
            p.StartInfo.Arguments = arg;

            p.StartInfo.UseShellExecute = false;    //输出信息重定向
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;

            p.OutputDataReceived += output;
            p.ErrorDataReceived += output;

            p.Start();                    //启动线程
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();            //等待进程结束
        }

    }

    public class HttpDownLoad
    {

        public static bool DownloadFileByAria2(string url, string strFileName)
        {
            var tool = AppDomain.CurrentDomain.BaseDirectory + "\\aria2\\aria2c.exe";
            var fi = new FileInfo(strFileName);
            var command = " -c -s 5 --check-certificate=false -d " + fi.DirectoryName + " -o " + fi.Name + " " + url;
            Process _p;
            using (_p = new Process())
            {
                GetIp.RedirectExcuteProcess(_p, tool, command, (s, e) => ShowInfo(e.Data));
            }
            return true;
        }

        public static void ShowInfo(string a)
        {
            if (a == null) return;

            const string re1 = ".*?"; // Non-greedy match on filler
            const string re2 = "(\\(.*\\))"; // Round Braces 1

            var r = new Regex(re1 + re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var m = r.Match(a);
            if (m.Success)
            {
                var rbraces1 = m.Groups[1].ToString().Replace("(", "").Replace(")", "").Replace("%", "");
                if (rbraces1 == "OK")
                {
                    rbraces1 = "100";
                }
                Console.WriteLine(rbraces1);
            }
        }
    }
}
