using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatchLadybug
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private EnvironmentPropData ladybugPropData;
        [SerializeField] private Basket basket;

        [SerializeField] private float waitTime;

        [SerializeField] private float percentage = 0.5f;

        [Header("Broadcasting on")]
        [SerializeField] private SpawnObjectsEventChannelSO spawnObjectsEventChannel;
        [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;

        [Header("Listening to")]
        [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;
        [SerializeField] private LevelEventChannelSO levelEventChannel;

        private List<Ladybug> ladybugs = new List<Ladybug>(); // all the instantiated flies
        private List<Ladybug> aliveLadybugs = new List<Ladybug>(); // only the alive flies

        private bool gameStarted;
        private bool gameCompleted;

        private void Awake()
        {
            levelEventChannel.OnEventRaised += OnLevelEventRaised; // need to subscribe before level start events are raised
            spawnObjectsEventChannel.OnObjectsSpawned += PrepLadybugs;
        }

        void Start()
        {
            gameStartEventChannel.OnEventRaised += StartGame;
        }

        private void OnDestroy()
        {
            gameStartEventChannel.OnEventRaised -= StartGame;
            levelEventChannel.OnEventRaised -= OnLevelEventRaised;
            spawnObjectsEventChannel.OnObjectsSpawned -= PrepLadybugs;
        }

        private void OnLevelEventRaised(LevelEventInfo data)
        {
            if (data.type == LevelEventType.LevelLoad) PrepGame();
            else if (data.type == LevelEventType.LevelComplete) OnLevelComplete();
        }

        public void PrepGame()
        {
            gameCompleted = false;
            gameStarted = false;

            // spawn flies
            SpawnData spawnData = new SpawnData
            {
                spawnType = SpawnType.Ladybug,
                environmentPropData = ladybugPropData,
            };
            spawnObjectsEventChannel.RaiseEvent(spawnData);

            // hide spray bottle
            basket.gameObject.SetActive(false);
        }

        private void PrepLadybugs()
        {
            // set up params
            ladybugs.Clear();
            aliveLadybugs.Clear();
            Debug.Log("how many ladybugs:");
            Debug.Log(spawnObjectsEventChannel.GetSpawnedObjects(SpawnType.Ladybug).Count);
            foreach (var ladybugObject in spawnObjectsEventChannel.GetSpawnedObjects(SpawnType.Ladybug))
            {
                Ladybug ladybug = ladybugObject.GetComponent<Ladybug>();
                // set up ladybug
                ladybug.SetUpParams(waitTime);
                ladybug.SetAlive();
                ladybugs.Add(ladybug);
                ladybug.gameObject.SetActive(false);
            }
        }

        private void RemoveDeadLadybug(Ladybug ladybug)
        {
            aliveLadybugs.Remove(ladybug);

            if (aliveLadybugs.Count < percentage * ladybugs.Count) // catch some percentage of all ladybugs to win
            {
                CompleteGame();
            }
        }

        private void StartGame(IndividualGameName data)
        {
            if (data != IndividualGameName.CatchLadybug)
            {
                if (gameStarted) EndGame();
                return;
            }

            StartGame();
        }

        public void StartGame()
        {
            if (gameCompleted)
            {
                return;
            }
            gameStarted = true;
            foreach (var ladybug in ladybugs)
            {
                ladybug.gameObject.SetActive(true);
                ladybug.SetAlive(); // if it's restarting the game, need to set it alive again
                ladybug.SetRandomStartingPoint();
                ladybug.Evt_OnLadybugCaught.AddListener(RemoveDeadLadybug);
                if (!aliveLadybugs.Contains(ladybug))
                {
                    aliveLadybugs.Add(ladybug);
                }
            }
            basket.gameObject.SetActive(true);
            basket.PutToSpawnLoc();
        }

        private void EndGame()
        {
            gameStarted = false;
            basket.gameObject.SetActive(false);
            foreach (var ladybug in ladybugs)
            {
                ladybug.Evt_OnLadybugCaught.RemoveListener(RemoveDeadLadybug);
                ladybug.gameObject.SetActive(false);
            }
        }

        private void CompleteGame()
        {
            gameCompleted = true;
            //basket.gameObject.SetActive(false);
            foreach (var ladybug in ladybugs)
            {
                ladybug.Evt_OnLadybugCaught.RemoveListener(RemoveDeadLadybug);
            }
            gameCompleteEventChannel.RaiseEvent(IndividualGameName.CatchLadybug);
        }

        private void OnLevelComplete()
        {
            foreach (var item in ladybugs)
            {
                Destroy(item.gameObject);
            }
            ladybugs.Clear();
            aliveLadybugs.Clear();
            // remove the instantiated flies and clear the dict in the SO
            spawnObjectsEventChannel.ClearSpawnedObjects(SpawnType.Ladybug);
        }
    }
}