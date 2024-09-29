using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PaintingGame
{
    [RequireComponent(typeof(SetObjectColor))]
    public class PaintBrushCollider : MonoBehaviour
    {
        private PaintingColor currentColor;
        [SerializeField] private AudioSource dipAudio;

        public PaintingColor GetCurrentColor()
        {
            return currentColor;
        }

        private void OnTriggerEnter(Collider other)
        {
            ColorOnPalette colorOnPalette = other.GetComponent<ColorOnPalette>();
            if (colorOnPalette != null)
            {
                currentColor = colorOnPalette.GetColor();
                GetComponent<SetObjectColor>().SetColor(currentColor);
            }
            if (colorOnPalette != null || other.GetComponent<PaintingPattern>() != null) dipAudio.Play();
        }

        public void ResetColor()
        {
            GetComponent<SetObjectColor>().ResetColor();
        }
    }
}