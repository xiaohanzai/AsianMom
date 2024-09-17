using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<LevelData> levelDatas; // levelDatas[i] is the LevelData for level i+1
    [SerializeField] private List<TabletButtonEventHandler> availableButtons; // tablet buttons

    [SerializeField] private Transform propInstantiationLoc; // where do you want new props e.g. hammer to appear

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
    [SerializeField] private VoidEventChannelSO allLevelsCompleteEventChannel;
    [SerializeField] private TransformEventChannelSO setPropLocEventChannel;
    [SerializeField] private TextEventChannelSO setInstructionTextEventChannel;
    [SerializeField] private AudioEventChannelSO setMomWalkOutAudioEventChannel;
    [SerializeField] private AudioEventChannelSO setMomAngryAudioEventChannel;
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
        pokeButtonEventChannel.OnEventRaised += PlayAgain;
        pokeButtonEventChannel.OnEventRaised += QuitGame;
        gameCompleteEventChannel.OnEventRaised += DelayedDisableGame;
        checkIndividualGamesEventChannel.OnEventRaised += CheckIfAnyGameIsOn;
    }

    private void OnDestroy()
    {
        pokeButtonEventChannel.OnEventRaised -= LoadNewLevelData;
        pokeButtonEventChannel.OnEventRaised -= StartLevel;
        pokeButtonEventChannel.OnEventRaised -= PlayAgain;
        pokeButtonEventChannel.OnEventRaised += QuitGame;
        gameCompleteEventChannel.OnEventRaised -= DelayedDisableGame;
        checkIndividualGamesEventChannel.OnEventRaised -= CheckIfAnyGameIsOn;
    }

    private void LoadNewLevelData(PokeButtonType type)
    {
        if (type != PokeButtonType.LoadLevel && type != PokeButtonType.TryAgain)
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
                allLevelsCompleteEventChannel.RaiseEvent();
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
        nGame = gameDataList.Count - 1; // last one is pretending to be studying
        for (int i = 0; i < nGame + 1; i++)
        {
            if (i < nGame)
            {
                InternalGameData d = new InternalGameData
                {
                    go = Instantiate(gameDataList[i].gamePrefab),
                    button = availableButtons[i],
                    isCompleted = false,
                };
                // add to dict
                gameNameDataPairs.Add(gameDataList[i].gameName, d);
            }
            // link to tablet buttons
            availableButtons[i].gameObject.SetActive(true);
            TabletButtonInfo td = new TabletButtonInfo
            {
                gameName = gameDataList[i].gameName,
                buttonText = gameDataList[i].buttonText,
                instructionText = gameDataList[i].instructionText,
                instructionVideo = gameDataList[i].instructionVideo,
                audio = gameDataList[i].audio,
            };
            availableButtons[i].SetUpButton(td);
        }
        // rest of the buttons disabled
        for (int i = nGame + 1; i < availableButtons.Count; i++)
        {
            availableButtons[i].gameObject.SetActive(false);
        }
        // disable buttons before level start
        for (int i = 0; i < nGame + 1; i++)
        {
            availableButtons[i].SetDisabled();
        }
        // move instantiated prop to the desired location, set instructions text, mom walk-out audio
        setPropLocEventChannel.RaiseEvent(propInstantiationLoc);
        setInstructionTextEventChannel.RaiseEvent(levelDatas[level - 1].instructionText);
        setMomWalkOutAudioEventChannel.RaiseEvent(levelDatas[level - 1].momWalkOutAudio);
        setMomAngryAudioEventChannel.RaiseEvent(levelDatas[level - 1].momAngryAudio);
        levelLoadEventChannel.RaiseEvent();
    }

    private void PlayAgain(PokeButtonType type)
    {
        if (type != PokeButtonType.PlayAgain) return;

        level = 1;
        isLevelComplete = true;
        ClearSpawnedGames();
        foreach (var item in availableButtons)
        {
            item.ResetButton();
        }
    }

    private void QuitGame(PokeButtonType type)
    {
        if (type != PokeButtonType.Quit) return;
        Application.Quit();
    }

    private void StartLevel(PokeButtonType type)
    {
        if (type != PokeButtonType.StartLevel) return;

        for (int i = 0; i < nGame + 1; i++)
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
            gameNameDataPairs[key].button.ResetButton();
        }
        gameNameDataPairs.Clear();
    }

    private void DisableGameButton(IndividualGameName data)
    {
        // disable tablet button; called when an individual game is complete
        availableButtons[nGame].SetToggleOn();
        gameNameDataPairs[data].button.SetDisabled();
    }

    private void DelayedDisableGame(IndividualGameName data)
    {
        StartCoroutine(Co_DisableGame(data));
    }

    private IEnumerator Co_DisableGame(IndividualGameName data)
    {
        yield return new WaitForSeconds(1);

        // disable the instantiated gameobjects of an individual game; called when an individual game is complete
        gameNameDataPairs[data].isCompleted = true;
        gameNameDataPairs[data].go.SetActive(false);

        DisableGameButton(data);

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

    private void CheckIfAnyGameIsOn(bool b)
    {
        if (isLevelComplete) return;
        // if b == false, don't check just fail this level
        // only need to check if the cancel button is toggled on
        if (!b || !availableButtons[nGame].CheckIfToggleOn())
        {
            ClearSpawnedGames();
            foreach (var item in availableButtons)
            {
                item.ResetButton();
            }
            levelFailedEventChannel.RaiseEvent();
        }
    }
}
