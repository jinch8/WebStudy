using System;
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
            //var url = @"http://img.blog.csdn.net/20130930152314703?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvamFja3lzdHVkaW8=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast";
            //var url = @"https://images2017.cnblogs.com/news/24442/201712/24442-20171206093739878-889349568.png";
            var url = @"http://img.blog.csdn.net/20130930152647500?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvamFja3lzdHVkaW8=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast";
            //HttpDownLoad.DownloadFileByAria2(url, Path.Combine(Environment.CurrentDirectory, @"3.jpg"));
            var request = WebRequest.Create(url);
            //var response = (HttpWebResponse)request.GetResponse();
            var response = request.GetResponse();
            //var headers = response.Headers;
            LogHelper.WriteLine("---------");
            foreach (string key in response.Headers.AllKeys)
            {
                LogHelper.WriteLine(key + ":" + response.Headers[key]);    
            }

            Uri uri = new Uri(url);
            //if (uri.IsFile)
            //{
            string localPath = uri.LocalPath;
            string absolutePath = uri.AbsolutePath;
                string filename = System.IO.Path.GetFileName(uri.LocalPath);
            //}

            //LogHelper.WriteLine(response.Headers[HttpRequestHeader.ContentLocation]);
            //Stream stream = response.GetResponseStream();
            //StreamReader sr = new StreamReader(stream);
            //string buff;
            //while ((buff = sr.ReadLine()) != null)
            //{
            //    LogHelper.WriteLine(buff);
            //}
            //LogHelper.WriteLine(response.ContentLength.ToString());
            //foreach (var header in headers)
            //{
            //    LogHelper.WriteLine(header.ToString());
            //    //MessageBox.Show(header.ToString());
            //}
            //var valueCollect = HttpHelper.GetNameValueCollection(url);
            //foreach (var v in valueCollect)
            //{
            //    MessageBox.Show(v.ToString());
            //}
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

        private void button6_Click(object sender, EventArgs e)
        {
            string url = textBox.Text;
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
            List<string> imgList = new List<string>();
            foreach (var img in imgs)
            {
                LogHelper.WriteLine(img.Src);
                imgList.Add(img.Src);
            }

            //LogHelper.WriteLine(HttpHelper.GetHtmlTitle(content));

            foreach (string address in imgList)
            {
                try
                {
                    string[] splits = address.Split('/');
                    Uri uri = new Uri(address);
                    string file = uri.LocalPath;
                    //string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"collect\" + splits[splits.Length - 1]);
                    string ext = splits[splits.Length - 1];
                    string[] arr = ext.Split('.');
                    ext = arr[arr.Length - 1];
                    string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"collect\" + file);
                    fileName.Replace("?", "");
                    HttpDownLoad.DownloadFileByAria2(address, fileName);
                    LogHelper.WriteLine("Dowoloaded " + fileName);
                }
                catch (System.Exception ex)
                {
                    continue;
                }
                

            }

            MessageBox.Show("ok");
            MessageBox.Show("Test1");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //string url = textBox.Text.Trim();
            //WebRequest request = WebRequest.Create(url);
            //WebResponse response = request.GetResponse();
            //Stream stream = response.GetResponseStream();
            //StreamReader sr = new StreamReader(stream);
            //LogHelper.WriteLine(sr.ReadToEnd());
            //MessageBox.Show("ok");
            MessageBox.Show(Environment.CommandLine);
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
            LogHelper.WriteLine(a);
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
