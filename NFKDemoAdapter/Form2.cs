using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFKDemoAdapter
{
    public partial class Form2 : Form
    {
        private string demoUrl;
        private NFKProcess np;

        public Form2(string demoUrl, NFKProcess np)
        {
            InitializeComponent();

            this.demoUrl = demoUrl;
            this.np = np;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            NFKHelper.DemoFile = string.Empty;
            np.Dispose();
            userMode = false;
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            progressBar1.MarqueeAnimationSpeed = 10;
        }


        private void Form2_Shown(object sender, EventArgs e)
        {
            lblUrl.Text = "Downloading " + this.demoUrl;

            NFKHelper.GetOrCreateConfigFile();

            try
            {
                // local file
                if (File.Exists(demoUrl))
                {
                    NFKHelper.DemoFile = demoUrl;
                    waitGameLoading();
                }
                // web link
                else
                {
                    downloadFile(demoUrl, NFKHelper.GetOrCreateDemoDownloadDir());
                }
            }
            catch (Exception ex)
            {
                Common.ShowError("Error", ex.Message);
                userMode = false;
                this.Close();
            }
        }


        private void downloadFile(string url, string dest)
        {
            using (var wc = new WebClient())
            {
                wc.DownloadProgressChanged += (s, e) =>
                {
                    progressBar1.Value = e.ProgressPercentage / 3;
                };

                wc.DownloadFileCompleted += (s, e) =>
                {
                    if (e.Error != null)
                    {
                        Common.ShowError("Download failed", e.Error.Message);
                        NFKHelper.DemoFile = string.Empty;
                    }

                    waitGameLoading();
                };
                
                NFKHelper.DemoFile = Path.Combine(dest, Common.GetFilenameFromWebServer(url));
                wc.DownloadFileAsync(new Uri(url), NFKHelper.DemoFile);
            }
        }

        /// <summary>
        /// Wait for game loading and adjust progress bar
        /// </summary>
        private void waitGameLoading()
        {
            lblUrl.Text = "Loading demo file ...";

            if (!NFKHelper.IsValidDemoFile(NFKHelper.DemoFile))
            {
                NFKHelper.DemoFile = null;
                Common.ShowError("Error", "Invalid NFK demo file\n" + NFKHelper.DemoFile);
                userMode = false;
                this.Close();
            }

            // run nfk
            np.Run(NFKHelper.DemoFile);
            np.WatchForExit();

            var gameLoadTime = Properties.Settings.Default.GameLoadTime > 0
                ? Properties.Settings.Default.GameLoadTime
                : NFKHelper.GAME_LOAD_TIME_DEFAULT;
            int interval = (int)(gameLoadTime - NFKHelper.INTRO_TIME) / (progressBar1.Maximum - progressBar1.Value);

            // run until prograss bar is filled for 100%
            for (; progressBar1.Value < progressBar1.Maximum; progressBar1.Value++)
            {
                // if abort button clicked
                if (String.IsNullOrEmpty(NFKHelper.DemoFile))
                    break;
                Thread.Sleep(interval);
                Application.DoEvents();
            }

            userMode = false;
            this.Close();
        }

        bool userMode = true;
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            // disable closing by ALT+F4
            if (userMode)
                e.Cancel = true;
        }
    }
}
