using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mom
{
    public class AngryState : BaseState
    {
        private Animator momAnimator;
        private AudioSource angryAudio;

        public AngryState(MomController controller) : base(controller)
        {
            angryAudio = _controller.GetAudio(MomStateName.GetAngry);
            momAnimator = _controller.GetMomAnimator();
        }

        public override void EnterState()
        {
            Debug.Log("callllllllll");
            angryAudio.Play();
            momAnimator.SetBool("IsWalking", false);
            momAnimator.SetBool("IsAngry", true);
            momAnimator.SetBool("IsLookingAround", false);

        }

        public override void ExitState()
        {
            angryAudio.Stop();
        }

        public override void RunState()
        {
            
        }
    }
}