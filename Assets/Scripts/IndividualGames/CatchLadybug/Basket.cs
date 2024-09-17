using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatchLadybug
{
    public class Basket : ToolToAppearAtDesk
    {
        private void OnTriggerEnter(Collider other)
        {
            Ladybug ladybug = other.GetComponent<Ladybug>();
            if (ladybug != null)
            {
                ladybug.SetDead();
            }
        }
    }
}