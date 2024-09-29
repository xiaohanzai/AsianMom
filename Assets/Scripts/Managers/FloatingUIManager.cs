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
    [SerializeField] private GameObject otherLevelFailedUI;
    [SerializeField] private GameObject levelFailedUI;
    [SerializeField] private GameObject levelFailedButtonsParent;

    [Header("Listening to")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;
    [SerializeField] private LevelEventChannelSO levelEventChannel;

    private void Start()
    {
        //SetPosition();
        HideShuffleEnvironmentUIAndButtons();
        HideLevelFailedUIAndButtons();

        pokeButtonEventChannel.OnEventRaised += OnPokeButtonEventRaised;

        levelEventChannel.OnEventRaised += OnLevelEventRaised;
    }

    private void OnDestroy()
    {
        pokeButtonEventChannel.OnEventRaised -= OnPokeButtonEventRaised;
        levelEventChannel.OnEventRaised -= OnLevelEventRaised;
    }

    private void OnLevelEventRaised(LevelEventInfo data)
    {
        if (data.type == LevelEventType.LevelFailed) DelayedShowLevelFailedUIAndButtons();
        else if (data.type == LevelEventType.LevelFailedOther) DelayedShowOtherLevelFailedUIAndButtons();
    }

    private void OnPokeButtonEventRaised(PokeButtonType type)
    {
        if (type == PokeButtonType.StartGame)
        {
            HideLogoUIAndButtons();
            ShowShuffleEnvironmentUIAndButtons();
        }
        else if (type == PokeButtonType.ConfirmEnvironment) HideShuffleEnvironmentUIAndButtons();
        else if (type == PokeButtonType.TryAgain) HideLevelFailedUIAndButtons();
    }

    //private void SetPosition()
    //{
    //    transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.7f;
    //    transform.LookAt(Camera.main.transform);
    //    transform.rotation *= Quaternion.Euler(0, 180, 0);
    //    transform.position += Vector3.up * 1.4f;
    //}

    private void HideLogoUIAndButtons()
    {
        logoUI.SetActive(false);
        startGameButton.SetActive(false);
    }

    private void HideShuffleEnvironmentUIAndButtons()
    {
        shuffleEnvironmentUI.SetActive(false);
        shuffleEnvironmentButtonsParent.SetActive(false);
    }

    private void ShowShuffleEnvironmentUIAndButtons()
    {
        shuffleEnvironmentUI.SetActive(true);
        shuffleEnvironmentButtonsParent.SetActive(true);
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

    private void DelayedShowOtherLevelFailedUIAndButtons()
    {
        Invoke("ShowOtherLevelFailedUIAndButtons", waitTimeToShowLevelFailedUI);
    }

    private void ShowOtherLevelFailedUIAndButtons()
    {
        otherLevelFailedUI.SetActive(true);
        levelFailedButtonsParent.SetActive(true);
    }

    private void HideLevelFailedUIAndButtons()
    {
        otherLevelFailedUI.SetActive(false);
        levelFailedUI.SetActive(false);
        levelFailedButtonsParent.SetActive(false);
    }
}
