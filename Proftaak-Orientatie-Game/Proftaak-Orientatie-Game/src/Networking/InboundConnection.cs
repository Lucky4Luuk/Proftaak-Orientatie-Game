using Proftaak_Orientatie_Game.Entities;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proftaak_Orientatie_Game.src.Networking
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Client information.  
        public Socket workSocket = null;
        public Client client = new Client((int)DateTime.Now.Ticks);
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();

        public StateObject(ref List<Client> clients)
        {
            clients.Add(client);
        }
    }

    public class Client
    {
        public int clientID;
        public Vector2f position;

        public Client(int id)
        {
            clientID = id;
            position = new Vector2f(0f, 0f);
        }
    }

    class InboundConnection
    {
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public int nextAvailableClientID = 0;

        public List<Client> clients;

        public InboundConnection(EntityManager entityManager)
        {
            clients = new List<Client>();

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("145.93.106.113");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8001);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Server: {0}:{1}", ipAddress.AddressFamily, 8001);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void OnPacket(string rawData)
        {
            try
            {
                string[] splitData = rawData.Split(':');
                int identifier = Convert.ToInt32(splitData[0]);
                string data = splitData[1];

                if (identifier == (int)PACKET_TYPES.CLIENT_POSITION)
                {
                    string[] dataParts = data.Split('-');
                    int cid = Convert.ToInt32(dataParts[0]);
                    float x = float.Parse(dataParts[1]);
                    float y = float.Parse(dataParts[2]);

                    foreach (Client client in clients)
                    {
                        if (client.clientID == cid)
                        {
                            //Console.WriteLine("Client with id {0} has a new position: [{1}; {2}]", cid, x, y);
                            client.position = new Vector2f(x, y);
                        }
                    }
                }
                //Console.WriteLine(data);
                else
                {
                    //Console.WriteLine(rawData);
                    return;
                }
            } catch (Exception ex)
            {
                //Console.WriteLine("Incorrect package received!");
                return;
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            //Signal the main thread to continue
            allDone.Set();

            //Get the socket that handles the client request
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            //Create the state object
            StateObject state = new StateObject(ref clients);
            state.workSocket = handler;

            Send(state.workSocket, ((int)PACKET_TYPES.SET_CLIENT_ID).ToString() + ":" + state.client.clientID.ToString());

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            //Retrieve the state object and the handler socket
            //from the asynchronous state object
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            //Read data from the client socket
            try
            {
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    //There might be more data, so store the data received far.
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    //Check for end-of-file tag. If it is not there, read
                    //more data.
                    content = state.sb.ToString();
                    if (content.IndexOf("<EOF>") > -1)
                    {
                        //All the data has been read from the client.
                        //content = content.Substring(0, content.Length - 5);
                        //Console.WriteLine("Read {0} bytes from socket. \nData: {1}", content.Length, content);
                        string[] splitContent = content.Replace("<EOF>", "~").Split('~');

                        foreach (string c in splitContent)
                        {
                            if (!(c == "" || c == " "))
                            {
                                //Console.WriteLine("Packet: {0}", c);
                                OnPacket(c);
                            }
                        }

                        Console.WriteLine("Done handling packets!");

                        //Start listening for more packets
                        state.buffer = new byte[StateObject.BufferSize];
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReadCallback), state);
                    }
                    else
                    {
                        //Not all data received. Get more
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReadCallback), state);
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine("Error occured while handling client {1}: {0}", e, state.client.clientID);
                DisconnectClient(state.client.clientID);
                Console.WriteLine("Clients left: {0}", clients.Count());
            }
        }

        private void DisconnectClient(int id)
        {
            for (int i = 0; i < clients.Count(); i++)
            {
                if (clients[i].clientID == id)
                {
                    clients.Remove(clients[i]);
                    break;
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            //Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data + "<EOF>");

            //Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                //Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                //Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
