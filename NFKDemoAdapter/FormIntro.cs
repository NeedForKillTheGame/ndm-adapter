using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFKDemoAdapter
{
    public partial class FormIntro : FormAutoClose
    {
        int w = 1270;
        int h = 720;

        public FormIntro()
        {
//#if !DEBUG
            this.TopMost = true;
//#endif
            this.Location = new Point(0, 0);
            this.StartPosition = FormStartPosition.Manual;
            this.ForeColor = Color.Black;
            this.ShowInTaskbar = false;


            // read options from the game config
            var lines = File.ReadAllLines(NFKHelper.GetOrCreateConfigFile());
            foreach (var l in lines)
            {
                // extract window size
                if (l.Trim().StartsWith("r_mode"))
                {
                    var parts = l.Trim().Split(' ');
                    if (parts.Length == 3)
                    {
                        int.TryParse(parts[1], out w);
                        int.TryParse(parts[2], out h);
                    }
                }
                // switch to full screen if game is zoomwindowed
                if (l.Trim().StartsWith("zoomwindow"))
                {
                    this.WindowState = FormWindowState.Maximized;
                }
            }

            InitializeComponent();
        }

        private void FormVideo_Load(object sender, EventArgs e)
        {
            Cursor.Hide();
            this.Width = w;
            this.Height = h;

            axWindowsMediaPlayer1.Dock = DockStyle.Fill;
            axWindowsMediaPlayer1.uiMode = "none";

            axWindowsMediaPlayer1.URL = getVideoFile();
            axWindowsMediaPlayer1.currentPlaylist = axWindowsMediaPlayer1.mediaCollection.getByName("nfkdemo");
            axWindowsMediaPlayer1.stretchToFit = true;

        }



        /// <summary>
        /// Put video file from resources to temp directory and return full path on it
        /// </summary>
        /// <returns></returns>
        private string getVideoFile()
        {
            var fileName = Path.Combine(Path.GetTempPath(), "nfkdemo.mp4");
            try
            {
                if (!File.Exists(fileName))
                    File.WriteAllBytes(fileName, Properties.Resources.intro);
            }
            catch
            {
                Common.ShowError("Error", "Could not save file\n" + fileName);
                this.Close();
            }
            return fileName;
        }





        #region fade out form effect

        Timer t1 = new Timer();

        void fadeOut(object sender, EventArgs e)
        {
            if (Opacity <= 0)     //check if opacity is 0
            {
                t1.Stop();    //if it is, we stop the timer
                Close();   //and we try to close the form
            }
            else
                Opacity -= 0.05;
        }

        private void FormIntro_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;    //cancel the event so the form won't be closed

            t1.Tick += new EventHandler(fadeOut);  //this calls the fade out function
            t1.Start();

            if (Opacity == 0)  //if the form is completly transparent
                e.Cancel = false;   //resume the event - the program can be closed

        }

        #endregion
    }
}
