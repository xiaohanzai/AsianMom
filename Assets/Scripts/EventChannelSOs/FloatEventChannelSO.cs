using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Float Event Channel", fileName = "NewFloatEventChannel")]
public class FloatEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<float> OnEventRaised;

    public void RaiseEvent(float data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}
