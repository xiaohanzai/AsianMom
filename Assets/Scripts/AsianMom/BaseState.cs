using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mom
{
    public abstract class BaseState
    {
        protected MomController _controller;

        public BaseState(MomController controller)
        {
            _controller = controller;
        }

        public abstract void EnterState();
        public abstract void RunState();
        public abstract void ExitState();
    }
}