﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

        private IPAddress ipAd = IPAddress.Parse("127.0.0.1");
        private TcpClient tcpClient;

        public override void OnCreate()
        {
            _entityManager = new EntityManager();
            _entityManager.Add(new Player(new Vector2f(300.0f, 300.0f), new KeyboardController()));

            _curLevel = new TileMap("res/maps/test.tmx");

            tcpClient = new TcpClient();
            Console.WriteLine("Connecting...");
            tcpClient.Connect(ipAd, 8001);
            Console.WriteLine("Connected!");
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
