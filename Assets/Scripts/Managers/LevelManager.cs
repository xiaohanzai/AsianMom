using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<LevelData> levelDatas; // levelDatas[i] is the LevelData for level i+1
    [SerializeField] private List<TabletButtonEventHandler> availableButtons; // tablet buttons

    [SerializeField] private Transform propInstantiationLoc; // where do you want new props e.g. hammer to appear

    [Header("Audios")]
    [SerializeField] private AudioSource successAudio; // when an individual game is complete play success audio
    [SerializeField] private AudioSource failedAudio; // when discovered by mom play failed audio

    private int level = 1;
    private int nGame;
    private bool isLevelComplete;

    [Header("Listening to")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;
    [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;
    [SerializeField] private BoolEventChannelSO checkIndividualGamesEventChannel;

    [Header("Broadcasting on")]
    [SerializeField] private VoidEventChannelSO levelLoadEventChannel;
    [SerializeField] private VoidEventChannelSO levelStartEventChannel;
    [SerializeField] private VoidEventChannelSO levelFailedEventChannel;
    [SerializeField] private VoidEventChannelSO levelCompleteEventChannel;
    [SerializeField] private TransformEventChannelSO setPropLocEventChannel;
    [SerializeField] private TextEventChannelSO setInstructionTextEventChannel;
    [SerializeField] private SpawnMomParametersEventChannelSO setSpawnMomParametersEventChannel;

    private class InternalGameData
    {
        public GameObject go;
        public TabletButtonEventHandler button;
        public bool isCompleted;
    }
    private Dictionary<IndividualGameName, InternalGameData> gameNameDataPairs = new Dictionary<IndividualGameName, InternalGameData>();

    void Start()
    {
        pokeButtonEventChannel.OnEventRaised += LoadNewLevelData;
        pokeButtonEventChannel.OnEventRaised += StartLevel;
        gameCompleteEventChannel.OnEventRaised += DisableGameButton;
        gameCompleteEventChannel.OnEventRaised += DisableGame;
        gameCompleteEventChannel.OnEventRaised += PlaySuccessAudio;
        checkIndividualGamesEventChannel.OnEventRaised += CheckIfAnyGameIsOn;
    }

    private void OnDestroy()
    {
        pokeButtonEventChannel.OnEventRaised -= LoadNewLevelData;
        pokeButtonEventChannel.OnEventRaised -= StartLevel;
        gameCompleteEventChannel.OnEventRaised -= DisableGameButton;
        gameCompleteEventChannel.OnEventRaised -= DisableGame;
        gameCompleteEventChannel.OnEventRaised -= PlaySuccessAudio;
        checkIndividualGamesEventChannel.OnEventRaised -= CheckIfAnyGameIsOn;
    }

    private void LoadNewLevelData(PokeButtonType type)
    {
        if (type != PokeButtonType.LoadLevel)
        {
            return;
        }
        LoadNewLevelData();
    }

    public void ClearLevel()
    {
        // clear data and reset tablet buttons if level complete
        isLevelComplete = true;
        levelCompleteEventChannel.RaiseEvent();
        ClearSpawnedGames();
        foreach (var item in availableButtons)
        {
            item.ResetButton();
        }
    }

    public void LoadNewLevelData()
    {
        ClearSpawnedGames();

        // this function is used when restarting a level as well; so check if level complete first
        if (isLevelComplete)
        {
            isLevelComplete = false;
            level++;
            if (level > levelDatas.Count)
            {
                return;
            }
        }

        // set time intervals
        SpawnMomParameters pars = new SpawnMomParameters
        {
            timeIntervalMax = levelDatas[level - 1].timeIntervalMax,
            timeIntervalMin = levelDatas[level - 1].timeIntervalMin,
            nRounds = levelDatas[level - 1].nRounds,
            waitTime = levelDatas[level - 1].waitTime,
        };
        setSpawnMomParametersEventChannel.RaiseEvent(pars);

        // instantiate prefabs
        List<IndividualGameData> gameDataList = levelDatas[level - 1].GetGameDataList();
        nGame = gameDataList.Count;
        for (int i = 0; i < nGame; i++)
        {
            gameDataList[i].GetAllFields(out IndividualGameName name, out GameObject prefab, out string btnText, out string instrText);
            InternalGameData d = new InternalGameData
            {
                go = Instantiate(prefab),
                button = availableButtons[i],
                isCompleted = false,
            };
            // link to tablet buttons
            availableButtons[i].gameObject.SetActive(true);
            availableButtons[i].ResetButton();
            availableButtons[i].SetButtonText(btnText);
            availableButtons[i].SetInstructionText(instrText);
            availableButtons[i].LinkToGame(name);
            // add to dict
            gameNameDataPairs.Add(name, d);
        }
        // set the cancel button
        availableButtons[nGame].SetToggleOn();
        availableButtons[nGame].SetButtonText("Stop Playing");
        availableButtons[nGame].SetInstructionText("Now pretend you are studying!");
        availableButtons[nGame].gameObject.SetActive(true);
        // rest of the buttons disabled
        for (int i = nGame + 1; i < availableButtons.Count; i++)
        {
            availableButtons[i].gameObject.SetActive(false);
        }
        // disable buttons before level start
        for (int i = 0; i < nGame; i++)
        {
            availableButtons[i].SetDisabled();
        }
        // move instantiated prop to the desired location, set instructions text
        setPropLocEventChannel.RaiseEvent(propInstantiationLoc);
        setInstructionTextEventChannel.RaiseEvent(levelDatas[level - 1].instructionText);
        levelLoadEventChannel.RaiseEvent();
    }

    private void StartLevel(PokeButtonType type)
    {
        if (type != PokeButtonType.StartLevel) return;

        for (int i = 0; i < nGame; i++)
        {
            availableButtons[i].SetEnabled();
        }
        availableButtons[nGame].SetToggleOn();

        levelStartEventChannel.RaiseEvent();
    }

    private void ClearSpawnedGames()
    {
        foreach (var key in gameNameDataPairs.Keys)
        {
            Destroy(gameNameDataPairs[key].go);
            gameNameDataPairs[key].button.UnlinkToGame();
        }
        gameNameDataPairs.Clear();
    }

    private void DisableGameButton(IndividualGameName data)
    {
        // disable tablet button; called when an individual game is complete
        gameNameDataPairs[data].button.SetDisabled();
        availableButtons[nGame].SetToggleOn();
    }

    private void DisableGame(IndividualGameName data)
    {
        // disable the instantiated gameobjects of an individual game; called when an individual game is complete
        gameNameDataPairs[data].isCompleted = true;
        gameNameDataPairs[data].go.SetActive(false);

        // check whether level is complete
        int sum = 0;
        foreach (var item in gameNameDataPairs.Keys)
        {
            if (gameNameDataPairs[item].isCompleted)
            {
                sum += 1;
            }
        }
        if (sum == gameNameDataPairs.Count)
        {
            ClearLevel();
        }
    }

    private void PlaySuccessAudio(IndividualGameName data)
    {
        successAudio.Play();
    }

    private void PlayFailedAudio()
    {
        failedAudio.Play();
    }

    private void CheckIfAnyGameIsOn(bool b)
    {
        // if b == false, don't check just fail this level
        // only need to check if the cancel button is toggled on
        if (!b || !availableButtons[nGame].CheckIfToggleOn())
        {
            Invoke("PlayFailedAudio", 1.5f);
            levelFailedEventChannel.RaiseEvent();
        }
    }
}
