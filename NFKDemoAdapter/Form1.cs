using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFKDemoAdapter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private Regedit wr;

        private void TxtGamePath_MouseClick(object sender, MouseEventArgs e)
        {
            var d = new OpenFileDialog();
            d.Filter = "Need For Kill|NFK.exe";
            DialogResult result = d.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtGamePath.Text = d.FileName;
                Properties.Settings.Default.GameExePath = d.FileName;
                Properties.Settings.Default.Save();

                linkConfig.Visible = true;
                chkLinkHandler.Enabled = chkFileHandler.Enabled = true;
                // set handlers by default
                chkLinkHandler.Checked = chkFileHandler.Checked = true;
            }
        }

        private bool userMode = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            wr = new Regedit();

            // set version
            lvlVersion.Text = string.Format("v{0}", Common.ProgramVersion);
            // check new version in separate thread
            Task.Factory.StartNew(Common.CheckNewVersion);

            chkLinkHandler.Enabled = chkFileHandler.Enabled = false;
            linkConfig.Visible = false;

            // load settings
            if (File.Exists(Properties.Settings.Default.GameExePath))
            {
                txtGamePath.Text = Properties.Settings.Default.GameExePath;
                chkLinkHandler.Enabled = chkFileHandler.Enabled = true;
                linkConfig.Visible = true;
            }

            userMode = false;
            chkLinkHandler.Checked = wr.HasLinkHandler();
            chkFileHandler.Checked = wr.HasFileHandler();
            userMode = true;
        }

        private void ChkLinkHandler_CheckedChanged(object sender, EventArgs e)
        {
            if (!userMode)
                return;

            // register handler
            if (chkLinkHandler.Checked)
            {
                if (!wr.RegisterHandler(Regedit.Handler.Link))
                { 
                    userMode = false;
                    // uncheck if operation was not successfull
                    chkLinkHandler.Checked = false;
                    userMode = true;
                }
            }
            // the same for unregister
            else
            {
                if (!wr.UnregisterHandler(Regedit.Handler.Link))
                {
                    userMode = false;
                    chkLinkHandler.Checked = true;
                    userMode = true;
                }
            }
        }

        private void ChkFileHandler_CheckedChanged(object sender, EventArgs e)
        {
            if (!userMode)
                return;

            // register handler
            if (chkFileHandler.Checked)
            {
                if (!wr.RegisterHandler(Regedit.Handler.File))
                {
                    userMode = false;
                    // uncheck if operation was not successfull
                    chkFileHandler.Checked = false;
                    userMode = true;
                }
            }
            // the same for unregister
            else
            {
                if (!wr.UnregisterHandler(Regedit.Handler.File))
                {
                    userMode = false;
                    chkFileHandler.Checked = true;
                    userMode = true;
                }
            }
        }

        private void LinkConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var fileName = NFKHelper.GetOrCreateConfigFile();
            var p = Process.Start("explorer.exe", "/select, \"" + fileName + "\"");
        }

        private void TxtGamePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void LvlVersion_Click(object sender, EventArgs e)
        {
            Common.OpenURL(Common.ProgramHomePage);
        }

        /*
        private void LnkDemoExample_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var p = new Process())
            {
                p.StartInfo = new ProcessStartInfo()
                {
                    FileName = "explorer.exe",
                    Arguments = lnkDemoExample.Text
                };
                p.Start();
            }
        }
        */
    }
}
