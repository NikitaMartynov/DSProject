using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace DSProject
{
    public partial class NodeForm : Form
    {
        private Node node;
        public NodeForm(Node node) {
            InitializeComponent();

            richTextBoxTemp.Enabled = true;

            this.node = node;
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {

            int id = Convert.ToInt16(this.textBoxID.Text);
            NodeType nodeType;
            int nodesNum = 0;
            node.setForm(this);
            node.nodeInit(id);

            this.textBoxID.Enabled = false;
            this.buttonStart.Enabled = false;
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) {

        }

        private void groupBox1_Enter(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void richTextBoxTemp_TextChanged(object sender, EventArgs e) {

        }

        private void label1_Click_1(object sender, EventArgs e) {

        }

        private void textBoxID_TextChanged(object sender, EventArgs e) {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (node.NodeAdmin != null) {
                node.NodeAdmin.StopReceive = true;
                node.NodeAdmin.TempReceiverSock.Close();
                if (node.NodeAdmin.ListenerTcp != null) {
                    node.NodeAdmin.ListenerTcp.Server.Close();
                    node.NodeAdmin.RegReceiverSock.Client.Close();
                }
            }
            node.StopSend = true;
            if (node.ListenerTcp != null) 
                node.ListenerTcp.Server.Close();
            node.sockUdpReg.Close();
            // this.Close();
        }

        private void rBAdmin_CheckedChanged(object sender, EventArgs e) {

        }

        private void label4_Click(object sender, EventArgs e) {

        }
    }
}
