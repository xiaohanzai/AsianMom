using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShootFly
{
    public class Fly : MonoBehaviour
    {
        [SerializeField] private GameObject flyObject;

        [SerializeField] private ParticleSystem getKilledParticles;

        [SerializeField] private AudioSource aliveAudio;
        [SerializeField] private AudioSource getKilledAudio;

        private enum MoveState
        {
            MoveUp,
            WaitTop,
            MoveDown,
            WaitBottom
        }

        private float moveVelocity;
        private float waitTime;

        private float timer;

        private bool isAlive;

        private MoveState moveState = MoveState.WaitTop;

        private Vector3 bottomPos;
        private Vector3 topPos;

        public UnityEvent<Fly> Evt_OnFlyDied = new UnityEvent<Fly>();

        // Start is called before the first frame update
        void Awake()
        {
            topPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isAlive)
            {
                return;
            }

            if (moveState == MoveState.MoveUp)
            {
                MoveUp();
            }
            else if (moveState == MoveState.WaitTop || moveState == MoveState.WaitBottom)
            {
                Wait();
            }
            else if (moveState == MoveState.MoveDown)
            {
                MoveDown();
            }
        }

        public void SetUpParams(float moveRange, float waitTime, float moveVelocity)
        {
            this.waitTime = waitTime;
            this.moveVelocity = moveVelocity;

            bottomPos = topPos + Vector3.down * moveRange;

            transform.localRotation *= Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        public void SetRandomPosition()
        {
            transform.position = new Vector3(topPos.x, Random.Range(bottomPos.y, topPos.y), topPos.z);
            float tmp = Random.Range(-1, 2);
            if (tmp > 0) moveState = MoveState.MoveUp;
            else moveState = MoveState.MoveDown;
            //Debug.Log(transform.position.y.ToString() + moveState.ToString());
        }

        public void SetAlive()
        {
            isAlive = true;
        }

        public void SetDead()
        {
            isAlive = false;
            flyObject.SetActive(false);
            aliveAudio.Stop();
            getKilledParticles.Play();
            getKilledAudio.Play();
            Evt_OnFlyDied.Invoke(this);
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

        private void MoveUp()
        {
            transform.position += moveVelocity * Vector3.up * Time.deltaTime;
            if (transform.position.y > topPos.y)
            {
                moveState = MoveState.WaitTop;
                timer = 0;
                aliveAudio.Play();
            }
        }

        private void Wait()
        {
            if (timer < waitTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                if (moveState == MoveState.WaitTop) moveState = MoveState.MoveDown;
                else moveState = MoveState.MoveUp;
                aliveAudio.Stop();
            }
        }

        private void MoveDown()
        {
            transform.position += moveVelocity * Vector3.down * Time.deltaTime;
            if (transform.position.y < bottomPos.y)
            {
                moveState = MoveState.WaitBottom;
                timer = 0;
                aliveAudio.Play();
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.tag == "FlySprayParticles") SetDead();
        }
    }
}