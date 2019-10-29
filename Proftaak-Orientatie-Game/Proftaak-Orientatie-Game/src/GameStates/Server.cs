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
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Server : IGameState
    {
        private EntityManager _entityManager;
        private InboundConnection conn;

        private Font font;
        private Text info;

        public override void OnCreate()
        {
            font = new Font("res/fonts/defaultFont.ttf");
            info = new Text("Server", font);

            _entityManager = new EntityManager();
            //_entityManager.Add(new Player(new KeyboardController()));

            //_curLevel = new TileMap("res/maps/test.tmx");

            conn = new InboundConnection(_entityManager);
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
