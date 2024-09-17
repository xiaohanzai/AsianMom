using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicGame
{
    [CreateAssetMenu(fileName = "NewMusicData", menuName = "Data/Individual Games/Music Game/Music Data")]
    public class MusicData : ScriptableObject
    {
        public Sprite notation;
        public List<MusicKeyName> sequence;
        public AudioClip audioClip;
        public float playSpeed = 1f;
        public float timeStart;
        public float timeEnd;
    }

    public enum MusicName
    {
        TwinkleLittleStar,
        TwoTigers,
        WhereIsSpring,
    }
}
