using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// 인게임 대화의 핵심 로직을 담당하는 클래스입니다.
/// Dialogue System for Unity를 래핑하여 사용합니다.
/// </summary>
public class IngameDialogueController
{
    private IngameDialogueUIManager ui = null;
    private bool readyForNext = false;

    public bool ReadyForNext => readyForNext;

    public void Initialize(IngameDialogueUIManager uiManager)
    {
        this.ui = uiManager;
        Debug.Log("[IngameDialogueController] Dialogue System 연동 모드로 초기화 완료");
    }

    public void StartDialogue(DialogueEvent e)
    {
        if (e == null) return;

        // ScriptableObject의 이름을 대화방 이름으로 간주하여 시작
        // (Dialogue Database에 해당 이름의 대화가 등록되어 있어야 함)
        DialogueManager.StartConversation(e.name);
        Debug.Log($"[IngameDialogueController] 대화 시작 시도: {e.name}");
    }

    public void ProceedNext()
    {
        // Dialogue System은 자체적으로 입력을 처리하거나 
        // Continue 이벤트를 통해 다음으로 넘어감
        if (DialogueManager.isConversationActive)
        {
            DialogueManager.instance.SendMessage("OnContinueConversation", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ProceedToNextLine()
    {
        ProceedNext();
    }
}
