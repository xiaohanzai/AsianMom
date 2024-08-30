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

        [SerializeField] private TextMeshProUGUI musicText;

        [Header("Audios")]
        [SerializeField] private AudioSource musicAudio;
        [SerializeField] private AudioSource successAudio;
        [SerializeField] private AudioSource wrongAudio;

        [Header("Broadcasting on")]
        [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;

        [Header("Listening to")]
        [SerializeField] private SetMusicEventChannelSO setMusicEventChannel;
        [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;

        private List<MusicKeyName> correctSequence = new List<MusicKeyName>();
        private int ind;

        private bool gameStarted;
        private bool gameCompleted;

        private void Start()
        {
            setMusicEventChannel.OnEventRaised += PrepGame;
            gameStartEventChannel.OnEventRaised += StartGame;

            foreach (var key in musicKeys)
            {
                key.Evt_OnKeyHit.AddListener(CheckSequence);
            }
        }

        private void OnDestroy()
        {
            setMusicEventChannel.OnEventRaised -= PrepGame;
            gameStartEventChannel.OnEventRaised -= StartGame;

            foreach (var key in musicKeys)
            {
                key.Evt_OnKeyHit.RemoveListener(CheckSequence);
            }
        }

        private void PrepGame(MusicData data)
        {
            gameCompleted = false;

            correctSequence = data.sequence;
            musicAudio.clip = data.audioClip;
            SetText();
        }

        private void StartGame(IndividualGameName gameName)
        {
            if (gameName != IndividualGameName.Music)
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

            musicAudio.Play();
            gameStarted = true;
        }

        private void EndGame()
        {
            gameStarted = false;
        }

        private void CompleteGame()
        {
            gameCompleted = true;
            successAudio.Play();
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
            string text = "";
            foreach (var item in correctSequence)
            {
                text += item.ToString() + " ";
            }
            musicText.text = text;
        }
    }
}
