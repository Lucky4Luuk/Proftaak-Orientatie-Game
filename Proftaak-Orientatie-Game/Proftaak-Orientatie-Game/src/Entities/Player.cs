using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.Entities
{
    class Player : IEntity
    {
        private readonly Sprite _sprite;

        public Player()
        {
            Texture tex = new Texture("res/textures/player.png");
            _sprite = new Sprite(tex)
            {
                Position = new Vector2f(200.0f, -250.0f)
            };
        }

        public override void OnUpdate(float deltatime)
        {
        }

        public override void OnFixedUpdate(float fixedDeltatime)
        {
            float deltaX = 0.0f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                deltaX -= 8.5f;

            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                deltaX += 8.5f;

            float deltaY = 0.0f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                deltaY -= 8.5f;

            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                deltaY += 8.5f;

            if (deltaX != 0.0f && deltaY != 0.0f)
            {
                deltaX *= 0.73f;
                deltaY *= 0.73f;
            }

            _sprite.Position = new Vector2f(_sprite.Position.X + deltaX, _sprite.Position.Y + deltaY);
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            window.Draw(_sprite);
        }
    }
}
