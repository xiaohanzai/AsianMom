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
    [SerializeField] private LevelEventChannelSO levelEventChannel;

    void Start()
    {
        gameCompleteEventChannel.OnEventRaised += DelayedPlaySuccessAudio;
        levelEventChannel.OnEventRaised += OnLevelEventRaised;
    }

    private void OnDestroy()
    {
        gameCompleteEventChannel.OnEventRaised -= DelayedPlaySuccessAudio;
        levelEventChannel.OnEventRaised -= OnLevelEventRaised;
    }

    private void OnLevelEventRaised(LevelEventInfo data)
    {
        if (data.type == LevelEventType.LevelFailed || data.type == LevelEventType.LevelFailedOther) DelayedPlayFailedAudio();
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
