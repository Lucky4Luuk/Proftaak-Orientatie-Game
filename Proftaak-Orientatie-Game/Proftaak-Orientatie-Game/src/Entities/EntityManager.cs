using System.Collections.Generic;
using Proftaak_Orientatie_Game.Entities.Bullet;
using Proftaak_Orientatie_Game.Networking;
using SFML.Graphics;

namespace Proftaak_Orientatie_Game.Entities
{
    class EntityManager
    {
        private readonly List<IEntity> _entities = new List<IEntity>();
        private readonly Queue<IEntity> _buffer =new Queue<IEntity>();

        public Player.Player ActivePlayer { get; set; }

        public void ShootBullet(Bullet.Bullet bullet, float range)
        {
            Add(new BulletTrail(bullet.Origin, bullet.Origin + bullet.Direction * range));
        }

        public void Add(IEntity entity)
        {
            lock(_buffer)
                _buffer.Enqueue(entity);
        }

        public void Update(float deltaTime, ConnectionBuffer buffer, RenderWindow window)
        {
            lock(_buffer)
                while (_buffer.Count != 0)
                    _entities.Add(_buffer.Dequeue());

            foreach(IEntity e in _entities)
                e.OnUpdate(deltaTime, this, buffer, window);

            DeleteMarkedEntities();
        }

        public void FixedUpdate(float fixedDeltaTime, ConnectionBuffer buffer, RenderWindow window)
        {
            lock (_buffer)
                while (_buffer.Count != 0)
                    _entities.Add(_buffer.Dequeue());

            foreach (IEntity e in _entities)
                e.OnFixedUpdate(fixedDeltaTime, this, buffer, window);

            DeleteMarkedEntities();
        }

        public void OnTick(ConnectionBuffer buffer)
        {
            foreach (IEntity e in _entities)
                e.OnTick(buffer);
        }

        public void Draw(float deltatime, RenderWindow window)
        {
            lock (_buffer)
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
