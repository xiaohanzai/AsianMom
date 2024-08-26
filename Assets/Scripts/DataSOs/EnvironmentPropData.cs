using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

[CreateAssetMenu(fileName = "NewEnvironmentPropData", menuName = "Data/Environment Prop", order = 0)]
public class EnvironmentPropData : ScriptableObject
{
    [Tooltip("Prefab to be placed into the scene, or object in the scene to be moved around.")]
    public GameObject SpawnObject;

    [Tooltip("Number of SpawnObject(s) to place into the scene, only applies to Prefabs.")]
    public int SpawnAmount = 8;

    [Tooltip("Maximum number of times to attempt spawning/moving an object before giving up.")]
    public int MaxIterations = 1000;

    [Tooltip("Checks if the gameobject is colliding with non environment props.")]
    public bool CheckForNonPropCollision = false;

    public enum SpawnLocation
    {
        Floating, // Spawn somewhere floating in the free space within the room
        AnySurface, // Spawn on any surface (i.e. a combination of all 3 options below)
        VerticalSurfaces, // Spawn only on vertical surfaces such as walls, windows, wall art, doors, etc...
        OnTopOfSurfaces, // Spawn on surfaces facing upwards such as ground, top of tables, beds, couches, etc...
        HangingDown, // Spawn on surfaces facing downwards such as the ceiling
        AgainstWall, // Spawn on the floor against the wall
        WallPainting, // Spawn with the Y Axis always pointed up
    }

    [Tooltip("Attach content to scene surfaces.")]
    public SpawnLocation SpawnLocations = SpawnLocation.Floating;

    [Tooltip("When using surface spawning, use this to filter which anchor labels should be included. Eg, spawn only on TABLE or OTHER.")]
    public MRUKAnchor.SceneLabels Labels = ~(MRUKAnchor.SceneLabels)0;

    [Tooltip("If enabled then the spawn position will be checked to make sure there is no overlap with physics colliders including themselves.")]
    public bool CheckOverlaps = true;

    [Tooltip("Required free space for the object (Set negative to auto-detect using GetPrefabBounds)")]
    public float OverrideBounds = -1;

    [Tooltip("Set the layer(s) for the physics bounding box checks, collisions will be avoided with these layers.")]
    public LayerMask LayerMask = -1;

    [Tooltip("The clearance distance required in front of the surface in order for it to be considered a valid spawn position")]
    public float SurfaceClearanceDistance = 0.1f;
}