using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintingGame
{
    public class PaintingDataCarrier : MonoBehaviour
    {
        [SerializeField] private PaintingData paintingData;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannelSO levelStartEventChannel;

        [Header("Broadcasting on")]
        [SerializeField] private SetPaintingEventChannelSO setPaintingEventChannel;

        private void Awake()
        {
            levelStartEventChannel.OnEventRaised += SetPainting;
        }

        private void OnDestroy()
        {
            levelStartEventChannel.OnEventRaised -= SetPainting;
        }

        private void SetPainting()
        {
            setPaintingEventChannel.RaiseEvent(paintingData);
        }
    }
}