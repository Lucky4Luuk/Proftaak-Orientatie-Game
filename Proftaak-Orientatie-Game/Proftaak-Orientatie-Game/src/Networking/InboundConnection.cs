using Proftaak_Orientatie_Game.Entities;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proftaak_Orientatie_Game.Networking
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
        private readonly ThreadLauncher _threadLauncher;

        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public int nextAvailableClientID = 0;

        public List<Client> clients;

        public InboundConnection(EntityManager entityManager, ThreadLauncher.OnPacket callback)
        {
            _threadLauncher = new ThreadLauncher(callback);

            clients = new List<Client>();

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 8001);

            Socket listener = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Server: {0}:{1}", IPAddress.Any.AddressFamily, 8001);

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
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            if (state.buffer.Length > 0)
            {
                byte packetLength = state.buffer[0];

                if (state.buffer.Length >= packetLength)
                {
                    byte[] data = state.buffer.Take(packetLength).ToArray();
                    int id = BitConverter.ToInt32(data, 1);

                    _threadLauncher.Request(id, data);

                    state.buffer = state.buffer.Skip(packetLength).ToArray();
                }
            }

            //Prepare receiving the next data
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
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
