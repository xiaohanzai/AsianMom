using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeButtonEventHandler : MonoBehaviour
{
    [SerializeField] private PokeButtonType pokeButtonType;

    [Header("Broadcasting on")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;

    public void RaiseEvent()
    {
        pokeButtonEventChannel.RaiseEvent(pokeButtonType);
    }
}
