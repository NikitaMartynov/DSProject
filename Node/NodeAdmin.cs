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
        public int NodesNum{get;set;}
        public bool StopReceive{get;set;}
        public UdpClient ReceiverSock;
        public TcpListener ListenerTcp;
        public bool StartedByUser { get; set; }


        private DataStore dataStore; //DictionaryOfNodes<ListOfNodeValues>
        private Node node;

        private Thread receiveTempThread;
        private Thread workWithUserThread;
        private int port;
        private List<int> registeredNodes;
        private List<int> markedNodes;

        private IPEndPoint userEndPoint;

        private static object lockObject = new object();




        public NodeAdmin(Node node, string userIP, int port, int nodesNum) {
            this.node = node;
            this.port = port;
            this.StopReceive = false;
            this.NodesNum = nodesNum;
            this.dataStore = new DataStore();
            this.registeredNodes = new List<int>(this.NodesNum);
            this.markedNodes = new List<int>(this.NodesNum);
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

        private void tcpSockTalkWithUser() {
            int servPort = 33336;
            ListenerTcp  = null;
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
            int bytesRcvd; 
            while (true) { 
                TcpClient client = null;
                NetworkStream netStream = null;

                try {
                    client = ListenerTcp.AcceptTcpClient(); // Get client connection
                     netStream = client.GetStream();

                    netStream.Read(data, 0, data.Length);
                    stringData = Encoding.ASCII.GetString(data, 0, data.Length).Trim('\0');
                    if (stringData == "getAverage") {
                        //compute average
                        double average = dataStore.getAverage();
                        Array.Clear(data, 0, data.Length);
                        data = Encoding.ASCII.GetBytes(average.ToString("#.##"));
                        netStream.Write(data, 0, data.Length);
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

        private void start(string userIP){
            this.userEndPoint = new IPEndPoint(IPAddress.Parse(userIP), 33334);
            StartedByUser = true;

            //workWithUserThread = new Thread(tcpSockTalkWithUser);
            //workWithUserThread.Start();

            receiveTempThread = new Thread(udpSockReceiverTemp);
            receiveTempThread.Start();  
        }

        private void udpSockReceiverTemp() {
            ReceiverSock = new UdpClient(11111);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 22222);
            byte[] data = new byte[1024];
            string[] stringData;
            while (true) {
                if (StopReceive == true) {
                    ReceiverSock.Close();
                    break;
                }
                try {
                    data = ReceiverSock.Receive(ref sender);
                }
                catch (Exception e) {
                    break;
                }
                //Thread.Sleep(5000);
                stringData = Encoding.ASCII.GetString(data, 0, data.Length).Split('_');
                if (stringData[0].Equals("regMe")) {
                    int id = Convert.ToInt16(stringData[1]);
                    if (!dataStore.addNewNode(id)) {
                        node.Form.appendTextToRichTB(String.Format("Node {0} already registered\n", id));
                    }
                    byte[] data1 = Encoding.ASCII.GetBytes("IamYourAdmin");

                    ReceiverSock.Send(data1, data1.Length, sender);
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
/*
        // return true is all nodes tempereture received 
        private bool writeTemp(int id, int val) {
            bool alreadyRegisteredAndWaitingOthers = false;
            if (!isAllNodesRegistered()) {
                if (registerNode(id)) {
                    markNode(id);
                }
                else{
                    node.Form.appendTextToRichTB(String.Format("Node {0} registered and wait for others\n", id) );
                    alreadyRegisteredAndWaitingOthers = true;
                }
            }
            if (alreadyRegisteredAndWaitingOthers) return false;

            if (isAllNodesRegistered()) {
                if (!registeredNodes.Contains(id)) {
                    node.Form.appendTextToRichTB(String.Format("NOT REGISTERED Node {0} denied!\n", id));

                    return false;
                }
                if (!markNode(id)) {
                    node.Form.appendTextToRichTB(String.Format("Node {0} marked and wait for others\n", id));
                }
            }
            if (dataStore.write(id, val)) {
                node.Form.appendTextToRichTB( String.Format("I am:{0}; Temp:{1}\n", id, val) );
                }
            else {
                node.Form.appendTextToRichTB(String.Format("I am:{0}; RenewTemp:{1}\n", id, val));
            }
            if (isAllNodesMarked()) {
                markedNodes.Clear();
                return true;
            }
            return false;
        }
        */
        private void udpSockSendRegTemp(IPEndPoint receiverEndPoint, string regularTemp, int portSender = 33333) {
            UdpClient sock = new UdpClient(portSender);
            byte[] data = Encoding.ASCII.GetBytes(regularTemp);
            sock.Send(data, data.Length, receiverEndPoint);
            sock.Close();
        }

        private void lastTempToString(List<int[]> lastTemp, out string str) {
            str = " ";
            foreach (int[] item in lastTemp) {
                str += String.Format("{0}_{1};", item[0], item[1]);
            }
            str = str.Remove(0,1);
            str = str.Remove(str.Length - 1);
        }

        private bool isAllNodesRegistered() {
            if (registeredNodes.Capacity == registeredNodes.Count) {
                return true;
            }
            return false;
        }

        private bool registerNode(int id) {
            if (!registeredNodes.Contains(id)) {
                registeredNodes.Add(id);
                return true;
            }
            return false;
        }

        private bool isAllNodesMarked() {
            if (markedNodes.Capacity == markedNodes.Count) {
                return true;
            }
            return false;
        }

        private bool markNode(int id) {
            if (!markedNodes.Contains(id)) {
                markedNodes.Add(id);
                return true;
            }
            return false;
        }


    }
}
