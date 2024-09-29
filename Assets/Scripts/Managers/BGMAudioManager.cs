using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource;
    private AudioClip originalAudio;

    [Header("Listening to")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;
    [SerializeField] private AudioEventChannelSO audioEventChannel;

    void Start()
    {
        originalAudio = bgmAudioSource.clip;
        audioEventChannel.OnEventRaised += SetBGM;
        pokeButtonEventChannel.OnEventRaised += OnPokeButtonEventRaised;
    }

    private void OnDestroy()
    {
        audioEventChannel.OnEventRaised -= SetBGM;
        pokeButtonEventChannel.OnEventRaised -= OnPokeButtonEventRaised;
    }

    private void OnPokeButtonEventRaised(PokeButtonType type)
    {
        if (type == PokeButtonType.PlayAgain)
        {
            bgmAudioSource.clip = originalAudio;
            bgmAudioSource.Play();
        }
    }

    private void SetBGM(AudioEventInfo data)
    {
        if (data.type != AudioType.BGM) return;
        bgmAudioSource.clip = data.clip;
        bgmAudioSource.Play();
    }
}
