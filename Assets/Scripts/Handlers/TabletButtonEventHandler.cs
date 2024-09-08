using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mom;

public class TabletButtonEventHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonTMP;
    [SerializeField] private Toggle toggle;

    private string instructionText;

    [Header("Broadcasting on")]
    [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;
    [SerializeField] private TextEventChannelSO setInstructionTextEventChannel;

    private IndividualGameName gameName;

    private void Start()
    {
        ResetButton();
    }

    public void StartGame(bool start)
    {
        if (start)
        {
            gameStartEventChannel.RaiseEvent(gameName);
            setInstructionTextEventChannel.RaiseEvent(instructionText);
        }
        else
        {

        }
    }

    public void ResetButton()
    {
        buttonTMP.text = "";
        toggle.enabled = true;
        toggle.isOn = false;
        gameName = IndividualGameName.Null;
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

    public void SetButtonText(string txt)
    {
        buttonTMP.text = txt;
    }

    public void SetInstructionText(string txt)
    {
        instructionText = txt;
    }

    public void SetDisabled()
    {
        toggle.isOn = false; // TODO: this did not work?
        toggle.enabled = false;
    }

    public void SetEnabled()
    {
        toggle.enabled = true;
    }

    public void LinkToGame(IndividualGameName name)
    {
        gameName = name;
    }

    public void UnlinkToGame()
    {
        gameName = IndividualGameName.Null;
    }

    public IndividualGameName GetCurrentGameName()
    {
        return gameName;
    }
}
