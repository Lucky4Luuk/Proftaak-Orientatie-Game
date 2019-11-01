using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.GameStates;
using Proftaak_Orientatie_Game.Networking;
using Proftaak_Orientatie_Game.UI;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Lobby : IGameState
    {
        private Font _font;
        private Text _statusText;
        private Text _playerCountText;
        private Button _leaveButton;

        private Connection _serverConnection;

        public override void OnCreate()
        {
            try
            {
                _serverConnection = new Connection(IPAddress.Parse("145.93.140.146"), 42069,
                    (connection, data) => { OnPacket(data); });
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                RequestNewState(new Menu());
            }

            _font = new Font("res/fonts/defaultFont.ttf");

            _statusText = new Text("Game is currently in progress, please wait.", _font);
            _playerCountText = new Text("Players in lobby: ", _font);
            _leaveButton = new Button(new Vector2f(200.0f, 100.0f), new Vector2f(300.0f, 50.0f),
                new Color(0, 0, 0, 100), new Color(50, 50, 50, 100), new Color(100, 100, 100, 100),
                Color.White,
                "Leave", _font
            );

            _leaveButton.OnPress += (object sender, EventArgs args) => { RequestNewState(new Menu()); _serverConnection.Close(); };
        }

        public void OnPacket(byte[] data)
        {
            if (data.Length == 0)
                return;

            PACKET_TYPES type = Packet.GetType(data);
            if (type == PACKET_TYPES.LOBBY_INFO)
            {
                LobbyInfoPacket packet = Packet.Deserialize<LobbyInfoPacket>(data);
                if(packet.state == LobbyInfoPacket.State.LOBBY_CLOSED)
                    RequestNewState(new Game(_serverConnection));
                

                if(packet.state == LobbyInfoPacket.State.STARTING)
                    _statusText.DisplayedString = "Game is starting in " + packet.countdown + " seconds!";
                

                if (packet.state == LobbyInfoPacket.State.GAME_IN_PROGRESS)
                    _statusText.DisplayedString = "Game is currently in progress, please wait.";
                

                if (packet.state == LobbyInfoPacket.State.WAITING_FOR_PLAYERS)
                    _statusText.DisplayedString = "Waiting for players...";
                
                _playerCountText.DisplayedString = "Players in lobby: " + packet.playerCount;
            }
        }

        public override void OnUpdate(float deltatime, RenderWindow window)
        {
            _leaveButton.Update(window);
        }

        public override void OnFixedUpdate(float fixedDeltaTime, RenderWindow window)
        {
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            window.SetView(new View(new Vector2f(0.0f, 0.0f), (Vector2f)window.Size));

            _leaveButton.SetPosition(window.GetView().Size * 0.5f - new Vector2f(180.0f, 50.0f));
            _playerCountText.Position = -window.GetView().Size * 0.5f;
            _statusText.Position = new Vector2f(-_statusText.GetLocalBounds().Width * 0.5f, 0.0f);

            window.Draw(_statusText);
            window.Draw(_playerCountText);
            _leaveButton.Draw(window);
        }

        public override void OnTick()
        {
        }

        public override void OnDestroy()
        {
        }
    }
}
