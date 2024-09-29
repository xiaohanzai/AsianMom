using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemAdjustment : MonoBehaviour
{
    [SerializeField] private GameObject debugCanvas;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        GetComponent<PointableCanvasModule>().enabled = false;
#else
        GetComponent<StandaloneInputModule>().enabled = false;
        debugCanvas.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
