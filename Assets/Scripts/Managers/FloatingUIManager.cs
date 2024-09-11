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
    [SerializeField] private GameObject shuffleEnvironmentButton;
    [SerializeField] private GameObject confirmEnvironmentButton;

    [Header("Listening to")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;

    private void Start()
    {
        //SetPosition();
        HideShuffleEnvironmentUIAndButtons();

        pokeButtonEventChannel.OnEventRaised += HideLogoUIAndButtons;
        pokeButtonEventChannel.OnEventRaised += ShowShuffleEnvironmentUIAndButtons;
        pokeButtonEventChannel.OnEventRaised += HideShuffleEnvironmentUIAndButtons;
    }

    private void OnDestroy()
    {
        pokeButtonEventChannel.OnEventRaised -= HideLogoUIAndButtons;
        pokeButtonEventChannel.OnEventRaised -= ShowShuffleEnvironmentUIAndButtons;
        pokeButtonEventChannel.OnEventRaised -= HideShuffleEnvironmentUIAndButtons;
    }

    private void SetPosition()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.7f;
        transform.LookAt(Camera.main.transform);
        transform.rotation *= Quaternion.Euler(0, 180, 0);
        transform.position += Vector3.up * 1.4f;
    }

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
        shuffleEnvironmentButton.SetActive(false);
        confirmEnvironmentButton.SetActive(false);
    }

    private void ShowShuffleEnvironmentUIAndButtons(PokeButtonType type)
    {
        if (type != PokeButtonType.StartGame)
        {
            return;
        }
        shuffleEnvironmentUI.SetActive(true);
        shuffleEnvironmentButton.SetActive(true);
        confirmEnvironmentButton.SetActive(true);
    }

    private void HideShuffleEnvironmentUIAndButtons(PokeButtonType type)
    {
        if (type != PokeButtonType.ConfirmEnvironment)
        {
            return;
        }
        HideShuffleEnvironmentUIAndButtons();
    }
}
