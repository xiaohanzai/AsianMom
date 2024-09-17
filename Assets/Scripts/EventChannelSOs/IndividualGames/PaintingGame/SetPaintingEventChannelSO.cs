using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PaintingGame
{
    [CreateAssetMenu(fileName = "NewSetPaintingEventChannel", menuName = "Event Channel/Individual Games/Painting Game/Set Painting Event Channel")]
    public class SetPaintingEventChannelSO : DescriptionSO
    {
        [Tooltip("The action to perform")]
        public UnityAction<PaintingData> OnEventRaised;

        public void RaiseEvent(PaintingData data)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(data);
        }
    }
}