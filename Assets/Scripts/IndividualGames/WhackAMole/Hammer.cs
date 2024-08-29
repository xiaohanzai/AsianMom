using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhackAMole
{
    public class Hammer : MonoBehaviour
    {
        [Header("Listening to")]
        [SerializeField] private TransformEventChannelSO transformEventChannel;

        private void Awake()
        {
            transformEventChannel.OnEventRaised += SetLocation;
        }

        private void OnDestroy()
        {
            transformEventChannel.OnEventRaised -= SetLocation;
        }

        private void SetLocation(Transform t)
        {
            transform.position = t.position;
            transform.rotation = t.rotation;
        }
    }
}