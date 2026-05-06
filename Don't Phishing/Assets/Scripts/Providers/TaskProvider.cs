using UnityEngine;

/// <summary>
/// TaskManager(Non-Mono)와 씬 내 오브젝트를 연결하는 프록시 컴포넌트입니다.
/// </summary>
public class TaskProvider : MonoBehaviour
{
    [SerializeField] private GameObject m_TaskBar = null;
    [SerializeField] private Transform m_TaskParent = null;
    [SerializeField] private GameObject m_TaskLayoutPrefab = null;

    public GameObject TaskBar => m_TaskBar;
    public Transform TaskParent => m_TaskParent;
    public GameObject TaskLayoutPrefab => m_TaskLayoutPrefab;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        TaskManager.Instance.Initialize(this);
    }
}
