using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.UI
{
    class Button
    {
        private readonly Color _defaultColor, _hoverColor, _pressColor;

        private bool _lastFramePressed = false;
        private readonly RectangleShape _background;
        private readonly Text _text;

        public event EventHandler OnPress;

        public Button(Vector2f position, Vector2f size, Color defaultColor, Color hoverColor, Color pressColor, Color textColor, string text, Font font)
        {
            _defaultColor = defaultColor;
            _hoverColor = hoverColor;
            _pressColor = pressColor;

            _text = new Text(text, font);
            _text.Position = new Vector2f(
                position.X - _text.GetLocalBounds().Width / 2,
                position.Y - 3 * _text.GetLocalBounds().Height / 4
            );
            _text.FillColor = textColor;

            _background = new RectangleShape(size)
            {
                Origin = size * 0.5f,
                Position = position,
                FillColor = defaultColor
            };
        }

        public void Update(RenderWindow window)
        {
            bool pressed = Mouse.IsButtonPressed(Mouse.Button.Left);

            Vector2f mousePos = new Vector2f(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y);
            mousePos -= window.GetView().Size * 0.5f - window.GetView().Center;

            Vector2f delta = mousePos - _background.Position;

            if (Math.Abs(delta.X) < _background.Size.X * 0.5f && Math.Abs(delta.Y) < _background.Size.Y * 0.5f)
            {
                // Hover
                _background.FillColor = pressed ? _pressColor : _hoverColor;

                if (!pressed && _lastFramePressed)
                {
                    OnPress?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                _background.FillColor = _defaultColor;
            }

            _lastFramePressed = pressed;
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(_background);
            window.Draw(_text);
        }

        public void SetPosition(Vector2f position)
        {
            _text.Position = new Vector2f(
                position.X - _text.GetLocalBounds().Width / 2,
                position.Y - 3 * _text.GetLocalBounds().Height / 4
            );
            _background.Position = position;
        }
    }
}
