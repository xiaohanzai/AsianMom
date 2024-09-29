using UnityEngine;
using UnityEngine.Events;
using TMPro;

[CreateAssetMenu(menuName = "Event Channel/UI Text Event Channel", fileName = "NewUITextEventChannel")]
public class TextEventChannelSO : DescriptionSO
{
    [Tooltip("The action to perform")]
    public UnityAction<string> OnEventRaised;

    public void RaiseEvent(string text)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(text);
    }
}
