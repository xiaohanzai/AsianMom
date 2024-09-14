using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;

public class TabletButtonEventHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonTMP;
    [SerializeField] private Toggle toggle;

    private IndividualGameName gameName;
    private string instructionText;
    private VideoClip instructionVideo;
    private AudioClip audioClip;

    [Header("Broadcasting on")]
    [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;
    [SerializeField] private UIInstructionEventChannelSO uIInstructionEventChannel;
    [SerializeField] private AudioEventChannelSO deskAudioEventChannel;

    private void Start()
    {
        ResetButton();
        SetDisabled();
    }

    public void StartGame(bool start)
    {
        if (start)
        {
            gameStartEventChannel.RaiseEvent(gameName);
            UIInstruction data = new UIInstruction
            {
                text = instructionText,
                video = instructionVideo,
            };
            uIInstructionEventChannel.RaiseEvent(data);
            deskAudioEventChannel.RaiseEvent(audioClip);
        }
        else
        {

        }
    }

    public void ResetButton()
    {
        buttonTMP.text = "";
        toggle.enabled = false;
        toggle.isOn = false;
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
        return toggle.isOn;
    }

    public void SetToggleOff()
    {
        toggle.isOn = false;
    }

    public void SetToggleOn()
    {
        toggle.isOn = true;
    }

    public void SetDisabled()
    {
        toggle.enabled = false;
    }

    public void SetEnabled()
    {
        toggle.enabled = true;
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