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
    private Dictionary<GameObject, Bounds?> prefabBoundsCache = new Dictionary<GameObject, Bounds?>();

    void Start()
    {
#if UNITY_EDITOR
        // for debugging
        if (MRUK.Instance)
        {
            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                SpawnEnvironment();
            });
        }
#endif
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
                SpawnEnvironment();
                break;
            case PokeButtonType.ConfirmEnvironment:
                break;
            default:
                break;
        }
    }

    private void SpawnEnvironment()
    {
        //int n = 0;
        //while (spawnedObjects.Count < propDataList.Count && n < 1) // TODO: I don't think this works properly
        //{
        ShuffleEnvironment();
        //    Debug.Log(n);
        //    n++;
        //}
        // TODO: protect mechanism for environment generation failure?
    }

    private void ShuffleEnvironment()
    {
        ClearEnvironment();
        foreach (var propData in propDataList)
        {
            SpawnObjects(propData);
            Debug.Log(spawnedObjects.Count);
        }
    }

    private void SpawnObjects(EnvironmentPropData propData)
    {
        var room = MRUK.Instance.GetCurrentRoom();
        //var prefabBounds = Utilities.GetPrefabBounds(propData.SpawnObject);
        var prefabBounds = GetPrefabBoundsFromBoxCollider(propData.SpawnObject);
        Debug.Log($"{propData.SpawnObject.name} Prefab Bounds: {prefabBounds?.ToString() ?? "null"}");

        float minRadius = 0.0f;
        const float clearanceDistance = 0.01f;
        float baseOffset = -prefabBounds?.min.y ?? 0.0f;
        float centerOffset = prefabBounds?.center.y ?? 0.0f;
        Bounds adjustedBounds = new();

        if (prefabBounds.HasValue)
        {
            minRadius = Mathf.Max(-prefabBounds.Value.min.x, -prefabBounds.Value.min.z, prefabBounds.Value.max.x, prefabBounds.Value.max.z);
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
                Vector3 size = new Vector3(propData.OverrideBounds * 2f, clearanceDistance * 2f, propData.OverrideBounds * 2f); // OverrideBounds represents the extents, not the size
                adjustedBounds = new Bounds(center, size);
            }
            Debug.Log(adjustedBounds.center.ToString() + "  " + adjustedBounds.extents.ToString());
        }

        for (int i = 0; i < propData.SpawnAmount; ++i)
        {
            for (int j = 0; j < propData.MaxIterations; ++j)
            {
                Vector3 spawnPosition = Vector3.zero;
                Vector3 spawnNormal = Vector3.zero;
                if (propData.SpawnLoc == EnvironmentPropData.SpawnLocation.Floating)
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
                    switch (propData.SpawnLoc)
                    {
                        case EnvironmentPropData.SpawnLocation.AnySurface:
                            surfaceType |= MRUK.SurfaceType.FACING_UP;
                            surfaceType |= MRUK.SurfaceType.VERTICAL;
                            surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                            break;
                        case EnvironmentPropData.SpawnLocation.VerticalSurfaces:
                            surfaceType |= MRUK.SurfaceType.VERTICAL;
                            break;
                        case EnvironmentPropData.SpawnLocation.AgainstWall:
                            surfaceType |= MRUK.SurfaceType.VERTICAL;
                            break;
                        case EnvironmentPropData.SpawnLocation.OnTopOfSurfaces:
                            surfaceType |= MRUK.SurfaceType.FACING_UP;
                            break;
                        case EnvironmentPropData.SpawnLocation.HangingDown:
                            surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                            break;
                    }
                    if (room.GenerateRandomPositionOnSurface(surfaceType, minRadius, LabelFilter.Included(propData.Labels), out var pos, out var normal))
                    {
                        spawnPosition = pos + normal * baseOffset;
                        spawnNormal = normal;
                        var center = spawnPosition + normal * centerOffset;
                        // In some cases, surfaces may protrude through walls and end up outside the room
                        // check to make sure the center of the prefab will spawn inside the room
                        if (!room.IsPositionInRoom(center))
                        {
                            continue;
                        }
                        // Ensure the center of the prefab will not spawn inside a scene volume
                        if (room.IsPositionInSceneVolume(center))
                        {
                            continue;
                        }
                        // Also make sure there is nothing close to the surface that would obstruct it
                        if (room.Raycast(new Ray(pos, normal), propData.SurfaceClearanceDistance, out _))
                        {
                            continue;
                        }
                    }
                }

                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);
                if (propData.SpawnLoc == EnvironmentPropData.SpawnLocation.AgainstWall) // TODO: I still don't know why this doesn't make physics.checkbox work
                {
                    spawnPosition.y = 0f;
                    spawnRotation = Quaternion.LookRotation(-spawnNormal);
                }
                if (propData.CheckOverlaps && prefabBounds.HasValue)
                {
                    if (Physics.CheckBox(spawnPosition + spawnRotation * adjustedBounds.center, adjustedBounds.extents, spawnRotation, propData.LayerMask, QueryTriggerInteraction.Ignore))
                    {
                        if (propData.name == "Bookshelf")
                        {
                            Debug.Log(spawnPosition + spawnRotation * adjustedBounds.center);
                        }
                        continue;
                    }
                }

                //if (propData.SpawnLoc == EnvironmentPropData.SpawnLocation.AgainstWall)
                //{
                //    spawnPosition.y = 0f;
                //    spawnRotation = Quaternion.LookRotation(-spawnNormal);
                //}

                GameObject spawnedObject;
                if (propData.SpawnObject.gameObject.scene.path == null)
                {
                    spawnedObject = Instantiate(propData.SpawnObject, spawnPosition, spawnRotation, transform);
                }
                else
                {
                    spawnedObject = propData.SpawnObject;
                    spawnedObject.transform.position = spawnPosition;
                    spawnedObject.transform.localRotation = spawnRotation;
                    return; // ignore SpawnAmount once we have a successful move of existing object in the scene
                }
                spawnedObjects.Add(spawnedObject);
                break;
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
            //Debug.Log($"{transform.name} BoxCollider Center: {boxCollider.center}, Size: {boxCollider.size}, Bounds: {boxCollider.bounds}");

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

            //Debug.Log(transform.name + " Bounds " + bounds);
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
