using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Networking;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.Entities.Player
{
    class KeyboardController : IPlayerController
    {
        private const float TOTAL_SHOOT_COOLDOWN = 0.4f;
        private const float REPRESS_COOLDOWN_REDUCTION = 0.1f;
        private float _shootCooldown;
        private bool _isShootPressed;

        private EntityManager _manager;
        public KeyboardController(EntityManager manager)
        {
            _manager = manager;
        }

        public override void FixedUpdate(RenderWindow window, float deltatime)
        {
            if (window.HasFocus())
            {
                const float speed = 100.0f;

                // Register inputs
                bool up = Keyboard.IsKeyPressed(Keyboard.Key.W) || Keyboard.IsKeyPressed(Keyboard.Key.Up);
                bool left = Keyboard.IsKeyPressed(Keyboard.Key.A) || Keyboard.IsKeyPressed(Keyboard.Key.Left);
                bool down = Keyboard.IsKeyPressed(Keyboard.Key.S) || Keyboard.IsKeyPressed(Keyboard.Key.Down);
                bool right = Keyboard.IsKeyPressed(Keyboard.Key.D) || Keyboard.IsKeyPressed(Keyboard.Key.Right);

                // Calculate direction
                float dx = (right ? 1.0f : 0.0f) - (left ? 1.0f : 0.0f);
                float dy = (down ? 1.0f : 0.0f) - (up ? 1.0f : 0.0f);

                // Normalize the direction
                if (dx != 0.0f && dy != 0.0f)
                {
                    dx *= 0.72f;
                    dy *= 0.72f;
                }

                // Give the direction a length of speed
                dx *= speed * deltatime;
                dy *= speed * deltatime;

                // Start moving
                Position = new Vector2f(Position.X + dx, Position.Y);

                bool collision = false;
                for (int x = -1; x <= 1; x += 2)
                    for (int y = -1; y <= 1; y += 2)
                        if (_manager.CheckCollision(new Vector2f(Position.X + x * 2, Position.Y + y * 2)))
                            collision = true;

                if(collision)
                    Position = new Vector2f(Position.X - dx, Position.Y);

                Position = new Vector2f(Position.X, Position.Y + dy);

                collision = false;
                for (int x = -1; x <= 1; x += 2)
                    for (int y = -1; y <= 1; y += 2)
                        if (_manager.CheckCollision(new Vector2f(Position.X + x * 2, Position.Y + y * 2)))
                            collision = true;

                if (collision)
                    Position = new Vector2f(Position.X, Position.Y - dy);

                Velocity = new Vector2f(dx, dy);
            }
        }

        public override void Update(RenderWindow window, float deltatime, Camera camera)
        {
            if (window.HasFocus())
            {
                // Get the direction the mouse is pointing in
                Vector2f mousePos = new Vector2f(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y);
                mousePos -= (Vector2f)window.Size * 0.5f;

                Direction = mousePos / (float) Math.Sqrt(mousePos.X * mousePos.X + mousePos.Y * mousePos.Y);

                bool shoot = Keyboard.IsKeyPressed(Keyboard.Key.Space);


                _shootCooldown -= deltatime;
                if (_isShootPressed && !shoot)
                    _shootCooldown -= REPRESS_COOLDOWN_REDUCTION;

                ShotOrigin = null;

                if (shoot && _shootCooldown <= 0.0f)
                {
                    ShotOrigin = Position;
                    ShotDirection = Direction;

                    camera.Shake(0.25f);
                    _shootCooldown = TOTAL_SHOOT_COOLDOWN;
                }

                _isShootPressed = shoot;
            }
        }
    }
}
