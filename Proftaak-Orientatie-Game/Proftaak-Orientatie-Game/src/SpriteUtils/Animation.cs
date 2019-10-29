using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.src.SpriteUtils
{
    class Animation
    {
        private readonly int _tileRowLength;
        private readonly Vector2i _tileSize;
        private readonly int[] _animation;

        private float _frame = 0.0f;

        public Animation(int tileW, int tileH, int imgW, int[] animation)
        {
            _tileRowLength = imgW / tileW;
            _tileSize = new Vector2i(tileW, tileH);
            _animation = animation;
        }

        private IntRect GetShape(int frame)
        {
            int x = frame % _tileRowLength;
            int y = (int) Math.Floor(frame / (float) _tileRowLength);

            return new IntRect(new Vector2i(_tileSize.X * x, _tileSize.Y * y), _tileSize);
        }

        public void SetFrame(float frame)
        {
            _frame = frame % _animation.Length;
        }

        public void Update(float deltatime)
        {
            SetFrame(_frame + deltatime);
        }

        public IntRect GetShape()
        {
            return GetShape(_animation[(int) Math.Floor(_frame)]);
        }
    }
}
