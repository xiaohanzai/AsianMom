using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CatchLadybug
{
    public class Ladybug : MonoBehaviour
    {
        [SerializeField] private GameObject ladybugObject;
        [SerializeField] private Rigidbody rb;

        [SerializeField] private ParticleSystem getCaughtParticles;

        [SerializeField] private AudioSource fallingAudio;
        [SerializeField] private AudioSource getCaughtAudio;

        private enum MoveState
        {
            Falling,
            Waiting,
        }

        private float waitTime;

        private float timer;
        private float timer0;

        private bool isAlive;
        private bool isInvoked; // in case it bounces or hit objects multiple times

        private MoveState moveState = MoveState.Waiting;

        private Vector3 origin;

        public UnityEvent<Ladybug> Evt_OnLadybugCaught = new UnityEvent<Ladybug>();

        void Awake()
        {
            origin = transform.position;
        }

        void Update()
        {
            if (!isAlive)
            {
                return;
            }

            if (moveState == MoveState.Waiting)
            {
                Wait();
            }
            else
            {
                if ((transform.position.y < 0 || (timer0 > 0.5f && rb.velocity.magnitude < 0.01f)) && !isInvoked)
                {
                    isInvoked = true;
                    Invoke("SendBackToOrigin", 0.5f);
                }

                if (!isInvoked)
                {
                    timer0 += Time.deltaTime;
                }
            }
        }

        public void SendBackToOrigin()
        {
            transform.position = origin;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            moveState = MoveState.Waiting;
            timer = 0;
            isInvoked = false;
            timer0 = 0;
        }

        public void SetUpParams(float waitTime)
        {
            this.waitTime = waitTime;
            transform.localRotation *= Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        public void SetRandomStartingPoint()
        {
            SendBackToOrigin();
            timer = Random.Range(0, waitTime) - waitTime / 2;
        }

        public void SetAlive()
        {
            isAlive = true;
        }

        public void SetDead()
        {
            isAlive = false;
            ladybugObject.SetActive(false);
            fallingAudio.Stop();
            getCaughtParticles.Play();
            getCaughtAudio.Play();
            Evt_OnLadybugCaught.Invoke(this);
            isInvoked = true;
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

        private void Wait()
        {
            if (timer < waitTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                moveState = MoveState.Falling;
                rb.constraints = RigidbodyConstraints.None;
                fallingAudio.Play();
            }
        }
    }
}