using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.Entities
{
    class BulletTrail : IEntity
    {
        private const float TOTAL_TIME = 1.0f;
        private float _time = TOTAL_TIME;

        private readonly VertexArray _line;

        public BulletTrail(Vector2f start, Vector2f stop)
        {
            _line = new VertexArray(PrimitiveType.LineStrip, 2);
            _line[0] = new Vertex(start, new Color(255, 255, 255, (byte)((_time / TOTAL_TIME) * 255)));
            _line[1] = new Vertex(stop, new Color(255, 255, 255, (byte)((_time / TOTAL_TIME) * 255)));
        }

        public override void OnUpdate(float deltatime, EntityManager entityManager, RenderWindow window)
        {
            _time -= deltatime;

            if(_time < 0.0f)
                MarkForDeletion();
        }

        public override void OnFixedUpdate(float fixedDeltatime, EntityManager entityManager, RenderWindow window)
        {
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            _line[0] = new Vertex(_line[0].Position, new Color(255, 255, 255, (byte)((_time / TOTAL_TIME) * 255)));
            _line[1] = new Vertex(_line[1].Position, new Color(255, 255, 255, (byte)((_time / TOTAL_TIME) * 255)));
            window.Draw(_line);
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
