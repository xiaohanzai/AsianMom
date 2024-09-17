using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMomManager : MonoBehaviour
{
    private float timeIntervalMax;
    private float timeIntervalMin;
    private int nRounds;
    private float waitTime;

    int ind;
    float dt;

    [Header("Listening to")]
    [SerializeField] private SpawnMomParametersEventChannelSO setSpawnMomParametersEventChannel;
    [SerializeField] private VoidEventChannelSO timeUpEventChannel;
    [SerializeField] private VoidEventChannelSO levelStartEventChannel;
    [SerializeField] private VoidEventChannelSO setNextRoundParamsEventChannel;

    [Header("Broadcasting on")]
    [SerializeField] private VoidEventChannelSO spawnMomEventChannel;
    [SerializeField] private FloatEventChannelSO setWaitTimeEventChannel;
    [SerializeField] private FloatEventChannelSO setTimeIntervalEventChannel;
    [SerializeField] private IntEventChannelSO setNRoundsEventChannel;

    void Start()
    {
        setSpawnMomParametersEventChannel.OnEventRaised += SetSpawnMomParameters;
        timeUpEventChannel.OnEventRaised += SpawnMom;
        levelStartEventChannel.OnEventRaised += SetInitialTimes;
        setNextRoundParamsEventChannel.OnEventRaised += SetNextRoundTimes;
    }

    private void OnDestroy()
    {
        setSpawnMomParametersEventChannel.OnEventRaised -= SetSpawnMomParameters;
        timeUpEventChannel.OnEventRaised -= SpawnMom;
        levelStartEventChannel.OnEventRaised -= SetInitialTimes;
        setNextRoundParamsEventChannel.OnEventRaised -= SetNextRoundTimes;
    }

    private void SetInitialTimes()
    {
        ind = 0;
        setTimeIntervalEventChannel.RaiseEvent(timeIntervalMax);
        setWaitTimeEventChannel.RaiseEvent(waitTime); // TODO: can in principle change the wait time according to nRounds too
    }

    private void SetSpawnMomParameters(SpawnMomParameters pars)
    {
        timeIntervalMax = pars.timeIntervalMax;
        timeIntervalMin = pars.timeIntervalMin;
        nRounds = pars.nRounds;
        waitTime = pars.waitTime;

        dt = (timeIntervalMax - timeIntervalMin) / nRounds;

        setNRoundsEventChannel.RaiseEvent(nRounds);
    }

    private void SetNextRoundTimes()
    {
        ind++;
        if (ind >= nRounds)
        {
            return;
        }
        setTimeIntervalEventChannel.RaiseEvent(timeIntervalMax - ind * dt); // set the time intervals for next round
        setWaitTimeEventChannel.RaiseEvent(waitTime); // TODO: can change this as a function of ind as well
    }

    private void SpawnMom()
    {
        spawnMomEventChannel.RaiseEvent();
    }
}

public class SpawnMomParameters
{
    public float timeIntervalMax;
    public float timeIntervalMin;
    public int nRounds;
    public float waitTime;
}