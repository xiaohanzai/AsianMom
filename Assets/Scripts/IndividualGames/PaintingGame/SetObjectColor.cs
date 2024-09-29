using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PaintingGame
{
    public class SetObjectColor : MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        private Color[] originalColors;

        private void Awake()
        {
            if (meshRenderers.Count == 0)
            {
                meshRenderers.Add(GetComponent<MeshRenderer>());
                meshRenderers.AddRange(transform.GetComponentsInChildren<MeshRenderer>().ToList());
            }
            originalColors = new Color[meshRenderers.Count];
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                originalColors[i] = meshRenderers[i].material.color;
            }
        }

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
                    c = new Color(250f/256, 160f/256, 34f / 256);
                    break;
                case PaintingColor.Yellow:
                    c = new Color(255f / 256, 232f / 256, 143f / 256);
                    break;
                case PaintingColor.SkinTone:
                    c = new Color(238f / 256, 202f / 256, 170f / 256);
                    break;
                case PaintingColor.Black:
                    c = Color.black;
                    break;
                default:
                    break;
            }
            for (int i = 0; i < meshRenderers.Count; i++) meshRenderers[i].material.color = c;
        }

        public void SetColor(PaintingColor paintingColor, bool b)
        {
            if (b)
            {
                for (int i = 0; i < meshRenderers.Count; i++) meshRenderers[i].material.color = originalColors[i];
                return;
            }

            SetColor(paintingColor);
        }

        public void ResetColor()
        {
            for (int i = 0; i < meshRenderers.Count; i++) meshRenderers[i].material.color = Color.white;
        }
    }
}