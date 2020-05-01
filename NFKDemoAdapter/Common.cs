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
            MessageBox.Show(message, caption, MessageBoxButtons.OK, type);
        }

        public static string GetFilenameFromWebServer(string url)
        {

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

                    if (CompareVersions(ProgramVersion, remoteVersion) == -1)
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

        /// <summary>
        /// return -1 if v1 > v2 | 0 if v1 == v2 | 1 if v1 > v2
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private static int CompareVersions(string v1, string v2)
        {
            int maj1, min1, maj2, min2;

            var c1 = v1.Split('.');
            var c2 = v2.Split('.');
            if (c1.Length != 2 || c2.Length != 2)
                return 0;

            var smaj1 = c1[0].PadRight(3, '0');
            var smin1 = c1[1].PadRight(3, '0');
            var smaj2 = c2[0].PadRight(3, '0');
            var smin2 = c2[1].PadRight(3, '0');
            int.TryParse(smaj1, out maj1);
            int.TryParse(smin1, out min1);
            int.TryParse(smaj2, out maj2);
            int.TryParse(smin2, out min2);
            // compare major
            if (maj1 > maj2)
                return 1;
            else if (maj1 < maj2)
                return -1;
            // compare minor
            if (min1 > min2)
                return 1;
            else if (min1 < min2)
                return -1;
            return 0;
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
