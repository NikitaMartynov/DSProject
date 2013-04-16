using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace User
{
    public class User
    {
        public bool StopReceive { get; set; }
        public UdpClient ReceiverSock;

        private UserForm userForm;
        private Thread receiveRegTempThread;

        public User() { }

        public void userInit(UserForm userForm) {

            this.userForm = userForm;

            this.receiveRegTempThread = new Thread(UdpSockReceiverRegTemp);
            this.receiveRegTempThread.Start();
        }

        public void tcpConnection(string serverIP, string operation, int serverPort = 33336) {
            TcpClient client = null;
            NetworkStream netStream = null;
            try {
                // Create socket that is connected to server on specified port
                client = new TcpClient(serverIP, serverPort);
                netStream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(operation);
                netStream.Write(data, 0, data.Length);

                if (operation == "getAverage") {
                    int bytesRcvd = 0; // Bytes received in last read
                    //Receive the same string back from the server
                    Array.Clear(data, 0, data.Length);
                    bytesRcvd = netStream.Read(data, 0, data.Length);
                    string stringData = Encoding.ASCII.GetString(data, 0, data.Length);
                    userForm.appendTextToRichTBGetAverage(stringData);
                }
                
            } 
            catch (Exception e) {
                Console.WriteLine(e.Message);
            } 
            finally{
                netStream.Close();
                client.Close();
            }
        }

        private void UdpSockReceiverRegTemp() {
            this.ReceiverSock = new UdpClient(33334);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 33333);
            byte[] data = new byte[1024];
            string[] strAr;
            List<string[]> listStrAr = new List<string[]>();

            while ( true ) {
                if ( StopReceive ) {
                    ReceiverSock.Close();
                    break;
                }
                try {
                    data = ReceiverSock.Receive(ref sender);
                }
                catch (Exception e) {
                    break;
                }
                strAr = Encoding.ASCII.GetString(data, 0, data.Length).Split(';');
                foreach (string str in strAr) {
                    listStrAr.Add( str.Split('_') );
                }
                writeRegTemp(listStrAr);
                listStrAr.Clear();
            }
        }

        private void writeRegTemp(List<string[]> regularTemp){
            foreach(string[] nodeTemp in regularTemp){
                userForm.appendTextToRichTB("ID: " + nodeTemp[0] + " Temp:" + nodeTemp[1] + "; ");
            }
             userForm.appendTextToRichTB("\n");
        }
    }
}
