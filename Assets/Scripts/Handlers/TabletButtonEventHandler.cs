using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class TabletButtonEventHandler : MonoBehaviour
{
    [SerializeField] private TextMeshPro buttonTMP;
    [SerializeField] private MeshRenderer meshRenderer;

    private IndividualGameName gameName;
    private string instructionText;
    private VideoClip instructionVideo;
    private AudioClip audioClip;

    [Header("Broadcasting on")]
    [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;
    [SerializeField] private UIInstructionEventChannelSO uIInstructionEventChannel;
    [SerializeField] private AudioEventChannelSO deskAudioEventChannel;

    private TabletButtonToggleGroup tabletButtonToggleGroup;

    private Color originalColor;
    private Color selectedColor;
    private Color disabledColor;

    [SerializeField] private bool isToggledOn;
    [SerializeField] private bool isEnabled;

    public void LinkToToggleGroup(TabletButtonToggleGroup group)
    {
        tabletButtonToggleGroup = group;
        var (oc, sc, dc) = group.GetColors();
        originalColor = oc;
        selectedColor = sc;
        disabledColor = dc;
    }

    public void RaiseEvent()
    {
        if (!isToggledOn && isEnabled)
        {
            gameStartEventChannel.RaiseEvent(gameName);
            UIInstruction data = new UIInstruction
            {
                text = instructionText,
                video = instructionVideo,
            };
            uIInstructionEventChannel.RaiseEvent(data);
            deskAudioEventChannel.RaiseEvent(audioClip);
            tabletButtonToggleGroup.SetCurrentButton(this);
            isToggledOn = true;
        }
    }

    public void ResetButton()
    {
        buttonTMP.text = "";
        isEnabled = false;
        isToggledOn = false;
        gameName = IndividualGameName.Null;
    }

    public void SetUpButton(TabletButtonInfo data)
    {
        gameName = data.gameName;
        buttonTMP.text = data.buttonText;
        instructionText = data.instructionText;
        instructionVideo = data.instructionVideo;
        audioClip = data.audio;
    }

    public bool CheckIfToggleOn()
    {
        return isToggledOn;
    }

    public void SetToggleOff()
    {
        isToggledOn = false;
        meshRenderer.material.color = originalColor;
    }

    public void SetToggleOn()
    {
        isToggledOn = true;
        meshRenderer.material.color = selectedColor;
    }

    public void SetDisabled()
    {
        isEnabled = false;
        meshRenderer.material.color = disabledColor;
    }

    public void SetEnabled()
    {
        isEnabled = true;
        meshRenderer.material.color = originalColor;
    }

    public IndividualGameName GetCurrentGameName()
    {
        return gameName;
    }
}

public class TabletButtonInfo
{
    public IndividualGameName gameName;
    public string buttonText;
    public string instructionText;
    public VideoClip instructionVideo;
    public AudioClip audio;
}