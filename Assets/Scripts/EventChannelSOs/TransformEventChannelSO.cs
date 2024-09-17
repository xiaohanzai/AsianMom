using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Transform Event Channel", fileName = "NewTransformEventChannel")]
public class TransformEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<Transform> OnEventRaised;

    public void RaiseEvent(Transform t)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(t);
    }
}
