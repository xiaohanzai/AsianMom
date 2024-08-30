using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicGame
{
    public class MusicDataCarrier : MonoBehaviour
    {
        [SerializeField] private MusicData musicData;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannelSO levelStartEventChannel;

        [Header("Broadcasting on")]
        [SerializeField] private SetMusicEventChannelSO setMusicEventChannel;

        private void Awake()
        {
            levelStartEventChannel.OnEventRaised += SetMusic;
        }

        private void OnDestroy()
        {
            levelStartEventChannel.OnEventRaised -= SetMusic;
        }

        private void SetMusic()
        {
            setMusicEventChannel.RaiseEvent(musicData);
        }
    }
}