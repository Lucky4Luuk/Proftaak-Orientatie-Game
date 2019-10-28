using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.GameStates;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game
{
    class Application
    {
        static void Main(string[] args)
        {
            new Application(new Game()).Run();
        }

        private IGameState _currentGameState;
        private RenderWindow _window;

        Application(IGameState startingState)
        {
            _currentGameState = startingState;
            _currentGameState.OnCreate();
        }

        void Run()
        {
            _window = new RenderWindow(new VideoMode(1280, 720), "Gayme");
            Clock deltatTimeClock = new Clock();

            while (_window.IsOpen)
            {
                float deltatime = deltatTimeClock.ElapsedTime.AsSeconds();
                deltatTimeClock.Restart();

                if (_window.HasFocus())
                    _currentGameState.OnUpdate(deltatime);

                _currentGameState.OnDraw(deltatime, _window);

                if (_currentGameState.IsNewStateRequested())
                {
                    _currentGameState.OnDestroy();
                    _currentGameState = _currentGameState.GetRequestedState();
                    _currentGameState.OnCreate();
                }

                _window.Display();
            }
        }
    }
}
