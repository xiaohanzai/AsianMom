using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "NewGameData", menuName = "Data/Individual Games/Individual Game Data")]
public class IndividualGameData : ScriptableObject
{
    public IndividualGameName gameName;
    public GameObject gamePrefab;
    public string buttonText;
    public string instructionText;
    public VideoClip instructionVideo;
    public AudioClip audio;
}

public enum IndividualGameName
{
    Null,
    WhackAMole,
    Music,
    Painting,
    ShootFly,
    CatchLadybug,
}
