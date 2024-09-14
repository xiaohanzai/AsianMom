using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class DeskUIManager : MonoBehaviour
{
    [Header("Instruction UI")]
    [SerializeField] private TextMeshProUGUI instructionTMP;

    [Header("Individual Game UI")]
    [SerializeField] private GameObject individualGameUIParent;
    [SerializeField] private TextMeshProUGUI individualGameTMP;
    [SerializeField] private VideoPlayer individualGameVideoPlayer;

    [Header("Watch Out UI")]
    [SerializeField] private GameObject watchOutUI;
    [SerializeField] private float watchOutPanelWaitTime;

    [Header("Load New Level UI")]
    [SerializeField] private GameObject level1LoadUI;
    [SerializeField] private GameObject level1LoadButton;
    [SerializeField] private GameObject nextLevelButton;

    [Header("Level Start UI")]
    [SerializeField] private GameObject levelStartButton;

    [Header("Level Complete UI")]
    [SerializeField] private GameObject levelCompleteUI;

    [Header("Game Complete UI")]
    [SerializeField] private GameObject gameCompleteUI;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject playAgainButton;

    [Header("Listening to")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;
    [SerializeField] private TextEventChannelSO setInstructionTextEventChannel;
    [SerializeField] private UIInstructionEventChannelSO uIInstructionEventChannel;
    [SerializeField] private VoidEventChannelSO levelLoadEventChannel;
    [SerializeField] private VoidEventChannelSO levelStartEventChannel;
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
        uIInstructionEventChannel.OnEventRaised += SetindividualGameTextAndVideo;

        levelLoadEventChannel.OnEventRaised += ShowLevelStartUIAndBtn;
        levelLoadEventChannel.OnEventRaised += HideLevel1LoadUIAndBtn;
        levelLoadEventChannel.OnEventRaised += HideLevelCompleteUIAndBtn;
        levelLoadEventChannel.OnEventRaised += HideIndividualGameUI;

        levelStartEventChannel.OnEventRaised += HideLevelStartUIAndBtn;

        levelCompleteEventChannel.OnEventRaised += ShowLevelCompleteUIAndBtn;
        levelCompleteEventChannel.OnEventRaised += HideIndividualGameUI;

        allLevelsCompleteEventChannel.OnEventRaised += ShowAllLevelsCompleteUIAndBtn;

        spawnMomEventChannel.OnEventRaised += ShowWatchOutPanel;
    }

    private void OnDestroy()
    {
        pokeButtonEventChannel.OnEventRaised -= ShowLevel1LoadUIAndBtn;
        pokeButtonEventChannel.OnEventRaised -= HideAllLevelsCompleteUIAndBtn;

        setInstructionTextEventChannel.OnEventRaised -= SetInstructionText;
        uIInstructionEventChannel.OnEventRaised -= SetindividualGameTextAndVideo;

        levelLoadEventChannel.OnEventRaised -= ShowLevelStartUIAndBtn;
        levelLoadEventChannel.OnEventRaised -= HideLevel1LoadUIAndBtn;
        levelLoadEventChannel.OnEventRaised -= HideLevelCompleteUIAndBtn;
        levelLoadEventChannel.OnEventRaised -= HideIndividualGameUI;

        levelStartEventChannel.OnEventRaised -= HideLevelStartUIAndBtn;

        levelCompleteEventChannel.OnEventRaised -= ShowLevelCompleteUIAndBtn;
        levelCompleteEventChannel.OnEventRaised -= HideIndividualGameUI;

        allLevelsCompleteEventChannel.OnEventRaised -= ShowAllLevelsCompleteUIAndBtn;

        spawnMomEventChannel.OnEventRaised -= ShowWatchOutPanel;
    }

    private void HideAllUI()
    {
        HideLevelCompleteUIAndBtn();
        HideLevel1LoadUIAndBtn();
        HideLevelStartUIAndBtn();
        HideWatchOutPanel();
        HideAllLevelsCompleteUIAndBtn();
        HideIndividualGameUI();
    }

    private void SetindividualGameTextAndVideo(UIInstruction data)
    {
        ShowIndividualGameUI();
        individualGameTMP.text = data.text;
        individualGameVideoPlayer.clip = data.video;
        individualGameVideoPlayer.Play();
    }

    private void ShowIndividualGameUI()
    {
        individualGameUIParent.SetActive(true);
    }

    private void HideIndividualGameUI()
    {
        individualGameVideoPlayer.Stop();
        individualGameUIParent.SetActive(false);
    }

    private void SetInstructionText(string text)
    {
        instructionTMP.text = text;
    }

    private void ResetInstructionText()
    {
        instructionTMP.text = "";
    }

    private void ShowWatchOutPanel()
    {
        watchOutUI.SetActive(true);
        Invoke("HideWatchOutPanel", watchOutPanelWaitTime);
    }

    private void HideWatchOutPanel()
    {
        watchOutUI.SetActive(false);
    }

    private void ShowLevelStartUIAndBtn()
    {
        instructionTMP.gameObject.SetActive(true);
        levelStartButton.SetActive(true);
    }

    private void HideLevelStartUIAndBtn()
    {
        instructionTMP.gameObject.SetActive(false);
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
    }

    private void HideLevel1LoadUIAndBtn()
    {
        level1LoadButton.SetActive(false);
        level1LoadUI.SetActive(false);
    }

    private void ShowLevelCompleteUIAndBtn()
    {
        levelCompleteUI.SetActive(true);
        nextLevelButton.SetActive(true);
    }

    private void HideLevelCompleteUIAndBtn()
    {
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
