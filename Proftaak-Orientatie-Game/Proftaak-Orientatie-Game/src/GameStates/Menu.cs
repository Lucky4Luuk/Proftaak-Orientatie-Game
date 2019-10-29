using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.GameStates;
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
            _playButton = new Button(new Vector2f(200.0f, 100.0f), new Vector2f(300.0f, 50.0f),
                new Color(0,0,0, 100), new Color(50, 50, 50, 100), new Color(100, 100, 100, 100),
                Color.White,
                "Play", font
            );
            _serverButton = new Button(new Vector2f(200.0f, 200.0f), new Vector2f(300.0f, 50.0f),
                new Color(0,0,0, 100), new Color(50, 50, 50, 100), new Color(100, 100, 100, 100),
                Color.White,
                "Server", font
            );
            _playButton.OnPress += (object sender, EventArgs args) => { RequestNewState(new Game()); };
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
            _playButton.Update(window);
            _serverButton.Update(window);

            _playButton.Draw(window);
            _serverButton.Draw(window);
        }

        public override void OnDestroy()
        {
        }
    }
}
