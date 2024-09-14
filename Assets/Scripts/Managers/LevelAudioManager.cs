using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAudioManager : MonoBehaviour
{
    [SerializeField] private float gameCompleteAudioDelayTime;
    [SerializeField] private float levelFailedAudioDelayTime;

    [Header("Audios")]
    [SerializeField] private AudioSource gameCompleteAudio; // when an individual game is complete play success audio
    [SerializeField] private AudioSource levelFailedAudio; // when discovered by mom play failed audio

    [Header("Listening to")]
    [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;
    [SerializeField] private VoidEventChannelSO levelFailedEventChannel;

    void Start()
    {
        gameCompleteEventChannel.OnEventRaised += DelayedPlaySuccessAudio;
        levelFailedEventChannel.OnEventRaised += DelayedPlayFailedAudio;
    }

    private void OnDestroy()
    {
        gameCompleteEventChannel.OnEventRaised -= DelayedPlaySuccessAudio;
        levelFailedEventChannel.OnEventRaised -= DelayedPlayFailedAudio;
    }

    private void DelayedPlaySuccessAudio(IndividualGameName data)
    {
        Invoke("PlaySuccessAudio", gameCompleteAudioDelayTime);
    }

    private void PlaySuccessAudio()
    {
        gameCompleteAudio.Play();
    }

    private void DelayedPlayFailedAudio()
    {
        Invoke("PlayFailedAudio", levelFailedAudioDelayTime);
    }

    private void PlayFailedAudio()
    {
        levelFailedAudio.Play();
    }
}
