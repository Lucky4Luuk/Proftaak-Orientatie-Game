using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.Entities.Player;
using Proftaak_Orientatie_Game.GameStates;
using Proftaak_Orientatie_Game.Networking;
using Proftaak_Orientatie_Game.UI;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Game : IGameState
    {
        private ConnectionBuffer _connectionBuffer;

        private EntityManager _entityManager;
        private Level _curLevel;

        private Connection _serverConnection;
        private SceneBuffer _sceneBuffer;

        private readonly Font _font = new Font("res/fonts/defaultFont.ttf");

        public override void OnCreate()
        {
            Texture playerTexture = new Texture("res/textures/player.png");
            Texture healthBarTexture = new Texture("res/textures/healthbar.png");

            _entityManager = new EntityManager();

            Player player = new Player(new Vector2f(300.0f, 300.0f), new KeyboardController(), playerTexture,
                healthBarTexture, _entityManager)
            {
                Active = true
            };

            _entityManager.Add(player);
            _entityManager.ActivePlayer = player;

            _curLevel = new TileMap("res/maps/test.tmx");

            _sceneBuffer = new SceneBuffer(playerTexture, healthBarTexture, _entityManager);

            Console.WriteLine("Connecting...");
            try
            {
                _serverConnection = new Connection(IPAddress.Parse("127.0.0.1"), 42069,
                    (connection, data) => { _sceneBuffer.Process(data); }
                );
                _connectionBuffer  = new ConnectionBuffer(_serverConnection);

                Console.WriteLine("Connected!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                RequestNewState(new Menu());
            }
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

        public override void OnTick()
        {
            _entityManager.OnTick(_connectionBuffer);

            _connectionBuffer.Send();
        }

        public override void OnDestroy()
        {}
    }
}
