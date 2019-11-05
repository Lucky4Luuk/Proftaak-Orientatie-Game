using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
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
        private Level _curLevel;

        private GamepadInputManager _inputManager = null;

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

            IPlayerController controller;

            try
            {
                var port = new SerialPort(SerialPort.GetPortNames()[0], 115200, Parity.None);
                port.Open();
                port.Close();


                _inputManager = new GamepadInputManager();
                controller = new GamepadController(_inputManager);
            }
            catch (Exception e)
            {
                controller = new KeyboardController();
            }

            Player player = new Player(new Vector2f(300.0f, 300.0f), controller, playerTexture,
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

            float scale = Math.Max(1.0f, (float) Math.Ceiling(window.Size.Y / 360.0f));

            camera.viewport.Size = (Vector2f)window.Size / scale;
            camera.SetTargetPosition(player.getPosition());
            window.SetView(camera.viewport);
            _curLevel.OnDraw(deltatime, window);
            _entityManager.Draw(deltatime, window);
        }

        public override void OnTick()
        {
            if (_sceneBuffer.ReturnToLobby)
            {
                RequestNewState(new Lobby());
            }
            else
            {
                _entityManager.OnTick(_connectionBuffer);

                _connectionBuffer.Send();
            }
        }

        public override void OnDestroy()
        {
            _inputManager?.Close();
            _serverConnection.Close();
        }
    }
}
