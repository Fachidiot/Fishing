using UnityEngine;

/// <summary>
/// 게임 전체의 UI 및 핸드폰 활성화/비활성화를 관리하는 컴포넌트입니다.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_OptionUI = null;
    [SerializeField] private GameObject m_PauseUI = null;
    [SerializeField] private GameObject m_TutorialUI = null;
    [SerializeField] private PhoneController m_PhoneController = null;

    private bool m_Paused = false;
    private bool m_Init = false;
    private bool m_UsePhone = false;

    private void Start()
    {
        // 신규 InputManager 이벤트 구독
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnSubmitPressed += HandleSubmit;
            InputManager.Instance.OnCancelPressed += HandleCancel;
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnSubmitPressed -= HandleSubmit;
            InputManager.Instance.OnCancelPressed -= HandleCancel;
        }
    }

    private void HandleSubmit()
    {
        if (!m_Init)
        {
            if (m_TutorialUI != null) m_TutorialUI.SetActive(false);
            m_Init = true;
        }

        m_UsePhone = !m_UsePhone;
        
        if (m_PhoneController != null)
        {
            if (m_UsePhone) m_PhoneController.Enable();
            else m_PhoneController.Disable();
        }
    }

    private void HandleCancel()
    {
        if (m_OptionUI != null && m_OptionUI.activeSelf)
        {
            m_OptionUI.SetActive(false);
        }
        else if (m_PauseUI != null)
        {
            m_Paused = !m_Paused;
            m_PauseUI.SetActive(m_Paused);
        }
    }
}
