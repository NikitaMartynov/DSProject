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
        public NodeForm(Node node)
        {
            InitializeComponent();
            richTextBoxTemp.Enabled = true;
            this.node = node;
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            int id = Convert.ToInt16(this.textBoxID.Text);
            node.setForm(this);
            node.nodeInit(id);

            this.textBoxID.Enabled = false;
            this.buttonStart.Enabled = false;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e) 
        {
            if (node.NodeAdmin != null)
            {
                node.NodeAdmin.StopReceive = true;
                node.NodeAdmin.TempReceiverSock.Close();
                if (node.NodeAdmin.ListenerTcp != null) 
                {
                    node.NodeAdmin.ListenerTcp.Server.Close();
                    node.NodeAdmin.RegReceiverSock.Client.Close();
                }
            }
            node.StopSend = true;
            if (node.ListenerTcp != null) 
                node.ListenerTcp.Server.Close();
            
            node.sockUdpReg.Close();
        }
    }
}
