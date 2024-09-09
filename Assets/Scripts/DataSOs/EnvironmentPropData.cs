using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

[CreateAssetMenu(fileName = "NewEnvironmentPropData", menuName = "Data/Environment Prop", order = 0)]
public class EnvironmentPropData : ScriptableObject
{
    [Header("Object Settings")]
    [Tooltip("Prefab to spawn in the environment.")]
    public GameObject prefab;

    [Tooltip("Number of objects to spawn.")]
    public int spawnAmount = 1;

    [Tooltip("Maximum number of attempts to find a valid spawn location.")]
    public int maxIterations = 10;

    [Tooltip("Minimum radius for random spawn positions.")]
    public float minRadius = 0.5f;

    [Header("Spawn Location Settings")]
    public SpawnLocation spawnLocation = SpawnLocation.OnTopOfSurfaces;

    [Tooltip("Labels to filter appropriate surfaces for spawning.")]
    public MRUKAnchor.SceneLabels labels;

    [Header("Visualization Settings")]
    [Tooltip("Color to use for visualizing invalid spawn positions.")]
    public Color visualizationColor = Color.red;

    public enum SpawnLocation
    {
        WallFloor,            // Vertical surfaces such as walls on the floor
        Wall,
        Floating,        // Floating in space
        Ceiling,         // Hanging from the ceiling
        OnTopOfSurfaces, // Surfaces facing upwards like tables, floors
        Window
    }
}