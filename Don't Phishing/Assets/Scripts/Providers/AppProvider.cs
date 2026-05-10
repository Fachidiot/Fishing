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
        // 만약 인스펙터에서 Apps를 직접 등록하지 않았다면, 컨테이너 하위의 모든 오브젝트를 앱 화면으로 간주
        if ((m_Apps == null || m_Apps.Length == 0) && m_AppScreenContainer != null)
        {
            int childCount = m_AppScreenContainer.transform.childCount;
            m_Apps = new GameObject[childCount];
            for (int i = 0; i < childCount; i++)
            {
                m_Apps[i] = m_AppScreenContainer.transform.GetChild(i).gameObject;
            }
            
            string appNames = "";
            foreach(var app in m_Apps) appNames += app.name + ", ";
            Debug.Log($"[AppProvider] {m_Apps.Length}개의 앱 화면을 자동 등록함: {appNames}");
        }

        // 리팩토링된 AppManager 초기화 (인스턴스에 자기 자신 전달)
        AppManager.Instance.Initialize(this);
    }

    private void OnDestroy()
    {
        if (AppManager.Instance != null)
            AppManager.Instance.Dispose();
    }
}
