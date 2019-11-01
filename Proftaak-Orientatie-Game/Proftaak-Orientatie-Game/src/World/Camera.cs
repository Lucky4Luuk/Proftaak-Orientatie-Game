using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proftaak_Orientatie_Game.World
{
    class Camera
    {
        public View viewport = new View(new Vector2f(0, 0), new Vector2f(256, 256));

        private Vector2f _position = new Vector2f(0f, 0f);
        private Vector2f _targetPosition = new Vector2f(0.0f, 0.0f);

        private const float SHAKE_INTENSITY_MULTIPLIER = 64.0f;
        private const float SHAKE_DROPOFF_SPEED = 1.0f;
        private const float SHAKE_FREQUENCY = 6.0f;
        private float _shakeIntensity;
        private float _time = 0.0f;

        public Camera()
        {}

        public void SetTargetPosition(Vector2f position)
        {
            _targetPosition = position;
        }

        public void Shake(float intensity)
        {
            _shakeIntensity += intensity;
            if (_shakeIntensity > 1.0f)
                _shakeIntensity = 1.0f;
        }

        public void Update(float deltaTime)
        {
            _position = (_targetPosition * 0.5f + _position * 0.5f);
            _time += deltaTime;

            // Decrease the amount of shaking over time
            _shakeIntensity -= SHAKE_DROPOFF_SPEED * deltaTime;
            if (_shakeIntensity < 0.0f)
                _shakeIntensity = 0.0f;

            // Square the intensity to get less shake in lower intensity levels
            float actualShakeIntensity = _shakeIntensity * _shakeIntensity * SHAKE_INTENSITY_MULTIPLIER;

            // Calculate the offset
            Vector2f shakeOffset = new Vector2f(Noise.Calc1D(_time, SHAKE_FREQUENCY), Noise.Calc1D(_time + 100.0f, SHAKE_FREQUENCY));
            shakeOffset *= actualShakeIntensity;

            viewport.Center = _position + shakeOffset;
        }

        public void FixedUpdate(float fixedDeltaTime)
        {}
    }
}
