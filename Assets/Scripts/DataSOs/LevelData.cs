using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private List<IndividualGameData> gameDataList;
    public string instructionText;
    public float timeIntervalMax; // maximum time interval to spawn mom
    public float timeIntervalMin; // minimum time interval when reaching maxRounds of spawning mom
    public float waitTime; // amount of time that mom spends in room
    public int nRounds; // maximum rounds of mom coming in
    public AudioClip momWalkOutAudio;
    public AudioClip momAngryAudio;
    public AudioClip bgm;

    public List<IndividualGameData> GetGameDataList()
    {
        return gameDataList;
    }
}

//[Serializable]
//public class GameNamePrefabPair
//{
//    public IndividualGameName gameName; // Enum as the key
//    public GameObject prefab; // GameObject prefab as the value
//}
