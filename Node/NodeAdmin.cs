using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace DSProject
{
    public class NodeAdmin
    {
        public bool StopReceive { get; set; }
        public UdpClient TempReceiverSock;
        public UdpClient RegReceiverSock;
        public TcpListener ListenerTcp;
        public bool StartedByUser { get; set; }

        private Dictionary<int, string> runningRegNodes;
        private bool initialAdmin;

        private DataStore dataStore; //DictionaryOfNodes<ListOfNodeValues>
        private Node node;
        private Thread regNodesThread;
        private Thread receiveTempThread;
        private Thread workWithUserThread;

        private IPEndPoint userEndPoint;

        private static object lockObject = new object();

        public NodeAdmin(Node node, string userIP, bool initialAdmin, Dictionary<int, string> runningRegNodes) 
        {
            this.node = node;
            this.StopReceive = false;
            this.initialAdmin = initialAdmin;
            this.runningRegNodes = runningRegNodes;
            this.dataStore = new DataStore();

            dataStore.addNewNode(node.Id);

            start(userIP);
        }

        public void sendT(int id, int val, int clk) 
        {
            if (StartedByUser)
                blockingWriteTransaction(id, val, getLocalIPAddress(), clk);
        }

        public string getLocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        private void start(string userIP) 
        {
            this.userEndPoint = new IPEndPoint(IPAddress.Parse(userIP), 33334);
            StartedByUser = true;

            workWithUserThread = new Thread(tcpSockGetAvrAndFailByUser);
            workWithUserThread.Start();

            regNodesThread = new Thread(udpSockRegNodes);
            regNodesThread.Start();

            receiveTempThread = new Thread(udpSockReceiverTemp);
            receiveTempThread.Start();
        }

        private void tcpSockGetAvrAndFailByUser() 
        {
            int localPort = 33000;
            ListenerTcp = null;
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
            while (true) 
            {
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
                }
            }
        }

        private void udpSockRegNodes() { 
            int localPort = 11100;
            int remoteDefPort = 22200;
            RegReceiverSock = new UdpClient(localPort);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, remoteDefPort);
            List<int> reregisteredNodes = new List<int>();

            byte[] data = new byte[1024];
            string[] stringData;
            while (true) 
            {
                if (StopReceive == true) 
                {
                    RegReceiverSock.Close();
                    break;
                }
                if (initialAdmin) 
                {
                    try
                    {
                        data = RegReceiverSock.Receive(ref sender);
                    }
                    catch (Exception) { }
                    //{
                    //    if(e.Message.Contains("connection attempt failed because the")) continue;
                    //    // TODO Exception Receive fail because connected party respond false on the request, after admin 
                    //    //node changed, so no new nodes can be registered
                    //    break;
                    //}
                    stringData = Encoding.ASCII.GetString(data, 0, data.Length).Split('_');
                    if (stringData[0].Equals("regMe"))
                    {
                        int id = Convert.ToInt16(stringData[1]);
                        if (!dataStore.addNewNode(id)) 
                        {
                            node.Form.appendTextToRichTB(String.Format("Node {0} already registered\n", id));
                        }
                        byte[] data1 = Encoding.ASCII.GetBytes("IamYourAdmin");

                        TempReceiverSock.Send(data1, data1.Length, sender);
                    }
                }
                else 
                {
                    byte[] data1 = Encoding.ASCII.GetBytes("IamYourNewAdmin");
                    Debug.Assert(runningRegNodes != null, "runningRegNodes is null when you run admin shifting!");
                    if (reregisteredNodes.Count == runningRegNodes.Count) 
                    {
                        initialAdmin = true;
                        continue;
                    }

                    foreach (KeyValuePair<int, string> item in runningRegNodes)
                    {
                        if (reregisteredNodes.Contains(item.Key)) continue;
                        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(item.Value), remoteDefPort + item.Key);
                        RegReceiverSock.Send(data1, data1.Length, remoteEndPoint);
                        RegReceiverSock.Client.ReceiveTimeout = 2000;

                        byte[] receivedData = null;
                        try 
                        {
                            receivedData = RegReceiverSock.Receive(ref sender);
                        }
                        catch (Exception e) { }
                        if (receivedData == null) continue;
                        string[] strAr = Encoding.ASCII.GetString(receivedData, 0, receivedData.Length).Split('_');
                        if (!strAr[0].Equals("reregMe")) continue;
                        int id = Convert.ToInt16(strAr[1]);
                        reregisteredNodes.Add(id);
                        if (!dataStore.addNewNode(id)) {
                            node.Form.appendTextToRichTB(String.Format("Node {0} already registered\n", id));
                        }
                    }
                }
            }
        }

        private void udpSockReceiverTemp() 
        {
            TempReceiverSock = new UdpClient(11111);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 22222);
            byte[] data = new byte[1024];
            string[] stringData;
            while (true) 
            {
                if (StopReceive == true) 
                {
                    TempReceiverSock.Close();
                    break;
                }
                try 
                {
                    data = TempReceiverSock.Receive(ref sender);
                }
                catch (Exception e) 
                {
                    break;
                }
                stringData = Encoding.ASCII.GetString(data, 0, data.Length).Split('_');
                int id = Convert.ToInt16(stringData[0]);
                int val = Convert.ToInt16(stringData[1]);
                int clk = Convert.ToInt16(stringData[2]);
                string senderIP = sender.Address.ToString();
                blockingWriteTransaction(id, val, senderIP, clk);
            }
        }

        private void blockingWriteTransaction(int id, int val, string ip, int clk)
        {
            lock (lockObject)
            {
                if (dataStore.write(id, val, clk))
                {
                    node.Form.appendTextToRichTB(String.Format("I am:{0}; Temp:{1}\n", id, val));
                    if(dataStore.newNode)
                        udpSockSendRegTemp(userEndPoint, id + "_" + val + "_" + ip);
                    else
                        udpSockSendRegTemp(userEndPoint, id + "_" + val);
                }
                else
                {
                    node.Form.appendTextToRichTB(String.Format("Not registered Node {0} trying to send data\n", id));
                }
            }
        }

        private void udpSockSendRegTemp(IPEndPoint receiverEndPoint, string regularTemp, int portSender = 33333) 
        {
            UdpClient sock = new UdpClient(portSender);
            byte[] data = Encoding.ASCII.GetBytes(regularTemp);
            sock.Send(data, data.Length, receiverEndPoint);
            sock.Close();
        }

    }
}
