using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SMS 메시지 레이아웃 및 클릭 이벤트를 처리하는 컴포넌트입니다.
/// </summary>
public class SMS_Layout : MonoBehaviour
{
    [SerializeField] private Button m_Button;

    [Header("TMP_Text")]
    [SerializeField] private TMP_Text m_TMPName;
    [SerializeField] private TMP_Text m_TMPMessage;
    [SerializeField] private TMP_Text m_TMPDate;
    [SerializeField] private DialogueEvent m_DialogueEvent;

    private List<Message> m_Message = new List<Message>();
    private int m_Index = -1;

    private void Awake()
    {
        if (m_Button == null) return;

        // 메시지 리스트 로드 이벤트 등록
        m_Button.onClick.AddListener(() => {
            if (SMSManager.Instance != null)
                SMSManager.Instance.LoadMessage(m_Message);
        });

        // 대화 시작 이벤트 등록
        m_Button.onClick.AddListener(() =>
        {
            DialogueController controller = GameFlowManager.Instance.MessageController;
            if (controller != null && m_DialogueEvent != null)
            {
                controller.StartDialogue(m_DialogueEvent);
            }
            else
            {
                Debug.LogWarning("[SMS_Layout] DialogueController 또는 DialogueEvent가 유효하지 않습니다.");
            }
        });
    }

    public void SetUp(Message message)
    {
        m_Message.Add(message);
        m_Index++;

        if (m_TMPName != null) m_TMPName.text = message.name;
        if (m_TMPMessage != null) m_TMPMessage.text = message.message.Contains("/") ? "Image" : message.message;
        if (m_TMPDate != null) m_TMPDate.text = message.date;
    }

    public Message GetMessage()
    {
        return (m_Index >= 0 && m_Index < m_Message.Count) ? m_Message[m_Index] : null;
    }
}
