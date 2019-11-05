using System;
using System.Collections.Generic;
using System.Data;
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

        private EntityManager _entityManager = new EntityManager();
        private TileMap _curLevel;

        private Connection _serverConnection;

        private Texture playerTexture = new Texture("res/textures/player.png");
        private Texture healthBarTexture = new Texture("res/textures/healthbar.png");


        private SceneBuffer _sceneBuffer;

        private readonly Font _font = new Font("res/fonts/defaultFont.ttf");

        private Camera camera = new Camera();

        private Text debugText;
        private Font font;

        public Game(Connection serverConnection)
        {
            _serverConnection = serverConnection;

            Player player = new Player(new Vector2f(300.0f, 300.0f), new KeyboardController(), playerTexture,
                healthBarTexture, _entityManager, camera)
            {
                Active = true
            };

            _entityManager.Add(player);
            _entityManager.ActivePlayer = player;

            _sceneBuffer = new SceneBuffer(playerTexture, healthBarTexture, _entityManager);
            _serverConnection.SetCallback((connection, data)=> { _sceneBuffer.Process(data); });
        }

        public override void OnCreate()
        {

            _connectionBuffer = new ConnectionBuffer(_serverConnection);

            font = new Font("res/fonts/defaultFont.ttf");
            _curLevel = new TileMap("res/maps/test.tmx");

            _entityManager._tilemap = (TileMap)_curLevel;
        }

        public override void OnUpdate(float deltatime, RenderWindow window)
        {
            _entityManager.Update(deltatime, _connectionBuffer, window);
            camera.Update(deltatime);
        }

        public override void OnFixedUpdate(float fixedDeltaTime, RenderWindow window)
        {
            _entityManager.FixedUpdate(fixedDeltaTime, _connectionBuffer, window);
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            Player player = _entityManager.ActivePlayer;
            camera.viewport.Size = (Vector2f)window.Size / 3.0f;
            //camera.viewport.Center = player.getPosition();
            camera.SetTargetPosition(player.getPosition());
            window.SetView(camera.viewport);
            _curLevel.OnDraw(deltatime, window, player.getPosition());
            _entityManager.Draw(deltatime, window);

            //Console.Write("FPS: " + 1.0f / deltatime + "                             \r");
        }

        public override void OnTick()
        {
            if (_sceneBuffer.ReturnToLobby)
            {
                _serverConnection.Close();
                RequestNewState(new Lobby());
            }
            else
            {
                _entityManager.OnTick(_connectionBuffer);

                try {
                    _connectionBuffer.Send();
                } catch (Exception ex) {
                    RequestNewState(new Menu());
                    _serverConnection.Close();
                }
            }
        }

        public override void OnDestroy()
        {}
    }
}
