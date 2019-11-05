using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities.Player;
using Proftaak_Orientatie_Game.GameStates;
using Proftaak_Orientatie_Game.Networking;
using Proftaak_Orientatie_Game.UI;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Menu : IGameState
    {
        private Button _playButton, _serverButton;

        public override void OnCreate()
        {
            Font font = new Font("res/fonts/defaultFont.ttf");
            _playButton = new Button(new Vector2f(0.0f, 0.0f), new Vector2f(500.0f, 50.0f),
                new Color(0,0,0, 100), new Color(50, 50, 50, 100), new Color(100, 100, 100, 100),
                Color.White,
                "Play", font
            );
            _serverButton = new Button(new Vector2f(0.0f, 100.0f), new Vector2f(500.0f, 50.0f),
                new Color(0,0,0, 100), new Color(50, 50, 50, 100), new Color(100, 100, 100, 100),
                Color.White,
                "Host Server", font
            );
            _playButton.OnPress += (object sender, EventArgs args) => { RequestNewState(new Lobby()); };
            _serverButton.OnPress += (object sender, EventArgs args) => { RequestNewState(new Server()); };
        }

        public override void OnUpdate(float deltatime, RenderWindow window)
        {
        }

        public override void OnFixedUpdate(float fixedDeltaTime, RenderWindow window)
        {
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            window.SetView(new View(new Vector2f(0.0f, 0.0f), (Vector2f)window.Size));

            _playButton.Update(window);
            _serverButton.Update(window);

            _playButton.Draw(window);
            _serverButton.Draw(window);
        }

        public override void OnTick()
        {
        }

        public override void OnDestroy()
        {
        }
    }
}
