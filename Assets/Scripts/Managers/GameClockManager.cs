using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClockManager : MonoBehaviour
{
    private float timeInterval;

    private float timer;

    [Header("Broadcasting on")]
    [SerializeField] private VoidEventChannelSO timeUpEventChannel;

    [Header("Listening to")]
    [SerializeField] private VoidEventChannelSO startTimerEventChannel;
    [SerializeField] private VoidEventChannelSO levelCompleteEventChannel;
    [SerializeField] private FloatEventChannelSO setTimeIntervalEventChannel;

    void Start()
    {
        SetTimerStop();

        setTimeIntervalEventChannel.OnEventRaised += SetTimeInterval;
        startTimerEventChannel.OnEventRaised += SetTimerStart;
        levelCompleteEventChannel.OnEventRaised += SetTimerStop;
    }

    private void OnDestroy()
    {
        setTimeIntervalEventChannel.OnEventRaised -= SetTimeInterval;
        startTimerEventChannel.OnEventRaised -= SetTimerStart;
        levelCompleteEventChannel.OnEventRaised -= SetTimerStop;
    }

    void Update()
    {
        CheckSpawnMom();
    }

    private void SetTimeInterval(float t)
    {
        timeInterval = t;
    }

    private void SetTimerStart()
    {
        timer = 0;
    }

    private void SetTimerStop()
    {
        timer = 1000000;
    }

    private void CheckSpawnMom()
    {
        if (timer > timeInterval) return;

        timer += Time.deltaTime;
        if (timer > timeInterval) timeUpEventChannel.RaiseEvent();
    }
}
