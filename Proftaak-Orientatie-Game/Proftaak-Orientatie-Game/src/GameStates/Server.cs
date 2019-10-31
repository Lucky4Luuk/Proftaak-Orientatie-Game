﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.Entities.Player;
using Proftaak_Orientatie_Game.GameStates;
using Proftaak_Orientatie_Game.Networking;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.GameStates
{
    class Server : IGameState
    {
        private Font font;
        private Text info;

        private int nextId = 0;

        private readonly List<Connection> _clients = new List<Connection>();
        private readonly Dictionary<Connection, PlayerUpdatePacket> _players = new Dictionary<Connection, PlayerUpdatePacket>();

        private readonly Queue<IPacket> _actions = new Queue<IPacket>();

        public override void OnCreate()
        {
            font = new Font("res/fonts/defaultFont.ttf");
            info = new Text("Server", font);

            // A separate thread to make connections on
            new Thread(() =>
            {
                while (true)
                {
                    Connection client = Connection.Listen(42069, 
                    (connection, data) => {

                        // Responding for a packet
                        if (Packet.GetType(data) == PACKET_TYPES.PLAYER_UPDATE)
                        {
                            PlayerUpdatePacket packet = Packet.Deserialize<PlayerUpdatePacket>(data);

                            lock (_players)
                            {
                                if (_players.ContainsKey(connection))
                                {
                                    packet.id = _players[connection].id;
                                    _players[connection] = packet;
                                }
                            }
                        }

                        if (Packet.GetType(data) == PACKET_TYPES.PLAYER_SHOOT)
                        {
                            var packet = Packet.Deserialize<PlayerShootPacket>(data);

                            lock (_players)
                                if (_players.ContainsKey(connection))
                                    packet.id = _players[connection].id;

                            lock (_actions)
                                _actions.Enqueue(packet);
                        }
                    });

                    // Adding new client
                    lock (_clients)
                    {
                        _clients.Add(client);
                    }

                    lock (_players)
                    {

                        _players.Add(client, new PlayerUpdatePacket(nextId++, 100,
                            new Vector2f(0.0f, 0.0f),
                            new Vector2f(0.0f, 0.0f),
                            new Vector2f(0.0f, 0.0f))
                        );
                        client.Send(Packet.Serialize(new PlayerSpawnPacket(_players.Last().Value.id, _players.Last().Value.position)));
                    }
                }
            }).Start();
        }

        public void BroadCast(byte[] data)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                try
                {
                    _clients[i].Send(data);
                }
                catch (Exception)
                {
                    int id = _players[_clients[i]].id;
                    _clients.RemoveAt(i--);
                    BroadCast(Packet.Serialize(new PlayerDisconnectPacket(id)));
                }
            }
        }

        public override void OnUpdate(float deltatime, RenderWindow window)
        {
            info = new Text(string.Format("Server\nPlayer count: {0}", _clients.Count()), font);
            //_entityManager.Update(deltatime);
        }

        public override void OnFixedUpdate(float fixedDeltaTime, RenderWindow window)
        {
            //_entityManager.FixedUpdate(fixedDeltaTime);
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            window.Draw(info);
        }

        public override void OnTick()
        {
            lock (_players)
            {
                foreach (var player in _players)
                    BroadCast(Packet.Serialize(player.Value));

                Queue<Connection> removal = new Queue<Connection>();
                foreach (var player in _players)
                    if (!_clients.Contains(player.Key))
                        removal.Enqueue(player.Key);

                while (removal.Count > 0)
                    _players.Remove(removal.Dequeue());
            }

            lock (_actions)
            {
                foreach (var action in _actions)
                    BroadCast(Packet.Serialize(action));
            }
        }

        public override void OnDestroy()
        {
            foreach (var client in _clients)
                client.Close();
        }
    }
}
