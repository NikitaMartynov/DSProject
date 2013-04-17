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

        private User m_user;

        public UserForm(User user) {
            InitializeComponent();

            rBPermanentFailure.Checked = true;
            buttonGetAverage.Enabled = false;
            buttonFailAdmin.Enabled = false;
            m_user = user;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void buttonInput_Click(object sender, EventArgs e) {
            m_user.userInit(this);
            m_user.tcpConnection(textBoxAdminIP.Text, "youInitialAdmin");
            buttonStart.Enabled = false;
            textBoxAdminIP.Enabled = false;
            buttonFailAdmin.Enabled = true;
            buttonGetAverage.Enabled = true;
        }

        private void UserForm_Load(object sender, EventArgs e) {

        }

        private void UserForm_FormClosing(object sender, FormClosingEventArgs e) {
            m_user.StopReceive = true;
            m_user.ReceiverSock.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e) {

        }

        private void buttonGetAverage_Click(object sender, EventArgs e) {
            m_user.tcpConnection(textBoxAdminIP.Text, "getAverage", 30000);
        }

        private void buttonFailAdmin_Click(object sender, EventArgs e) {
            int numNodes;
            if (rBPermanentFailure.Checked == true) {
                m_user.tcpConnection(textBoxAdminIP.Text, "permanentFail", 30000);
                numNodes = m_user.NodeIds.Count-1;
            }
            else {
                m_user.tcpConnection(textBoxAdminIP.Text, "temporaryFail", 30000);
                numNodes = m_user.NodeIds.Count;
            }
            Thread.Sleep(1000);
            //TODO how to delete previous admin from list
            m_user.tcpConnection(textBoxNewAdminIP.Text, "youNewAdmin-NodesNum_" + numNodes);
            textBoxAdminIP.Text = textBoxNewAdminIP.Text;
        }

        private void label3_Click(object sender, EventArgs e) {

        }
    }
}
