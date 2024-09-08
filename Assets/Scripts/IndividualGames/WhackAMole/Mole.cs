using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WhackAMole
{
    public class Mole : MonoBehaviour
    {
        private enum PopState
        {
            Hidden,
            MoveUp,
            Wait,
            MoveDown,
        }

        [SerializeField] private float distFromGround; // positive number
        [SerializeField] private GameObject moleObject;
        [SerializeField] private MoleCollider moleCollider;

        [SerializeField] private ParticleSystem getKilledParticles;

        [SerializeField] private AudioSource popUpAudio;
        [SerializeField] private AudioSource getKilledAudio;

        private float moveTime;
        private float waitTime;
        private float timer;

        private Vector3 popPos; // above ground position
        private Vector3 originalPos; // underground position

        private bool isAlive;

        private PopState popState = PopState.Hidden;

        public UnityEvent<Mole> Evt_OnMoleDied = new UnityEvent<Mole>();

        // Start is called before the first frame update
        void Start()
        {
            MoveUnderground();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isAlive)
            {
                return;
            }

            if (popState == PopState.MoveUp)
            {
                MoveUp();
            }
            else if (popState == PopState.Wait)
            {
                Wait();
            }
            else if (popState == PopState.MoveDown)
            {
                MoveDown();
            }
        }

        public void SetUpParams(float moveTime, float waitTime)
        {
            this.moveTime = moveTime;
            this.waitTime = waitTime;
        }

        public void StartPopingUp()
        {
            popState = PopState.MoveUp;
            popUpAudio.Play();
            moleCollider.gameObject.SetActive(true);
            timer = 0;
        }

        public void SetAlive()
        {
            isAlive = true;
        }

        public void SetDead()
        {
            isAlive = false;
            moleObject.SetActive(false);
            moleCollider.gameObject.SetActive(false);
            getKilledParticles.Play();
            getKilledAudio.Play();
            Evt_OnMoleDied.Invoke(this);
            Invoke("DisableSelf", 1f);
        }

        private void DisableSelf()
        {
            gameObject.SetActive(false);
        }

        public bool IsAlive()
        {
            return isAlive;
        }

        public void MoveUnderground()
        {
            popState = PopState.Hidden;

            transform.position = new Vector3(transform.position.x, -distFromGround, transform.position.z);
            transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            popPos = new Vector3(transform.position.x, 0, transform.position.z);
            originalPos = transform.position;

            moleObject.transform.position = originalPos;

            moleObject.SetActive(true);
            moleCollider.gameObject.SetActive(false);
        }

        private void MoveUp()
        {
            moleObject.transform.position = Vector3.Lerp(originalPos, popPos, timer / moveTime);
            timer += Time.deltaTime;
            if (Mathf.Approximately(moleObject.transform.position.y, popPos.y))
            {
                popState = PopState.Wait;
                timer = waitTime;
            }
        }

        private void Wait()
        {
            if (timer < moveTime + waitTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                popState = PopState.MoveDown;
            }
        }

        private void MoveDown()
        {
            moleObject.transform.position = Vector3.Lerp(popPos, originalPos, (timer - moveTime - waitTime) / moveTime);
            timer += Time.deltaTime;
            if (Mathf.Approximately(moleObject.transform.position.y, originalPos.y))
            {
                popState = PopState.Hidden;
            }
        }
    }
}