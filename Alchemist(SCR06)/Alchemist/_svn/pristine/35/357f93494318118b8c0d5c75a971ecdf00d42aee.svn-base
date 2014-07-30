using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class wireChangeInfofrm : Form
    {
        delegate void refreshdelegate();

        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);
        }

        // 描画処理
        public void refresh()
        {
            if (checkBox1.Checked && checkBox2.Checked && checkBox3.Checked)
            {
                btnCLOSE.Enabled = true;
            }
            else
            {
                btnCLOSE.Enabled = false;
            }
        }

        public wireChangeInfofrm()
        {
            InitializeComponent();
        }

        private void wireChangeInfofrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        private void wireChangeInfofrm_Activated(object sender, EventArgs e)
        {
            string wirecode = "";
            string wirename = "";
            string wiretype = "";
            string wiresize = "";
            string guide1 = "";
            string guide2 = "";
            string blade = "";
            SCR06DBController db = Program.SCR06DB;
            R_Work workdata = new R_Work();
            db.dbGetStartResultWorkData(ref workdata);

            wirename = workdata.Sensyu + workdata.Size + workdata.Iro;
            wirecode = workdata.Densen_code;

            lblWIRENAME.Text = wirename;

            if (wirename != "" && wirecode.Length == 10)
            {
                wiretype = wirename.Substring(0, 5);
                wiresize = wirename.Substring(5, 6);

                if (db.dbGetGuideName(wiretype, wiresize, ref guide1, ref guide2) == SystemConstants.SQL_SUCCESS)
                {
                    lblGUIDENAME1.Text = guide1;
                    lblGUIDENAME2.Text = guide2;
                }

                if (db.dbGetBladeName(wiretype, wiresize, ref blade) == SystemConstants.SQL_SUCCESS)
                {
                    lblBLADENAME.Text = blade;
                }
            }

            //checkBox1.Checked = checkBox2.Checked = checkBox3.Checked = false;
            btnCLOSE.Enabled = false;
        }

        private void wireChangeInfofrm_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (; ; )
            {
                Invoke(new refreshdelegate(refresh));

                System.Threading.Thread.Sleep(100);

                if (backgroundWorker1.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void btnCLOSE_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            Visible = false;           
        }
    }
}
