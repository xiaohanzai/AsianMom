using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Poke Button Event Channel", fileName = "NewPokeButtonEventChannel")]
public class PokeButtonEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<PokeButtonData> OnEventRaised;

    public void RaiseEvent(PokeButtonData data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}

public class PokeButtonData
{
    public PokeButtonType pokeButtonType;
}

public enum PokeButtonType
{
    StartGame,
    ShuffleEnvironment,
    ConfirmEnvironment
}
