﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.Entities.Player
{
    class GamepadController : IPlayerController
    {
        private const float TOTAL_SHOOT_COOLDOWN = 0.4f;
        private const float REPRESS_COOLDOWN_REDUCTION = 0.0f;
        private float _shootCooldown;
        private bool _isShootPressed;

        private readonly GamepadInputManager _inputManager;

        public GamepadController(GamepadInputManager inputManager)
        {
            _inputManager = inputManager;
        }

        public override void FixedUpdate(RenderWindow window, float deltatime)
        {
            if (window.HasFocus())
            {
                const float speed = 100.0f;

                // Calculate direction
                Vector2f delta = _inputManager.StickLeft;
                float len = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                if (len > 0.25f)
                {
                    delta /= len;
                    delta *= speed;
                    delta *= deltatime;

                    // Start moving
                    Position = new Vector2f(Position.X + delta.X, Position.Y + delta.Y);
                    Velocity = new Vector2f(delta.X, delta.Y);
                }
            }
        }

        public override void Update(RenderWindow window, float deltatime, Camera camera)
        {
            _inputManager.Update();

            if (window.HasFocus())
            {
                // Get the direction the mouse is pointing in
                Direction = _inputManager.StickRight / (float)Math.Sqrt(_inputManager.StickRight.X * _inputManager.StickRight.X + 
                                                                        _inputManager.StickRight.Y * _inputManager.StickRight.Y);

                bool shoot = Keyboard.IsKeyPressed(Keyboard.Key.Space);//_inputManager.Button;


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