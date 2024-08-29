using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TabletButtonEventHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Toggle toggle;

    [Header("Broadcasting on")]
    [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;

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
        }
        else
        {

        }
    }

    public void ResetButton()
    {
        textMeshPro.text = "";
        toggle.enabled = true;
        toggle.isOn = false;
        gameName = IndividualGameName.Null;
    }

    public void SetToggleOff()
    {
        toggle.isOn = false;
    }

    public void SetToggleOn()
    {
        toggle.isOn = true;
    }

    public void SetText(string txt)
    {
        textMeshPro.text = txt;
    }

    public void SetDisabled()
    {
        toggle.isOn = false; // TODO: this did not work?
        toggle.enabled = false;
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
