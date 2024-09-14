using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Audio Event Channel", fileName = "NewAudioEventChannel")]
public class AudioEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<AudioClip> OnEventRaised;

    public void RaiseEvent(AudioClip data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}
