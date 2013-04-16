using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Net.Sockets;



namespace DSProject
{

    public enum NodeType{ REGULAR, ADMIN };

    public class Node
    {
        public NodeType NodeType{get; set;} // Check may it be deleted
        
        public int Id{get; set;}
        public NodeForm Form{get;set;}
        public bool StopSend { get; set; }
        public NodeAdmin NodeAdmin;
        public TcpListener ListenerTcp;

        //private INodeAdmin remoteObject;
        //private IPEndPoint adminEndpoint;
        private string adminIP;
        private Thread sendTempThread;
        private Thread tcpWaitUserCammandThread;
        
        public Node() {
        }
        public Node(int id, NodeType nodeType) {
            this.Id = id;
            this.NodeType = nodeType;
            //nodeInit();
        }

        public void nodeInit(int id) {
            this.Id = id;
            //this.NodeType = nodeType;
            this.adminIP = null;
           // this.adminEndpoint = adminEndpoint;

           // if (this.NodeType == NodeType.ADMIN) {
            //    adminInit(this.adminEndpoint.Port, nodesNum);
           // }
          //  else {
          //      regularInit();
           // }
            this.sendTempThread = new Thread(UdpSocketSendT);
            this.sendTempThread.Start();

            this.tcpWaitUserCammandThread = new Thread(tcpSockTalkWithUser);
            this.tcpWaitUserCammandThread.Start();
        }

        public void setForm(NodeForm form) {
            this.Form = form;
        }



        void adminInit(string userIP, int port = 11111, int nodesNum = 3){

            this.NodeAdmin = new NodeAdmin(this, userIP, port, nodesNum);
            NodeType = NodeType.ADMIN;
            adminIP = this.NodeAdmin.getLocalIPAddress();
        }

        void regularInit() {
            //TcpChannel tcpChannel = new TcpChannel();
            //ChannelServices.RegisterChannel(tcpChannel, true);
            //Type requiredType = typeof(INodeAdmin);
            //remoteObject = (INodeAdmin)Activator.GetObject(requiredType, "tcp://localhost:33333/Admin");

        }

        private void tcpSockTalkWithUser() {
            int servPort = 33336 + Id; //TEST
            ListenerTcp = null;
            try {
                // Create a TCPListener to accept client connections
                ListenerTcp = new TcpListener(IPAddress.Any, servPort);
                ListenerTcp.Start();
            }
            catch (SocketException e) {
                // IPAddress.Any
                Console.WriteLine(e.ErrorCode + ": " + e.Message);
                Environment.Exit(e.ErrorCode);
            }

            byte[] data = new byte[1024];
            string stringData;
            while (true) {
                TcpClient client = null;
                NetworkStream netStream = null;

                try {
                    client = ListenerTcp.AcceptTcpClient(); // Get client connection
                    netStream = client.GetStream();

                    string userIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    netStream.Read(data, 0, data.Length);
                    stringData = Encoding.ASCII.GetString(data, 0, data.Length).Trim('\0');
                    // Receive until client closes connection, indicated by 0 return value
                    if (stringData == "youInitialAdmin") {
                        adminInit(userIP);
                    }
                    netStream.Close();
                    client.Close();
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    break;
                    //netStream.Close();
                }
            }
        }

        void UdpSocketSendT() {
            int adminPort = 11111;
            int localPort = 22220 + Id;//TEST

            Random rndVal = new Random(Id);
            int val = 0;
            //int sleepT = 0;
            IPEndPoint adminEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), adminPort);
            IPEndPoint broadCast = new IPEndPoint(IPAddress.Parse("255.255.255.255"), adminPort);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, adminPort);
            // adminEndpoint = new IPEndPoint(IPAddress.Parse(adminIP), 11111);
                                        
            while (true) {
                if (StopSend == true) {
                    break;
                }
                val = rndVal.Next(20, 31);

                if (adminIP == null) {

                    UdpClient sock = new UdpClient(localPort);
                    byte[] data = Encoding.ASCII.GetBytes("regMe_"+Id.ToString());
                    sock.Send(data, data.Length, broadCast);

                    sock.Client.ReceiveTimeout = 5000;

                    byte[] receivedData = null;
                    try {
                        receivedData = sock.Receive(ref sender);
                    }
                    catch (Exception e) {
                        byte[] receivedData1 = null;
                    }
                    if( receivedData != null){
                        string stringData = Encoding.ASCII.GetString(receivedData, 0, receivedData.Length);
                        if (stringData.Equals("IamYourAdmin")) {
                            adminIP = sender.Address.ToString();
                            adminEndpoint.Address = IPAddress.Parse(adminIP);
                        }
                    }
                    sock.Close();
                }
                else{
                    if (NodeType == NodeType.ADMIN) {
                        NodeAdmin.sendT(Id, val);
                    }
                    else {
                        UdpClient sock = new UdpClient(22222);
                        byte[] data = Encoding.ASCII.GetBytes(Id.ToString() + "_" + val.ToString());

                        Form.appendTextToRichTB(String.Format("Sent: I am:{0}; Temp:{1}\n", Id, val));

                        sock.Send(data, data.Length, adminEndpoint);
                        sock.Close();
                    }
                    //sleepT = rndVal.Next(1000, 3000);
                    //Thread.Sleep(sleepT);
                    Thread.Sleep(3000);
                }
            }
        }



        void reportT() {
           // int[] currentTempArray = new int[m_nodesNum];
           // int i = 0;
           // foreach (List<int> item in m_dataStore.getDataStore()) {
           //     currentTempArray[i] = item.ElementAt(transactionCounter);
           //     i++;
           // }
            //Send current temp array to user;
        
        }

        void respAvrT() {

            //Send Avr Temp to user
        }


    }
}
