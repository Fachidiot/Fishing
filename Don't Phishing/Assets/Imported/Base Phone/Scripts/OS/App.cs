using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 핸드폰 홈 화면의 개별 앱 아이콘을 관리하는 컴포넌트입니다.
/// ScriptableObject인 AppData로부터 정보를 가져옵니다.
/// </summary>
public class App : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private AppData m_AppData = null;     // 앱 데이터 에셋

    [Header("References")]
    [SerializeField] private Image m_IconImage = null;      // 앱 아이콘 이미지 컴포넌트
    [SerializeField] private TMP_Text m_TMPName = null;     // 앱 이름 텍스트 컴포넌트
    [SerializeField] private GameObject m_BadgeObject = null; // 알림 빨간 점 오브젝트
    [SerializeField] private TMP_Text m_BadgeText = null;    // 알림 숫자 텍스트

    private void Awake()
    {
        // 컴포넌트 자동 캐싱 (비어있을 경우)
        if (m_IconImage == null) m_IconImage = GetComponent<Image>();
        if (m_TMPName == null) m_TMPName = GetComponentInChildren<TMP_Text>();
        
        // 버튼 컴포넌트 자동 검색 (자기 자신 또는 자식)
        Button btn = GetComponent<Button>();
        if (btn == null) btn = GetComponentInChildren<Button>();

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
            Debug.Log($"[App] '{gameObject.name}' 버튼을 찾아 자동 연결했습니다.");
        }
        else
        {
            Debug.LogWarning($"[App] '{gameObject.name}'에서 Button 컴포넌트를 찾을 수 없습니다! (자식 오브젝트 포함)");
        }
        
        ApplyAppData();
    }

    private void OnEnable()
    {
        if (OSManager.Instance != null)
        {
            OSManager.Instance.OnLanguageChanged += UpdateUI;
        }

        if (AppManager.Instance != null)
        {
            AppManager.Instance.OnNotificationChanged += HandleNotificationChanged;
            // 초기 알림 상태 반영
            UpdateBadge(AppManager.Instance.GetNotification(m_AppData?.AppID));
        }

        UpdateUI();
    }

    private void OnDisable()
    {
        if (OSManager.Instance != null)
        {
            OSManager.Instance.OnLanguageChanged -= UpdateUI;
        }

        if (AppManager.Instance != null)
        {
            AppManager.Instance.OnNotificationChanged -= HandleNotificationChanged;
        }
    }

    /// <summary>
    /// 버튼 클릭 시 호출되는 함수 (인스펙터의 On Click 이벤트에 연결하세요!)
    /// </summary>
    public void OnClick()
    {
        Debug.Log($"[App] '{gameObject.name}' 아이콘 클릭됨 (AppID: {m_AppData?.AppID})");
        
        if (m_AppData == null || string.IsNullOrEmpty(m_AppData.AppID))
        {
            Debug.LogWarning($"[App] {gameObject.name}의 AppData가 유효하지 않습니다.");
            return;
        }

        if (AppManager.Instance != null)
        {
            AppManager.Instance.RunApp(m_AppData.AppID);
        }
    }

    /// <summary>
    /// AppData로부터 아이콘과 정보를 적용합니다.
    /// </summary>
    private void ApplyAppData()
    {
        if (m_AppData == null) return;

        if (m_IconImage != null && m_AppData.Icon != null)
        {
            m_IconImage.sprite = m_AppData.Icon;
            m_IconImage.color = Color.white; // 빨간 점 현상 방지
        }
    }

    /// <summary>
    /// 언어나 설정 변경 시 UI를 갱신합니다.
    /// </summary>
    private void UpdateUI()
    {
        if (m_AppData == null || OSManager.Instance == null) return;

        if (m_TMPName != null && m_AppData.AppName != null)
        {
            m_TMPName.text = m_AppData.AppName.GetText(OSManager.Instance.GetLanguage());
        }
    }

    private void HandleNotificationChanged(string id, int count)
    {
        if (m_AppData != null && m_AppData.AppID == id)
        {
            UpdateBadge(count);
        }
    }

    private void UpdateBadge(int count)
    {
        if (m_BadgeObject != null)
        {
            m_BadgeObject.SetActive(count > 0);
        }

        if (m_BadgeText != null)
        {
            m_BadgeText.text = count > 99 ? "99+" : count.ToString();
        }
    }
}
