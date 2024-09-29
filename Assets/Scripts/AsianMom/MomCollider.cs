using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mom
{
    public class MomCollider : MonoBehaviour
    {
        [SerializeField] private MomController momController;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Weapon")
            {
                momController.StartDeadState();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
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