using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<LevelData> levelDatas;
    [SerializeField] private List<TabletButtonEventHandler> availableButtons;

    [SerializeField] private Transform propInstantiationLoc;

    private int level = 0;

    [Header("Listening to")]
    [SerializeField] private PokeButtonEventChannelSO pokeButtonEventChannel;
    //[SerializeField] private IndividualGameEventChannelSO gameEndEventChannel;
    [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;

    [Header("Broadcasting on")]
    [SerializeField] private VoidEventChannelSO levelStartEventChannel;
    [SerializeField] private TransformEventChannelSO transformEventChannel;

    private class InternalGameData
    {
        public GameObject go;
        public TabletButtonEventHandler button;
        public string buttonText;
        public string instructionText;
    }
    private Dictionary<IndividualGameName, InternalGameData> gameNameDataPairs = new Dictionary<IndividualGameName, InternalGameData>();

    // Start is called before the first frame update
    void Start()
    {
        pokeButtonEventChannel.OnEventRaised += LoadNewLevelData;
        gameCompleteEventChannel.OnEventRaised += DisableGameButton;
        gameCompleteEventChannel.OnEventRaised += DisableGame;
    }

    private void OnDestroy()
    {
        pokeButtonEventChannel.OnEventRaised -= LoadNewLevelData;
        gameCompleteEventChannel.OnEventRaised -= DisableGameButton;
        gameCompleteEventChannel.OnEventRaised -= DisableGame;
    }

    private void LoadNewLevelData(PokeButtonType data)
    {
        if (data != PokeButtonType.NewLevel)
        {
            return;
        }
        LoadNewLevelData();
    }

    public void LoadNewLevelData()
    {
        ClearSpawnedGames();
        // for debugging
        level = 1;
        //level++;
        //if (level > levelDatas.Count)
        //{
        //    return;
        //}

        // instantiate prefabs
        List<IndividualGameData> gameDataList = levelDatas[level - 1].GetGameDataList();
        for (int i = 0; i < gameDataList.Count; i++)
        {
            gameDataList[i].GetAllFields(out IndividualGameName name, out GameObject prefab, out string btnText, out string instrText);
            InternalGameData d = new InternalGameData
            {
                go = Instantiate(prefab),
                button = availableButtons[i],
                buttonText = btnText,
                instructionText = instrText,
            };
            availableButtons[i].gameObject.SetActive(true);
            availableButtons[i].ResetButton();
            availableButtons[i].SetText(btnText);
            availableButtons[i].LinkToGame(name);
            gameNameDataPairs.Add(name, d);
        }
        availableButtons[gameDataList.Count].SetToggleOn();
        availableButtons[gameDataList.Count].SetText("Cancel");
        availableButtons[gameDataList.Count].gameObject.SetActive(true);
        for (int i = gameDataList.Count + 1; i < availableButtons.Count; i++)
        {
            availableButtons[i].gameObject.SetActive(false);
        }
        levelStartEventChannel.RaiseEvent();
        transformEventChannel.RaiseEvent(propInstantiationLoc);
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
        gameNameDataPairs[data].button.SetDisabled();
    }

    private void DisableGame(IndividualGameName data)
    {
        gameNameDataPairs[data].go.SetActive(false);
    }
}
