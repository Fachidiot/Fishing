using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SMS �޽��� ������ ���� ���� �� Ŭ�� �� ��ȭ ����
/// </summary>
public class SMS_Layout : MonoBehaviour
{
    [SerializeField] private Button m_Button;

    [Header("TMP_Text")]
    [SerializeField] private TMP_Text m_TMPName;
    [SerializeField] private TMP_Text m_TMPMessage;
    [SerializeField] private TMP_Text m_TMPDate;
    [SerializeField] private DialogueEvent m_DialogueEvent;

    [SerializeField] private List<Message> m_Message;
    private int m_Index = -1;

    private void Awake()
    {
        m_Message = new List<Message>();

        // Ŭ�� �� �޽��� �ε�
        m_Button.onClick.AddListener(() => SMSManager.Instance.LoadMessage(m_Message));

        // Ŭ�� �� ��ȭ ����
        m_Button.onClick.AddListener(() =>
        {
            DialogueController controller = FindObjectOfType<DialogueController>();
            if (controller != null && m_DialogueEvent != null)
                controller.StartDialogue(m_DialogueEvent);
            else
                Debug.LogWarning("[SMS_Layout] DialogueController �Ǵ� DialogueEvent�� ��� �ֽ��ϴ�.");
        });
    }

    public void SetUp(Message message)
    {
        m_Message.Add(message);
        m_Index++;

        m_TMPName.text = m_Message[m_Index].name;
        m_TMPMessage.text = message.message.Contains("/") ? "Image" : m_Message[m_Index].message;
        m_TMPDate.text = m_Message[m_Index].date;
    }

    public Message GetMessage()
    {
        return m_Message[m_Index];
    }
}
