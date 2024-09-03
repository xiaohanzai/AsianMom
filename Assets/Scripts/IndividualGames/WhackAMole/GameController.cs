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

        [SerializeField] private float moveTime;
        [SerializeField] private float waitTime;

        [Header("Broadcasting on")]
        [SerializeField] private SpawnObjectsEventChannelSO spawnObjectsEventChannel;
        //[SerializeField] private IndividualGameEventChannelSO gameEndEventChannel;
        [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;

        [Header("Listening to")]
        [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;
        [SerializeField] private VoidEventChannelSO levelStartEventChannel;
        [SerializeField] private VoidEventChannelSO levelCompleteEventChannel;

        private List<Mole> moles = new List<Mole>();
        private List<Mole> aliveMoles = new List<Mole>();

        private bool gameStarted;
        private bool gameCompleted;
        private float timer;
        private float totalTime;

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

            // set up params
            moles.Clear();
            foreach (var moleObject in spawnObjectsEventChannel.GetSpawnedObjects(SpawnType.Mole))
            {
                Mole mole = moleObject.GetComponent<Mole>();
                if (mole.transform.position.x > 500)
                {
                    continue;
                }
                mole.SetUpParams(moveTime, waitTime);
                mole.MoveUnderground();
                mole.SetAlive();
                mole.Evt_OnMoleDied.AddListener(RemoveDeadMole);
                moles.Add(mole);
                mole.gameObject.SetActive(false);
            }

            // hide hammer
            hammer.gameObject.SetActive(false);
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
                EndGame();
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
                mole.SetAlive();
                if (!aliveMoles.Contains(mole))
                {
                    aliveMoles.Add(mole);
                }
            }
            hammer.gameObject.SetActive(true);
        }

        private void EndGame()
        {
            gameStarted = false;
            hammer.gameObject.SetActive(false);
            foreach (var mole in moles)
            {
                mole.MoveUnderground();
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
        }

        private void Awake()
        {
            levelStartEventChannel.OnEventRaised += PrepGame;
        }

        // Start is called before the first frame update
        void Start()
        {
            gameStartEventChannel.OnEventRaised += StartGame;
            levelCompleteEventChannel.OnEventRaised += OnLevelComplete;

            totalTime = 2 * moveTime + waitTime + 0.5f;
        }

        private void OnDestroy()
        {
            gameStartEventChannel.OnEventRaised -= StartGame;
            levelStartEventChannel.OnEventRaised -= PrepGame;
            levelCompleteEventChannel.OnEventRaised -= OnLevelComplete;

            foreach (var mole in moles)
            {
                mole.Evt_OnMoleDied.RemoveListener(RemoveDeadMole);
            }
        }

        // Update is called once per frame
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
                timer = 0;
                int ind = Random.Range(0, aliveMoles.Count);
                aliveMoles[ind].StartPopingUp();
            }
        }
    }
}
