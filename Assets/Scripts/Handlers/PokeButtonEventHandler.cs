using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeButtonEventHandler : MonoBehaviour
{
    [SerializeField] private PokeButtonType pokeButtonType;

    [Header("Broadcasting on")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;

    private float timer;
    private float timeToEnable;

    private void Update()
    {
        if (timer < timeToEnable) timer += Time.deltaTime;
    }

    public void RaiseEvent()
    {
        if (timer < timeToEnable) return;
        pokeButtonEventChannel.RaiseEvent(pokeButtonType);
    }

    public PokeButtonType GetButtonType()
    {
        return pokeButtonType;
    }

    public void SetUpTimer(float t)
    {
        timeToEnable = t;
        timer = 0;
    }
}
