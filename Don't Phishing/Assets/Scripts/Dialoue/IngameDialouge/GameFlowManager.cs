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
    public void Initialize(IngameDialogueController story, DialogueController message)
    {
        this.storyController = story;
        this.messageController = message;

        Debug.Log("[GameFlowManager] 초기화 완료");
    }

    public void StartGame()
    {
        // 현재 저장된 날짜에 따라 상태 설정 및 대화 시작
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
                StartStory("ch01_intro"); 
                break;
            case GameState.Day2:
                StartStory("ch02_main");
                break;
            case GameState.Day3:
                StartStory("ch03_climax");
                break;
        }
    }

    private void StartStory(string conversationTitle)
    {
        if (storyController != null)
        {
            Debug.Log($"[GameFlowManager] 스토리 시작 시도: {conversationTitle}");
            storyController.StartDialogue(conversationTitle);
        }
        else
        {
            Debug.LogError($"[GameFlowManager] storyController(IngameDialogueController)가 할당되지 않았습니다!");
        }
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
