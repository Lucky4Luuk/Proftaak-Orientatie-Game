using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.GameStates;
using Proftaak_Orientatie_Game.Networking;
using Proftaak_Orientatie_Game.Networking.Server;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Server : IGameState
    {
        private EntityManager _entityManager;

        private Font font;
        private Text info;

        private readonly List<Connection> _clients = new List<Connection>();

        public override void OnCreate()
        {
            font = new Font("res/fonts/defaultFont.ttf");
            info = new Text("Server", font);

            _entityManager = new EntityManager();

            // A separate thread to make connections on
            new Thread(() =>
            {
                while (true)
                {
                    Connection client = Connection.Listen(42069, OnPacket);

                    lock(_clients)
                        _clients.Add(client);
                }
            }).Start();
        }

        public void BroadCast(byte[] data)
        {
            foreach (var client in _clients)
                client.Send(data);
        }

        private static void OnPacket(byte[] data)
        {
            Console.WriteLine("Server received: " + Encoding.ASCII.GetString(data));
        }

        public override void OnUpdate(float deltatime, RenderWindow window)
        {
            //_entityManager.Update(deltatime);
        }

        public override void OnFixedUpdate(float fixedDeltaTime, RenderWindow window)
        {
            //_entityManager.FixedUpdate(fixedDeltaTime);
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            window.Draw(info);
        }

        public override void OnDestroy()
        {
            foreach (var client in _clients)
                client.Close();
        }
    }
}
