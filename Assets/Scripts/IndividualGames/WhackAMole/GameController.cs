using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace WhackAMole
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private EnvironmentPropData molePropData;
        [SerializeField] private Hammer hammer;
        [SerializeField] private Transform molesParent;

        [SerializeField] private float moveTime; // time for mole to travel up
        [SerializeField] private float waitTime; // time for mole to stay above ground

        [Header("Broadcasting on")]
        [SerializeField] private SpawnObjectsEventChannelSO spawnObjectsEventChannel; // TODO: could just make changes to environment manager?
        [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;

        [Header("Listening to")]
        [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;
        [SerializeField] private VoidEventChannelSO levelLoadEventChannel;
        [SerializeField] private VoidEventChannelSO levelCompleteEventChannel;

        private List<Mole> moles = new List<Mole>(); // all the instantiated moles
        private List<Mole> aliveMoles = new List<Mole>(); // only the alive moles

        private bool gameStarted;
        private bool gameCompleted;
        private float timer;
        private float totalTime; // time interval between poping up moles

        private void Awake()
        {
            levelLoadEventChannel.OnEventRaised += PrepGame; // need to subscribe before level start events are raised
            spawnObjectsEventChannel.OnObjectsSpawned += PrepMoles;
        }

        void Start()
        {
            gameStartEventChannel.OnEventRaised += StartGame;
            levelCompleteEventChannel.OnEventRaised += OnLevelComplete;

            totalTime = 2 * moveTime + waitTime + 0.5f;
        }

        private void OnDestroy()
        {
            gameStartEventChannel.OnEventRaised -= StartGame;
            levelLoadEventChannel.OnEventRaised -= PrepGame;
            levelCompleteEventChannel.OnEventRaised -= OnLevelComplete;
            spawnObjectsEventChannel.OnObjectsSpawned -= PrepMoles;
        }

        void Update()
        {
            if (!gameStarted)
            {
                return;
            }

            if (timer < totalTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                // choose a random alive mole to pop up
                timer = 0;
                int ind = Random.Range(0, aliveMoles.Count);
                aliveMoles[ind].StartPopingUp();
            }
        }

        public void PrepGame()
        {
            gameCompleted = false;
            gameStarted = false;

            // spawn moles
            SpawnData spawnData = new SpawnData
            {
                spawnType = SpawnType.Mole,
                environmentPropData = molePropData,
            };
            spawnObjectsEventChannel.RaiseEvent(spawnData);

            // hide hammer
            hammer.gameObject.SetActive(false);
        }

        private void PrepMoles()
        {
            // set up params
            moles.Clear();
            aliveMoles.Clear();
            Debug.Log("how many moles:");
            Debug.Log(spawnObjectsEventChannel.GetSpawnedObjects(SpawnType.Mole).Count);
            foreach (var moleObject in spawnObjectsEventChannel.GetSpawnedObjects(SpawnType.Mole))
            {
                Mole mole = moleObject.GetComponent<Mole>();
                //if (mole.transform.position.x > 500) // did not find a valid instantiation location
                //{
                //    continue;
                //}
                // set up mole
                mole.SetUpParams(moveTime, waitTime);
                mole.MoveUnderground();
                mole.SetAlive();
                moles.Add(mole);
                mole.gameObject.SetActive(false);
            }
        }

        private void RemoveDeadMole(Mole mole)
        {
            aliveMoles.Remove(mole);
            
            if (aliveMoles.Count == 0)
            {
                CompleteGame();
            }
        }

        private void StartGame(IndividualGameName data)
        {
            if (data != IndividualGameName.WhackAMole)
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
            timer = 100;
            foreach (var mole in moles)
            {
                mole.gameObject.SetActive(true);
                mole.SetAlive(); // if it's restarting the game, need to set it alive again
                mole.Evt_OnMoleDied.AddListener(RemoveDeadMole);
                if (!aliveMoles.Contains(mole))
                {
                    aliveMoles.Add(mole);
                }
            }
            hammer.gameObject.SetActive(true);
            hammer.PutToSpawnLoc();
        }

        private void EndGame()
        {
            gameStarted = false;
            hammer.gameObject.SetActive(false); // hide hammer
            foreach (var mole in moles) // move moles underground and hide them
            {
                mole.MoveUnderground();
                mole.Evt_OnMoleDied.RemoveListener(RemoveDeadMole);
                mole.gameObject.SetActive(false);
            }
        }

        private void CompleteGame()
        {
            gameCompleted = true;
            hammer.gameObject.SetActive(false);
            foreach (var mole in moles)
            {
                mole.Evt_OnMoleDied.RemoveListener(RemoveDeadMole);
            }
            gameCompleteEventChannel.RaiseEvent(IndividualGameName.WhackAMole);
        }

        private void OnLevelComplete()
        {
            foreach (var item in moles)
            {
                Destroy(item.gameObject);
            }
            moles.Clear();
            aliveMoles.Clear();
            // remove the instantiated moles and clear the dict in the SO
            spawnObjectsEventChannel.ClearSpawnedObjects(SpawnType.Mole);
        }
    }
}
