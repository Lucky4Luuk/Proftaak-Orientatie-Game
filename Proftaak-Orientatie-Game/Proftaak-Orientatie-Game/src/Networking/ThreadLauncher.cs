using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proftaak_Orientatie_Game.Networking
{
    class ThreadLauncher
    {
        public delegate void OnPacket(byte[] data);

        private readonly Dictionary<int, Queue<byte[]>> _packets = new Dictionary<int, Queue<byte[]>>();
        private readonly OnPacket _callback;

        public ThreadLauncher(OnPacket callback)
        {
            _callback = callback;
        }

        public void Request(int id, byte[] packet)
        {
            bool threadRunning = false;
            lock (_packets)
            {
                if (!_packets.ContainsKey(id))
                    _packets.Add(id, new Queue<byte[]>());

                if (_packets[id].Count > 0)
                    threadRunning = true;
                _packets[id].Enqueue(packet);
            }

            if(!threadRunning)
                StartThread(id, _packets, _callback);
        }

        private static void StartThread(int id, Dictionary<int, Queue<byte[]>> packets, OnPacket callback)
        {
            new Thread(() =>
            {
                Queue<byte[]> myPackets = packets[id];
                while (myPackets.Count > 0)
                {
                    byte[] packet;

                    lock (packets)
                        packet = myPackets.Dequeue();

                    callback(packet);
                }
            }).Start();
        }
    }
}
