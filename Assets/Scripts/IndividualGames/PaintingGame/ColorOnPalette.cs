using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PaintingGame
{
    public class ColorOnPalette : MonoBehaviour
    {
        [SerializeField] private PaintingColor paintingColor;

        public PaintingColor GetColor()
        {
            return paintingColor;
        }
    }
}