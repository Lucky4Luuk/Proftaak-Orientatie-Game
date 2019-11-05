using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Ports;
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

        private int _nextId = 0;
        private bool _gameBusy = false;
        private float _countdown;
        private bool _countingDown = false;
        private float _gameTime;

        private const int PLAYERS_REQUIRED_FOR_GAME = 3;
        private const int COUNTDOWN_AFTER_ENOUGH_PLAYERS = 30;

        private readonly List<Connection> _lobbyClients = new List<Connection>();
        private readonly List<Connection> _gameClients = new List<Connection>();
        private readonly Dictionary<Connection, PlayerUpdatePacket> _players = new Dictionary<Connection, PlayerUpdatePacket>();

        private readonly Queue<IPacket> _actions = new Queue<IPacket>();
        private bool _closeRequested;

        private Vector2f[] _spawnpoints = {
            new Vector2f(316.4665f, 290.3999f),
            new Vector2f(484.2668f, 145.9335f),
            new Vector2f(767.8028f, 369.3341f),
            new Vector2f(1102.604f, 440.8014f),
            new Vector2f(1069.338f, 791.3385f),
            new Vector2f(673.0693f, 973.5405f),
            new Vector2f(312.0002f, 587.1363f)
        };

        public override void OnCreate()
        {
            font = new Font("res/fonts/defaultFont.ttf");
            info = new Text("Server", font);

            // A separate thread to make connections on
            new Thread(() =>
            {
                while (!_closeRequested)
                {
                    Connection client = Connection.Listen(42069, 
                    (connection, data) =>
                    {

                        if (data.Length == 0)
                            return;

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

                            if (packet.health <= 0.0f)
                                MoveToLobby(connection);
                        }

                        if (Packet.GetType(data) == PACKET_TYPES.PLAYER_SHOOT)
                        {
                            PlayerShootPacket packet = Packet.Deserialize<PlayerShootPacket>(data);

                            lock (_players)
                                if (_players.ContainsKey(connection))
                                    packet.id = _players[connection].id;

                            lock (_actions)
                                _actions.Enqueue(packet);
                        }
                    });

                    // Adding new client
                
                    lock (_lobbyClients)
                        _lobbyClients.Add(client);
                }
            }).Start();
        }

        public void BroadCastGame(byte[] data)
        {
           // new Thread(() =>
           // {
                lock (_gameClients)
                {
                    for (int i = 0; i < _gameClients.Count; i++)
                    {
                        try
                        {
                            _gameClients[i].Send(data);
                        }
                        catch (Exception)
                        {
                            lock (_players)
                            {
                                int id = _players[_gameClients[i]].id;
                                _gameClients.RemoveAt(i--);
                                BroadCastGame(Packet.Serialize(new PlayerDisconnectPacket(id)));
                            }
                        }
                    }
                }
           // }).Start();
        }

        public void BroadCastLobby(byte[] data)
        {
           // new Thread(() =>
            //{
                lock (_lobbyClients)
                {
                    for (int i = 0; i < _lobbyClients.Count; i++)
                    {
                        try
                        {
                            _lobbyClients[i].Send(data);
                        }
                        catch (Exception)
                        {
                            _lobbyClients.RemoveAt(i--);
                        }
                    }
                }
            //}).Start();
        }

        public override void OnUpdate(float deltatime, RenderWindow window)
        {
            info = new Text(string.Format("Server\nPlayer count: " + _gameClients.Count + "\nLobby count: " + _lobbyClients.Count), font);

            // Check if there are enough players in the lobby
            bool enoughPlayers;
            lock (_lobbyClients)
                enoughPlayers = _lobbyClients.Count >= PLAYERS_REQUIRED_FOR_GAME;

            if(enoughPlayers && !_gameBusy)
            {
                // Count down until the game starts
                if(_countingDown == false)
                    _countdown = COUNTDOWN_AFTER_ENOUGH_PLAYERS;

                _countingDown = true;
                _countdown -= deltatime;

                if (_countdown < 0.0f)
                {
                    _gameBusy = true;
                }
            }
            else
            {
                // Stop counting down
                _countingDown = false;
            }

            if (_gameBusy && _gameTime > 5.0f && _gameClients.Count == 0)
            {
                lock(_players)
                    _players.Clear();

                lock(_gameClients)
                    _gameClients.Clear();

                _gameBusy = false;
            }
            
            if (_gameBusy)
            {
                _gameTime += deltatime;
            }
            else
            {
                _gameTime = 0.0f;
            }
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
            // Update the game
            {
                lock (_players)
                {
                    foreach (var player in _players)
                        BroadCastGame(Packet.Serialize(player.Value));

                    Queue<Connection> removal = new Queue<Connection>();
                    foreach (var player in _players)
                        if (!_gameClients.Contains(player.Key))
                            removal.Enqueue(player.Key);

                    while (removal.Count > 0)
                        _players.Remove(removal.Dequeue());
                }

                lock (_actions)
                {
                    while (_actions.Count > 0)
                        BroadCastGame(Packet.Serialize(_actions.Dequeue()));
                }

                Connection winner = null;

                if(_gameBusy && _gameTime > 5.0f) {
                    lock (_gameClients)
                    {
                        if (_gameClients.Count == 1)
                        {
                            winner = _gameClients[0];

                            _gameBusy = false;

                            _gameClients.Clear();
                            _players.Clear();
                        }
                    }
                }

                if (winner != null)
                {
                    winner.Send(Packet.Serialize(new EpicVictoryRoyalePacket(0)));

                    lock(_lobbyClients)
                        _lobbyClients.Add(winner);
                }
            }

            // Update the lobby
            {
                LobbyInfoPacket.State state = LobbyInfoPacket.State.WAITING_FOR_PLAYERS;

                if (_gameBusy)
                {
                    state = LobbyInfoPacket.State.GAME_IN_PROGRESS;
                    if (_gameTime < 5.0f)
                        state = LobbyInfoPacket.State.LOBBY_CLOSED;
                }

                if (_countingDown)
                    state = LobbyInfoPacket.State.STARTING;

                lock (_lobbyClients)
                {
                    BroadCastLobby(Packet.Serialize(new LobbyInfoPacket(state, _lobbyClients.Count,
                        (int) Math.Round(_countdown))));

                    if (_lobbyClients.Count > 0 && state == LobbyInfoPacket.State.LOBBY_CLOSED)
                    {
                        Thread.Sleep(500);

                        // TODO: pick spawnpoints
                        foreach(var client in _lobbyClients)
                        {
                            int playerId = ++_nextId;

                            client.Send(Packet.Serialize(new PlayerSpawnPacket(playerId, _spawnpoints[playerId % _spawnpoints.Length])));
                            _gameClients.Add(client);
                            _players.Add(client, new PlayerUpdatePacket(
                                playerId, Player.MAX_HEALTH, new Vector2f(0.0f, 0.0f), new Vector2f(0.0f, 0.0f), new Vector2f(0.0f, 0.0f)
                            ));
                        }
                        _lobbyClients.Clear();
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            foreach (var client in _gameClients)
                client.Close();

            foreach (var client in _lobbyClients)
                client.Close();

            _closeRequested = true;
        }

        private void MoveToLobby(Connection connection)
        {
            int id = -1;

            lock (_players)
            {
                if (_players.ContainsKey(connection))
                {
                    id = _players[connection].id;
                    _players.Remove(connection);
                }
            }

            lock (_gameClients)
            {
                _gameClients.Remove(connection);
            }

            lock (_lobbyClients)
            {
                _lobbyClients.Add(connection);
            }

            if(id != -1)
                BroadCastGame(Packet.Serialize(new PlayerDisconnectPacket(id)));
        }
    }
}
