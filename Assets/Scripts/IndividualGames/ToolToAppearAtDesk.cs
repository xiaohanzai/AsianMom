using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolToAppearAtDesk : MonoBehaviour
{
    private Vector3 spawnPos;
    private Quaternion spawnRot;

    [Header("Listening to")]
    [SerializeField] private TransformEventChannelSO setPropLocEventChannel;

    private void Awake()
    {
        setPropLocEventChannel.OnEventRaised += SetLocation;
    }

    private void OnDestroy()
    {
        setPropLocEventChannel.OnEventRaised -= SetLocation;
    }

    private void SetLocation(Transform t)
    {
        spawnPos = t.position;
        spawnRot = t.rotation;
    }

    public void PutToSpawnLoc()
    {
        transform.position = spawnPos;
        transform.rotation = spawnRot;
    }
}