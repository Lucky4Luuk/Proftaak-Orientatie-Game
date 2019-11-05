using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Networking;
using Proftaak_Orientatie_Game.src.Entities.Bullet;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.Entities.Bullet
{
    class BulletTrail : IEntity
    {
        private const float TOTAL_TIME = 0.25f;
        private float _time = TOTAL_TIME;

        private readonly Line _trail;

        public BulletTrail(Vector2f start, Vector2f direction, float distance)
        {
            _trail = new Line(start, direction, Color.White, distance, 5.0f);
        }

        public override void OnUpdate(float deltatime, EntityManager entityManager, ConnectionBuffer buffer, RenderWindow window)
        {
            _time -= deltatime;

            if(_time < 0.0f)
                MarkForDeletion();
        }

        public override void OnFixedUpdate(float fixedDeltatime, EntityManager entityManager, ConnectionBuffer buffer, RenderWindow window)
        {
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            _trail.Color = new Color(255, 255, 255, (byte)((_time / TOTAL_TIME) * 255));
            _trail.Thickness = 2.0f + (1.0f - _time / TOTAL_TIME) * (1.0f - _time / TOTAL_TIME) * 8.0f;
            _trail.Draw(window);
        }

        public override void OnTick(ConnectionBuffer buffer)
        {
        }

        public override Vector2f getPosition()
        {
            return new Vector2f(0.0f, 0.0f);
        }

        public override Vector2f getSize()
        {
            return new Vector2f(0.0f, 0.0f);
        }
    }
}
