using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.GameStates;
using Proftaak_Orientatie_Game.Networking;
using Proftaak_Orientatie_Game.UI;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Game : IGameState
    {
        private EntityManager _entityManager;
        private Level _curLevel;

        private Text debugText;
        private Font font = new Font("res/fonts/defaultFont.ttf");

        public override void OnCreate()
        {



            Texture playerTexture = new Texture("res/textures/player.png");
            Texture healthBarTexture = new Texture("res/textures/healthbar.png");

            _entityManager = new EntityManager();
            _entityManager.Add(new Player(new Vector2f(300.0f, 300.0f), new KeyboardController(), playerTexture, healthBarTexture, _entityManager));

            _curLevel = new TileMap("res/maps/test.tmx");

            debugText = new Text("UNDEFINED", font);
        }

        public override void OnUpdate(float deltatime, RenderWindow window)
        {
            _entityManager.Update(deltatime, window);
        }

        public override void OnFixedUpdate(float fixedDeltaTime, RenderWindow window)
        {
            _entityManager.FixedUpdate(fixedDeltaTime, window);
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            _curLevel.OnDraw(deltatime, window);
            _entityManager.Draw(deltatime, window);

            window.Draw(debugText);
        }

        public override void OnDestroy()
        {}
    }
}
