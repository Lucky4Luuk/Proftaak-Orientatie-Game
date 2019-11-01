using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.src.Entities.Bullet
{
    class Line
    {
        private readonly RectangleShape _shape;

        public float Thickness
        {
            get => _shape.Size.Y;
            set
            {
                _shape.Origin = new Vector2f(0.0f, value * 0.5f);
                _shape.Size = new Vector2f(_shape.Size.X, value);
            }
        }

        public float Distance
        {
            get => _shape.Size.X;
            set => _shape.Size = new Vector2f(value, _shape.Size.Y);
        }
        public Color Color
        {
            get => _shape.FillColor;
            set => _shape.FillColor = value;
        }

        public Line(Vector2f origin, Vector2f direction, Color color, float distance, float thickness)
        {
            _shape = new RectangleShape();
            _shape.Position = origin;

            Thickness = thickness;
            Distance = distance;
            Color = color;

            _shape.Rotation = 90.0f - (float) Math.Atan2(direction.X, direction.Y) * 180.0f / (float)Math.PI;
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(_shape);
        }

    }
}
