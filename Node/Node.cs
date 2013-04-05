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
        public NodeType M_nodeType{get; set;}
        
        public int M_id{get; set;}
        public NodeForm M_form{get;set;}
        public bool M_stopSend { get; set; }
        public NodeAdmin m_nodeAdmin;

        //private INodeAdmin remoteObject;
        private IPEndPoint m_adminEndpoint;
        private Thread m_sendTempThread;
        
        public Node() {
        }
        public Node(int id, NodeType nodeType) {
            M_id = id;
            M_nodeType = nodeType;
            //nodeInit();
        }

        public void nodeInit(int id, NodeType nodeType, int nodesNum, IPEndPoint adminEndpoint) {
            M_id = id;
            M_nodeType = nodeType;
            m_adminEndpoint = adminEndpoint;

            if (M_nodeType == NodeType.ADMIN) {
                adminInit(m_adminEndpoint.Port, nodesNum);
            }
            else {
                regularInit();
            }
            m_sendTempThread = new Thread(UdpSocketSendT);
            m_sendTempThread.Start();
        }

        public void setForm(NodeForm form) {
            M_form = form;
        }



        void adminInit(int port, int nodesNum){
            //TcpChannel channel = new TcpChannel(33333);
            //ChannelServices.RegisterChannel(channel, true);

            m_nodeAdmin = new NodeAdmin(this, port, nodesNum);
            //RemotingServices.Marshal(nodeAdmin, "Admin");

            //        m_dataStore = new DataStore(m_nodesNum);
            //        m_nodesNum = nodesNum;
        }

        void regularInit() {
            //TcpChannel tcpChannel = new TcpChannel();
            //ChannelServices.RegisterChannel(tcpChannel, true);
            //Type requiredType = typeof(INodeAdmin);
            //remoteObject = (INodeAdmin)Activator.GetObject(requiredType, "tcp://localhost:33333/Admin");

        }



        void UdpSocketSendT() {
            

            Random rndVal = new Random(M_id);
            int val = 0;
            //int sleepT = 0;
            while (true) {
                if (M_stopSend == true) {
                    break;
                }
                val = rndVal.Next(20, 31);
                if (M_nodeType == NodeType.ADMIN) {
                    m_nodeAdmin.sendT(M_id, val);
                }
                else {
                    UdpClient sock = new UdpClient(22222);
                    byte[] data = Encoding.ASCII.GetBytes(Convert.ToString(M_id) + "_" + Convert.ToString(val));
                    sock.Send(data, data.Length, m_adminEndpoint);
                    sock.Close();
                    //remoteObject.sendT(M_id, M_id);
                }
                //sleepT = rndVal.Next(1000, 3000);
                //Thread.Sleep(sleepT);
                Thread.Sleep(3000);
                
            }
            

            //Send to admin
         //   if (m_nodeType == NodeType.ADMIN) {
         //       m_dataStore.write(m_id, value);
         //   }
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
