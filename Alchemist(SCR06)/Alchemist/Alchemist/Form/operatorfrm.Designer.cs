namespace Alchemist
{
    partial class operatorfrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(operatorfrm));
            this.btnCLOSE = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtBARCODE = new Alchemist.CustomTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCLOSE
            // 
            resources.ApplyResources(this.btnCLOSE, "btnCLOSE");
            this.btnCLOSE.Name = "btnCLOSE";
            this.btnCLOSE.TabStop = false;
            this.btnCLOSE.UseVisualStyleBackColor = true;
            this.btnCLOSE.Click += new System.EventHandler(this.btnCLOSE_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtBARCODE);
            this.panel1.Controls.Add(this.btnCLOSE);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // txtBARCODE
            // 
            this.txtBARCODE.AllowAll = true;
            this.txtBARCODE.AllowDot = true;
            this.txtBARCODE.AllowHex = false;
            this.txtBARCODE.AllowSign = false;
            this.txtBARCODE.FocusColor = System.Drawing.Color.Pink;
            this.txtBARCODE.FocusStringSelect = true;
            resources.ApplyResources(this.txtBARCODE, "txtBARCODE");
            this.txtBARCODE.ForeColor = System.Drawing.Color.White;
            this.txtBARCODE.Name = "txtBARCODE";
            this.txtBARCODE.NormalColor = System.Drawing.Color.White;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Navy;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // operatorfrm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "operatorfrm";
            this.Activated += new System.EventHandler(this.operatorfrm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.operatorfrm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCLOSE;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private CustomTextBox txtBARCODE;
    }
}