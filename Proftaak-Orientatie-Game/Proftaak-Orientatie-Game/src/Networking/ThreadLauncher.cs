using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proftaak_Orientatie_Game.Networking.Server
{
    class ThreadLauncher
    {
        public delegate void OnPacket(byte[] data);

        private readonly Queue<byte[]> _packets = new Queue<byte[]>();
        private readonly OnPacket _callback;

        public ThreadLauncher(OnPacket callback)
        {
            _callback = callback;
        }

        public void Request(byte[] packet)
        {
            bool threadRunning = false;
            lock (_packets)
            {
                if (_packets.Count > 0)
                    threadRunning = true;

                _packets.Enqueue(packet);
            }

            if(!threadRunning)
                StartThread(_packets, _callback);
        }

        private static void StartThread(Queue<byte[]> packets, OnPacket callback)
        {
            new Thread(() =>
            {
                while (packets.Count > 0)
                {
                    byte[] packet;

                    lock (packets)
                        packet = packets.Dequeue();

                    callback(packet);
                }
            }).Start();
        }
    }
}
