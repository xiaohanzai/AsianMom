using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mom
{
    public class LookAroundState : BaseState
    {
        private Animator momAnimator;
        private AudioSource lookAroundAudio;
        private float waitTime;
        private float timer;

        public LookAroundState(MomController controller) : base(controller)
        {
            lookAroundAudio = _controller.GetAudio(MomStateName.LookAround);
            momAnimator = _controller.GetMomAnimator();
            waitTime = _controller.GetWaitTime();
        }

        public override void EnterState()
        {
            lookAroundAudio.Play();
            momAnimator.SetBool("IsWalking", false);
            momAnimator.SetBool("IsAngry", false);
            momAnimator.SetBool("IsLookingAround", true);
            timer = 0;
        }

        public override void ExitState()
        {
            lookAroundAudio.Stop();
        }

        public override void RunState()
        {
            _controller.CheckIndividualGames();
            if (timer < waitTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                _controller.StartWalkOutState();
            }
        }
    }
}