using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mom
{
    public class WalkState : BaseState
    {
        private Transform visualsParent;

        private float speed;

        private Transform startTransform;
        private Transform endTransform;
        private Transform doorTriggerLoc;
        private Vector3 direction;

        private AudioSource walkingAudio;

        private Animator momAnimator;
        private Animator doorAnimator;

        private bool isDestReached;
        private bool isDoorAnimPlayed;

        private bool isWalkingIn;

        public WalkState(MomController controller, bool b) : base(controller)
        {
            isWalkingIn = b;

            if (isWalkingIn)
            {
                startTransform = _controller.GetRestLoc();
                endTransform = _controller.GetDestLoc();
            }
            else
            {
                startTransform = _controller.GetDestLoc();
                endTransform = _controller.GetRestLoc();
            }

            visualsParent = _controller.GetVisualsTransform();

            doorTriggerLoc = _controller.GetDoorTriggerLoc();
            walkingAudio = _controller.GetAudio(MomStateName.WalkIn);

            speed = _controller.GetWalkSpeed();

            momAnimator = _controller.GetMomAnimator();
            doorAnimator = _controller.GetDoorAnimator();
        }

        public override void EnterState()
        {
            visualsParent.position = startTransform.position;
            direction = (endTransform.position - startTransform.position).normalized;
            visualsParent.LookAt(endTransform); // TODO: might have to rotate 180 deg

            momAnimator.SetBool("IsWalking", true);
            momAnimator.SetBool("IsAngry", false);
            momAnimator.SetBool("IsLookingAround", false);

            walkingAudio.Play();

            isDestReached = false;

            if (!isWalkingIn)
            {
                _controller.GetAudio(MomStateName.WalkOut).Play();
            }
        }

        public override void ExitState()
        {
            walkingAudio.Stop();
        }

        public override void RunState()
        {
            if (isDestReached) return;

            //Debug.Log(Vector3.Dot(visualsParent.transform.position - endTransform.position, direction));
            if (Vector3.Dot(visualsParent.transform.position - endTransform.position, direction) > 0)
            {
                isDestReached = true;
                if (isWalkingIn)
                {
                    _controller.StartLookAroundState();
                }
                else
                {
                    _controller.StartRestState();
                }
            }

            visualsParent.transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (!isDoorAnimPlayed)
            {
                if (Vector3.Dot(visualsParent.transform.position - doorTriggerLoc.position, direction) > 0)
                {
                    if (isWalkingIn)
                    {
                        doorAnimator.SetBool("IsDoorOpening", true);
                        _controller.GetDoorAudio(true).Play();
                    }
                    else
                    {
                        doorAnimator.SetBool("IsDoorOpening", false);
                        _controller.GetDoorAudio(false).Play();
                    }
                    isDoorAnimPlayed = true;
                }
            }
        }
    }
}