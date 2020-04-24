using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFKDemoAdapter
{
    public static class Common
    {
        public static void ShowError(string caption, string message)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

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

    }
}
