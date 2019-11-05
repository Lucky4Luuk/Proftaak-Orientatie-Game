using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.Entities.Player
{
    class GamepadInputManager
    {
        private readonly SerialPort _port;

        public bool Button { get; private set; }
        public Vector2f StickLeft { get; private set; }
        public Vector2f StickRight { get; private set; }

        public GamepadInputManager()
        {
            Debug.Assert(IsGamepadConnected());

            _port = new SerialPort(SerialPort.GetPortNames()[0], 115200, Parity.None);
            _port.Open();
        }

        public static bool IsGamepadConnected()
        {
            return SerialPort.GetPortNames().Length > 0;
        }

        public void Close()
        {
            _port.Close();
        }

        public void Update()
        {
            while (_port.BytesToRead > 0)
            {
                byte[] data = new byte[1];
                _port.Read(data, 0, 1);

                // Read the data
                int header = data[0] >> 5;
                int value = data[0] & 0x1F;

                //Console.WriteLine(header + "\t" + value);

                switch (header)
                {
                    case 0:
                        StickLeft = new Vector2f(value == 31 ? -1.0f : (value == 0 ? 1.0f : 0.0f), StickLeft.Y );
                        break;
                    case 1:
                        StickLeft = new Vector2f(StickLeft.X, value == 31 ? -1.0f : (value == 0 ? 1.0f : 0.0f));
                        break;
                    case 2:
                        StickRight = new Vector2f(value == 31 ? 1.0f : (value == 0 ? -1.0f : 0.0f), StickRight.Y);
                        break;
                    case 3:
                        StickRight = new Vector2f(StickRight.X, value == 31 ? 1.0f : (value == 0 ? -1.0f : 0.0f));
                        break;
                    case 4:
                        Button = value != 0;
                        break;
                }
            }
        }
    }
}
