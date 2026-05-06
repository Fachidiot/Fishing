using UnityEngine;
using TMPro;

/// <summary>
/// OSManager(Non-Mono)와 씬 내 오브젝트를 연결하는 프록시 컴포넌트입니다.
/// </summary>
public class OSProvider : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private Animator m_mainAnimator = null;
    [SerializeField] private GameObject m_mainScreen = null;
    [SerializeField] private GameObject m_lockScreen = null;
    [SerializeField] private GameObject m_controlScreen = null;
    [SerializeField] private Camera m_BackgroundCaptureCamera = null;
    
    [Header("Background")]
    [SerializeField] private BackgroundManager m_background = null;

    [Header("UI Components")]
    [SerializeField] private GameObject m_bottombar = null;
    [SerializeField] private TMP_Text m_TDate = null;
    [SerializeField] private TMP_Text m_TLanguage = null;

    public Animator MainAnimator => m_mainAnimator;
    public GameObject MainScreen => m_mainScreen;
    public GameObject LockScreen => m_lockScreen;
    public GameObject ControlScreen => m_controlScreen;
    public Camera BackgroundCaptureCamera => m_BackgroundCaptureCamera;
    public BackgroundManager Background => m_background;
    public GameObject BottomBar => m_bottombar;
    public TMP_Text TDate => m_TDate;
    public TMP_Text TLanguage => m_TLanguage;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        // OSManager 초기화 시 이 Provider를 전달
        OSManager.Instance.Initialize(this);
    }

    private void Update()
    {
        // OSManager의 로직 실행 (필요한 경우에만)
        OSManager.Instance.OnUpdate();
    }
}
