using UnityEngine;
using UnityEngine.EventSystems;

public class TaskAction : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private GameObject m_TaskBar = null;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (m_TaskBar == null) Debug.LogError("[TaskAction] m_TaskBar가 할당되지 않았습니다.");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_TaskBar != null && eventData.delta.y > 0)
        {
            m_TaskBar.SetActive(true);
        }
    }
}
