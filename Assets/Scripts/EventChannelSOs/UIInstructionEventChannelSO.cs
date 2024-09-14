using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Event Channel/UI Instruction Event Channel", fileName = "NewUIInstructionEventChannel")]
public class UIInstructionEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<UIInstruction> OnEventRaised;

    public void RaiseEvent(UIInstruction data)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(data);
    }
}

public class UIInstruction
{
    public string text;
    public VideoClip video;
}
