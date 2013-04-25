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

    public enum NodeType { REGULAR, ADMIN };

    public class Node
    {
        public NodeType NodeType { get; set; } // Check may it be deleted

        public int Id { get; set; }
        public NodeForm Form { get; set; }
        public bool StopSend { get; set; } // check does it helpful or not
        public NodeAdmin NodeAdmin;
        public TcpListener ListenerTcp;
        public UdpClient sockUdpReg;

        
        private string adminIP;
        private IPEndPoint adminEndpointData;
        private IPEndPoint adminEndpointReg;
        private Thread regNodeThread;
        private Thread sendTempThread;
        private Thread tcpWaitUserCammandThread;

        public Node() {
        }
        public Node(int id, NodeType nodeType) {
            this.Id = id;
            this.NodeType = nodeType;

        }

        public void nodeInit(int id) {
            this.Id = id;
            this.adminIP = null;
            this.adminEndpointData = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);
            this.adminEndpointReg = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11100);
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
            adminEndpointData = null;
            adminEndpointReg = null;
            sockUdpReg.Close();
            this.NodeAdmin = null;
            this.NodeType = NodeType.REGULAR;
            Thread.Sleep(10000);
            nodeInit(Id);
        }


        private void adminInit(string userIP, bool initialAdmin = true, Dictionary<int, string> runningRegNodes = null) {

            this.NodeAdmin = new NodeAdmin(this, userIP, initialAdmin, runningRegNodes);
            NodeType = NodeType.ADMIN;
            adminIP = this.NodeAdmin.getLocalIPAddress();
        }

        private void tcpSockSetAdminByUser() {
            int servPort = 30000 + Id; //OK
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
                    client = ListenerTcp.AcceptTcpClient(); 
                    netStream = client.GetStream();

                    string userIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    netStream.Read(data, 0, data.Length);
                    stringData = Encoding.ASCII.GetString(data, 0, data.Length).Trim('\0');
                    // Receive until client closes connection, indicated by 0 return value
                    if (stringData == "youInitialAdmin") {
                        adminInit(userIP);
                    }
                    if (stringData.Contains("youNewAdmin")) {
                        string[] strArray = stringData.Split(';');
                        int i = -1;
                        Dictionary<int, string> runningRegNodes = new Dictionary<int, string>();
                        int id;
                        string ip;
                        foreach (string item in strArray) {
                            i++;
                            string[] strAr = item.Split('_');
                            if (i == 0) continue;
                            id = Convert.ToInt16(strAr[0]);
                            ip = strAr[1];
                            runningRegNodes.Add(id, ip);
                        }
                        adminInit(userIP, false, runningRegNodes);
                    }
                    netStream.Close();
                    client.Close();
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    break;

                }
            }
        }

        void udpSockReg() {
            int adminPort = 11100;
            int localPort = 22200 + Id;//   OK 
            adminEndpointReg.Port = adminPort;

            IPEndPoint broadCast = new IPEndPoint(IPAddress.Broadcast, adminPort);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, adminPort);

            while (true) {
                if (StopSend == true) {
                    break;
                }
                sockUdpReg = new UdpClient(localPort);
                sockUdpReg.EnableBroadcast = true;
                if (adminIP == null) {

                    byte[] data = Encoding.ASCII.GetBytes("regMe_" + Id.ToString());
                    sockUdpReg.Send(data, data.Length, broadCast);

                    sockUdpReg.Client.ReceiveTimeout = 5000;

                    byte[] receivedData = null;
                    try {
                        receivedData = sockUdpReg.Receive(ref sender);
                    }
                    catch (Exception e) {
                        byte[] receivedData1 = null;
                    }
                    if (receivedData != null) {
                        string stringData = Encoding.ASCII.GetString(receivedData, 0, receivedData.Length);
                        if (stringData.Equals("IamYourAdmin")) {
                            adminIP = sender.Address.ToString();
                            adminEndpointReg.Address = IPAddress.Parse(adminIP);
                        }
                    }
                }
                else {
                    byte[] receivedData = null;
                    try {
                        receivedData = sockUdpReg.Receive(ref sender);
                    }
                    catch (Exception e) {
                        byte[] receivedData1 = null;
                    }
                    if (receivedData == null) break;
                    string stringData = Encoding.ASCII.GetString(receivedData, 0, receivedData.Length);
                    if (stringData.Equals("IamYourNewAdmin")) {
                        adminIP = sender.Address.ToString();
                        adminEndpointReg.Address = IPAddress.Parse(adminIP);
                        byte[] data = Encoding.ASCII.GetBytes("reregMe_" + Id.ToString());
                        sockUdpReg.Send(data, data.Length, adminEndpointReg);
                    }
                }
                sockUdpReg.Close();
            }
        }

        void UdpSocketSendT() {
            int adminPort = 11111;
            int localPort = 22220 + Id;//  OK
            adminEndpointData.Port = adminPort;
            Random rndVal = new Random(Id);
            int val = 0;

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, adminPort);

            while (true) {
                if (StopSend == true) {
                    break;
                }
                val = rndVal.Next(20, 31);

                if (adminIP == null) {
                    Thread.Sleep(3000); // TMP 
                }
                else {
                    if (NodeType == NodeType.ADMIN) {
                        NodeAdmin.sendT(Id, val);
                    }
                    else {
                        UdpClient sock = new UdpClient(localPort);
                        byte[] data = Encoding.ASCII.GetBytes(Id.ToString() + "_" + val.ToString());

                        Form.appendTextToRichTB(String.Format("Sent: I am:{0}; Temp:{1}\n", Id, val));

                        sock.Send(data, data.Length, adminEndpointData);
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
