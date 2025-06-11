using UnityEngine;
using System.Collections.Generic;

public class GameFlowManager : MonoBehaviour
{
    public enum GameState { Day1, Day2, Day3 }

    [Header("Controllers")]
    public IngameDialogueController storyController;
    public DialogueController messageController; // �� ������ ������ ���ܵ�

    [Header("Events")]
    public List<DialogueEvent> dialogueEvents;

    private GameState currentState;

    public static GameFlowManager Instance { get; private set; }
    public GameState _CurrentState => currentState;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        Debug.Log("[GameFlowManager] ���� ���۵� - Start() ȣ��");
        SetState(GameState.Day1);
    }

    public void SetState(GameState newState)
    {
        currentState = newState;
        Debug.Log($"[GameFlowManager] ���� ��ȯ�� �� {newState}");

        switch (newState)
        {
            case GameState.Day1:
                StartStory("ch01");
                break;
            case GameState.Day2:
                StartStory("ch02");
                break;
            case GameState.Day3:
                StartStory("ch03");
                break;
        }
    }

    private void StartStory(string chapterKeyword)
    {
        var story = GetDialogueEvent(chapterKeyword);
        if (story != null && storyController != null)
        {
            Debug.Log($"[GameFlowManager] ���丮 ����: {chapterKeyword}");
            storyController.StartDialogue(story);
        }
        else
        {
            Debug.LogError($"[GameFlowManager] ���丮 ������ �Ǵ� ��Ʈ�ѷ� ����: {chapterKeyword}");
        }
    }

    private DialogueEvent GetDialogueEvent(string partialName)
    {
        foreach (var evt in dialogueEvents)
        {
            Debug.Log("[GameFlowManager] �̺�Ʈ �˻� ��: " + (evt != null ? evt.name : "null"));
            if (evt != null && evt.name.ToLower().Contains(partialName.ToLower()))
                return evt;
        }
        Debug.LogError("[GameFlowManager] �̺�Ʈ�� ã�� �� ����: " + partialName);
        return null;
    }

    public void OnAppMessageTag()
    {
        // �޽��� ���� �±� (app:message_start) �� ���� �ߴ�
        Debug.Log("[GameFlowManager] �޽��� ���� �±� ������ �� ���丮 ���� (��� �帧�� ����)");
        // �ƹ� ���� ����
    }

    public void OnMessageDialogueEnd()
    {
        Debug.Log("[GameFlowManager] �޽��� ���� �� ���丮 ����");

        // ���� �ΰ��� �帧 �����
        var story = GetDialogueEvent("ch01"); // Ȥ�� ������ é��
        if (story != null && storyController != null)
        {
            storyController.StartDialogue(story);
        }
        else
        {
            Debug.LogError("[GameFlowManager] ���丮 ���� ����: �̺�Ʈ �Ǵ� ��Ʈ�ѷ� ����");
        }
    }

}
