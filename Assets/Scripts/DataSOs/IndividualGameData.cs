using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameData", menuName = "Data/Individual Games/Individual Game Data")]
public class IndividualGameData : ScriptableObject
{
    [SerializeField] private IndividualGameName gameName;
    [SerializeField] private GameObject gamePrefab;
    [SerializeField] private string buttonText;
    [SerializeField] private string instructionText;

    public void GetAllFields(out IndividualGameName name, out GameObject prefab, out string btnText, out string instrText)
    {
        name = gameName;
        prefab = gamePrefab;
        btnText = buttonText;
        instrText = instructionText;
    }
}

public enum IndividualGameName
{
    Null,
    WhackAMole,
    Music,
    Painting
}
