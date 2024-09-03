using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintingGame
{
    public class SetObjectColor : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;

        public void SetColor(PaintingColor paintingColor)
        {
            Color c = Color.white;
            switch (paintingColor)
            {
                case PaintingColor.Red:
                    c = Color.red;
                    break;
                case PaintingColor.Blue:
                    c = Color.blue;
                    break;
                case PaintingColor.Green:
                    c = Color.green;
                    break;
                case PaintingColor.Orange:
                    c = new Color(1, 0.5f, 0);
                    break;
                case PaintingColor.Yellow:
                    c = Color.yellow;
                    break;
                case PaintingColor.Purple:
                    c = Color.magenta;
                    break;
                case PaintingColor.Black:
                    c = Color.black;
                    break;
                default:
                    break;
            }
            meshRenderer.material.color = c;
            Debug.Log("called");
        }

        public void ResetColor()
        {
            meshRenderer.material.color = Color.white;
        }
    }
}