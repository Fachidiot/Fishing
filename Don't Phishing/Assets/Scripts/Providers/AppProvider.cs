using UnityEngine;

/// <summary>
/// AppManager(Non-Mono)와 씬 내 오브젝트를 연결하는 프록시 컴포넌트입니다.
/// </summary>
public class AppProvider : MonoBehaviour
{
    [Header("App Management")]
    [SerializeField] private GameObject m_AppScreenContainer = null;
    [SerializeField] private GameObject[] m_Apps = null;

    public GameObject AppScreenContainer => m_AppScreenContainer;
    public GameObject[] Apps => m_Apps;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        // 리팩토링된 AppManager 초기화 (인스턴스에 자기 자신 전달)
        AppManager.Instance.Initialize(this);
    }

    private void OnDestroy()
    {
        if (AppManager.Instance != null)
            AppManager.Instance.Dispose();
    }
}
