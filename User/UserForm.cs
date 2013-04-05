using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace User
{
    public partial class UserForm : Form
    {

        private User m_user;

        public UserForm(User user) {
            InitializeComponent();

            rBtemporaryFailure.Checked = true;
            buttonGetAverage.Enabled = false;
            m_user = user;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void buttonInput_Click(object sender, EventArgs e) {
            m_user.userInit(this);
            m_user.tcpConnection(textBoxAdminIP.Text, "start");
            buttonStart.Enabled = false;
            buttonGetAverage.Enabled = true;
        }

        private void UserForm_Load(object sender, EventArgs e) {

        }

        private void UserForm_FormClosing(object sender, FormClosingEventArgs e) {
            m_user.M_stopReceive = true;
            m_user.m_receiverSock.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e) {

        }

        private void buttonGetAverage_Click(object sender, EventArgs e) {
            m_user.tcpConnection(textBoxAdminIP.Text, "getAverage");
        }
    }
}
