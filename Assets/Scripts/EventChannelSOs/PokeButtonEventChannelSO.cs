using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Poke Button Event Channel", fileName = "NewPokeButtonEventChannel")]
public class PokeButtonEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<PokeButtonType> OnEventRaised;

    public void RaiseEvent(PokeButtonType data)
    {
        //// for debugging
        //if (data == PokeButtonType.PlayWhackAMole)
        //{
        //    TabletButtonEventHandler[] tabletButtonEventHandlers = FindObjectsOfType<TabletButtonEventHandler>();
        //    foreach (var handler in tabletButtonEventHandlers)
        //    {
        //        if (handler.GetCurrentGameName() == IndividualGameName.WhackAMole)
        //        {
        //            handler.StartGame(true);
        //        }
        //    }
        //    return;
        //}

        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}

//public class PokeButtonData
//{
//    public PokeButtonType pokeButtonType;
//}

public enum PokeButtonType
{
    StartGame,
    ShuffleEnvironment,
    ConfirmEnvironment,
    LoadLevel,
    StartLevel,
    Quit,
    PlayAgain,
    TryAgain,
}
