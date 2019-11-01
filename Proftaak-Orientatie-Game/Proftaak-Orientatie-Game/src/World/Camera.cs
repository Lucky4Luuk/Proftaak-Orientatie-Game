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

        private Random rng = new Random();

        public float shakeIntensity = 0f;
        public float shakeDropOff = 1f;
        public float shakeFrequency = 0f;
        public float shakeDuration = 0f;
        public Vector2f shakeVelocity = new Vector2f(0f, 0f);
        private Vector2f shakeOffset = new Vector2f(0f, 0f);

        public float recoilIntensity = 0f;
        public Vector2f recoilVelocity = new Vector2f(0f, 0f);
        private Vector2f recoilOffset = new Vector2f(0f, 0f);

        public Camera()
        {}

        public void SetTargetPosition(Vector2f position)
        {
            _targetPosition = position;
        }

        public void Shake(float intensity, float dropOff, float frequency, float duration)
        {
            shakeIntensity = intensity;
            shakeDropOff = dropOff;
            shakeFrequency = frequency;
            shakeDuration = duration;
        }

        public void Recoil(float intensity, Vector2f direction)
        {
            recoilVelocity -= direction * intensity;
        }

        public void Update(float deltaTime)
        {
            if (shakeDuration > 0)
            {
                if ((shakeDuration * 500) % 1f < 0.1f)
                {
                    Console.WriteLine("New shake velocity");
                    float weightX = shakeOffset.X / shakeIntensity;
                    float weightY = shakeOffset.Y / shakeIntensity;
                    float velX = ((float)rng.NextDouble()) * 2f - 1f + weightX * 0.5f;
                    float velY = ((float)rng.NextDouble()) * 2f - 1f + weightY * 0.5f;
                    shakeVelocity = new Vector2f(velX * shakeIntensity, velY * shakeIntensity);
                }

                shakeOffset += shakeVelocity * deltaTime;

                shakeIntensity *= shakeDropOff;
                shakeVelocity *= shakeDropOff;
                shakeDuration -= deltaTime;
            } else
            {
                shakeOffset = new Vector2f(0f, 0f);
            }

            _position = (_targetPosition * 0.5f + _position * 0.5f);

            recoilVelocity *= 0.9f;
            recoilOffset = recoilVelocity * deltaTime;

            viewport.Center = _position + shakeOffset * shakeIntensity + recoilOffset;
        }

        public void FixedUpdate(float fixedDeltaTime)
        {}
    }
}
