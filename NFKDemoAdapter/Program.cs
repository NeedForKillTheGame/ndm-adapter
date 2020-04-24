using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
                Application.Run(new Form1());
            else
            {
                var np = new NFKProcess();
                var demoUrl = getPath(args[0]);
                Application.Run(new Form2(demoUrl, np));
                if (!String.IsNullOrEmpty(NFKHelper.DemoFile))
                {
                    // show video intro after the download form
                    if (np.Intro != null && !np.Intro.CloseFlag)
                        Application.Run(np.Intro);
                }
                if (np.StubForm != null && !np.StubForm.CloseFlag)
                    Application.Run(np.StubForm);
/*
                while (np.GameRunning)
                {
                    //if (np.GetProcess() == null || np.GetProcess().HasExited)
                    //    break;
                    Thread.Sleep(1000);
                }
*/
            }

        }

        private static string getPath(string arg)
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
