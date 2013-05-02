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

        delegate void StringDelegate(string someValue);

        public void setFormText(string str) {
            try {
                if (InvokeRequired)
                    Invoke(new setFormTextDelegate(setFormText), str);
                else
                    this.Text = str;
            }
            catch (System.Exception ex) {
            }
        }

        delegate void setFormTextDelegate(string value);


        public void UpdateTitle(string AdminOrNode)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(new StringDelegate(UpdateTitle), AdminOrNode);
                else
                    this.Text=AdminOrNode;
            }
            catch (System.Exception ex){}
            //this.Invoke(new InvokeDelegate(delegate()
            //{ this.Form.Text = "Whatever2"; }));
            //if (this.InvokeRequired)
            //    this.Invoke(new StringDelgate(UpdateLabel, new object[] { newLabelText }));
            //else
            //    this.labelToUpdate.Text = newLabelText;
        }


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
            this.richTextBoxTemp = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(73, 3);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(28, 3);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(29, 20);
            this.textBoxID.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "ID";
            // 
            // richTextBoxTemp
            // 
            this.richTextBoxTemp.Location = new System.Drawing.Point(12, 48);
            this.richTextBoxTemp.Name = "richTextBoxTemp";
            this.richTextBoxTemp.Size = new System.Drawing.Size(137, 147);
            this.richTextBoxTemp.TabIndex = 6;
            this.richTextBoxTemp.Text = "";
            // 
            // NodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 256);
            this.Controls.Add(this.richTextBoxTemp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.buttonStart);
            this.Name = "NodeForm";
            this.Text = "Node";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBoxTemp;
    }
}

