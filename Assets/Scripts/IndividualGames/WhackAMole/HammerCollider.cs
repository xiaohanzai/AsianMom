using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhackAMole
{
    public class HammerCollider : MonoBehaviour
    {
        [SerializeField] private AudioSource hitAudio;

        private void OnTriggerEnter(Collider other)
        {
            MoleCollider mole = other.GetComponent<MoleCollider>();
            hitAudio.Play();
            if (mole != null)
            {
                mole.SetDead();
            }
        }
    }
}