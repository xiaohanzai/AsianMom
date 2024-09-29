using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicGame
{
    public class MusicDataCarrier : MonoBehaviour
    {
        [SerializeField] private MusicData musicData;

        [Header("Listening to")]
        [SerializeField] private LevelEventChannelSO levelEventChannel;

        [Header("Broadcasting on")]
        [SerializeField] private SetMusicEventChannelSO setMusicEventChannel;

        private void Awake()
        {
            levelEventChannel.OnEventRaised += OnLevelEventRaised;
        }

        private void OnDestroy()
        {
            levelEventChannel.OnEventRaised -= OnLevelEventRaised;
        }

        private void OnLevelEventRaised(LevelEventInfo data)
        {
            if (data.type == LevelEventType.LevelLoad) SetMusic();
        }

        private void SetMusic()
        {
            setMusicEventChannel.RaiseEvent(musicData);
        }
    }
}