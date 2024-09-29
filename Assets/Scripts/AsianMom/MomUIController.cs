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
        [SerializeField] private LevelEventChannelSO levelEventChannel;
        [SerializeField] private FloatEventChannelSO updateMomUIEventChannel;
        [SerializeField] private BoolEventChannelSO showMomUIEventChannel;

        private void Start()
        {
            levelEventChannel.OnEventRaised += OnLevelEventRaised;
            updateMomUIEventChannel.OnEventRaised += UpdateSuspicionBar;
            showMomUIEventChannel.OnEventRaised += EnableSuspicionBar;
        }

        private void OnDestroy()
        {
            levelEventChannel.OnEventRaised -= OnLevelEventRaised;
            updateMomUIEventChannel.OnEventRaised -= UpdateSuspicionBar;
            showMomUIEventChannel.OnEventRaised -= EnableSuspicionBar;
        }

        private void Update()
        {
            if (suspicionBarParent.activeInHierarchy)
            {
                Vector3 directionToCamera = suspicionBarParent.transform.position - Camera.main.transform.position;
                directionToCamera.y = 0;
                if (directionToCamera != Vector3.zero)
                {
                    suspicionBarParent.transform.rotation = Quaternion.LookRotation(directionToCamera);
                }
            }
        }

        private void OnLevelEventRaised(LevelEventInfo data)
        {
            if (data.type == LevelEventType.LevelStart) ResetSuspicionBar();
            else if (data.type == LevelEventType.LevelFailed) SetMaxSuspicionBar();
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

        private void SetMaxSuspicionBar()
        {
            suspicionBar.fillAmount = 1;
        }
    }
}