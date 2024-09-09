using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Listening to")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;
    [SerializeField] private SpawnObjectsEventChannelSO spawnObjectsEventChannel;

    [SerializeField, Tooltip("List of environment prop data objects to spawn upon start game.")]
    private List<EnvironmentPropData> propDataList;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        if(pokeButtonEventChannel != null)
            pokeButtonEventChannel.OnEventRaised += OnPokeButtonEvent;
        if (spawnObjectsEventChannel != null)
            spawnObjectsEventChannel.OnEventRaised += SpawnObjects;
    }

    private void OnDestroy()
    {
        if (pokeButtonEventChannel != null)
            pokeButtonEventChannel.OnEventRaised -= OnPokeButtonEvent;
        if (spawnObjectsEventChannel != null)
            spawnObjectsEventChannel.OnEventRaised -= SpawnObjects;

        //spawnObjectsEventChannel.ClearAllSpawnedObjects();
    }

    private void OnPokeButtonEvent(PokeButtonType data)
    {
        switch (data)
        {
            case PokeButtonType.StartGame:
                SpawnEnvironment();
                break;
            case PokeButtonType.ShuffleEnvironment:
                ShuffleEnvironment();
                break;
            case PokeButtonType.ConfirmEnvironment:
                break;
            default:
                break;
        }
    }

    public void SpawnEnvironment()
    {
        foreach (var data in propDataList)
        {
            for (int i = 0; i < data.spawnAmount; i++)
            {
                StartCoroutine(TrySpawnObject(MRUK.Instance.GetCurrentRoom(), data, spawnedObjects));
            }
        }

        StartCoroutine(CleanUpEnvironment());
    }

    public void ShuffleEnvironment()
    {
        ClearEnvironment();

        SpawnEnvironment();
    }

    private IEnumerator TrySpawnObject(MRUKRoom room, EnvironmentPropData data, List<GameObject> objects)
    {
        for (int attempt = 0; attempt < data.maxIterations; attempt++)
        {
            var (spawnPosition, spawnRotation, isValidSpawn) = GetSpawnPosition(room, data);
            if (!isValidSpawn) break;

            // Instantiate temporary object for collision check
            GameObject tempObject = Instantiate(data.prefab, spawnPosition, spawnRotation);
            tempObject.transform.name = "TempObject";
            var propHandler = tempObject.GetComponent<EnvironmentPropHandler>();

            // Wait for the next fixed update to ensure the physics system has time to process collisions
            yield return new WaitForFixedUpdate();

            // Check if the temp object is touching another object
            if (propHandler != null)
            {
                if (!propHandler.IsTouchingObject() && !propHandler.IsOverlappingWithMRUKAnchor())
                {
                    Destroy(tempObject);
                    InstantiateEnvironmentObject(spawnPosition, spawnRotation, data, objects);
                    yield break;
                }
            }

            Destroy(tempObject);
        }
    }

    private (Vector3 position, Quaternion rotation, bool isValid) GetSpawnPosition(MRUKRoom room,
            EnvironmentPropData data)
    {
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;
        bool isValidSpawn = false;

        switch (data.spawnLocation)
        {
            case EnvironmentPropData.SpawnLocation.Wall:
                // Find a vertical surface for direct wall placement
                isValidSpawn = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.VERTICAL, data.minRadius,
                    LabelFilter.Included(data.labels), out spawnPosition, out var wallOnlyNormal);

                if (isValidSpawn)
                {
                    // Directly place on the wall with appropriate rotation
                    spawnRotation = Quaternion.LookRotation(-wallOnlyNormal, Vector3.up);
                }
                break;
            case EnvironmentPropData.SpawnLocation.WallFloor:
                // Find a vertical surface first
                isValidSpawn = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.VERTICAL, data.minRadius,
                    LabelFilter.Included(data.labels), out spawnPosition, out var wallNormal);

                if (isValidSpawn)
                {
                    //// Project spawn position down to the floor level by setting the Y to zero or slightly above the floor level
                    //RaycastHit hit;
                    //if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity))
                    //{
                    //    spawnPosition = hit.point; // Set spawn position to the floor hit point
                    //}
                    //else
                    //{
                    //    spawnPosition.y = 0.1f; // Fallback to zero Y if floor detection fails
                    //}
                    spawnPosition.y = 0;
                    spawnRotation = Quaternion.LookRotation(-wallNormal, Vector3.up);
                }

                break;
            case EnvironmentPropData.SpawnLocation.Floating:
                var randomPosition = room.GenerateRandomPositionInRoom(data.minRadius, true);
                isValidSpawn = randomPosition.HasValue && room.IsPositionInRoom(randomPosition.Value);
                spawnPosition = randomPosition ?? Vector3.zero;
                spawnRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                break;
            case EnvironmentPropData.SpawnLocation.Ceiling:
                isValidSpawn = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_DOWN, data.minRadius,
                    LabelFilter.Included(data.labels), out spawnPosition, out var ceilingNormal);
                spawnRotation = Quaternion.LookRotation(-ceilingNormal, Vector3.up);
                break;
            case EnvironmentPropData.SpawnLocation.OnTopOfSurfaces:
                isValidSpawn = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_UP, data.minRadius,
                    LabelFilter.Included(data.labels), out spawnPosition, out var floorNormal);
                spawnRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                break;
            case EnvironmentPropData.SpawnLocation.Window:
                // Find a surface labeled as WINDOW_FRAME
                isValidSpawn = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.VERTICAL, data.minRadius,
                    LabelFilter.Included(data.labels),
                    out spawnPosition, out var windowNormal);

                if (isValidSpawn)
                {
                    // Place the object on the window frame with correct rotation
                    spawnRotation = Quaternion.LookRotation(-windowNormal, Vector3.up);
                }
                break;
        }

        return (spawnPosition, spawnRotation, isValidSpawn);
    }

    private void InstantiateEnvironmentObject(Vector3 position, Quaternion rotation, EnvironmentPropData data, List<GameObject> objects)
    {
        GameObject spawnedObject = Instantiate(data.prefab, position, rotation);
        spawnedObject.transform.parent = transform; // Parent to this manager for organization
        objects.Add(spawnedObject);
    }

    private void ClearEnvironment()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
    }

    private IEnumerator CleanUpEnvironment()
    {
        // Wait to ensure all objects have been instantiated and physics have settled
        yield return new WaitForSeconds(1f);

        foreach (var spawnedObject in
                 spawnedObjects.ToArray()) // Use ToArray to avoid modifying the list while iterating
        {
            if (spawnedObject == null) continue;

            var propHandler = spawnedObject.GetComponent<EnvironmentPropHandler>();
            if (propHandler != null)
            {
                // Destroy objects that are either touching other props or overlapping with MRUKAnchors
                if (propHandler.IsTouchingObject() || propHandler.IsOverlappingWithMRUKAnchor())
                {
                    Destroy(spawnedObject);
                    spawnedObjects.Remove(spawnedObject);
                }
            }
        }
    }

    private void SpawnObjects(SpawnData data)
    {
        StartCoroutine(Co_SpawnObjects(data));
    }

    private IEnumerator Co_SpawnObjects(SpawnData data)
    {
        List<GameObject> objects = new List<GameObject>();
        for (int i = 0; i < data.environmentPropData.spawnAmount; i++)
        {
            yield return StartCoroutine(TrySpawnObject(MRUK.Instance.GetCurrentRoom(), data.environmentPropData, objects));
        }
        spawnObjectsEventChannel.AddToSpawnedObjectsDict(data.spawnType, objects);
    }
}
