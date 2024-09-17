using UnityEngine;
using UnityEngine.Events;

namespace Mom
{
    [CreateAssetMenu(menuName = "Event Channel/Mom State Channel", fileName = "NewMomStateEventChannel")]
    public class MomStateEventChannelSO : DescriptionSO
    {
        [Tooltip("The action to perform")]
        public UnityAction<MomStateName> OnEventRaised;

        public void RaiseEvent(MomStateName data)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(data);
        }
    }
}