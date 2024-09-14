using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUIManager : MonoBehaviour
{
    [Header("Logo UI")]
    [SerializeField] private GameObject logoUI;
    [SerializeField] private GameObject startGameButton;

    [Header("Shuffle Environment UI")]
    [SerializeField] private GameObject shuffleEnvironmentUI;
    [SerializeField] private GameObject shuffleEnvironmentButtonsParent;

    [Header("Level Failed UI")]
    [SerializeField] private float waitTimeToShowLevelFailedUI;
    [SerializeField] private GameObject levelFailedUI;
    [SerializeField] private GameObject levelFailedButtonsParent;

    [Header("Listening to")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;
    [SerializeField] private VoidEventChannelSO levelFailedEventChannel;

    private void Start()
    {
        //SetPosition();
        HideShuffleEnvironmentUIAndButtons();
        HideLevelFailedUIAndButtons();

        pokeButtonEventChannel.OnEventRaised += HideLogoUIAndButtons;
        pokeButtonEventChannel.OnEventRaised += ShowShuffleEnvironmentUIAndButtons;
        pokeButtonEventChannel.OnEventRaised += HideShuffleEnvironmentUIAndButtons;
        pokeButtonEventChannel.OnEventRaised += HideLevelFailedUIAndButtons;

        levelFailedEventChannel.OnEventRaised += DelayedShowLevelFailedUIAndButtons;
    }

    private void OnDestroy()
    {
        pokeButtonEventChannel.OnEventRaised -= HideLogoUIAndButtons;
        pokeButtonEventChannel.OnEventRaised -= ShowShuffleEnvironmentUIAndButtons;
        pokeButtonEventChannel.OnEventRaised -= HideShuffleEnvironmentUIAndButtons;
        pokeButtonEventChannel.OnEventRaised -= HideLevelFailedUIAndButtons;
        levelFailedEventChannel.OnEventRaised -= DelayedShowLevelFailedUIAndButtons;
    }

    //private void SetPosition()
    //{
    //    transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.7f;
    //    transform.LookAt(Camera.main.transform);
    //    transform.rotation *= Quaternion.Euler(0, 180, 0);
    //    transform.position += Vector3.up * 1.4f;
    //}

    private void HideLogoUIAndButtons(PokeButtonType type)
    {
        if (type != PokeButtonType.StartGame)
        {
            return;
        }
        logoUI.SetActive(false);
        startGameButton.SetActive(false);
    }

    private void HideShuffleEnvironmentUIAndButtons()
    {
        shuffleEnvironmentUI.SetActive(false);
        shuffleEnvironmentButtonsParent.SetActive(false);
    }

    private void ShowShuffleEnvironmentUIAndButtons(PokeButtonType type)
    {
        if (type != PokeButtonType.StartGame)
        {
            return;
        }
        shuffleEnvironmentUI.SetActive(true);
        shuffleEnvironmentButtonsParent.SetActive(true);
    }

    private void HideShuffleEnvironmentUIAndButtons(PokeButtonType type)
    {
        if (type != PokeButtonType.ConfirmEnvironment)
        {
            return;
        }
        HideShuffleEnvironmentUIAndButtons();
    }

    private void DelayedShowLevelFailedUIAndButtons()
    {
        Invoke("ShowLevelFailedUIAndButtons", waitTimeToShowLevelFailedUI);
    }

    private void ShowLevelFailedUIAndButtons()
    {
        levelFailedUI.SetActive(true);
        levelFailedButtonsParent.SetActive(true);
    }

    private void HideLevelFailedUIAndButtons()
    {
        levelFailedUI.SetActive(false);
        levelFailedButtonsParent.SetActive(false);
    }

    private void HideLevelFailedUIAndButtons(PokeButtonType type)
    {
        if (type != PokeButtonType.TryAgain) return;
        HideLevelFailedUIAndButtons();
    }
}
