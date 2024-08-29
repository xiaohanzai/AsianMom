using System.Collections;
using System.Collections.Generic;
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

        private List<Mole> moles = new List<Mole>();

        private bool gameStarted;
        private bool gameCompleted;
        private float timer;
        private float totalTime;

        public void PrepGame()
        {
            // spawn moles
            SpawnData spawnData = new SpawnData
            {
                spawnType = SpawnType.Mole,
                environmentPropData = molePropData,
            };
            spawnObjectsEventChannel.RaiseEvent(spawnData);

            // set up params
            foreach (var mole in moles)
            {
                mole.Evt_OnMoleDied.RemoveListener(RemoveDeadMole);
            }
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
                mole.gameObject.SetActive(true);
            }

            // hide hammer
            hammer.gameObject.SetActive(false);
        }

        private void RemoveDeadMole(Mole mole)
        {
            moles.Remove(mole);

            if (moles.Count == 0)
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
            }
            hammer.gameObject.SetActive(true);
        }

        private void EndGame()
        {
            gameStarted = false;

            foreach (var mole in moles)
            {
                mole.MoveUnderground();
            }
        }

        private void CompleteGame()
        {
            gameCompleted = true;
            gameCompleteEventChannel.RaiseEvent(IndividualGameName.WhackAMole);
        }

        private void Awake()
        {
            levelStartEventChannel.OnEventRaised += PrepGame;
        }

        // Start is called before the first frame update
        void Start()
        {
            gameStartEventChannel.OnEventRaised += StartGame;

            totalTime = 2 * moveTime + waitTime + 0.5f;
        }

        private void OnDestroy()
        {
            gameStartEventChannel.OnEventRaised -= StartGame;
            levelStartEventChannel.OnEventRaised -= PrepGame;

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
                int ind = Random.Range(0, moles.Count);
                moles[ind].StartPopingUp();
            }
        }
    }
}
