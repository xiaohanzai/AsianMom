using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Audio Event Channel", fileName = "NewAudioEventChannel")]
public class AudioEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<AudioEventInfo> OnEventRaised;

    public void RaiseEvent(AudioEventInfo data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}

public class AudioEventInfo
{
    public AudioType type;
    public AudioClip clip;
}

public enum AudioType
{
    BGM,
    Desk,
    MomWalkOut,
    MomAngry,
}