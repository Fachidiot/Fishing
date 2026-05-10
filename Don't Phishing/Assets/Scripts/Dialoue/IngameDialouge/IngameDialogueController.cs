using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// Pixel Crushers Dialogue System을 호출하여 대화를 실행하는 단순 컨트롤러입니다.
/// 모든 대화 데이터는 Pixel Crushers Database 에디터에서 관리됩니다.
/// </summary>
public class IngameDialogueController
{
    private IngameDialogueUIManager ui = null;
    
    // 대화가 진행 중이지 않을 때만 입력 가능하도록 체크
    public bool ReadyForNext => !DialogueManager.isConversationActive;

    public void Initialize(IngameDialogueUIManager uiManager)
    {
        this.ui = uiManager;
        Debug.Log("[IngameDialogueController] Pixel Crushers 직접 연동 모드 활성화");
    }

    /// <summary>
    /// 대화 이름(Conversation Title)을 기반으로 Pixel Crushers 대화를 시작합니다.
    /// </summary>
    public void StartDialogue(string conversationTitle)
    {
        if (string.IsNullOrEmpty(conversationTitle)) return;

        if (DialogueManager.instance != null)
        {
            DialogueManager.StartConversation(conversationTitle);
            Debug.Log($"[IngameDialogueController] Pixel Crushers 대화 시작: {conversationTitle}");
        }
        else
        {
            Debug.LogError("[IngameDialogueController] 씬에 Dialogue Manager가 없습니다!");
        }
    }

    /// <summary>
    /// 다음 대사로 넘기기 위한 명령을 전달합니다.
    /// </summary>
    public void ProceedNext()
    {
        if (DialogueManager.isConversationActive)
        {
            // Dialogue System의 Continue 이벤트를 발생시킴
            DialogueManager.instance.SendMessage("OnContinueConversation", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ProceedToNextLine() => ProceedNext();
}
