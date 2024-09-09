using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Spawn Mom Parameters Event Channel", fileName = "NewSpawnMomParametersEventChannel")]
public class SpawnMomParametersEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<SpawnMomParameters> OnEventRaised;

    public void RaiseEvent(SpawnMomParameters data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}
