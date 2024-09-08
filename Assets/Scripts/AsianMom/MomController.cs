using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mom
{
    public enum MomStateName
    {
        Rest,
        WalkIn,
        LookAround,
        GetAngry,
        WalkOut,
    }

    public class MomController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed;
        [SerializeField] private Transform visualsParent;

        [Header("Animators")]
        [SerializeField] private Animator momAnimator;
        [SerializeField] private Animator doorAnimator;

        [Header("Locations")]
        [SerializeField] private Transform restLoc;
        [SerializeField] private Transform destLoc;
        [SerializeField] private Transform doorTriggerLoc;

        [Header("Audios")]
        [SerializeField] private AudioSource walkingAudio;
        [SerializeField] private AudioSource lookAroundAudio;
        [SerializeField] private AudioSource angryAudio;
        [SerializeField] private AudioSource doorOpenAudio;
        [SerializeField] private AudioSource doorCloseAudio;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannelSO spawnMomEventChannel;
        [SerializeField] private VoidEventChannelSO levelStartEventChannel;
        [SerializeField] private VoidEventChannelSO levelFailedEventChannel;
        [SerializeField] private FloatEventChannelSO setWaitTimeEventChannel;

        [Header("Broadcasting on")]
        [SerializeField] private VoidEventChannelSO checkIndividualGamesEventChannel;
        [SerializeField] private VoidEventChannelSO startTimerEventChannel;

        private BaseState currentState;
        private MomStateName currentStateName;

        private float waitTime;

        void Start()
        {
            spawnMomEventChannel.OnEventRaised += StartWalkInState;
            levelStartEventChannel.OnEventRaised += StartRestState;
            setWaitTimeEventChannel.OnEventRaised += SetWaitTime;
            levelFailedEventChannel.OnEventRaised += StartAngryState;
        }

        private void OnDestroy()
        {
            spawnMomEventChannel.OnEventRaised -= StartWalkInState;
            levelStartEventChannel.OnEventRaised -= StartRestState;
            setWaitTimeEventChannel.OnEventRaised -= SetWaitTime;
            levelFailedEventChannel.OnEventRaised -= StartAngryState;
        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState.RunState();
            }
        }

        private void ChangeState(MomStateName stateName)
        {
            if (currentState != null && currentStateName == stateName) return;

            BaseState newState = null;
            switch (stateName)
            {
                case MomStateName.Rest:
                    newState = new RestState(this);
                    startTimerEventChannel.RaiseEvent();
                    break;
                case MomStateName.WalkIn:
                    newState = new WalkState(this, true);
                    break;
                case MomStateName.LookAround:
                    newState = new LookAroundState(this);
                    break;
                case MomStateName.GetAngry:
                    newState = new AngryState(this);
                    break;
                case MomStateName.WalkOut:
                    newState = new WalkState(this, false);
                    break;
                default:
                    break;
            }
            currentStateName = stateName;
            if (currentState != null)
                currentState.ExitState();
            currentState = newState;
            currentState.EnterState();
        }

        public void StartRestState()
        {
            ChangeState(MomStateName.Rest);
        }

        public void StartWalkInState()
        {
            ChangeState(MomStateName.WalkIn);
        }

        public void StartLookAroundState()
        {
            ChangeState(MomStateName.LookAround);
        }

        public void StartAngryState()
        {
            Debug.Log("called");
            ChangeState(MomStateName.GetAngry);
        }

        public void StartWalkOutState()
        {
            ChangeState(MomStateName.WalkOut);
        }

        public void CheckIndividualGames()
        {
            checkIndividualGamesEventChannel.RaiseEvent();
        }

        private void SetWaitTime(float t)
        {
            waitTime = t;
        }

        public float GetWaitTime()
        {
            return waitTime;
        }

        public Animator GetMomAnimator()
        {
            return momAnimator;
        }

        public Animator GetDoorAnimator()
        {
            return doorAnimator;
        }

        public Transform GetRestLoc()
        {
            return restLoc;
        }

        public Transform GetDestLoc()
        {
            return destLoc;
        }

        public Transform GetDoorTriggerLoc()
        {
            return doorTriggerLoc;
        }

        public Transform GetVisualsTransform()
        {
            return visualsParent;
        }

        public float GetWalkSpeed()
        {
            return walkSpeed;
        }

        public AudioSource GetAudio(MomStateName name)
        {
            switch (name)
            {
                case MomStateName.WalkIn:
                    return walkingAudio;
                case MomStateName.WalkOut:
                    return walkingAudio;
                case MomStateName.LookAround:
                    return lookAroundAudio;
                case MomStateName.GetAngry:
                    return angryAudio;
                default:
                    return null;
            }
        }

        public AudioSource GetDoorAudio(bool doorOpen)
        {
            if (doorOpen) return doorOpenAudio;
            else return doorCloseAudio;
        }
    }
}