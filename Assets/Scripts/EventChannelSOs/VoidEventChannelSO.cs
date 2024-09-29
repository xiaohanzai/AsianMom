using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Void Event Channel", fileName = "NewVoidEventChannel")]
public class VoidEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction OnEventRaised;

    public void RaiseEvent()
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke();
    }
}
