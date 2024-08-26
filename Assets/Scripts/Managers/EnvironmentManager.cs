using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;

    [SerializeField, Tooltip("List of environment prop data objects to spawn upon start game.")]
    private List<EnvironmentPropData> propDataList;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    // List to store potential spawn positions and their validity
    private List<(Vector3 position, Quaternion rotation, Bounds bounds, bool isValid)> potentialSpawns = new List<(Vector3, Quaternion, Bounds, bool)>();

    // Cache for prefab bounds
    private Dictionary<GameObject, Bounds?> prefabBoundsCache = new Dictionary<GameObject, Bounds?>();

    // Start is called before the first frame update
    void Start()
    {
        pokeButtonEventChannel.OnEventRaised += OnPokeButtonEvent;
    }

    private void OnDestroy()
    {
        pokeButtonEventChannel.OnEventRaised -= OnPokeButtonEvent;
    }

    private void OnPokeButtonEvent(PokeButtonData data)
    {
        switch (data.pokeButtonType)
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

    private void SpawnEnvironment()
    {
        foreach (var propData in propDataList)
        {
            SpawnObjects(propData);
        }
    }

    private void ShuffleEnvironment()
    {
        ClearEnvironment();
        SpawnEnvironment();
    }

    private void SpawnObjects(EnvironmentPropData propData)
    {
        var room = MRUK.Instance.GetCurrentRoom();
        var prefabBounds = Utilities.GetPrefabBounds(propData.SpawnObject);
        //var prefabBounds = GetPrefabBoundsFromBoxCollider(propData.SpawnObject);
        Debug.Log($"{propData.SpawnObject.name} Prefab Bounds: {prefabBounds?.ToString() ?? "null"}");

        float minRadius = 0.0f;
        const float clearanceDistance = 0.01f;
        float baseOffset = -prefabBounds?.min.y ?? 0.0f;
        float centerOffset = prefabBounds?.center.y ?? 0.0f;
        Bounds adjustedBounds = new Bounds();

        if (prefabBounds.HasValue)
        {
            minRadius = Mathf.Min(-prefabBounds.Value.min.x, -prefabBounds.Value.min.z, prefabBounds.Value.max.x, prefabBounds.Value.max.z);
            if (minRadius < 0f)
            {
                minRadius = 0f;
            }

            var min = prefabBounds.Value.min;
            var max = prefabBounds.Value.max;
            min.y += clearanceDistance;
            if (max.y < min.y)
            {
                max.y = min.y;
            }

            adjustedBounds.SetMinMax(min, max);
            if (propData.OverrideBounds > 0)
            {
                Vector3 center = new Vector3(0f, clearanceDistance, 0f);
                Vector3 size = new Vector3(propData.OverrideBounds * 2f, clearanceDistance * 2f, propData.OverrideBounds * 2f);
                adjustedBounds = new Bounds(center, size);
            }
        }

        for (int i = 0; i < propData.SpawnAmount; ++i)
        {
            for (int j = 0; j < propData.MaxIterations; ++j)
            {
                Vector3 spawnPosition = Vector3.zero;
                Vector3 spawnNormal = Vector3.zero;
                if (propData.SpawnLocations == EnvironmentPropData.SpawnLocation.Floating)
                {
                    var randomPos = room.GenerateRandomPositionInRoom(minRadius, true);
                    if (!randomPos.HasValue)
                    {
                        break;
                    }

                    spawnPosition = randomPos.Value;
                }
                else
                {
                    MRUK.SurfaceType surfaceType = 0;
                    switch (propData.SpawnLocations)
                    {
                        case EnvironmentPropData.SpawnLocation.AnySurface:
                            surfaceType |= MRUK.SurfaceType.FACING_UP;
                            surfaceType |= MRUK.SurfaceType.VERTICAL;
                            surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                            break;
                        case EnvironmentPropData.SpawnLocation.VerticalSurfaces:
                            surfaceType |= MRUK.SurfaceType.VERTICAL;
                            break;
                        case EnvironmentPropData.SpawnLocation.WallPainting:
                            surfaceType |= MRUK.SurfaceType.VERTICAL;
                            break;
                        case EnvironmentPropData.SpawnLocation.OnTopOfSurfaces:
                            surfaceType |= MRUK.SurfaceType.FACING_UP;
                            break;
                        case EnvironmentPropData.SpawnLocation.HangingDown:
                            surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                            break;
                        case EnvironmentPropData.SpawnLocation.AgainstWall:
                            surfaceType |= MRUK.SurfaceType.VERTICAL;
                            break;
                    }

                    if (room.GenerateRandomPositionOnSurface(surfaceType, minRadius, LabelFilter.Included(propData.Labels), out var pos, out var normal))
                    {
                        spawnPosition = pos + normal * baseOffset;
                        spawnNormal = normal;

                        var center = spawnPosition + normal * centerOffset;

                        if (!room.IsPositionInRoom(center))
                        {
                            continue;
                        }

                        if (room.IsPositionInSceneVolume(center))
                        {
                            continue;
                        }

                        if (room.Raycast(new Ray(pos, normal), propData.SurfaceClearanceDistance, out _))
                        {
                            continue;
                        }
                    }
                }

                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);

                if (propData.SpawnLocations == EnvironmentPropData.SpawnLocation.WallPainting)
                {
                    spawnRotation = Quaternion.Euler(-90, spawnRotation.eulerAngles.y, SnapToNearest90Degrees(spawnRotation.eulerAngles.z));
                }

                bool isValidSpawn = true;
                if (propData.CheckOverlaps && prefabBounds.HasValue)
                {
                    if (Physics.CheckBox(spawnPosition + spawnRotation * adjustedBounds.center, adjustedBounds.extents, spawnRotation, propData.LayerMask, QueryTriggerInteraction.UseGlobal))
                    {
                        isValidSpawn = false;
                    }
                }

                if (isValidSpawn && propData.CheckForNonPropCollision)
                {
                    var spawnedObject = Instantiate(propData.SpawnObject);

                    spawnedObject.transform.position = spawnPosition;
                    spawnedObject.transform.localRotation = spawnRotation;

                    Destroy(spawnedObject);
                }

                potentialSpawns.Add((spawnPosition, spawnRotation, adjustedBounds, isValidSpawn));

                if (isValidSpawn)
                {
                    GameObject spawnedObject;

                    if (propData.SpawnLocations == EnvironmentPropData.SpawnLocation.AgainstWall)
                    {
                        spawnPosition.y = 0f; // Adjusting to place the object on the floor
                        spawnRotation = Quaternion.LookRotation(-spawnNormal, Vector3.up);
                    }

                    if (propData.SpawnObject.gameObject.scene.path == null)
                    {
                        spawnedObject = Instantiate(propData.SpawnObject, spawnPosition, spawnRotation, transform);
                    }
                    else
                    {
                        spawnedObject = propData.SpawnObject;
                        spawnedObject.transform.position = spawnPosition;
                        spawnedObject.transform.localRotation = spawnRotation;
                        return;
                    }

                    spawnedObjects.Add(spawnedObject);
                    break;
                }
            }
        }
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
        potentialSpawns.Clear();
        spawnedObjects.Clear();
    }

    private Bounds? GetPrefabBoundsFromBoxCollider(GameObject prefab, bool forceRecalculate = false)
    {
        if (forceRecalculate || !prefabBoundsCache.TryGetValue(prefab, out Bounds? cachedBounds))
        {
            Bounds? bounds = CalculateBoundsFromBoxColliderRecursively(prefab.transform);
            prefabBoundsCache[prefab] = bounds;
            return bounds;
        }

        return cachedBounds;
    }

    private Bounds? CalculateBoundsFromBoxColliderRecursively(Transform transform)
    {
        BoxCollider boxCollider = transform.GetComponent<BoxCollider>();

        if (boxCollider != null)
        {

            // Log the properties of the BoxCollider
            Debug.Log($"{transform.name} BoxCollider Center: {boxCollider.center}, Size: {boxCollider.size}, Bounds: {boxCollider.bounds}");

            // Calculate the bounds using the BoxCollider size and center
            Vector3 worldCenter = transform.TransformPoint(boxCollider.center);
            Vector3 worldSize = Vector3.Scale(boxCollider.size, transform.lossyScale);

            // Create an oriented bounds using the object's rotation
            Quaternion rotation = transform.rotation;
            Bounds bounds = new Bounds(worldCenter, Vector3.zero);
            foreach (Vector3 corner in GetCorners(worldCenter, worldSize, rotation))
            {
                bounds.Encapsulate(corner);
            }

            Debug.Log(transform.name + " Bounds " + bounds);
            return bounds; // Return immediately after finding the first BoxCollider
        }

        // Recursively process children
        foreach (Transform child in transform)
        {
            Bounds? childBounds = CalculateBoundsFromBoxColliderRecursively(child);
            if (childBounds != null)
            {
                return childBounds; // Return immediately after finding the first BoxCollider in children
            }
        }

        return null;
    }

    // Utility function to get the corners of an oriented box
    private static IEnumerable<Vector3> GetCorners(Vector3 center, Vector3 size, Quaternion rotation)
    {
        Vector3 halfSize = size / 2;
        Vector3[] corners = new Vector3[8];
        corners[0] = center + rotation * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
        corners[1] = center + rotation * new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
        corners[2] = center + rotation * new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
        corners[3] = center + rotation * new Vector3(halfSize.x, halfSize.y, -halfSize.z);
        corners[4] = center + rotation * new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
        corners[5] = center + rotation * new Vector3(halfSize.x, -halfSize.y, halfSize.z);
        corners[6] = center + rotation * new Vector3(-halfSize.x, halfSize.y, halfSize.z);
        corners[7] = center + rotation * new Vector3(halfSize.x, halfSize.y, halfSize.z);
        return corners;
    }

    private static float SnapToNearest90Degrees(float angle)
    {
        return Mathf.Round(angle / 90.0f) * 90.0f;
    }
}
