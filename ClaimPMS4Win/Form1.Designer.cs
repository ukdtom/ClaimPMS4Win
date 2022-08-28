namespace ClaimPMS4Win
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
            this.rtbOut = new System.Windows.Forms.RichTextBox();
            this.tbUsrName = new System.Windows.Forms.TextBox();
            this.lblUsrName = new System.Windows.Forms.Label();
            this.lblPwd = new System.Windows.Forms.Label();
            this.tbPwd = new System.Windows.Forms.TextBox();
            this.lbl2Fa = new System.Windows.Forms.Label();
            this.tb2FA = new System.Windows.Forms.TextBox();
            this.btnClaim = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbOut
            // 
            this.rtbOut.Location = new System.Drawing.Point(12, 12);
            this.rtbOut.Name = "rtbOut";
            this.rtbOut.ReadOnly = true;
            this.rtbOut.Size = new System.Drawing.Size(427, 426);
            this.rtbOut.TabIndex = 1;
            this.rtbOut.Text = "";
            // 
            // tbUsrName
            // 
            this.tbUsrName.Location = new System.Drawing.Point(458, 28);
            this.tbUsrName.Name = "tbUsrName";
            this.tbUsrName.Size = new System.Drawing.Size(289, 20);
            this.tbUsrName.TabIndex = 2;
            // 
            // lblUsrName
            // 
            this.lblUsrName.AutoSize = true;
            this.lblUsrName.Location = new System.Drawing.Point(455, 12);
            this.lblUsrName.Name = "lblUsrName";
            this.lblUsrName.Size = new System.Drawing.Size(63, 13);
            this.lblUsrName.TabIndex = 3;
            this.lblUsrName.Text = "User Name:";
            // 
            // lblPwd
            // 
            this.lblPwd.AutoSize = true;
            this.lblPwd.Location = new System.Drawing.Point(458, 55);
            this.lblPwd.Name = "lblPwd";
            this.lblPwd.Size = new System.Drawing.Size(96, 13);
            this.lblPwd.TabIndex = 4;
            this.lblPwd.Text = "Plex TV Password:";
            // 
            // tbPwd
            // 
            this.tbPwd.Location = new System.Drawing.Point(461, 72);
            this.tbPwd.Name = "tbPwd";
            this.tbPwd.PasswordChar = '*';
            this.tbPwd.Size = new System.Drawing.Size(286, 20);
            this.tbPwd.TabIndex = 5;
            // 
            // lbl2Fa
            // 
            this.lbl2Fa.AutoSize = true;
            this.lbl2Fa.Location = new System.Drawing.Point(461, 99);
            this.lbl2Fa.Name = "lbl2Fa";
            this.lbl2Fa.Size = new System.Drawing.Size(104, 13);
            this.lbl2Fa.TabIndex = 6;
            this.lbl2Fa.Text = "2FA code (Optional):";
            // 
            // tb2FA
            // 
            this.tb2FA.Location = new System.Drawing.Point(464, 116);
            this.tb2FA.Name = "tb2FA";
            this.tb2FA.Size = new System.Drawing.Size(283, 20);
            this.tb2FA.TabIndex = 7;
            // 
            // btnClaim
            // 
            this.btnClaim.Location = new System.Drawing.Point(550, 166);
            this.btnClaim.Name = "btnClaim";
            this.btnClaim.Size = new System.Drawing.Size(75, 23);
            this.btnClaim.TabIndex = 8;
            this.btnClaim.Text = "Claim";
            this.btnClaim.UseVisualStyleBackColor = true;
            this.btnClaim.Click += new System.EventHandler(this.btnClaim_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnClaim);
            this.Controls.Add(this.tb2FA);
            this.Controls.Add(this.lbl2Fa);
            this.Controls.Add(this.tbPwd);
            this.Controls.Add(this.lblPwd);
            this.Controls.Add(this.lblUsrName);
            this.Controls.Add(this.tbUsrName);
            this.Controls.Add(this.rtbOut);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtbOut;
        private System.Windows.Forms.TextBox tbUsrName;
        private System.Windows.Forms.Label lblUsrName;
        private System.Windows.Forms.Label lblPwd;
        private System.Windows.Forms.TextBox tbPwd;
        private System.Windows.Forms.Label lbl2Fa;
        private System.Windows.Forms.TextBox tb2FA;
        private System.Windows.Forms.Button btnClaim;
    }
}

