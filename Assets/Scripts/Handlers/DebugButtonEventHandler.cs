using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButtonEventHandler : MonoBehaviour
{
    public enum DebugButtonType
    {
        StartGame,
        ShuffleEnvironment,
        ConfirmEnvironment,
        LoadLevel, // load current level if level not complete, otherwise load new level
        StartLevel, // start playing this level
        PlayWhackAMole,
        PlayMusicGame,
        PlayPaintingGame,
        PlayShootFly,
        PlayCatchLadybug,
        Cancel,
        TryAgain,
    }
    [SerializeField] private DebugButtonType buttonType;

    public void RaiseEvent()
    {
        switch (buttonType)
        {
            case DebugButtonType.StartGame:
                FindAndCallPokeButton(PokeButtonType.StartGame);
                break;
            case DebugButtonType.ShuffleEnvironment:
                FindAndCallPokeButton(PokeButtonType.ShuffleEnvironment);
                break;
            case DebugButtonType.ConfirmEnvironment:
                FindAndCallPokeButton(PokeButtonType.ConfirmEnvironment);
                break;
            case DebugButtonType.LoadLevel:
                FindAndCallPokeButton(PokeButtonType.LoadLevel);
                break;
            case DebugButtonType.StartLevel:
                FindAndCallPokeButton(PokeButtonType.StartLevel);
                break;
            case DebugButtonType.TryAgain:
                FindAndCallPokeButton(PokeButtonType.TryAgain);
                break;
            case DebugButtonType.PlayWhackAMole:
                FindAndCallTabletButton(IndividualGameName.WhackAMole);
                break;
            case DebugButtonType.PlayMusicGame:
                FindAndCallTabletButton(IndividualGameName.Music);
                break;
            case DebugButtonType.PlayPaintingGame:
                FindAndCallTabletButton(IndividualGameName.Painting);
                break;
            case DebugButtonType.PlayShootFly:
                FindAndCallTabletButton(IndividualGameName.ShootFly);
                break;
            case DebugButtonType.PlayCatchLadybug:
                FindAndCallTabletButton(IndividualGameName.CatchLadybug);
                break;
            case DebugButtonType.Cancel:
                FindAndCallTabletButton(IndividualGameName.Null);
                break;
            default:
                break;
        }
    }

    private void FindAndCallPokeButton(PokeButtonType type)
    {
        PokeButtonEventHandler[] pokeButtonEventHandlers = FindObjectsOfType<PokeButtonEventHandler>();
        foreach (var handler in pokeButtonEventHandlers)
        {
            if (handler.GetButtonType() == type)
            {
                handler.RaiseEvent();
            }
        }
    }

    private void FindAndCallTabletButton(IndividualGameName name)
    {
        TabletButtonEventHandler[] tabletButtonEventHandlers = FindObjectsOfType<TabletButtonEventHandler>();
        foreach (var handler in tabletButtonEventHandlers)
        {
            if (handler.GetCurrentGameName() == name)
            {
                handler.StartGame(true);
            }
        }
    }
}
