using System;
using System.Linq;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class EnvironmentPropHandler : MonoBehaviour
{
    //[SerializeField] private GameObject mesh;
    //[SerializeField] private LayerMask environmentLayerMask; // 
    [SerializeField] private MRUKAnchor.SceneLabels ignoreAnchorLabels; // Labels to ignore when checking for MRUKAnchors

    [Header("Overlap Box Settings")]
    [SerializeField] public Vector3 overlapBoxSize = new Vector3(1, 1, 1); // Size of the overlap box
    [SerializeField] private Vector3 overlapBoxCenter = Vector3.zero; // Center offset of the overlap box

    private bool _isTouchingObject;
    private bool _isOverlappingWithMRUKAnchor; // Field to track overlap with MRUKAnchors

    private Collider[] _overlapResults = new Collider[10]; // Pre-allocated array for OverlapBoxNonAlloc

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        DetectEnvironmentCollisions();
    }

    public void DetectEnvironmentCollisions()
    {
        // Calculate the world center and extents of the custom overlap box
        Vector3 worldCenter = transform.TransformPoint(overlapBoxCenter);
        Vector3 worldExtents = overlapBoxSize * 0.5f;

        // Use Physics.OverlapBoxNonAlloc to check for overlaps, passing in the pre-allocated array
        int overlapCount = Physics.OverlapBoxNonAlloc(worldCenter, worldExtents, _overlapResults, transform.rotation);

        _isTouchingObject = false;
        _isOverlappingWithMRUKAnchor = false;

        for (int i = 0; i < overlapCount; i++)
        {
            var collider = _overlapResults[i];

            // Check if colliding with another EnvironmentPropHandler
            EnvironmentPropHandler otherPropHandler = collider.GetComponent<EnvironmentPropHandler>();
            if (otherPropHandler && otherPropHandler != this)
            {
                _isTouchingObject = true;
            }

            // Check if colliding with an MRUKAnchor and filter by labels
            MRUKAnchor anchor = collider.GetComponentInParent<MRUKAnchor>();
            if (anchor != null && !ShouldIgnoreAnchor(anchor))
            {
                _isOverlappingWithMRUKAnchor = true;
            }

            // If we detect either type of collision, we don't need to check further
            if (_isTouchingObject || _isOverlappingWithMRUKAnchor)
            {
                break;
            }
        }
    }

    private bool ShouldIgnoreAnchor(MRUKAnchor anchor)
    {
        // Check if the anchor's labels match any of the labels to be ignored
        if ((ignoreAnchorLabels & anchor.Label) == 0)
        {
            return false;
        }
       
        return true;
    }

    private void OnDrawGizmos()
    {
        // Visualize the overlap box with Gizmos
        Gizmos.color = _isTouchingObject || _isOverlappingWithMRUKAnchor ? Color.red : Color.green;
        Vector3 worldCenter = transform.TransformPoint(overlapBoxCenter);
        Vector3 worldExtents = overlapBoxSize * 0.5f;
        Gizmos.matrix = Matrix4x4.TRS(worldCenter, transform.rotation, worldExtents * 2);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one); // Draw the box at the calculated position and size
    }

    //public void ShowMesh()
    //{
    //    mesh.SetActive(true);
    //}

    public bool IsTouchingObject()
    {
        return _isTouchingObject;
    }

    public bool IsOverlappingWithMRUKAnchor()
    {
        return _isOverlappingWithMRUKAnchor;
    }
}

