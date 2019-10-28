using System.Collections.Generic;
using SFML.Graphics;

namespace Proftaak_Orientatie_Game.Entities
{
    class EntityManager
    {
        private readonly List<IEntity> _entities = new List<IEntity>();

        public void Add(IEntity entity)
        {
            _entities.Add(entity);
        }

        public void Update(float deltaTime)
        {
            foreach(IEntity e in _entities)
                e.OnUpdate(deltaTime);

            DeleteMarkedEntities();
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            foreach (IEntity e in _entities)
                e.OnFixedUpdate(fixedDeltaTime);

            DeleteMarkedEntities();
        }

        public void Draw(float deltatime, RenderWindow window)
        {
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
