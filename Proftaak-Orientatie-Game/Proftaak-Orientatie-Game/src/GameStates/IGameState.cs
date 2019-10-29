using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace Proftaak_Orientatie_Game.GameStates
{
    abstract class IGameState
    {
        private IGameState _requestedState = null;

        public abstract void OnCreate();
        public abstract void OnUpdate(float deltatime, RenderWindow window);
        public abstract void OnFixedUpdate(float fixedDeltaTime, RenderWindow window);
        public abstract void OnDraw(float deltatime, RenderWindow window);
        public abstract void OnDestroy();

        protected void RequestNewState(IGameState state)
        {
            _requestedState = state;
        }

        public bool IsNewStateRequested()
        {
            return _requestedState != null;
        }

        public IGameState GetRequestedState()
        {
            Debug.Assert(IsNewStateRequested());
            return _requestedState;
        }
    }
}
