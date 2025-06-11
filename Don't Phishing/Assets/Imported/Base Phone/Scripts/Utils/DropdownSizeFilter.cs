using TMPro;
using UnityEngine;

public class DropdownSizeFilter : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown m_Dropdown;
    [SerializeField]
    private Vector2 m_Size;

    void Start()
    {
        // Template�� ũ�⸦ ���� ����
        RectTransform template = m_Dropdown.template;
        template.sizeDelta = m_Size;

        // ��ũ�� ���� ũ�⵵ �����Ϸ���
        RectTransform scrollRect = template.Find("Viewport").GetComponent<RectTransform>();
        if (scrollRect != null)
        {
            scrollRect.sizeDelta = new Vector2(scrollRect.sizeDelta.x, m_Size.y - 20); // ���� ���
        }
    }
}
