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
        [SerializeField] private AudioSource walkOutAudio;
        [SerializeField] private AudioSource doorOpenAudio;
        [SerializeField] private AudioSource doorCloseAudio;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannelSO spawnMomEventChannel;
        [SerializeField] private VoidEventChannelSO levelStartEventChannel;
        [SerializeField] private VoidEventChannelSO levelLoadEventChannel;
        [SerializeField] private VoidEventChannelSO levelFailedEventChannel;
        [SerializeField] private FloatEventChannelSO setWaitTimeEventChannel;
        [SerializeField] private IntEventChannelSO setNRoundsEventChannel;
        [SerializeField] private AudioEventChannelSO momWalkOutAudioEventChannel;
        [SerializeField] private AudioEventChannelSO momAngryAudioEventChannel;

        [Header("Broadcasting on")]
        [SerializeField] private BoolEventChannelSO checkIndividualGamesEventChannel;
        [SerializeField] private VoidEventChannelSO setNextRoundParamsEventChannel;
        [SerializeField] private FloatEventChannelSO updateMomUIEventChannel;
        [SerializeField] private BoolEventChannelSO showMomUIEventChannel;
        [SerializeField] private VoidEventChannelSO timerStartEventChannel;

        private BaseState currentState;
        //private MomStateName currentStateName;

        private float waitTime;

        private int nRounds;
        private int currRound;

        void Start()
        {
            spawnMomEventChannel.OnEventRaised += StartWalkInState;
            levelStartEventChannel.OnEventRaised += StartRestState;
            levelLoadEventChannel.OnEventRaised += PutBehindDoor;
            levelFailedEventChannel.OnEventRaised += StartAngryState;
            setWaitTimeEventChannel.OnEventRaised += SetWaitTime;
            setNRoundsEventChannel.OnEventRaised += SetNRounds;
            momWalkOutAudioEventChannel.OnEventRaised += SetWalkOutAudio;
            momAngryAudioEventChannel.OnEventRaised += SetAngryAudio;
        }

        private void OnDestroy()
        {
            spawnMomEventChannel.OnEventRaised -= StartWalkInState;
            levelStartEventChannel.OnEventRaised -= StartRestState;
            levelLoadEventChannel.OnEventRaised -= PutBehindDoor;
            levelFailedEventChannel.OnEventRaised -= StartAngryState;
            setWaitTimeEventChannel.OnEventRaised -= SetWaitTime;
            setNRoundsEventChannel.OnEventRaised -= SetNRounds;
            momWalkOutAudioEventChannel.OnEventRaised -= SetWalkOutAudio;
            momAngryAudioEventChannel.OnEventRaised -= SetAngryAudio;
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
            //if (currentState != null && currentStateName == stateName) return;

            BaseState newState = null;
            switch (stateName)
            {
                case MomStateName.Rest:
                    newState = new RestState(this);
                    timerStartEventChannel.RaiseEvent();
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
            //currentStateName = stateName;
            if (currentState != null)
                currentState.ExitState();
            currentState = newState;
            currentState.EnterState();
        }

        private void PutBehindDoor()
        {
            visualsParent.transform.position = doorTriggerLoc.position;
            if (doorAnimator.GetBool("IsDoorOpening"))
            {
                doorAnimator.SetBool("IsDoorOpening", false);
            }
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
            ChangeState(MomStateName.GetAngry);
        }

        public void StartWalkOutState()
        {
            ChangeState(MomStateName.WalkOut);
        }

        public void CheckIndividualGames()
        {
            checkIndividualGamesEventChannel.RaiseEvent(CheckRemainingRound());
        }

        private void SetWaitTime(float t)
        {
            waitTime = t;
        }

        public float GetWaitTime()
        {
            return waitTime;
        }

        public void SetNRounds(int n)
        {
            nRounds = n;
            currRound = 0;
            if (nRounds < 100)
                showMomUIEventChannel.RaiseEvent(true);
            else
                showMomUIEventChannel.RaiseEvent(false);
        }

        public bool CheckRemainingRound()
        {
            return nRounds - currRound > 0;
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
                    return walkOutAudio;
                case MomStateName.LookAround:
                    return lookAroundAudio;
                case MomStateName.GetAngry:
                    return angryAudio;
                default:
                    return null;
            }
        }

        private void SetWalkOutAudio(AudioClip audio)
        {
            walkOutAudio.clip = audio;
        }

        private void SetAngryAudio(AudioClip audio)
        {
            angryAudio.clip = audio;
        }

        public AudioSource GetDoorAudio(bool doorOpen)
        {
            if (doorOpen) return doorOpenAudio;
            else return doorCloseAudio;
        }

        public void SetNextRoundParams()
        {
            setNextRoundParamsEventChannel.RaiseEvent();
            currRound++;
            updateMomUIEventChannel.RaiseEvent((float)currRound / nRounds);
        }
    }
}