using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace WebStudy
{
    public partial class Browser : Form
    {
        public Browser()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var url = textBox.Text;

            if (!url.StartsWith(@"http://"))
            { 
                url = string.Format(@"http://{0}", url);
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));

            string content = sr.ReadToEnd();
            webBrowser.DocumentText = content;

            MessageBox.Show("2part test");
            
        }
    }
}
