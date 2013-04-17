namespace User
{
    partial class UserForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void appendTextToRichTB(string str) {
            try {
                if (InvokeRequired)
                    Invoke(new appendTextToRichTBDelegate(appendTextToRichTB), str);
                else
                    richTextBoxRegTemp.AppendText(str);
            }
            catch (System.Exception ex) {
            }
        }

        delegate void appendTextToRichTBDelegate(string value);

        public void appendTextToRichTBGetAverage(string str) {
            try {
                if (InvokeRequired)
                    Invoke(new appendTextToRichTBGetAverageDelegate(appendTextToRichTBGetAverage), str);
                else
                    rBAverage.AppendText(str);
            }
            catch (System.Exception ex) {
            }
        }

        delegate void appendTextToRichTBGetAverageDelegate(string value);

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonStart = new System.Windows.Forms.Button();
            this.richTextBoxRegTemp = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rBPermanentFailure = new System.Windows.Forms.RadioButton();
            this.rBtemporaryFailure = new System.Windows.Forms.RadioButton();
            this.buttonFailAdmin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxAdminIP = new System.Windows.Forms.TextBox();
            this.buttonGetAverage = new System.Windows.Forms.Button();
            this.rBAverage = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxNewAdminIP = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 30);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonInput_Click);
            // 
            // richTextBoxRegTemp
            // 
            this.richTextBoxRegTemp.Location = new System.Drawing.Point(182, 32);
            this.richTextBoxRegTemp.Name = "richTextBoxRegTemp";
            this.richTextBoxRegTemp.Size = new System.Drawing.Size(235, 160);
            this.richTextBoxRegTemp.TabIndex = 3;
            this.richTextBoxRegTemp.Text = "";
            this.richTextBoxRegTemp.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Regular Temperature";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rBPermanentFailure);
            this.groupBox1.Controls.Add(this.rBtemporaryFailure);
            this.groupBox1.Location = new System.Drawing.Point(12, 211);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(100, 65);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Failure type";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // rBPermanentFailure
            // 
            this.rBPermanentFailure.AutoSize = true;
            this.rBPermanentFailure.Location = new System.Drawing.Point(6, 19);
            this.rBPermanentFailure.Name = "rBPermanentFailure";
            this.rBPermanentFailure.Size = new System.Drawing.Size(76, 17);
            this.rBPermanentFailure.TabIndex = 1;
            this.rBPermanentFailure.TabStop = true;
            this.rBPermanentFailure.Text = "Permanent";
            this.rBPermanentFailure.UseVisualStyleBackColor = true;
            // 
            // rBtemporaryFailure
            // 
            this.rBtemporaryFailure.AutoSize = true;
            this.rBtemporaryFailure.Location = new System.Drawing.Point(6, 42);
            this.rBtemporaryFailure.Name = "rBtemporaryFailure";
            this.rBtemporaryFailure.Size = new System.Drawing.Size(75, 17);
            this.rBtemporaryFailure.TabIndex = 0;
            this.rBtemporaryFailure.TabStop = true;
            this.rBtemporaryFailure.Text = "Temporary";
            this.rBtemporaryFailure.UseVisualStyleBackColor = true;
            // 
            // buttonFailAdmin
            // 
            this.buttonFailAdmin.Location = new System.Drawing.Point(12, 282);
            this.buttonFailAdmin.Name = "buttonFailAdmin";
            this.buttonFailAdmin.Size = new System.Drawing.Size(75, 23);
            this.buttonFailAdmin.TabIndex = 6;
            this.buttonFailAdmin.Text = "Fail admin";
            this.buttonFailAdmin.UseVisualStyleBackColor = true;
            this.buttonFailAdmin.Click += new System.EventHandler(this.buttonFailAdmin_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Admin IP";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBoxAdminIP
            // 
            this.textBoxAdminIP.Location = new System.Drawing.Point(92, 4);
            this.textBoxAdminIP.Name = "textBoxAdminIP";
            this.textBoxAdminIP.Size = new System.Drawing.Size(88, 20);
            this.textBoxAdminIP.TabIndex = 2;
            this.textBoxAdminIP.Text = "127.0.0.1";
            // 
            // buttonGetAverage
            // 
            this.buttonGetAverage.Location = new System.Drawing.Point(12, 140);
            this.buttonGetAverage.Name = "buttonGetAverage";
            this.buttonGetAverage.Size = new System.Drawing.Size(75, 23);
            this.buttonGetAverage.TabIndex = 7;
            this.buttonGetAverage.Text = "Get Average";
            this.buttonGetAverage.UseVisualStyleBackColor = true;
            this.buttonGetAverage.Click += new System.EventHandler(this.buttonGetAverage_Click);
            // 
            // rBAverage
            // 
            this.rBAverage.Location = new System.Drawing.Point(12, 59);
            this.rBAverage.Name = "rBAverage";
            this.rBAverage.Size = new System.Drawing.Size(140, 75);
            this.rBAverage.TabIndex = 8;
            this.rBAverage.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "New Admin IP";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // textBoxNewAdminIP
            // 
            this.textBoxNewAdminIP.Location = new System.Drawing.Point(92, 192);
            this.textBoxNewAdminIP.Name = "textBoxNewAdminIP";
            this.textBoxNewAdminIP.Size = new System.Drawing.Size(84, 20);
            this.textBoxNewAdminIP.TabIndex = 10;
            this.textBoxNewAdminIP.Text = "127.0.0.1";
            // 
            // UserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 339);
            this.Controls.Add(this.textBoxNewAdminIP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rBAverage);
            this.Controls.Add(this.buttonGetAverage);
            this.Controls.Add(this.buttonFailAdmin);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.richTextBoxRegTemp);
            this.Controls.Add(this.textBoxAdminIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonStart);
            this.Name = "UserForm";
            this.Text = "User";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserForm_FormClosing);
            this.Load += new System.EventHandler(this.UserForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.RichTextBox richTextBoxRegTemp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rBPermanentFailure;
        private System.Windows.Forms.RadioButton rBtemporaryFailure;
        private System.Windows.Forms.Button buttonFailAdmin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxAdminIP;
        private System.Windows.Forms.Button buttonGetAverage;
        private System.Windows.Forms.RichTextBox rBAverage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxNewAdminIP;
    }
}

