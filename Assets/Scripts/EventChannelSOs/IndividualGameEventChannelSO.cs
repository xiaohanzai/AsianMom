using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Individual Game Event Channel", fileName = "NewIndividualGameEventChannel")]
public class IndividualGameEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<IndividualGameName> OnEventRaised;

    public void RaiseEvent(IndividualGameName data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}
