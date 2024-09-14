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
    [SerializeField] private AudioEventChannelSO deskAudioEventChannel;
    [SerializeField] private VoidEventChannelSO levelCompleteEventChannel;

    void Start()
    {
        deskAudioEventChannel.OnEventRaised += PlayIndividualGameAudio;
        spawnMomEventChannel.OnEventRaised += PlayAlarmAudio;
        pokeButtonEventChannel.OnEventRaised += PlayComputerStartAudio;
        levelCompleteEventChannel.OnEventRaised += StopIndividualGameAudio;
        deskAudioEventChannel.OnEventRaised += PlayIndividualGameAudio;
    }

    private void OnDestroy()
    {
        deskAudioEventChannel.OnEventRaised -= PlayIndividualGameAudio;
        spawnMomEventChannel.OnEventRaised -= PlayAlarmAudio;
        pokeButtonEventChannel.OnEventRaised -= PlayComputerStartAudio;
        levelCompleteEventChannel.OnEventRaised -= StopIndividualGameAudio;
        deskAudioEventChannel.OnEventRaised -= PlayIndividualGameAudio;
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

    private void PlayComputerStartAudio(PokeButtonType type)
    {
        if (type != PokeButtonType.ConfirmEnvironment) return;
        computerStartAudio.Play();
    }

    private void PlayIndividualGameAudio(AudioClip data)
    {
        individualGameAudio.clip = data;
        if (data != null) individualGameAudio.Play();
    }

    private void StopIndividualGameAudio()
    {
        if (individualGameAudio.clip != null) individualGameAudio.Stop();
    }
}
