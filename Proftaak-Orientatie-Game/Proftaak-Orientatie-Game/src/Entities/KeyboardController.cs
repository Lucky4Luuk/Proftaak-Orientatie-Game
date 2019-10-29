using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.Entities
{
    class KeyboardController : IPlayerController
    {
        public override void FixedUpdate(float deltatime)
        {
            const float SPEED = 200.0f;

            bool up = Keyboard.IsKeyPressed(Keyboard.Key.W) || Keyboard.IsKeyPressed(Keyboard.Key.Up);
            bool left = Keyboard.IsKeyPressed(Keyboard.Key.A) || Keyboard.IsKeyPressed(Keyboard.Key.Left);
            bool down = Keyboard.IsKeyPressed(Keyboard.Key.S) || Keyboard.IsKeyPressed(Keyboard.Key.Down);
            bool right = Keyboard.IsKeyPressed(Keyboard.Key.D) || Keyboard.IsKeyPressed(Keyboard.Key.Right);

            float dx = (right ? 1.0f : 0.0f) - (left ? 1.0f : 0.0f);
            float dy = (down ? 1.0f : 0.0f) - (up ? 1.0f : 0.0f);

            if (dx != 0.0f && dy != 0.0f)
            {
                dx *= 0.72f;
                dy *= 0.72f;
            }

            dx *= SPEED * deltatime;
            dy *= SPEED * deltatime;

            Position = new Vector2f(Position.X + dx, Position.Y + dy);
            Velocity = new Vector2f(dx, dy);
        }

        public override void Update(float fixedDeltatime)
        {
        }
    }
}
