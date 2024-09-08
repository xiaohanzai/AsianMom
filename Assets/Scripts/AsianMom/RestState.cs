using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mom
{
    public class RestState : BaseState
    {
        public RestState(MomController controller) : base(controller)
        {
        }

        public override void EnterState()
        {
            Transform visualsParent = _controller.GetVisualsTransform();
            Transform restLoc = _controller.GetRestLoc();
            visualsParent.position = restLoc.position;
            visualsParent.rotation = restLoc.rotation;

            Animator momAnimator = _controller.GetMomAnimator();
            momAnimator.SetBool("IsWalking", false);
            momAnimator.SetBool("IsAngry", false);
            momAnimator.SetBool("IsLookingAround", false);

            Animator doorAnimator = _controller.GetDoorAnimator();
            doorAnimator.SetBool("IsDoorOpening", false);
        }

        public override void ExitState()
        {
            
        }

        public override void RunState()
        {
            
        }
    }
}