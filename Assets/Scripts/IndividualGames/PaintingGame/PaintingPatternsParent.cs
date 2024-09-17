using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintingGame
{
    public class PaintingPatternsParent : MonoBehaviour
    {
        [SerializeField] private List<PaintingPattern> paintingPatterns;

        public List<PaintingPattern> GetAllPatterns()
        {
            return paintingPatterns;
        }
    }
}