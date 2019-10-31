using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace Proftaak_Orientatie_Game.Entities.Bullet
{
    class Bullet
    {
        public Vector2f Origin { get; }
        public Vector2f Direction { get; }

        public Bullet(Vector2f origin, Vector2f direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public float? Intersects(Vector2f center, Vector2f size)
        {
            if (Direction.X == 0.0f && Direction.Y == 0.0f)
                return null;

            float tmin = float.MinValue;
            float tmax = float.MaxValue;

            if (Direction.X != 0.0f)
            {
                float tx1 = (center.X - size.X * 0.5f - Origin.X) / Direction.X;
                float tx2 = (center.X + size.X * 0.5f - Origin.X) / Direction.X;

                tmin = Math.Min(tx1, tx2);
                tmax = Math.Max(tx1, tx2);
            }
            else if (Origin.X < center.X - size.X * 0.5f || Origin.X > center.X + size.X * 0.5f)
            {
                return null;
            }

            if (Direction.Y != 0.0f)
            {
                float ty1 = (center.Y - size.Y * 0.5f - Origin.Y) / Direction.Y;
                float ty2 = (center.Y + size.Y * 0.5f - Origin.Y) / Direction.Y;

                tmin = Math.Max(tmin, Math.Min(ty1, ty2));
                tmax = Math.Min(tmax, Math.Max(ty1, ty2));
            }
            else if (Origin.Y < center.Y - size.Y * 0.5f || Origin.Y > center.Y + size.Y * 0.5f)
            {
                return null;
            }

            if (tmax >= tmin)
            {
                if (tmin > 0.0f)
                    return tmin;
                if (tmax > 0.0f)
                    return tmax;
            }
            return null;
        }
    }
}
