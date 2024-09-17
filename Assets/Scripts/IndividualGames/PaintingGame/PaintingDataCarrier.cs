using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintingGame
{
    public class PaintingDataCarrier : MonoBehaviour
    {
        [SerializeField] private PaintingData paintingData;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannelSO levelLoadEventChannel;

        [Header("Broadcasting on")]
        [SerializeField] private SetPaintingEventChannelSO setPaintingEventChannel;

        private void Awake()
        {
            levelLoadEventChannel.OnEventRaised += SetPainting;
        }

        private void OnDestroy()
        {
            levelLoadEventChannel.OnEventRaised -= SetPainting;
        }

        private void SetPainting()
        {
            setPaintingEventChannel.RaiseEvent(paintingData);
        }
    }
}