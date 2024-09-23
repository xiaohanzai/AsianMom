using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mom
{
    public class MomCollider : MonoBehaviour
    {
        [SerializeField] private MomController momController;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.tag);
            Debug.Log(other.name);
            if (other.tag == "Weapon")
            {
                momController.StartDeadState();
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.tag == "FlySprayParticles")
            {
                momController.StartDeadState();
            }
        }
    }
}