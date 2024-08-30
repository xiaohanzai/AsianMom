using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MusicGame
{
    [CreateAssetMenu(fileName = "NewSetMusicEventChannel", menuName = "Event Channel/Individual Games/Music Game/Set Music Event Channel")]
    public class SetMusicEventChannelSO : DescriptionSO
    {
        [Tooltip("The action to perform")]
        public UnityAction<MusicData> OnEventRaised;

        public void RaiseEvent(MusicData data)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(data);
        }
    }
}