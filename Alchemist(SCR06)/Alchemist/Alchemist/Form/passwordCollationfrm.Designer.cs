namespace Alchemist
{
    partial class passwordCollationfrm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(passwordCollationfrm));
            this.lblPassword1 = new System.Windows.Forms.Label();
            this.btnPassCollation = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblPassword1
            // 
            resources.ApplyResources(this.lblPassword1, "lblPassword1");
            this.lblPassword1.ForeColor = System.Drawing.Color.White;
            this.lblPassword1.Name = "lblPassword1";
            // 
            // btnPassCollation
            // 
            this.btnPassCollation.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnPassCollation, "btnPassCollation");
            this.btnPassCollation.Name = "btnPassCollation";
            this.btnPassCollation.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // textPassword
            // 
            resources.ApplyResources(this.textPassword, "textPassword");
            this.textPassword.Name = "textPassword";
            this.textPassword.UseSystemPasswordChar = true;
            // 
            // passwordCollationfrm
            // 
            this.AcceptButton = this.btnPassCollation;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPassCollation);
            this.Controls.Add(this.textPassword);
            this.Controls.Add(this.lblPassword1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "passwordCollationfrm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.passwordCollationfrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label lblPassword1;
        private System.Windows.Forms.Button btnPassCollation;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.TextBox textPassword;
    }
}