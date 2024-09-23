using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channel/Level Event Channel", fileName = "NewLevelEventChannel")]
public class LevelEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<LevelEventInfo> OnEventRaised;

    public void RaiseEvent(LevelEventInfo data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}

public class LevelEventInfo
{
    public LevelEventType type;
}

public enum LevelEventType
{
    LevelLoad,
    LevelStart,
    LevelFailed,
    LevelComplete,
}