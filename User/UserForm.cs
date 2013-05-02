using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace User
{
    public partial class UserForm : Form
    {

        private User user;
        //private Thread receiveRegNodeThread;
        private int defaultNodePort = 30000;

        public UserForm(User user) 
        {
            InitializeComponent();

            rBPermanentFailure.Checked = true;
            buttonGetAverage.Enabled = false;
            buttonFailAdmin.Enabled = false;
            this.user = user;

            //this.receiveRegNodeThread = new Thread(this.user.UdpSockReceiverRegNode);
            //this.receiveRegNodeThread.Start();
        }

        private void buttonInput_Click(object sender, EventArgs e) {
            user.userInit(this);
            int AdminNodeId = Convert.ToInt32(textBoxAdminID.Text);
            user.AdminNodeId = AdminNodeId;
            bool res = user.tcpConnection(textBoxAdminIP.Text, "youInitialAdmin", defaultNodePort + AdminNodeId);
            if (res) {
                buttonStart.Enabled = false;
                textBoxAdminID.Enabled = false;
                textBoxAdminIP.Enabled = false;
                buttonFailAdmin.Enabled = true;
                buttonGetAverage.Enabled = true;
            }
            else {
                user.StopReceive = true;
            }
        }


        private void UserForm_FormClosing(object sender, FormClosingEventArgs e) 
        {
            user.StopReceive = true;
            user.ReceiverSock.Close();
            user.ListenerTcp.Server.Close();
        }


        private void buttonGetAverage_Click(object sender, EventArgs e) {
            user.tcpConnection(textBoxAdminIP.Text, "getAverage", 33000);
        }

        private void buttonFailAdmin_Click(object sender, EventArgs e) 
        {
            int numNodes;
            string newAdminIp;
            int newAdminId;
            if(user.NodeIds.Count == 1)
            {
                MessageBox.Show("In oder to execute the function you should have at least 2 nodes!");
                return;
            }
            try 
            {
                newAdminId = Convert.ToInt16(textBoxNewAdminID.Text);
            }
            catch (FormatException ex) 
            {
                MessageBox.Show("Use integer number for the new admin! Try again!");
                return;
            }
            if (!user.NodeIds.TryGetValue(newAdminId, out newAdminIp)) 
            {
                MessageBox.Show("The ID of new admin does not exist! Try again!");
                return;
            }
            int prevAdminId = prevAdminId = Convert.ToInt16(textBoxAdminID.Text) ;

            if (newAdminId == prevAdminId) 
            {
                MessageBox.Show("New Admin should be different from previous admin, choose different Id!");
                return;
            }

            if (rBPermanentFailure.Checked == true) 
            {
                user.tcpConnection(textBoxAdminIP.Text, "permanentFail", 33000);
            }
            else 
            {
                user.tcpConnection(textBoxAdminIP.Text, "temporaryFail", 33000);
            }
            user.NodeIds.Remove(prevAdminId);
            user.NodeIds.Remove(newAdminId);
            numNodes = user.NodeIds.Count - 1;

            Thread.Sleep(500);
            string str = "youNewAdmin-NodesNum_" + numNodes + ";";
            foreach(KeyValuePair<int, string> item in user.NodeIds)
            {
                str += item.Key + "_" + item.Value + ";";
            }
            str = str.TrimEnd(';');
            user.tcpConnection(newAdminIp, str, defaultNodePort + newAdminId);
            textBoxAdminIP.Text = newAdminIp;
            textBoxAdminID.Text = newAdminId.ToString();
        }
    }
}
