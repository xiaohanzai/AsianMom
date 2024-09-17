using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Int Event Channel", fileName = "NewIntEventChannel")]
public class IntEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<int> OnEventRaised;

    public void RaiseEvent(int data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}
