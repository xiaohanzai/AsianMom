using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeButtonEventHandler : MonoBehaviour
{
    [SerializeField] private PokeButtonEventChannelSO _pokeButtonEventChannel;
    [SerializeField] private PokeButtonType _pokeButtonType;

    public void RaiseEvent()
    {
        PokeButtonData pokeButtonData = new PokeButtonData
        {
            pokeButtonType = _pokeButtonType,
        };

        _pokeButtonEventChannel.RaiseEvent(pokeButtonData);
    }
}
