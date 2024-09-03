using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintingGame
{
    [CreateAssetMenu(fileName = "NewPaintingData", menuName = "Data/Individual Games/Painting Game/Painting Data")]
    public class PaintingData : ScriptableObject
    {
        public Sprite image;
        public PaintingPatternsParent paintingPatternsParent;
    }

    public enum PaintingName
    {
        Penguin,
    }

    public enum PaintingColor
    {
        Red,
        Blue,
        Green,
        Orange,
        Yellow,
        Purple,
        Black,
    }
}
