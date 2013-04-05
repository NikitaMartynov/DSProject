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
        public int M_nodesNum{get;set;}
        public bool M_stopReceive{get;set;}
        public UdpClient m_receiverSock;

        private DataStore m_dataStore; //DictionaryOfNodes<ListOfNodeValues>
        private Node m_node;

        private Thread receiveTempThread;
        private int m_port;
        private List<int> m_registeredNodes;
        private List<int> m_markedNodes;

        private IPEndPoint m_userEndPoint;

        private static object m_lockObject = new object();




        public NodeAdmin(Node node, int port, int nodesNum) {
            m_node = node;
            m_port = port;
            M_stopReceive = false;
            M_nodesNum = nodesNum;
            m_dataStore = new DataStore();
            m_registeredNodes = new List<int>(M_nodesNum);
            m_markedNodes = new List<int>(M_nodesNum);
            //Todo make via gui
            m_userEndPoint = new IPEndPoint(IPAddress.Parse( "127.0.0.1"), 33334 );
            
            receiveTempThread = new Thread(UdpSockReceiverTemp);
            receiveTempThread.Start();
            
        }

        public void sendT(int id, int val) {
            blockingWriteTemp(id, val);
        }

        private void UdpSockReceiverTemp() {
            m_receiverSock = new UdpClient(m_port);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 22222);
            byte[] data = new byte[1024];
            string[] stringData;
            while (true) {
                if (M_stopReceive == true) {
                    m_receiverSock.Close();
                    break;
                }
                try {
                    data = m_receiverSock.Receive(ref sender);
                }
                catch (Exception e) {
                    break;
                }
                stringData = Encoding.ASCII.GetString(data, 0, data.Length).Split('_');
                //Write to dataStore
                int id = Convert.ToInt16(stringData[0]);
                int val = Convert.ToInt16(stringData[1]);

                blockingWriteTemp(id, val);
            }
        }

        private void blockingWriteTemp(int id, int val) {
            lock (m_lockObject) {
                writeTemp(id, val);
            }
        }

        private void writeTemp(int id, int val) {
            bool alreadyRegisteredAndWaitingOthers = false;
            if (!isAllNodesRegistered()) {
                if (registerNode(id)) {
                    markNode(id);
                }
                else{
                    m_node.M_form.appendTextToRichTB(String.Format("Node {0} registered and wait for others\n", id) );
                    alreadyRegisteredAndWaitingOthers = true;
                }
            }
            if (alreadyRegisteredAndWaitingOthers) return;

            if (isAllNodesRegistered()) {
                if (!m_registeredNodes.Contains(id)) {
                    m_node.M_form.appendTextToRichTB(String.Format("NOT REGISTERED Node {0} denied!\n", id));

                    return;
                }
                if (!markNode(id)) {
                    m_node.M_form.appendTextToRichTB(String.Format("Node {0}marked and wait for others\n", id));
                }
            }
            if (m_dataStore.write(id, val)) {
                m_node.M_form.appendTextToRichTB( String.Format("I am:{0}; Temp:{1}\n", id, val) );
                }
            else {
                m_node.M_form.appendTextToRichTB(String.Format("I am:{0}; RenewTemp:{1}\n", id, val));
            }
            if (isAllNodesMarked()) {
                m_markedNodes.Clear();
                udpSockSendRegTemp(m_userEndPoint);
            }
        }

        private void udpSockSendRegTemp(IPEndPoint receiverEndPoint, int port = 33333) {
            UdpClient sock = new UdpClient(port);
            List<int[]> dataList = m_dataStore.getLastTempfromAllNodes();
            string dataString;
            lastTempToString(dataList, out dataString);

            byte[] data = Encoding.ASCII.GetBytes(dataString);
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
            if (m_registeredNodes.Capacity == m_registeredNodes.Count) {
                return true;
            }
            return false;
        }

        private bool registerNode(int id) {
            if (!m_registeredNodes.Contains(id)) {
                m_registeredNodes.Add(id);
                return true;
            }
            return false;
        }

        private bool isAllNodesMarked() {
            if (m_markedNodes.Capacity == m_markedNodes.Count) {
                return true;
            }
            return false;
        }

        private bool markNode(int id) {
            if (!m_markedNodes.Contains(id)) {
                m_markedNodes.Add(id);
                return true;
            }
            return false;
        }


    }
}
