using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicGame
{
    public class DrumstickCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);
            Debug.Log(other.GetComponent<MusicKey>() == null);
        }
    }
}
