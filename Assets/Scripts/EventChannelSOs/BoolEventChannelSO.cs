using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Bool Event Channel", fileName = "NewBoolEventChannel")]
public class BoolEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<bool> OnEventRaised;

    public void RaiseEvent(bool data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}
