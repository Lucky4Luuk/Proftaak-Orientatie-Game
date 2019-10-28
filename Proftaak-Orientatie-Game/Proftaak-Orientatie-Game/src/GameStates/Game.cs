using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.GameStates;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Game : IGameState
    {
        public override void OnCreate()
        {}

        public override void OnUpdate(float deltatime)
        {}

        public override void OnFixedUpdate(float fixedDeltaTime)
        {}

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            CircleShape shape = new CircleShape(100.0f);
            shape.Position = new Vector2f(300.0f, 400.0f);
            window.Draw(shape);
        }

        public override void OnDestroy()
        {}
    }
}
