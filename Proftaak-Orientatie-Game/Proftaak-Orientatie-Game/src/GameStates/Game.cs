using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.GameStates;
using Proftaak_Orientatie_Game.Networking;
using Proftaak_Orientatie_Game.src.Networking;
using Proftaak_Orientatie_Game.UI;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Game : IGameState
    {
        private EntityManager _entityManager;
        private Level _curLevel;

        private Connection _serverConnection;
        private SceneBuffer _sceneBuffer;

        private readonly Font _font = new Font("res/fonts/defaultFont.ttf");

        public override void OnCreate()
        {
            Console.WriteLine("Connecting...");
            try
            {
                _serverConnection = new Connection(IPAddress.Parse("127.0.0.1"), 42069,
                    (data) => { _sceneBuffer.Process(data); }
                );

                Console.WriteLine("Connected!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                RequestNewState(new Menu());
            }

            Texture playerTexture = new Texture("res/textures/player.png");
            Texture healthBarTexture = new Texture("res/textures/healthbar.png");

            _entityManager = new EntityManager();
            _entityManager.Add(new Player(new Vector2f(300.0f, 300.0f), new KeyboardController(), playerTexture, healthBarTexture, _entityManager));

            _curLevel = new TileMap("res/maps/test.tmx");
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
        }

        public override void OnDestroy()
        {}
    }
}
