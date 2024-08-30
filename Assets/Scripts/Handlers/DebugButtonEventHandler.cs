using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButtonEventHandler : MonoBehaviour
{
    public enum DebugButtonType
    {
        ShuffleEnvironment,
        NewLevel,
        PlayWhackAMole,
        PlayMusicGame,
    }
    [SerializeField] private DebugButtonType buttonType;

    public void RaiseEvent()
    {
        if (buttonType == DebugButtonType.ShuffleEnvironment)
        {
            FindObjectOfType<EnvironmentManager>().SpawnEnvironment();
        }
        else if(buttonType == DebugButtonType.NewLevel)
        {
            FindObjectOfType<LevelManager>().LoadNewLevelData();
        }
        else if (buttonType == DebugButtonType.PlayWhackAMole)
        {
            TabletButtonEventHandler[] tabletButtonEventHandlers = FindObjectsOfType<TabletButtonEventHandler>();
            foreach (var handler in tabletButtonEventHandlers)
            {
                if (handler.GetCurrentGameName() == IndividualGameName.WhackAMole)
                {
                    handler.StartGame(true);
                }
            }
        }
        else if (buttonType == DebugButtonType.PlayMusicGame)
        {
            TabletButtonEventHandler[] tabletButtonEventHandlers = FindObjectsOfType<TabletButtonEventHandler>();
            foreach (var handler in tabletButtonEventHandlers)
            {
                if (handler.GetCurrentGameName() == IndividualGameName.Music)
                {
                    handler.StartGame(true);
                }
            }
        }
    }
}
