namespace NFKDemoAdapter
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.txtGamePath = new System.Windows.Forms.TextBox();
            this.chkLinkHandler = new System.Windows.Forms.CheckBox();
            this.chkFileHandler = new System.Windows.Forms.CheckBox();
            this.linkConfig = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path to NFK.exe";
            // 
            // txtGamePath
            // 
            this.txtGamePath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtGamePath.Location = new System.Drawing.Point(12, 32);
            this.txtGamePath.Name = "txtGamePath";
            this.txtGamePath.ReadOnly = true;
            this.txtGamePath.Size = new System.Drawing.Size(368, 22);
            this.txtGamePath.TabIndex = 2;
            this.txtGamePath.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TxtGamePath_MouseClick);
            this.txtGamePath.TextChanged += new System.EventHandler(this.TxtGamePath_TextChanged);
            // 
            // chkLinkHandler
            // 
            this.chkLinkHandler.AutoSize = true;
            this.chkLinkHandler.Location = new System.Drawing.Point(12, 71);
            this.chkLinkHandler.Name = "chkLinkHandler";
            this.chkLinkHandler.Size = new System.Drawing.Size(203, 21);
            this.chkLinkHandler.TabIndex = 3;
            this.chkLinkHandler.Text = "Handler for links nfkdemo://";
            this.chkLinkHandler.UseVisualStyleBackColor = true;
            this.chkLinkHandler.CheckedChanged += new System.EventHandler(this.ChkLinkHandler_CheckedChanged);
            // 
            // chkFileHandler
            // 
            this.chkFileHandler.AutoSize = true;
            this.chkFileHandler.Location = new System.Drawing.Point(12, 98);
            this.chkFileHandler.Name = "chkFileHandler";
            this.chkFileHandler.Size = new System.Drawing.Size(269, 21);
            this.chkFileHandler.TabIndex = 3;
            this.chkFileHandler.Text = "Associate *.ndm files with the program";
            this.chkFileHandler.UseVisualStyleBackColor = true;
            this.chkFileHandler.CheckedChanged += new System.EventHandler(this.ChkFileHandler_CheckedChanged);
            // 
            // linkConfig
            // 
            this.linkConfig.Location = new System.Drawing.Point(218, 9);
            this.linkConfig.Name = "linkConfig";
            this.linkConfig.Size = new System.Drawing.Size(162, 23);
            this.linkConfig.TabIndex = 4;
            this.linkConfig.TabStop = true;
            this.linkConfig.Text = "ndmadapter.cfg";
            this.linkConfig.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.linkConfig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkConfig_LinkClicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 136);
            this.Controls.Add(this.linkConfig);
            this.Controls.Add(this.chkFileHandler);
            this.Controls.Add(this.chkLinkHandler);
            this.Controls.Add(this.txtGamePath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NFK Demo Adapter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGamePath;
        private System.Windows.Forms.CheckBox chkLinkHandler;
        private System.Windows.Forms.CheckBox chkFileHandler;
        private System.Windows.Forms.LinkLabel linkConfig;
    }
}

