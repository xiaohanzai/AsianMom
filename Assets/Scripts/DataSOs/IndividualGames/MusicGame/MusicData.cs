using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicGame
{
    [CreateAssetMenu(fileName = "NewMusicData", menuName = "Data/Individual Games/Music Game/Music Data")]
    public class MusicData : ScriptableObject
    {
        public MusicName musicName;
        public List<MusicKeyName> sequence;
        public AudioClip audioClip;
    }

    public enum MusicName
    {
        TwinkleLittleStar,
        TwoTigers,
        WhereIsSpring,
    }
}
