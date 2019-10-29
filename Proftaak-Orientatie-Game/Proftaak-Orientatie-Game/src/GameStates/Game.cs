using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.GameStates;
using Proftaak_Orientatie_Game.src.Networking;
using Proftaak_Orientatie_Game.UI;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.GameStates
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Client information.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    class Game : IGameState
    {
        private EntityManager _entityManager;
        private Level _curLevel;

        private IPAddress ipAd = IPAddress.Parse("145.93.105.11");
        private Socket socket;

        private int clientID = 0;
        private StateObject state;

        private Text debugText;
        private Font font = new Font("res/fonts/defaultFont.ttf");

        public override void OnCreate()
        {
            _entityManager = new EntityManager();
            _entityManager.Add(new Player(new Vector2f(300.0f, 300.0f), new KeyboardController()));

            _curLevel = new TileMap("res/maps/test.tmx");

            debugText = new Text("UNDEFINED", font);

            socket = new Socket(ipAd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Connecting...");
            socket.Connect(new IPEndPoint(ipAd, 8001));
            Console.WriteLine("Connected!");

            state = new StateObject();
            state.workSocket = socket;

            socket.BeginReceive(state.buffer, 0, 1024, 0, new AsyncCallback(ReadCallback), state);
        }

        public override void OnUpdate(float deltatime, RenderWindow window)
        {
            _entityManager.Update(deltatime, window);
        }

        public override void OnFixedUpdate(float fixedDeltaTime, RenderWindow window)
        {
            _entityManager.FixedUpdate(fixedDeltaTime, window);
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            _curLevel.OnDraw(deltatime, window);
            _entityManager.Draw(deltatime, window);

            window.Draw(debugText);
        }

        public override void OnDestroy()
        {}

        public void OnPacket(string rawData)
        {
            string[] splitData = rawData.Split(':');
            int identifier = Convert.ToInt32(splitData[0]);
            string data = splitData[1];

            if (identifier == (int)PACKET_TYPES.SET_CLIENT_ID)
            {
                clientID = Convert.ToInt32(data);
                debugText = new Text(data, font);
            }
            //Console.WriteLine(data);
            else
            {
                Console.WriteLine(rawData);
            }
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
                        content = content.Substring(0, content.Length - 5);
                        Console.WriteLine("Read {0} bytes from socket. \nData: {1}", content.Length, content);
                        OnPacket(content);
                    }
                    else
                    {
                        //Not all data received. Get more
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReadCallback), state);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured while handling client: {0}", e);
            }
        }
    }
}
