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
        const float fixedDeltaTime = 1f / 60f;
        private float fixedTime = 0f;

        private Color clearColor { get; set; } = new Color(133, 108, 161, 255);

    Application(IGameState startingState)
        {
            _currentGameState = startingState;
            _currentGameState.OnCreate();
        }
        

        static void CloseWindow(object sender, EventArgs e)
        {
            ((RenderWindow)sender).Close();
        }

        void Run()
        {
            _window = new RenderWindow(new VideoMode(1280, 720), "*insert epic name here*");

            //Register events
            _window.Closed += new EventHandler(CloseWindow);

            Clock deltatTimeClock = new Clock();

            while (_window.IsOpen)
            {
                _window.DispatchEvents();

                float deltatime = deltatTimeClock.ElapsedTime.AsSeconds();
                deltatTimeClock.Restart();

                if (_window.HasFocus())
                {
                    _currentGameState.OnUpdate(deltatime);
                    fixedTime += deltatime;
                    while (fixedTime > fixedDeltaTime)
                    {
                        _currentGameState.OnFixedUpdate(fixedDeltaTime);
                        fixedTime -= fixedDeltaTime;
                    }
                }

                _currentGameState.OnDraw(deltatime, _window);

                if (_currentGameState.IsNewStateRequested())
                {
                    _currentGameState.OnDestroy();
                    _currentGameState = _currentGameState.GetRequestedState();
                    _currentGameState.OnCreate();
                }

                _window.Display();
                _window.Clear(clearColor);
            }
        }
    }
}
