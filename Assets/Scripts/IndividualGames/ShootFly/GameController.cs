using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootFly
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private EnvironmentPropData flyPropData;
        [SerializeField] private SprayBottle sprayBottle;

        [SerializeField] private float moveRange;
        [SerializeField] private float waitTime;
        [SerializeField] private float moveVelocity;

        [Header("Broadcasting on")]
        [SerializeField] private SpawnObjectsEventChannelSO spawnObjectsEventChannel;
        [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;

        [Header("Listening to")]
        [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;
        [SerializeField] private VoidEventChannelSO levelLoadEventChannel;
        [SerializeField] private VoidEventChannelSO levelCompleteEventChannel;

        private List<Fly> flies = new List<Fly>(); // all the instantiated flies
        private List<Fly> aliveFlies = new List<Fly>(); // only the alive flies

        private bool gameStarted;
        private bool gameCompleted;

        private void Awake()
        {
            levelLoadEventChannel.OnEventRaised += PrepGame; // need to subscribe before level start events are raised
            spawnObjectsEventChannel.OnObjectsSpawned += PrepFlies;
        }

        void Start()
        {
            gameStartEventChannel.OnEventRaised += StartGame;
            levelCompleteEventChannel.OnEventRaised += OnLevelComplete;
        }

        private void OnDestroy()
        {
            gameStartEventChannel.OnEventRaised -= StartGame;
            levelLoadEventChannel.OnEventRaised -= PrepGame;
            levelCompleteEventChannel.OnEventRaised -= OnLevelComplete;
            spawnObjectsEventChannel.OnObjectsSpawned -= PrepFlies;
        }

        void Update()
        {
            
        }

        public void PrepGame()
        {
            gameCompleted = false;
            gameStarted = false;

            // spawn flies
            SpawnData spawnData = new SpawnData
            {
                spawnType = SpawnType.Fly,
                environmentPropData = flyPropData,
            };
            spawnObjectsEventChannel.RaiseEvent(spawnData);

            // hide spray bottle
            sprayBottle.gameObject.SetActive(false);
        }

        private void PrepFlies()
        {
            // set up params
            flies.Clear();
            aliveFlies.Clear();
            Debug.Log("how many flies:");
            Debug.Log(spawnObjectsEventChannel.GetSpawnedObjects(SpawnType.Fly).Count);
            foreach (var flyObject in spawnObjectsEventChannel.GetSpawnedObjects(SpawnType.Fly))
            {
                Fly fly = flyObject.GetComponent<Fly>();
                // set up fly
                fly.SetUpParams(moveRange, waitTime, moveVelocity);
                fly.SetAlive();
                flies.Add(fly);
                fly.gameObject.SetActive(false);
            }
        }

        private void RemoveDeadFly(Fly fly)
        {
            aliveFlies.Remove(fly);

            if (aliveFlies.Count == 0)
            {
                CompleteGame();
            }
        }

        private void StartGame(IndividualGameName data)
        {
            if (data != IndividualGameName.ShootFly)
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
            foreach (var fly in flies)
            {
                fly.gameObject.SetActive(true);
                fly.SetAlive(); // if it's restarting the game, need to set it alive again
                fly.SetRandomPosition();
                fly.Evt_OnFlyDied.AddListener(RemoveDeadFly);
                if (!aliveFlies.Contains(fly))
                {
                    aliveFlies.Add(fly);
                }
            }
            sprayBottle.gameObject.SetActive(true);
            sprayBottle.PutToSpawnLoc();
        }

        private void EndGame()
        {
            gameStarted = false;
            sprayBottle.gameObject.SetActive(false);
            foreach (var fly in flies)
            {
                fly.Evt_OnFlyDied.RemoveListener(RemoveDeadFly);
                fly.gameObject.SetActive(false);
            }
        }

        private void CompleteGame()
        {
            gameCompleted = true;
            //sprayBottle.gameObject.SetActive(false);
            foreach (var fly in flies)
            {
                fly.Evt_OnFlyDied.RemoveListener(RemoveDeadFly);
            }
            gameCompleteEventChannel.RaiseEvent(IndividualGameName.ShootFly);
        }

        private void OnLevelComplete()
        {
            foreach (var item in flies)
            {
                Destroy(item.gameObject);
            }
            flies.Clear();
            aliveFlies.Clear();
            // remove the instantiated flies and clear the dict in the SO
            spawnObjectsEventChannel.ClearSpawnedObjects(SpawnType.Fly);
        }
    }
}