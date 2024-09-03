using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PaintingGame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Image originalImage;
        [SerializeField] private Transform paintingTransform;
        [SerializeField] private PaintBrush paintBrush;

        [Header("Broadcasting on")]
        [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;

        [Header("Listening to")]
        [SerializeField] private SetPaintingEventChannelSO setPaintingEventChannel;
        [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;
        [SerializeField] private VoidEventChannelSO levelCompleteEventChannel;

        private PaintingPatternsParent paintingPatternsParent;
        private Dictionary<PaintingPattern, bool> dict = new Dictionary<PaintingPattern, bool>();

        private bool gameStarted;
        private bool gameCompleted;

        private void Start()
        {
            setPaintingEventChannel.OnEventRaised += PrepGame;
            gameStartEventChannel.OnEventRaised += StartGame;
            levelCompleteEventChannel.OnEventRaised += OnLevelComplete;
        }

        private void OnDestroy()
        {
            setPaintingEventChannel.OnEventRaised -= PrepGame;
            gameStartEventChannel.OnEventRaised -= StartGame;
            levelCompleteEventChannel.OnEventRaised -= OnLevelComplete;
        }

        private void PrepGame(PaintingData data)
        {
            gameStarted = false;
            gameCompleted = false;

            paintingPatternsParent = Instantiate(data.paintingPatternsParent, paintingTransform);
            dict.Clear();
            foreach (var item in paintingPatternsParent.GetAllPatterns())
            {
                dict.Add(item, false);
                item.SetCanPaint(true);
                item.Evt_OnPatternColored.AddListener(CheckPatternColor);
            }
            paintingPatternsParent.gameObject.SetActive(false);

            originalImage.sprite = data.image;
            originalImage.gameObject.SetActive(false);

            paintBrush.gameObject.SetActive(false);
        }

        private void StartGame(IndividualGameName gameName)
        {
            if (gameName != IndividualGameName.Painting)
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
            paintingPatternsParent.gameObject.SetActive(true);
            originalImage.gameObject.SetActive(true);

            Debug.Log(dict.Count);
            foreach (var item in dict.Keys.ToList())
            {
                item.SetCanPaint(true);
                item.ResetColor();
            }

            paintBrush.gameObject.SetActive(true);
            paintBrush.ResetColor();
        }

        private void EndGame()
        {
            gameStarted = false;
            originalImage.gameObject.SetActive(false);
            paintBrush.gameObject.SetActive(false);
            if (paintingPatternsParent != null)
                paintingPatternsParent.gameObject.SetActive(false);
        }

        private void CompleteGame()
        {
            gameCompleted = true;
            paintBrush.gameObject.SetActive(false);
            foreach (var item in dict.Keys)
            {
                item.SetCanPaint(false);
                item.Evt_OnPatternColored.RemoveListener(CheckPatternColor);
            }
            originalImage.gameObject.SetActive(false);
            paintingPatternsParent.gameObject.SetActive(false);
            paintBrush.ResetColor();
            gameCompleteEventChannel.RaiseEvent(IndividualGameName.Painting);
        }

        private void OnLevelComplete()
        {
            Destroy(paintingPatternsParent);
            originalImage.sprite = null;
        }

        private void CheckPatternColor(PaintingPattern p, bool b)
        {
            dict[p] = b;
            int sum = 0;
            foreach (var item in dict.Values)
            {
                sum += item ? 1 : 0;
            }
            if (sum == dict.Count)
            {
                CompleteGame();
            }
        }
    }
}