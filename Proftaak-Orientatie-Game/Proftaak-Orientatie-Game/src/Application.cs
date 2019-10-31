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
            new Application(new Menu()).Run();
        }

        private IGameState _currentGameState;
        private RenderWindow _window;
        const float fixedDeltaTime = 1.0f / 60.0f;
        private float fixedTime = 0.0f;

        private const float tickDeltaTime = 1.0f / 20.0f;
        private float tickTime = 0.0f;

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

                _currentGameState.OnUpdate(deltatime, _window);
                fixedTime += deltatime;
                while (fixedTime > fixedDeltaTime)
                {
                    _currentGameState.OnFixedUpdate(fixedDeltaTime, _window);
                    fixedTime -= fixedDeltaTime;
                }

                tickTime += deltatime;
                while (tickTime > tickDeltaTime)
                {
                    _currentGameState.OnTick();
                    tickTime -= tickDeltaTime;
                }

                _currentGameState.OnDraw(deltatime, _window);

                while (_currentGameState.IsNewStateRequested())
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
