using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PaintingGame
{
    [RequireComponent(typeof(SetObjectColor))]
    public class PaintingPattern : MonoBehaviour
    {
        [SerializeField] private PaintingColor correctColor;

        private PaintingColor currentColor;

        public UnityEvent<PaintingPattern, bool> Evt_OnPatternColored = new UnityEvent<PaintingPattern, bool>();

        private bool canPaint;

        public void SetCanPaint(bool b)
        {
            canPaint = b;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!canPaint) return;

            PaintBrushCollider paintBrushCollider = other.GetComponent<PaintBrushCollider>();
            if (paintBrushCollider != null)
            {
                currentColor = paintBrushCollider.GetCurrentColor();
                GetComponent<SetObjectColor>().SetColor(currentColor, currentColor == correctColor);
                Evt_OnPatternColored.Invoke(this, currentColor == correctColor);
            }
        }

        public void ResetColor()
        {
            GetComponent<SetObjectColor>().ResetColor();
            Evt_OnPatternColored.Invoke(this, false);
        }
    }
}