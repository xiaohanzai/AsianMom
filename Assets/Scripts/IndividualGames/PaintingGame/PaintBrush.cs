using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PaintingGame
{
    public class PaintBrush : MonoBehaviour
    {
        [SerializeField] private PaintBrushCollider paintBrushCollider;

        public void ResetColor()
        {
            paintBrushCollider.ResetColor();
        }
    }
}