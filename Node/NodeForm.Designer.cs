namespace DSProject
{
    partial class NodeForm
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

        public System.Windows.Forms.RichTextBox getRichTextBoxTemp(){
            return richTextBoxTemp;
        }

        public void appendTextToRichTB(string str) {
            try {
                if (InvokeRequired)
                    Invoke(new appendTextToRichTBDelegate(appendTextToRichTB), str);
                else
                    richTextBoxTemp.AppendText(str);
            }
            catch (System.Exception ex) {
            }
        }

        delegate void appendTextToRichTBDelegate(string value);


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonStart = new System.Windows.Forms.Button();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rBRegular = new System.Windows.Forms.RadioButton();
            this.rBAdmin = new System.Windows.Forms.RadioButton();
            this.groupBoxNodeType = new System.Windows.Forms.GroupBox();
            this.richTextBoxTemp = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxAdminIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxAdminPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxNodesNum = new System.Windows.Forms.TextBox();
            this.groupBoxNodeType.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 173);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(33, 50);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(29, 20);
            this.textBoxID.TabIndex = 2;
            this.textBoxID.TextChanged += new System.EventHandler(this.textBoxID_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "ID";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // rBRegular
            // 
            this.rBRegular.AutoSize = true;
            this.rBRegular.Location = new System.Drawing.Point(6, 19);
            this.rBRegular.Name = "rBRegular";
            this.rBRegular.Size = new System.Drawing.Size(62, 17);
            this.rBRegular.TabIndex = 4;
            this.rBRegular.TabStop = true;
            this.rBRegular.Text = "Regular";
            this.rBRegular.UseVisualStyleBackColor = true;
            this.rBRegular.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // rBAdmin
            // 
            this.rBAdmin.AutoSize = true;
            this.rBAdmin.Location = new System.Drawing.Point(6, 42);
            this.rBAdmin.Name = "rBAdmin";
            this.rBAdmin.Size = new System.Drawing.Size(54, 17);
            this.rBAdmin.TabIndex = 5;
            this.rBAdmin.TabStop = true;
            this.rBAdmin.Text = "Admin";
            this.rBAdmin.UseVisualStyleBackColor = true;
            this.rBAdmin.CheckedChanged += new System.EventHandler(this.rBAdmin_CheckedChanged);
            // 
            // groupBoxNodeType
            // 
            this.groupBoxNodeType.Controls.Add(this.rBRegular);
            this.groupBoxNodeType.Controls.Add(this.rBAdmin);
            this.groupBoxNodeType.Location = new System.Drawing.Point(12, 76);
            this.groupBoxNodeType.Name = "groupBoxNodeType";
            this.groupBoxNodeType.Size = new System.Drawing.Size(75, 62);
            this.groupBoxNodeType.TabIndex = 6;
            this.groupBoxNodeType.TabStop = false;
            this.groupBoxNodeType.Text = "Node type";
            this.groupBoxNodeType.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // richTextBoxTemp
            // 
            this.richTextBoxTemp.Location = new System.Drawing.Point(331, 49);
            this.richTextBoxTemp.Name = "richTextBoxTemp";
            this.richTextBoxTemp.Size = new System.Drawing.Size(137, 147);
            this.richTextBoxTemp.TabIndex = 6;
            this.richTextBoxTemp.Text = "";
            this.richTextBoxTemp.TextChanged += new System.EventHandler(this.richTextBoxTemp_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Admin IP";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // textBoxAdminIP
            // 
            this.textBoxAdminIP.Location = new System.Drawing.Point(64, 6);
            this.textBoxAdminIP.Name = "textBoxAdminIP";
            this.textBoxAdminIP.Size = new System.Drawing.Size(77, 20);
            this.textBoxAdminIP.TabIndex = 8;
            this.textBoxAdminIP.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(147, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Admin Port";
            this.label3.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // textBoxAdminPort
            // 
            this.textBoxAdminPort.Location = new System.Drawing.Point(211, 6);
            this.textBoxAdminPort.Name = "textBoxAdminPort";
            this.textBoxAdminPort.Size = new System.Drawing.Size(46, 20);
            this.textBoxAdminPort.TabIndex = 9;
            this.textBoxAdminPort.Text = "11111";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Nodes Num";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // textBoxNodesNum
            // 
            this.textBoxNodesNum.Location = new System.Drawing.Point(81, 138);
            this.textBoxNodesNum.Name = "textBoxNodesNum";
            this.textBoxNodesNum.Size = new System.Drawing.Size(34, 20);
            this.textBoxNodesNum.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 262);
            this.Controls.Add(this.textBoxNodesNum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxAdminPort);
            this.Controls.Add(this.textBoxAdminIP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBoxTemp);
            this.Controls.Add(this.groupBoxNodeType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.buttonStart);
            this.Name = "Form1";
            this.Text = "Node";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBoxNodeType.ResumeLayout(false);
            this.groupBoxNodeType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rBRegular;
        private System.Windows.Forms.RadioButton rBAdmin;
        private System.Windows.Forms.GroupBox groupBoxNodeType;
        private System.Windows.Forms.RichTextBox richTextBoxTemp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxAdminIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAdminPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxNodesNum;
    }
}

