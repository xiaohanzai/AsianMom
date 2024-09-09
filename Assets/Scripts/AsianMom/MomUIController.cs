using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mom
{
    public class MomUIController : MonoBehaviour
    {
        [SerializeField] private GameObject suspicionBarParent;
        [SerializeField] private Image suspicionBar;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannelSO levelStartEventChannel;
        [SerializeField] private FloatEventChannelSO updateMomUIEventChannel;
        [SerializeField] private BoolEventChannelSO showMomUIEventChannel;

        private void Start()
        {
            levelStartEventChannel.OnEventRaised += ResetSuspicionBar;
            updateMomUIEventChannel.OnEventRaised += UpdateSuspicionBar;
            showMomUIEventChannel.OnEventRaised += EnableSuspicionBar;
        }

        private void OnDestroy()
        {
            levelStartEventChannel.OnEventRaised -= ResetSuspicionBar;
            updateMomUIEventChannel.OnEventRaised -= UpdateSuspicionBar;
            showMomUIEventChannel.OnEventRaised -= EnableSuspicionBar;
        }

        private void ResetSuspicionBar()
        {
            suspicionBar.fillAmount = 0;
        }

        private void UpdateSuspicionBar(float t)
        {
            suspicionBar.fillAmount = t;
        }

        private void EnableSuspicionBar(bool b)
        {
            suspicionBarParent.SetActive(b);
        }
    }
}