// script taken from unity crpytic cabinet

using UnityEngine;

/// <summary>
///     Utility script to make UI follow the user's gaze, so that it can stay within the field of view of the camera.
/// </summary>
public class FollowUserGaze : MonoBehaviour
{
    [SerializeField] private Transform m_uiElement;

    [SerializeField] private bool m_lockToView;

    [SerializeField] private float m_followSpeed = 0.25f;

    [SerializeField] private float m_menuHeight;

    [SerializeField] private float m_gazeOffsets;

    private Transform m_cameraTransform;

    [SerializeField] private float timeToTurnOffTracking;
    private float timer;

    [Header("Listening to")]
    [SerializeField] private LevelEventChannelSO levelEventChannel;

    private void Awake()
    {
        if (m_uiElement == null)
        {
            m_uiElement = transform;
        }
    }

    private void Start()
    {
        levelEventChannel.OnEventRaised += OnLevelEventRaised;
    }

    private void OnDestroy()
    {
        levelEventChannel.OnEventRaised -= OnLevelEventRaised;
    }

    private void Update()
    {
        if (timer > timeToTurnOffTracking) return;

        UpdatePosition();
        timer += Time.deltaTime;
    }

    private void OnEnable()
    {
        if (Camera.main == null)
        {
            return;
        }

        // Jump to the target position
        m_cameraTransform = Camera.main.transform;
        var playerPos = m_cameraTransform.position;
        var targetDirection =
            Vector3.ProjectOnPlane(m_cameraTransform.forward, Vector3.up).normalized;
        var targetPosition = playerPos + targetDirection;
        targetPosition.y = playerPos.y + m_menuHeight;
        m_uiElement.position = targetPosition;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_uiElement == null)
        {
            m_uiElement = transform;
        }
    }
#endif

    private void ResetTimer()
    {
        timer = 0;
    }

    private void UpdatePosition()
    {
        var playerPos = m_cameraTransform.position;
        var uiElementPos = m_uiElement.position;

        var toUIElement = uiElementPos - playerPos;
        if (toUIElement != Vector3.zero)
        {
            m_uiElement.rotation = Quaternion.LookRotation(toUIElement);
        }

        var targetDirection = Vector3.ProjectOnPlane(m_cameraTransform.forward, Vector3.up).normalized;

        var targetPosition = playerPos + targetDirection;

        if (!m_lockToView)
        {
            targetPosition = Vector3.Slerp(uiElementPos, targetPosition, m_followSpeed * Time.deltaTime);

            var toTarget = (targetPosition - playerPos).normalized;
            targetPosition = playerPos + m_gazeOffsets * toTarget;
        }

        targetPosition.y = playerPos.y + m_menuHeight;

        m_uiElement.position = targetPosition;
    }

    private void OnLevelEventRaised(LevelEventInfo data)
    {
        if (data.type == LevelEventType.LevelFailed || data.type == LevelEventType.LevelFailedOther) ResetTimer();
    }
}
