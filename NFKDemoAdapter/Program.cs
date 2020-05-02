using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFKDemoAdapter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // set allowed protocol to download from
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
                Application.Run(new Form1());
            else
            {
                var np = new NFKProcess();
                // connect to server
                if (args[0].StartsWith("nfk://"))
                {
                    var serverAddress = getServerAddress(args[0]);
                    var spectator = !args[0].Contains("play");
                    if (!NFKHelper.SendPing(serverAddress))
                    {
                        Common.ShowError("Connection timeout", "Could not connect to NFK server " + serverAddress);
                        return;
                    }
                    if (!spectator)
                    {
                        // if play mode then just run a game and exit
                        np.RunGame(serverAddress);
                        return;
                    }
                    // run nfk
                    np.Run(serverAddress, false);

                    np.WatchForExit();
                }
                // play demo
                else
                {
                    var demoUrl = getDemoPath(args[0]);
                    Application.Run(new Form2(demoUrl, np));
                    if (!String.IsNullOrEmpty(NFKHelper.DemoFile))
                    {
                        // show video intro after the download form
                        if (np.Intro != null && !np.Intro.CloseFlag && np.GameRunning)
                            Application.Run(np.Intro);
                    }
                }
                // this form is used to keep program running
                if (np.StubForm != null && !np.StubForm.CloseFlag && np.GameRunning)
                    Application.Run(np.StubForm);
            }

        }
        private static string getServerAddress(string arg)
        {
            string url = arg.Replace("nfk://", "");
            return dropQueryUrl(url);
        }

        /// <summary>
        /// Take only useful part and drop all text after a slash "/"
        /// </summary>
        /// <returns></returns>
        private static string dropQueryUrl(string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if (c == '/')
                    break;
                sb.Append(c);
            }
            return sb.ToString();
        }
        private static string getDemoPath(string arg)
        {
            string url = arg.Replace("nfkdemo://", "");
            long demoId;

            // support local files
            if (File.Exists(url))
            {
                return url;
            }
            // native support for demos from web stats by id
            if (long.TryParse(url.Replace("/", ""), out demoId))
            {
                url = "https://stats.needforkill.ru/demo/" + demoId;
            }
            // other urls without http
            if (!url.StartsWith("http"))
                url = "http://" + url;

            return url;
        }
    }
}
