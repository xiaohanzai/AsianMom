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
    [SerializeField] private VoidEventChannelSO timerStartEventChannel;
    [SerializeField] private LevelEventChannelSO levelEventChannel;
    [SerializeField] private FloatEventChannelSO setTimeIntervalEventChannel;

    void Start()
    {
        SetTimerStop();

        setTimeIntervalEventChannel.OnEventRaised += SetTimeInterval;
        timerStartEventChannel.OnEventRaised += SetTimerStart;
        levelEventChannel.OnEventRaised += OnLevelEventRaised;
    }

    private void OnDestroy()
    {
        setTimeIntervalEventChannel.OnEventRaised -= SetTimeInterval;
        timerStartEventChannel.OnEventRaised -= SetTimerStart;
        levelEventChannel.OnEventRaised -= OnLevelEventRaised;
    }

    void Update()
    {
        CheckSpawnMom();
    }

    private void OnLevelEventRaised(LevelEventInfo data)
    {
        if (data.type == LevelEventType.LevelComplete) SetTimerStop();
    }

    private void SetTimeInterval(float t)
    {
        timeInterval = t;
    }

    private void SetTimerStart()
    {
        timer = 0;
        Debug.Log("timer started");
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
