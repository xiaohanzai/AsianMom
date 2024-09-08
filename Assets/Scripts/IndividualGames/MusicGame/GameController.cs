using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MusicGame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private MusicKey[] musicKeys;
        [SerializeField] private Drumstick drumstick;
        [SerializeField] private Transform drumstickLoc;
        [SerializeField] private TextMeshProUGUI musicText;

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
            drumstick.gameObject.SetActive(false);
            SetText();
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

            musicAudio.Play();
            gameStarted = true;
            drumstick.gameObject.SetActive(true);
            drumstick.transform.position = drumstickLoc.transform.position;
            drumstick.transform.rotation = drumstickLoc.transform.rotation;
            ind = 0; // always a fresh start; no memory of previous play
        }

        private void EndGame()
        {
            gameStarted = false;
            musicAudio.Stop();
            drumstick.gameObject.SetActive(false);
        }

        private void CompleteGame()
        {
            gameCompleted = true;
            drumstick.gameObject.SetActive(false);
            gameCompleteEventChannel.RaiseEvent(IndividualGameName.Music);
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

        private void SetText()
        {
            string text = "Play sequence:\n";
            foreach (var item in correctSequence)
            {
                text += item.ToString() + " ";
            }
            musicText.text = text;
        }

        private void OnLevelComplete()
        {
            musicText.text = "";
            correctSequence.Clear();
            musicAudio.clip = null;
        }
    }
}
