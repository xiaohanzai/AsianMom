using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class DeskUIManager : MonoBehaviour
{
    [SerializeField] private float timeToEnableButton = 0.5f;

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
    [SerializeField] private PokeButtonEventHandler level1LoadButton;
    [SerializeField] private PokeButtonEventHandler nextLevelButton;

    [Header("Level Start UI")]
    [SerializeField] private PokeButtonEventHandler levelStartButton;

    [Header("Level Complete UI")]
    [SerializeField] private GameObject levelCompleteUI;

    [Header("Game Complete UI")]
    [SerializeField] private GameObject gameCompleteUI;
    [SerializeField] private PokeButtonEventHandler quitButton;
    [SerializeField] private PokeButtonEventHandler playAgainButton;

    [Header("Listening to")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;
    [SerializeField] private TextEventChannelSO setInstructionTextEventChannel;
    [SerializeField] private UIInstructionEventChannelSO uIInstructionEventChannel;
    [SerializeField] private LevelEventChannelSO levelEventChannel;
    [SerializeField] private VoidEventChannelSO allLevelsCompleteEventChannel;
    [SerializeField] private VoidEventChannelSO spawnMomEventChannel;

    private void Start()
    {
        HideAllUI();
        ResetInstructionText();

        pokeButtonEventChannel.OnEventRaised += OnPokeButtonEventRaised;

        setInstructionTextEventChannel.OnEventRaised += SetInstructionText;
        uIInstructionEventChannel.OnEventRaised += SetindividualGameTextAndVideo;

        levelEventChannel.OnEventRaised += OnLevelEventRaised;

        allLevelsCompleteEventChannel.OnEventRaised += ShowAllLevelsCompleteUIAndBtn;

        spawnMomEventChannel.OnEventRaised += ShowWatchOutPanel;
    }

    private void OnDestroy()
    {
        pokeButtonEventChannel.OnEventRaised -= OnPokeButtonEventRaised;

        setInstructionTextEventChannel.OnEventRaised -= SetInstructionText;
        uIInstructionEventChannel.OnEventRaised -= SetindividualGameTextAndVideo;

        levelEventChannel.OnEventRaised -= OnLevelEventRaised;

        allLevelsCompleteEventChannel.OnEventRaised -= ShowAllLevelsCompleteUIAndBtn;

        spawnMomEventChannel.OnEventRaised -= ShowWatchOutPanel;
    }

    private void OnLevelEventRaised(LevelEventInfo data)
    {
        if(data.type == LevelEventType.LevelLoad)
        {
            HideLevel1LoadUIAndBtn();
            HideLevelCompleteUIAndBtn();
            HideIndividualGameUI();
            ShowLevelStartUIAndBtn();
        }
        else if (data.type == LevelEventType.LevelStart)
        {
            HideLevelStartUIAndBtn();
        }
        else if (data.type == LevelEventType.LevelComplete)
        {
            HideIndividualGameUI();
            ShowLevelCompleteUIAndBtn();
        }
    }

    private void OnPokeButtonEventRaised(PokeButtonType type)
    {
        if (type == PokeButtonType.ConfirmEnvironment) ShowLevel1LoadUIAndBtn();
        else if (type == PokeButtonType.PlayAgain)
        {
            HideAllLevelsCompleteUIAndBtn();
            ShowLevel1LoadUIAndBtn();
        }
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
        levelStartButton.gameObject.SetActive(true);
        levelStartButton.SetUpTimer(timeToEnableButton);
    }

    private void HideLevelStartUIAndBtn()
    {
        instructionTMP.gameObject.SetActive(false);
        levelStartButton.gameObject.SetActive(false);
    }

    private void ShowLevel1LoadUIAndBtn()
    {
        level1LoadButton.gameObject.SetActive(true);
        level1LoadButton.SetUpTimer(timeToEnableButton);
        level1LoadUI.SetActive(true);
    }

    private void HideLevel1LoadUIAndBtn()
    {
        level1LoadButton.gameObject.SetActive(false);
        level1LoadUI.SetActive(false);
    }

    private void ShowLevelCompleteUIAndBtn()
    {
        levelCompleteUI.SetActive(true);
        nextLevelButton.gameObject.SetActive(true);
        nextLevelButton.SetUpTimer(timeToEnableButton);
    }

    private void HideLevelCompleteUIAndBtn()
    {
        levelCompleteUI.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
    }

    private void ShowAllLevelsCompleteUIAndBtn()
    {
        HideLevelCompleteUIAndBtn();
        instructionTMP.gameObject.SetActive(false);
        gameCompleteUI.SetActive(true);
        quitButton.gameObject.SetActive(true);
        quitButton.SetUpTimer(timeToEnableButton);
        playAgainButton.gameObject.SetActive(true);
        playAgainButton.SetUpTimer(timeToEnableButton);
    }

    private void HideAllLevelsCompleteUIAndBtn()
    {
        gameCompleteUI.SetActive(false);
        quitButton.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
    }
}
