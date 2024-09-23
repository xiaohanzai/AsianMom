using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mom
{
    public class DeadState : BaseState
    {
        public DeadState(MomController controller) : base(controller)
        {
        }

        public override void EnterState()
        {
            Animator momAnimator = _controller.GetMomAnimator();
            //momAnimator.SetBool("IsWalking", false);
            //momAnimator.SetBool("IsAngry", false);
            //momAnimator.SetBool("IsLookingAround", false);
            momAnimator.SetBool("IsDead", true);

            _controller.GetAudio(MomStateName.Dead).Play();
        }

        public override void ExitState()
        {
            
        }

        public override void RunState()
        {
            
        }
    }
}