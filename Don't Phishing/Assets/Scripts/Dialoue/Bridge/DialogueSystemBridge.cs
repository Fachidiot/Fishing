using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;

/// <summary>
/// Dialogue System for Unity와 기존 UI 매니저들을 연결하는 브릿지 클래스입니다.
/// </summary>
public class DialogueSystemBridge : MonoBehaviour
{
    [SerializeField] private IngameDialogueUIManager inGameUI = null;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (inGameUI == null)
        {
            inGameUI = FindObjectOfType<IngameDialogueUIManager>();
        }
    }

    // Dialogue System의 대사 출력 시 호출되는 콜백 (Dialogue System Events 컴포넌트에 연결)
    public void OnConversationLine(Subtitle subtitle)
    {
        if (subtitle == null || string.IsNullOrEmpty(subtitle.formattedText.text)) return;

        // 시스템 메시지 여부 판단 (태그 또는 액터 이름 활용)
        bool isSystem = subtitle.speakerInfo.Name.ToLower().Contains("system");

        if (isSystem)
        {
            inGameUI.ShowSystemMessage(subtitle.formattedText.text, null);
        }
        else
        {
            inGameUI.ShowMessage(subtitle.formattedText.text, null);
        }
        
        // SMS 연동이 필요한 경우 SMSManager에 전달
        if (SMSManager.Instance != null && !isSystem)
        {
            // NPC 대사 업데이트
            SMSManager.Instance.SaveMessage(subtitle.formattedText.text, false);
        }
    }

    public void OnConversationResponseMenu(Response[] responses)
    {
        if (responses == null || responses.Length == 0) return;

        var choices = new List<(string text, int nextId)>();
        foreach (var response in responses)
        {
            // Response 객체에서 텍스트와 다음 ID 추출 (내부적으로 destinationEntry 사용)
            choices.Add((response.formattedText.text, response.destinationEntry.id));
        }

        // 기존 UI의 선택지 시스템 호출
        inGameUI.ShowChoices(choices, id => 
        {
            var selectedResponse = System.Array.Find(responses, r => r.destinationEntry.id == id);
            if (selectedResponse != null)
            {
                DialogueManager.standardDialogueUI.OnClick(selectedResponse);
            }
        });
    }

    public void OnConversationEnd(Transform actor)
    {
        inGameUI.HideChoices();
        Debug.Log("[DialogueSystemBridge] 대화 종료 이벤트 수신");
    }
}
