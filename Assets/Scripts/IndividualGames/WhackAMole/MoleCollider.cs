using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhackAMole
{
    public class MoleCollider : MonoBehaviour
    {
        [SerializeField] private Mole mole;

        public void SetDead()
        {
            mole.SetDead();
        }
    }
}