namespace Alchemist
{
    partial class connectOperationfrm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(connectOperationfrm));
            this.panelOnline = new System.Windows.Forms.Panel();
            this.lblOnline = new System.Windows.Forms.Label();
            this.panelConnect = new System.Windows.Forms.Panel();
            this.btnDisConnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.panelClose = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelOnline.SuspendLayout();
            this.panelConnect.SuspendLayout();
            this.panelClose.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelOnline
            // 
            this.panelOnline.BackColor = System.Drawing.Color.Green;
            this.panelOnline.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelOnline.Controls.Add(this.lblOnline);
            resources.ApplyResources(this.panelOnline, "panelOnline");
            this.panelOnline.Name = "panelOnline";
            // 
            // lblOnline
            // 
            this.lblOnline.BackColor = System.Drawing.Color.Transparent;
            this.lblOnline.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lblOnline, "lblOnline");
            this.lblOnline.ForeColor = System.Drawing.Color.White;
            this.lblOnline.Name = "lblOnline";
            // 
            // panelConnect
            // 
            this.panelConnect.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelConnect.Controls.Add(this.btnDisConnect);
            this.panelConnect.Controls.Add(this.btnConnect);
            resources.ApplyResources(this.panelConnect, "panelConnect");
            this.panelConnect.Name = "panelConnect";
            // 
            // btnDisConnect
            // 
            resources.ApplyResources(this.btnDisConnect, "btnDisConnect");
            this.btnDisConnect.Name = "btnDisConnect";
            this.btnDisConnect.UseVisualStyleBackColor = true;
            this.btnDisConnect.Click += new System.EventHandler(this.btnDisConnect_Click);
            // 
            // btnConnect
            // 
            resources.ApplyResources(this.btnConnect, "btnConnect");
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // panelClose
            // 
            this.panelClose.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelClose.Controls.Add(this.btnClose);
            resources.ApplyResources(this.panelClose, "panelClose");
            this.panelClose.Name = "panelClose";
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // connectOperationfrm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.panelClose);
            this.Controls.Add(this.panelConnect);
            this.Controls.Add(this.panelOnline);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "connectOperationfrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.connectOperationfrm_FormClosing);
            this.panelOnline.ResumeLayout(false);
            this.panelConnect.ResumeLayout(false);
            this.panelClose.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelOnline;
        private System.Windows.Forms.Label lblOnline;
        private System.Windows.Forms.Panel panelConnect;
        private System.Windows.Forms.Button btnDisConnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Panel panelClose;
        private System.Windows.Forms.Button btnClose;
    }
}