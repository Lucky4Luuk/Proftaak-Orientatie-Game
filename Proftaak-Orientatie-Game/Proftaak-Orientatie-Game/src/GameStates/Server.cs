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

        public override void OnCreate()
        {
            font = new Font("res/fonts/defaultFont.ttf");
            info = new Text("Server", font);

            _entityManager = new EntityManager();

            new Thread(() => {
                Connection c = Connection.Listen(42069, OnPacket);
                c.Send(Encoding.ASCII.GetBytes("Hello World! ~Server"));

            }).Start();
            new Thread(() =>
            {
                Connection c = new Connection(IPAddress.Parse("127.0.0.1"), 42069, OnClientPacket);

                for (int i = 0; i < 3; i++) {
                    c.Send(Encoding.ASCII.GetBytes("Hello World! ~Client"));
                }
            }).Start();


        }

        public static void OnClientPacket(byte[]data)
        {
            Console.WriteLine("Client received: " + Encoding.ASCII.GetString(data));
        }

        public static void OnPacket(byte[] data)
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
        { }
    }
}
