using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.GameStates;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Game : IGameState
    {
        private EntityManager _entityManager;

        public override void OnCreate()
        {
            _entityManager = new EntityManager();
            _entityManager.Add(new Player(new KeyboardController()));
        }

        public override void OnUpdate(float deltatime)
        {
            _entityManager.Update(deltatime);
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            _entityManager.FixedUpdate(fixedDeltaTime);
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            _entityManager.Draw(deltatime, window);
        }

        public override void OnDestroy()
        {}
    }
}
