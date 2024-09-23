using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskAudioManager : MonoBehaviour
{
    [SerializeField] private float alarmAudioPlayTime;

    [Header("Audios")]
    [SerializeField] private AudioSource computerStartAudio;
    [SerializeField] private AudioSource alarmAudio;
    [SerializeField] private AudioSource individualGameAudio;

    [Header("Listening to")]
    [SerializeField] private VoidEventChannelSO spawnMomEventChannel;
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;
    [SerializeField] private AudioEventChannelSO audioEventChannel;
    [SerializeField] private LevelEventChannelSO levelEventChannel;

    void Start()
    {
        audioEventChannel.OnEventRaised += PlayIndividualGameAudio;
        spawnMomEventChannel.OnEventRaised += PlayAlarmAudio;
        pokeButtonEventChannel.OnEventRaised += OnPokeButtonEventRaised;
        levelEventChannel.OnEventRaised += OnLevelEventRaised;
    }

    private void OnDestroy()
    {
        audioEventChannel.OnEventRaised -= PlayIndividualGameAudio;
        spawnMomEventChannel.OnEventRaised -= PlayAlarmAudio;
        pokeButtonEventChannel.OnEventRaised -= OnPokeButtonEventRaised;
        levelEventChannel.OnEventRaised -= OnLevelEventRaised;
    }

    private void OnLevelEventRaised(LevelEventInfo data)
    {
        if (data.type == LevelEventType.LevelComplete || data.type == LevelEventType.LevelFailed || data.type == LevelEventType.LevelFailedOther) StopIndividualGameAudio();
    }

    private void OnPokeButtonEventRaised(PokeButtonType type)
    {
        if (type == PokeButtonType.ConfirmEnvironment) PlayComputerStartAudio();
    }

    private void PlayAlarmAudio()
    {
        alarmAudio.Play();
        Invoke("StopAlarmAudio", alarmAudioPlayTime);
    }

    private void StopAlarmAudio()
    {
        alarmAudio.Stop();
    }

    private void PlayComputerStartAudio()
    {
        computerStartAudio.Play();
    }

    private void PlayIndividualGameAudio(AudioEventInfo data)
    {
        if (data.type != AudioType.Desk) return;
        individualGameAudio.clip = data.clip;
        if (individualGameAudio.clip != null) individualGameAudio.Play();
    }

    private void StopIndividualGameAudio()
    {
        if (individualGameAudio.clip != null) individualGameAudio.Stop();
    }
}
