using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 게임의 흐름을 관리하는 매니저 클래스입니다.
/// MonoBehaviour를 상속받지 않으며 AppEntryPoint에서 초기화됩니다.
/// </summary>
public class GameFlowManager
{
    private static GameFlowManager instance = null;

    public enum GameState { Day1, Day2, Day3 }

    private IngameDialogueController storyController = null;
    private DialogueController messageController = null;
    private List<DialogueEvent> dialogueEvents = null;
    private GameState currentState = GameState.Day1;

    public static GameFlowManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameFlowManager();
            }
            return instance;
        }
    }

    public GameState CurrentState => currentState;
    public DialogueController MessageController => messageController;

    private GameFlowManager() { }

    /// <summary>
    /// 매니저 초기화 함수입니다. 외부(AppEntryPoint 등)에서 호출됩니다.
    /// </summary>
    public void Initialize(IngameDialogueController story, DialogueController message, List<DialogueEvent> events)
    {
        this.storyController = story;
        this.messageController = message;
        this.dialogueEvents = events;

        Debug.Log("[GameFlowManager] 초기화 완료");
        
        // 현재 저장된 날짜에 따라 상태 설정
        SetStateByDay(DayProgressManager.Instance.CurrentDay);
    }

    private void SetStateByDay(int day)
    {
        switch (day)
        {
            case 1: SetState(GameState.Day1); break;
            case 2: SetState(GameState.Day2); break;
            case 3: SetState(GameState.Day3); break;
            default: SetState(GameState.Day1); break;
        }
    }

    public void SetState(GameState newState)
    {
        currentState = newState;
        Debug.Log($"[GameFlowManager] 상태 변경: {newState}");

        switch (newState)
        {
            case GameState.Day1:
                StartStory("ch01_intro"); // 더 명확한 시나리오 이름 사용
                break;
            case GameState.Day2:
                StartStory("ch02_main");
                break;
            case GameState.Day3:
                StartStory("ch03_climax");
                break;
        }
    }

    private void StartStory(string chapterKeyword)
    {
        var story = GetDialogueEvent(chapterKeyword);
        if (story != null && storyController != null)
        {
            Debug.Log($"[GameFlowManager] 스토리 시작: {chapterKeyword}");
            storyController.StartDialogue(story);
        }
        else
        {
            // Dialogue System for Unity 모드에서는 에셋 데이터베이스에 
            // chapterKeyword와 일치하는 Conversation이 있는지 확인해야 함
            Debug.LogWarning($"[GameFlowManager] 스토리 '{chapterKeyword}'를 찾을 수 없거나 데이터베이스 확인이 필요합니다.");
        }
    }

    private DialogueEvent GetDialogueEvent(string partialName)
    {
        if (dialogueEvents == null) return null;

        foreach (var evt in dialogueEvents)
        {
            if (evt != null && evt.name.ToLower().Contains(partialName.ToLower()))
                return evt;
        }
        return null;
    }

    public void OnAppMessageTag()
    {
        Debug.Log("[GameFlowManager] 앱 메시지 태그 감지");
        // 예: 피싱 메시지 알림 발생 시 보안 점수 하락
        SecurityAppManager.Instance.NotifyPhishingAttack(10f);
    }

    public void OnMessageDialogueEnd()
    {
        Debug.Log("[GameFlowManager] 메시지 대화 종료");
        
        // 특정 대화 종료 후 다음 날로 진행하는 로직 예시
        if (DayProgressManager.Instance.CurrentDay == 1)
        {
            DayProgressManager.Instance.AdvanceDay();
            SetStateByDay(DayProgressManager.Instance.CurrentDay);
        }
    }
}
