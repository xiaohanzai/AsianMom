using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemAdjustment : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        GetComponent<PointableCanvasModule>().enabled = false;
#else
        GetComponent<StandaloneInputModule>().enabled = false;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
