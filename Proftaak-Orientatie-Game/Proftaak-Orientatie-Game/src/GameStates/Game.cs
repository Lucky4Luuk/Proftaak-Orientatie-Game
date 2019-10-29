using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.GameStates;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Game : IGameState
    {
        private EntityManager _entityManager;
        private Level _curLevel;

        public override void OnCreate()
        {
            _entityManager = new EntityManager();
            _entityManager.Add(new Player());
            _curLevel = new TileMap("res/maps/test.tmx");
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
            _curLevel.OnDraw(deltatime, window);
            _entityManager.Draw(deltatime, window);
        }

        public override void OnDestroy()
        {}
    }
}
