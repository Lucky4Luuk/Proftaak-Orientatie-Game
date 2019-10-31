using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.Networking;

namespace Proftaak_Orientatie_Game.Networking
{
    class ConnectionBuffer
    {
        private readonly Queue<IPacket> _packets = new Queue<IPacket>();
        private readonly Connection _connection;

        public ConnectionBuffer(Connection connection)
        {
            _connection = connection;
        }

        public void Add(IPacket packet)
        {
            lock(_packets)
                _packets.Enqueue(packet);
        }

        public void Send()
        {
            lock(_packets)
                while(_packets.Count > 0)
                    _connection.Send(Packet.Serialize(_packets.Dequeue()));
        }
    }
}
