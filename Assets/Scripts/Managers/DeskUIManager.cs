using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeskUIManager : MonoBehaviour
{
    [Header("Instruction UI")]
    [SerializeField] private TextMeshProUGUI instructionTMP;

    [Header("Watch Out UI")]
    [SerializeField] private GameObject watchOutUI;
    [SerializeField] private float watchOutPanelWaitTime;

    [Header("Load New Level UI")]
    [SerializeField] private GameObject level1LoadUI;
    [SerializeField] private GameObject level1LoadButton;
    [SerializeField] private GameObject nextLevelButton;

    [Header("Level Start UI")]
    [SerializeField] private GameObject levelStartButton;

    [Header("Level Failed UI")]
    [SerializeField] private GameObject levelFailedUI;
    [SerializeField] private GameObject tryAgainButton;

    [Header("Level Complete UI")]
    [SerializeField] private GameObject levelCompleteUI;

    [Header("Game Complete UI")]
    [SerializeField] private GameObject gameCompleteUI;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject playAgainButton;

    [Header("Audios")]
    [SerializeField] private AudioSource computerStartAudio;
    [SerializeField] private AudioSource alarmAudio;

    [Header("Listening to")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;
    [SerializeField] private TextEventChannelSO setInstructionTextEventChannel;
    [SerializeField] private VoidEventChannelSO levelLoadEventChannel;
    [SerializeField] private VoidEventChannelSO levelStartEventChannel;
    [SerializeField] private VoidEventChannelSO levelFailedEventChannel;
    [SerializeField] private VoidEventChannelSO levelCompleteEventChannel;
    [SerializeField] private VoidEventChannelSO allLevelsCompleteEventChannel;
    [SerializeField] private VoidEventChannelSO spawnMomEventChannel;

    private void Start()
    {
        HideAllUI();
        ResetInstructionText();

        pokeButtonEventChannel.OnEventRaised += ShowLevel1LoadUIAndBtn;
        pokeButtonEventChannel.OnEventRaised += HideAllLevelsCompleteUIAndBtn;

        setInstructionTextEventChannel.OnEventRaised += SetInstructionText;

        levelLoadEventChannel.OnEventRaised += ShowLevelStartBtn;
        levelLoadEventChannel.OnEventRaised += HideLevel1LoadUIAndBtn;
        levelLoadEventChannel.OnEventRaised += HideLevelFailedUIAndBtn;
        levelLoadEventChannel.OnEventRaised += HideLevelCompleteUIAndBtn;

        levelStartEventChannel.OnEventRaised += HideLevelStartBtn;

        levelFailedEventChannel.OnEventRaised += ShowLevelFailedUIAndBtn;

        levelCompleteEventChannel.OnEventRaised += ShowLevelCompleteUIAndBtn;

        allLevelsCompleteEventChannel.OnEventRaised += ShowAllLevelsCompleteUIAndBtn;

        spawnMomEventChannel.OnEventRaised += ShowWatchOutPanel;
    }

    private void OnDestroy()
    {
        pokeButtonEventChannel.OnEventRaised -= ShowLevel1LoadUIAndBtn;
        pokeButtonEventChannel.OnEventRaised -= HideAllLevelsCompleteUIAndBtn;

        setInstructionTextEventChannel.OnEventRaised -= SetInstructionText;

        levelLoadEventChannel.OnEventRaised -= ShowLevelStartBtn;
        levelLoadEventChannel.OnEventRaised -= HideLevel1LoadUIAndBtn;
        levelLoadEventChannel.OnEventRaised -= HideLevelFailedUIAndBtn;
        levelLoadEventChannel.OnEventRaised -= HideLevelCompleteUIAndBtn;

        levelStartEventChannel.OnEventRaised -= HideLevelStartBtn;

        levelFailedEventChannel.OnEventRaised -= ShowLevelFailedUIAndBtn;

        levelCompleteEventChannel.OnEventRaised -= ShowLevelCompleteUIAndBtn;

        allLevelsCompleteEventChannel.OnEventRaised -= ShowAllLevelsCompleteUIAndBtn;

        spawnMomEventChannel.OnEventRaised -= ShowWatchOutPanel;
    }

    private void HideAllUI()
    {
        ResetInstructionText();
        HideLevelCompleteUIAndBtn();
        HideLevelFailedUIAndBtn();
        HideLevel1LoadUIAndBtn();
        HideLevelStartBtn();
        HideWatchOutPanel();
        HideAllLevelsCompleteUIAndBtn();
    }

    private void SetInstructionText(string t)
    {
        instructionTMP.text = t;
    }

    private void ResetInstructionText()
    {
        SetInstructionText("");
    }

    private void ShowWatchOutPanel()
    {
        watchOutUI.SetActive(true);
        alarmAudio.Play();
        Invoke("HideWatchOutPanel", watchOutPanelWaitTime);
    }

    private void HideWatchOutPanel()
    {
        watchOutUI.SetActive(false);
        alarmAudio.Stop();
    }

    private void ShowLevelStartBtn()
    {
        levelStartButton.SetActive(true);
    }

    private void HideLevelStartBtn()
    {
        levelStartButton.SetActive(false);
    }

    private void ShowLevel1LoadBtn()
    {
        level1LoadButton.SetActive(true);
    }

    private void ShowLevel1LoadUIAndBtn(PokeButtonType type)
    {
        if (type != PokeButtonType.ConfirmEnvironment) return;
        level1LoadButton.SetActive(true);
        level1LoadUI.SetActive(true);
        computerStartAudio.Play();
    }

    private void HideLevel1LoadUIAndBtn()
    {
        level1LoadButton.SetActive(false);
        level1LoadUI.SetActive(false);
    }

    private void ShowLevelFailedUIAndBtn()
    {
        instructionTMP.gameObject.SetActive(false);
        levelFailedUI.SetActive(true);
        tryAgainButton.SetActive(true);
    }

    private void HideLevelFailedUIAndBtn()
    {
        instructionTMP.gameObject.SetActive(true);
        levelFailedUI.SetActive(false);
        tryAgainButton.SetActive(false);
    }

    private void ShowLevelCompleteUIAndBtn()
    {
        instructionTMP.gameObject.SetActive(false);
        levelCompleteUI.SetActive(true);
        nextLevelButton.SetActive(true);
    }

    private void HideLevelCompleteUIAndBtn()
    {
        instructionTMP.gameObject.SetActive(true);
        levelCompleteUI.SetActive(false);
        nextLevelButton.SetActive(false);
    }

    private void ShowAllLevelsCompleteUIAndBtn()
    {
        HideLevelCompleteUIAndBtn();
        instructionTMP.gameObject.SetActive(false);
        gameCompleteUI.SetActive(true);
        quitButton.SetActive(true);
        playAgainButton.SetActive(true);
    }

    private void HideAllLevelsCompleteUIAndBtn()
    {
        gameCompleteUI.SetActive(false);
        quitButton.SetActive(false);
        playAgainButton.SetActive(false);
    }

    private void HideAllLevelsCompleteUIAndBtn(PokeButtonType type)
    {
        if (type != PokeButtonType.PlayAgain) return;
        HideAllLevelsCompleteUIAndBtn();
    }
}
