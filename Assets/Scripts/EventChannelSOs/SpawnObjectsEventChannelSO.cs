using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Spawn Objects Event Channel", fileName = "NewSpawnObjectsEventChannel")]
public class SpawnObjectsEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<SpawnData> OnEventRaised;

    public UnityAction OnObjectsSpawned;

    // stores the spawned objects
    private Dictionary<SpawnType, List<GameObject>> dictSpawnedObjects = new Dictionary<SpawnType, List<GameObject>>();

    //public Dictionary<SpawnType, List<GameObject>> DictSpawnedObjects => dictSpawnedObjects;

    public void RaiseEvent(SpawnData data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }

    public void AddToSpawnedObjectsDict(SpawnType spawnType, List<GameObject> spawnedObjects)
    {
        if (dictSpawnedObjects.ContainsKey(spawnType))
        {
            ClearSpawnedObjects(spawnType);
            dictSpawnedObjects[spawnType].AddRange(spawnedObjects);
        }
        else
        {
            dictSpawnedObjects.Add(spawnType, spawnedObjects);
        }
        Debug.Log(spawnType);
        OnObjectsSpawned.Invoke();
    }

    public List<GameObject> GetSpawnedObjects(SpawnType spawnType)
    {
        return dictSpawnedObjects[spawnType];
    }

    public int GetNumberOfSpawnedObjects(SpawnType spawnType)
    {
        if (!dictSpawnedObjects.ContainsKey(spawnType))
        {
            return 0;
        }
        else
        {
            return dictSpawnedObjects[spawnType].Count;
        }
    }

    public void ClearAllSpawnedObjects()
    {
        foreach (var key in dictSpawnedObjects.Keys)
        {
            ClearSpawnedObjects(key);
        }
    }

    public void ClearSpawnedObjects(SpawnType spawnType)
    {
        foreach (var obj in dictSpawnedObjects[spawnType])
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        dictSpawnedObjects[spawnType].Clear();
    }
}

public enum SpawnType
{
    Mole,
    Fly,
    Ladybug
}

public class SpawnData
{
    public SpawnType spawnType;
    public EnvironmentPropData environmentPropData;
}
