using System.Collections.Generic;
using SFML.Graphics;

namespace Proftaak_Orientatie_Game.Entities
{
    class EntityManager
    {
        private readonly List<IEntity> _entities = new List<IEntity>();
        private readonly Queue<IEntity> _buffer =new Queue<IEntity>();

        public void ShootBullet(Bullet bullet, float range)
        {
            Add(new BulletTrail(bullet.Origin, bullet.Origin + bullet.Direction * range));
        }

        public void Add(IEntity entity)
        {
            _buffer.Enqueue(entity);
        }

        public void Update(float deltaTime, RenderWindow window)
        {
            while (_buffer.Count != 0)
                _entities.Add(_buffer.Dequeue());

            foreach(IEntity e in _entities)
                e.OnUpdate(deltaTime, this, window);

            DeleteMarkedEntities();
        }

        public void FixedUpdate(float fixedDeltaTime, RenderWindow window)
        {
            while (_buffer.Count != 0)
                _entities.Add(_buffer.Dequeue());

            foreach (IEntity e in _entities)
                e.OnFixedUpdate(fixedDeltaTime, this, window);

            DeleteMarkedEntities();
        }

        public void Draw(float deltatime, RenderWindow window)
        {
            while (_buffer.Count != 0)
                _entities.Add(_buffer.Dequeue());

            foreach (IEntity e in _entities)
                e.OnDraw(deltatime, window);
        }

        private void DeleteMarkedEntities()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                if (_entities[i].HasDeletionMark())
                {
                    _entities.RemoveAt(i);
                    --i;
                }
            }
        }
    }
}
