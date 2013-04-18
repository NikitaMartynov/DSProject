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
        public bool StopSend { get; set; } // check does it helpful or not
        public NodeAdmin NodeAdmin;
        public TcpListener ListenerTcp;

        //private INodeAdmin remoteObject;
        //private IPEndPoint adminEndpoint;
        private string adminIP;
        private IPEndPoint adminEndpoint;
        private Thread regNodeThread;
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
            this.adminIP = null;
            this.adminEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);
            this.StopSend = false;

            this.regNodeThread = new Thread(udpSockReg);
            this.regNodeThread.Start();

            this.sendTempThread = new Thread(UdpSocketSendT);
            this.sendTempThread.Start();

            this.tcpWaitUserCammandThread = new Thread(tcpSockSetAdminByUser);
            this.tcpWaitUserCammandThread.Start();
        }

        public void setForm(NodeForm form) {
            this.Form = form;
        }

        public void stopNode() {
            ListenerTcp.Server.Close();
            StopSend = true;
            Environment.Exit(0);
        }
        public void rebootNode() {
            ListenerTcp.Server.Close();
            StopSend = true;
            adminIP = null;
            adminEndpoint = null;
            this.NodeAdmin = null;
            this.NodeType = NodeType.REGULAR;
            Thread.Sleep(10000);
            nodeInit(Id);
        }


        private void adminInit(string userIP, bool initialAdmin = true, int nodesNum = 0) {

            this.NodeAdmin = new NodeAdmin(this, userIP,initialAdmin, nodesNum);
            NodeType = NodeType.ADMIN;
            adminIP = this.NodeAdmin.getLocalIPAddress();
        }

        private void tcpSockSetAdminByUser() {
            int servPort = 33336; //TEST 33336 + Id; (id = 2) // real life 33336
            ListenerTcp = null;
            try {
                // Create a TCPListener to accept client connections
                ListenerTcp = new TcpListener(IPAddress.Any, servPort);
                ListenerTcp.Start();
            }
            catch (SocketException e) {
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
                    if (stringData.Contains("youNewAdmin")) {
                        string[] strAr = stringData.Split('_');
                        int numNodes = Convert.ToInt16(strAr[1]);
                        adminInit(userIP, false, numNodes);
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

        void udpSockReg() {
            int adminPort = 11100;
            int localPort = 22202;//TEST  22200 + Id; (id = 2) // real life 22202

            IPEndPoint broadCast = new IPEndPoint(IPAddress.Parse("255.255.255.255"), adminPort);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, adminPort);

            while (true) {
                if (StopSend == true) {
                    break;
                }
                UdpClient sock = new UdpClient(localPort); //Exception when try to switch admin
                if (adminIP == null) {
                    
                    byte[] data = Encoding.ASCII.GetBytes("regMe_" + Id.ToString());
                    sock.Send(data, data.Length, broadCast);

                    sock.Client.ReceiveTimeout = 5000;

                    byte[] receivedData = null;
                    try {
                        receivedData = sock.Receive(ref sender);
                    }
                    catch (Exception e) {
                        byte[] receivedData1 = null;
                    }
                    if (receivedData != null) {
                        string stringData = Encoding.ASCII.GetString(receivedData, 0, receivedData.Length);
                        if (stringData.Equals("IamYourAdmin")) {
                            adminIP = sender.Address.ToString();
                            adminEndpoint.Address = IPAddress.Parse(adminIP);
                        }
                    }
                    sock.Close();
                }
                else {
                    byte[] receivedData = null;
                    try {
                        receivedData = sock.Receive(ref sender);
                    }
                    catch (Exception e) {
                        byte[] receivedData1 = null;
                    }
                    string stringData = Encoding.ASCII.GetString(receivedData, 0, receivedData.Length);
                    if (stringData.Equals("IamYourNewAdmin")) {
                        adminIP = sender.Address.ToString();
                        adminEndpoint.Address = IPAddress.Parse(adminIP);
                        byte[] data = Encoding.ASCII.GetBytes("reregMe_" + Id.ToString());
                        sock.Send(data, data.Length, adminEndpoint);
                    }
                }
            }
        }

        void UdpSocketSendT() {
            int adminPort = 11111;
            int localPort = 22222;//TEST 22220 + Id; (id = 2) // real life 22222
            adminEndpoint.Port = adminPort;
            Random rndVal = new Random(Id);
            int val = 0;

            //IPEndPoint broadCast = new IPEndPoint(IPAddress.Parse("255.255.255.255"), adminPort);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, adminPort);
                                        
            while (true) {
                if (StopSend == true) {
                    break;
                }
                val = rndVal.Next(20, 31);

                if (adminIP == null) {
                    Thread.Sleep(3000); // TMP 
/*
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
 * */
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

    }
}
