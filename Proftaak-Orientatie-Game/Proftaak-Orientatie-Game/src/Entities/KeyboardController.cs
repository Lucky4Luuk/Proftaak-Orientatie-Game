﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.Entities
{
    class KeyboardController : IPlayerController
    {
        public override void FixedUpdate(RenderWindow window, float deltatime)
        {
            const float speed = 200.0f;

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
            Position = new Vector2f(Position.X + dx, Position.Y + dy);
            Velocity = new Vector2f(dx, dy);
        }

        public override void Update(RenderWindow window, float fixedDeltatime)
        {
            // Get the direction the mouse is pointing in
            Vector2f mousePos = new Vector2f(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y);
            mousePos -= (Vector2f)window.Size / 2f;
            //mousePos -= Position;

            Direction = mousePos / (float) Math.Sqrt(mousePos.X * mousePos.X + mousePos.Y * mousePos.Y);

        }
    }
}
