using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFKDemoAdapter
{
    public class FormAutoClose : Form
    {
        /// <summary>
        /// Safe thread flag to close the form
        /// </summary>
        public bool CloseFlag = false;

        Timer t2 = new Timer();

        public FormAutoClose()
        {
            // wait when external class will set CloseFlag = true to close it
            // (do it with a flag because of different threads)
            t2.Interval = 200;
            t2.Tick += (object sender2, EventArgs e2) => {
                if (CloseFlag)
                    this.Close();
            };
            t2.Start();
        }


        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormAutoClose
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "FormAutoClose";
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.FormAutoClose_Load);
            this.ResumeLayout(false);

        }

        private void FormAutoClose_Load(object sender, EventArgs e)
        {

        }
    }
}
