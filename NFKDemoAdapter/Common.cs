using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFKDemoAdapter
{
    public static class Common
    {
        public static string ProgramVersion
        {
            get
            {
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0}.{1}", ver.Major, ver.Minor);
            }
        }

        public static string ProgramHomePage = "https://github.com/NeedForKillTheGame/ndm-adapter";

        public static void ShowError(string caption, string message)
        {
            ShowMessage(caption, message, MessageBoxIcon.Error);
        }
        public static void ShowMessage(string caption, string message, MessageBoxIcon type = MessageBoxIcon.Information)
        {
            MessageBox.Show(caption, message, MessageBoxButtons.OK, type);
        }

        public static string GetFilenameFromWebServer(string url)
        {
            // set allowed protocol to download from
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            string fileName = "";

            var req = (HttpWebRequest)System.Net.WebRequest.Create(url);
            req.AllowAutoRedirect = true;
            req.Method = "HEAD";
            using (System.Net.WebResponse resp = req.GetResponse())
            {
                // Try to extract the filename from the Content-Disposition header
                if (!string.IsNullOrEmpty(resp.Headers["Content-Disposition"]))
                {
                    fileName = resp.Headers["Content-Disposition"].Substring(resp.Headers["Content-Disposition"].IndexOf("filename=") + 10).Replace("\"", "");
                }
                else
                {
                    fileName = Path.GetFileName(resp.ResponseUri.LocalPath);
                }
            }

            return fileName;
        }



        public static void CheckNewVersion()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    var remoteVersion = wc.DownloadString("https://raw.githubusercontent.com/NeedForKillTheGame/ndm-adapter/master/version.txt");
                    remoteVersion = remoteVersion.Trim();
                    if (ProgramVersion != remoteVersion)
                    {
                        var result = MessageBox.Show("A new version of the program is available (" + remoteVersion + ")!\nDo you want open the website for download?", "New version released!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (result == DialogResult.Yes)
                        {
                            OpenURL(ProgramHomePage);
                        }
                    }
                }
            }
            catch
            { }
        }

        internal static void OpenURL(string url)
        {
            using (var p = new Process())
            {
                p.StartInfo = new ProcessStartInfo()
                {
                    FileName = "explorer.exe",
                    Arguments = url
                };
                p.Start();
            }
        }
    }
}
