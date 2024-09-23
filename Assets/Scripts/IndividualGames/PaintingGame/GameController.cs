using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PaintingGame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject viualsParent;

        [SerializeField] private Image originalImage;
        [SerializeField] private Transform paintingTransform;
        [SerializeField] private PaintBrush paintBrush;
        [SerializeField] private Transform paintBrushLoc;

        [Header("Broadcasting on")]
        [SerializeField] private IndividualGameEventChannelSO gameCompleteEventChannel;

        [Header("Listening to")]
        [SerializeField] private SetPaintingEventChannelSO setPaintingEventChannel;
        [SerializeField] private IndividualGameEventChannelSO gameStartEventChannel;
        [SerializeField] private LevelEventChannelSO levelEventChannel;

        private PaintingPatternsParent paintingPatternsParent;
        private Dictionary<PaintingPattern, bool> dict = new Dictionary<PaintingPattern, bool>();

        private bool gameStarted;
        private bool gameCompleted;

        private void Start()
        {
            paintBrush.gameObject.SetActive(false);

            setPaintingEventChannel.OnEventRaised += PrepGame;
            gameStartEventChannel.OnEventRaised += StartGame;
            levelEventChannel.OnEventRaised += OnLevelEventRaised;

            viualsParent.SetActive(false);
        }

        private void OnDestroy()
        {
            setPaintingEventChannel.OnEventRaised -= PrepGame;
            gameStartEventChannel.OnEventRaised -= StartGame;
            levelEventChannel.OnEventRaised -= OnLevelEventRaised;
        }

        private void OnLevelEventRaised(LevelEventInfo data)
        {
            if (data.type == LevelEventType.LevelComplete) OnLevelComplete();
        }

        private void PrepGame(PaintingData data)
        {
            gameStarted = false;
            gameCompleted = false;

            if (paintingPatternsParent == null)
            {
                paintingPatternsParent = Instantiate(data.paintingPatternsParent, paintingTransform);
                originalImage.sprite = data.image;
            }

            paintingPatternsParent.gameObject.SetActive(false);
            originalImage.gameObject.SetActive(false);

            paintBrush.gameObject.SetActive(false);

            viualsParent.SetActive(false);
        }

        private void StartGame(IndividualGameName gameName)
        {
            if (gameName != IndividualGameName.Painting)
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

            viualsParent.SetActive(true);

            gameStarted = true;
            paintingPatternsParent.gameObject.SetActive(true);
            originalImage.gameObject.SetActive(true);

            foreach (var item in paintingPatternsParent.GetAllPatterns())
            {
                item.SetCanPaint(true);
                item.ResetColor();
                item.Evt_OnPatternColored.AddListener(CheckPatternColor);
            }

            dict.Clear();
            foreach (var item in paintingPatternsParent.GetAllPatterns())
            {
                dict.Add(item, false);
            }

            paintBrush.transform.position = paintBrushLoc.position;
            paintBrush.transform.rotation = paintBrushLoc.rotation;
            paintBrush.gameObject.SetActive(true);
            paintBrush.ResetColor();
        }

        private void EndGame()
        {
            gameStarted = false;
            originalImage.gameObject.SetActive(false);
            paintBrush.gameObject.SetActive(false);
            foreach (var item in paintingPatternsParent.GetAllPatterns())
            {
                item.Evt_OnPatternColored.RemoveListener(CheckPatternColor);
            }
            paintingPatternsParent.gameObject.SetActive(false);
            viualsParent.SetActive(false);
        }

        private void CompleteGame()
        {
            gameCompleted = true;
            //paintBrush.gameObject.SetActive(false);
            foreach (var item in paintingPatternsParent.GetAllPatterns())
            {
                item.SetCanPaint(false);
                item.Evt_OnPatternColored.RemoveListener(CheckPatternColor);
            }
            //originalImage.gameObject.SetActive(false);
            //paintingPatternsParent.gameObject.SetActive(false);
            //paintBrush.ResetColor();
            gameCompleteEventChannel.RaiseEvent(IndividualGameName.Painting);
            Invoke("HideVisuals", 0.5f);
        }

        private void OnLevelComplete()
        {
            Destroy(paintingPatternsParent);
            originalImage.sprite = null;
        }

        private void HideVisuals()
        {
            viualsParent.SetActive(false);
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