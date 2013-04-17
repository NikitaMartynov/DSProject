using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace DSProject
{
    public class NodeAdmin//: MarshalByRefObject, INodeAdmin
    {
        
        public bool StopReceive{get;set;}
        public UdpClient TempReceiverSock;
        public UdpClient RegReceiverSock;
        public TcpListener ListenerTcp;
        public bool StartedByUser { get; set; }

        private int NodesNum { get; set; }
        private bool initialAdmin; 


        private DataStore dataStore; //DictionaryOfNodes<ListOfNodeValues>
        private Node node;

        private Thread regNodesThread;
        private Thread receiveTempThread;
        private Thread workWithUserThread;
        //private List<int> registeredNodes;
        //private List<int> markedNodes;

        private IPEndPoint userEndPoint;

        private static object lockObject = new object();




        public NodeAdmin(Node node, string userIP, bool initialAdmin, int nodesNum) {
            this.node = node;
            this.StopReceive = false;
            this.initialAdmin = initialAdmin;
            this.NodesNum = nodesNum;
            this.dataStore = new DataStore();

            dataStore.addNewNode(node.Id);

            start(userIP);
        }

        public void sendT(int id, int val) {
            if(StartedByUser)
                blockingWriteTransaction(id, val);
        }

        public string getLocalIPAddress() {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        private void start(string userIP) {
            this.userEndPoint = new IPEndPoint(IPAddress.Parse(userIP), 33334);
            StartedByUser = true;

            workWithUserThread = new Thread(tcpSockGetAvrAndFailByUser);
            workWithUserThread.Start();

            regNodesThread = new Thread(udpSockRegNodes);
            regNodesThread.Start();

            receiveTempThread = new Thread(udpSockReceiverTemp);
            receiveTempThread.Start();
        }

        private void tcpSockGetAvrAndFailByUser() {
            int localPort = 30000;
            ListenerTcp  = null;
            try {
                // Create a TCPListener to accept client connections
                ListenerTcp = new TcpListener(IPAddress.Any, localPort);
                ListenerTcp.Start();
            }
            catch (SocketException e) {
                // IPAddress.Any
                Console.WriteLine(e.ErrorCode + ": " + e.Message);
                Environment.Exit(e.ErrorCode);
            }

            
            string stringData;
            while (true) { 
                TcpClient client = null;
                NetworkStream netStream = null;

                try {
                    client = ListenerTcp.AcceptTcpClient(); // Get client connection
                     netStream = client.GetStream();

                    byte[] data = new byte[100]; 
                    netStream.Read(data, 0, data.Length);
                    stringData = Encoding.ASCII.GetString(data, 0, data.Length).Trim('\0');
                    if (stringData == "getAverage") {
                        //compute average
                        double average = dataStore.getAverage();
                        byte[] data1 = new byte[100]; 
                        data1 = Encoding.ASCII.GetBytes(average.ToString("#.##"));
                        netStream.Write(data1, 0, data1.Length);
                    }
                    if (stringData == "permanentFail") {
                        netStream.Close();
                        client.Close();
                        ListenerTcp.Server.Close();
                        TempReceiverSock.Close();
                        RegReceiverSock.Close();
                        node.stopNode();
                        break;
                    }
                    if (stringData == "temporaryFail") {
                        netStream.Close();
                        client.Close();
                        ListenerTcp.Server.Close();
                        TempReceiverSock.Close();
                        RegReceiverSock.Close();
                        node.rebootNode();
                        break;
                    }
                    netStream.Close();
                    client.Close();
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    ListenerTcp.Server.Close();
                    break;
                    //netStream.Close();
                }
            }
        }

        private void udpSockRegNodes() {
            int localPort = 11100;
            int remotePort = 22202;
            RegReceiverSock = new UdpClient(localPort);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, remotePort);
            IPEndPoint broadCast = new IPEndPoint(IPAddress.Parse("255.255.255.255"), remotePort);
            List<int> registeredNodes = new List<int> ();

            byte[] data = new byte[1024];
            string[] stringData;
            while (true) {
                if (StopReceive == true) {
                    RegReceiverSock.Close();
                    break;
                }
                if (initialAdmin) {
                    try {
                        data = RegReceiverSock.Receive(ref sender);
                    }
                    catch (Exception e) {
                        break;
                    }
                    stringData = Encoding.ASCII.GetString(data, 0, data.Length).Split('_');
                    if (stringData[0].Equals("regMe")) {
                        int id = Convert.ToInt16(stringData[1]);
                        if (!dataStore.addNewNode(id)) {
                            node.Form.appendTextToRichTB(String.Format("Node {0} already registered\n", id));
                        }
                        byte[] data1 = Encoding.ASCII.GetBytes("IamYourAdmin");

                        TempReceiverSock.Send(data1, data1.Length, sender);
                    }
                }
                else {
                    byte[] data1 = Encoding.ASCII.GetBytes("IamYourAdmin");
                    RegReceiverSock.Send(data1, data1.Length, broadCast);

                    RegReceiverSock.Client.ReceiveTimeout = 5000;

                    byte[] receivedData = null;
                    try {
                        receivedData = RegReceiverSock.Receive(ref sender);
                    }
                    catch (Exception e) {
                        byte[] receivedData1 = null;
                    }
                    if (receivedData == null) continue;
                    string[] strAr = Encoding.ASCII.GetString(data, 0, data.Length).Split('_');
                    if (!strAr[0].Equals("regMe")) continue;
                    int id = Convert.ToInt16(strAr[1]);
                    if (!registeredNodes.Contains(id)) {
                        registeredNodes.Add(id);
                        if (registeredNodes.Count == NodesNum) {// TODO check consistency with list of nodes to be forced
                            initialAdmin = true;
                        }
                    }
                    
                }

            }
   
        }

        private void udpSockReceiverTemp() {
            TempReceiverSock = new UdpClient(11111);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 22222);
            byte[] data = new byte[1024];
            string[] stringData;
            while (true) {
                if (StopReceive == true) {
                    TempReceiverSock.Close();
                    break;
                }
                try {
                    data = TempReceiverSock.Receive(ref sender);
                }
                catch (Exception e) {
                    break;
                }
                stringData = Encoding.ASCII.GetString(data, 0, data.Length).Split('_');
                
                if (stringData[0].Equals("regMe")) {
                    int id = Convert.ToInt16(stringData[1]);
                    if (!dataStore.addNewNode(id)) {
                        node.Form.appendTextToRichTB(String.Format("Node {0} already registered\n", id));
                    }
                    byte[] data1 = Encoding.ASCII.GetBytes("IamYourAdmin");

                    TempReceiverSock.Send(data1, data1.Length, sender);
                }
                else {
                    int id = Convert.ToInt16(stringData[0]);
                    int val = Convert.ToInt16(stringData[1]);
                    blockingWriteTransaction(id, val);
                }
            }
        }

        private void blockingWriteTransaction(int id, int val) {
            lock (lockObject) {
                if (dataStore.write(id, val)) {
                    node.Form.appendTextToRichTB(String.Format("I am:{0}; Temp:{1}\n", id, val));
                    udpSockSendRegTemp(userEndPoint, id + "_" + val);
                }
                else {
                    node.Form.appendTextToRichTB(String.Format("Not registered Node {0} trying to send data\n", id));
                }
            }
        }

        private void udpSockSendRegTemp(IPEndPoint receiverEndPoint, string regularTemp, int portSender = 33333) {
            UdpClient sock = new UdpClient(portSender);
            byte[] data = Encoding.ASCII.GetBytes(regularTemp);
            sock.Send(data, data.Length, receiverEndPoint);
            sock.Close();
        }
       
    }
}
