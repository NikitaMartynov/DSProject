using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace User
{
    public class User
    {
        public Dictionary<int, string> NodeIds { get; set; }
        public bool StopReceive { get; set; }
        public UdpClient ReceiverSock;

        private UserForm userForm;
        private Thread receiveRegTempThread;

        


        public User() { }

        public void userInit(UserForm userForm) {

            this.userForm = userForm;
            this.NodeIds = new Dictionary<int, string>();

            this.receiveRegTempThread = new Thread(UdpSockReceiverRegTemp);
            this.receiveRegTempThread.Start();
        }

        public bool tcpConnection(string remoteIP, string operation, int remotePort = 30000) {
            bool result = false;
            TcpClient client = null;
            NetworkStream netStream = null;
            try {
                // Create socket that is connected to server on specified port
                client = new TcpClient(remoteIP, remotePort);
                netStream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(operation);
                netStream.Write(data, 0, data.Length);

                if (operation == "getAverage") {
                    int bytesRcvd = 0; // Bytes received in last read
                    //Receive the same string back from the server
                    Array.Clear(data, 0, data.Length);
                    bytesRcvd = netStream.Read(data, 0, data.Length);
                    string stringData = Encoding.ASCII.GetString(data, 0, data.Length).Trim('\0');
                    userForm.appendTextToRichTBGetAverage(stringData+"\n");
                }
                result = true;
            } 
            catch (Exception e) {
                if (e is SocketException) {
                    if (e.Message.Contains("No connection could be made")) {
                        MessageBox.Show("Node you are trying to connect to is currently off! Please start it first!");
                        result = false;
                    }
                    
                }
            } 
            finally{
                if(netStream != null)
                    netStream.Close();
                if(client != null)
                    client.Close();
            }
            return result;
        }

        private void UdpSockReceiverRegTemp() {
            this.ReceiverSock = new UdpClient(33334);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 33333);
            byte[] data = new byte[1024];
            string[] strAr;

            while ( true ) {
                if ( StopReceive ) {
                    break;
                }
                try {
                    data = ReceiverSock.Receive(ref sender);
                }
                catch (Exception e) {
                    break;
                }
                strAr = Encoding.ASCII.GetString(data, 0, data.Length).Split('_');
                userForm.appendTextToRichTB("ID: " + strAr[0] + " Temp:" + strAr[1] + "\n ");

                int id = Convert.ToInt16( strAr[0]);
                string ip = strAr[2];
                if (!NodeIds.ContainsKey(id)) {
                    NodeIds.Add(id,ip);
                }
            }
            ReceiverSock.Close();
        }

    }
}
