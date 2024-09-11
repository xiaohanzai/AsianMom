using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MusicGame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private MusicKey[] musicKeys;
        [SerializeField] private Drumstick drumstick;
        [SerializeField] private Transform drumstickLoc;
        [SerializeField] private Image musicNotation;

        private float musicStartTime;
        private float musicEndTime;
        private float musicSpeed;

        private Sprite originalNotation;
        private Sprite newNotation;

        [Header("Audios")]
        [SerializeField] private AudioSource musicAudio;
        [SerializeField] private AudioSource wrongAudio;

        [Header("Broadcasting on")]
        [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;

        [Header("Listening to")]
        [SerializeField] private SetMusicEventChannelSO setMusicEventChannel;
        [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;
        [SerializeField] private VoidEventChannelSO levelCompleteEventChannel;

        private List<MusicKeyName> correctSequence = new List<MusicKeyName>();
        private int ind;

        private bool gameStarted;
        private bool gameCompleted;

        private void Start()
        {
            originalNotation = musicNotation.sprite;

            setMusicEventChannel.OnEventRaised += PrepGame;
            gameStartEventChannel.OnEventRaised += StartGame;
            levelCompleteEventChannel.OnEventRaised += OnLevelComplete;

            foreach (var key in musicKeys)
            {
                key.Evt_OnKeyHit.AddListener(CheckSequence);
            }
        }

        private void OnDestroy()
        {
            setMusicEventChannel.OnEventRaised -= PrepGame;
            gameStartEventChannel.OnEventRaised -= StartGame;
            levelCompleteEventChannel.OnEventRaised -= OnLevelComplete;

            foreach (var key in musicKeys)
            {
                key.Evt_OnKeyHit.RemoveListener(CheckSequence);
            }
        }

        private void PrepGame(MusicData data)
        {
            gameCompleted = false;
            gameStarted = false;

            correctSequence = data.sequence;
            musicAudio.clip = data.audioClip;
            newNotation = data.notation;
            musicStartTime = data.timeStart;
            musicEndTime = data.timeEnd;
            musicSpeed = data.playSpeed;

            drumstick.gameObject.SetActive(false);
        }

        private void StartGame(IndividualGameName gameName)
        {
            if (gameName != IndividualGameName.Music)
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

            StartCoroutine(Co_PlayMusicAudio());
            gameStarted = true;
            musicNotation.sprite = newNotation;
            drumstick.gameObject.SetActive(true);
            drumstick.transform.position = drumstickLoc.transform.position;
            drumstick.transform.rotation = drumstickLoc.transform.rotation;
            ind = 0; // always a fresh start; no memory of previous play
        }

        private void EndGame()
        {
            gameStarted = false;
            if (musicAudio.isPlaying) musicAudio.Stop();
            drumstick.gameObject.SetActive(false);
            musicNotation.sprite = originalNotation;
        }

        private void CompleteGame()
        {
            gameCompleted = true;
            //drumstick.gameObject.SetActive(false);
            gameCompleteEventChannel.RaiseEvent(IndividualGameName.Music);
        }

        private IEnumerator Co_PlayMusicAudio()
        {
            musicAudio.pitch = musicSpeed;
            musicAudio.time = musicStartTime;
            musicAudio.Play();
            float timeElapsed = 0f;
            while (timeElapsed < (musicEndTime - musicStartTime) / musicSpeed)
            {
                // If the audio is no longer playing, exit the coroutine early
                if (!musicAudio.isPlaying)
                {
                    yield break;
                }
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            musicAudio.Stop();
        }

        private void CheckSequence(MusicKeyName k)
        {
            if (!gameStarted)
            {
                return;
            }

            if (correctSequence[ind] == k)
            {
                ind++;
            }
            else
            {
                ind = 0;
                wrongAudio.Play();
            }

            if (ind == correctSequence.Count)
            {
                CompleteGame();
            }
        }

        private void OnLevelComplete()
        {
            musicNotation.sprite = originalNotation;
            musicAudio.clip = null;
        }
    }
}
