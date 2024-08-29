using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private List<IndividualGameData> gameDataList;

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
