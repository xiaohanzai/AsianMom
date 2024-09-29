using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintingGame
{
    public class PaintingDataCarrier : MonoBehaviour
    {
        [SerializeField] private PaintingData paintingData;

        [Header("Listening to")]
        [SerializeField] private LevelEventChannelSO levelEventChannel;

        [Header("Broadcasting on")]
        [SerializeField] private SetPaintingEventChannelSO setPaintingEventChannel;

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
            if (data.type == LevelEventType.LevelLoad) SetPainting();
        }

        private void SetPainting()
        {
            setPaintingEventChannel.RaiseEvent(paintingData);
        }
    }
}