using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 대화 메시지(텍스트/이미지)와 선택지 버튼을 관리하는 UI 매니저입니다.
/// </summary>
public class DialogueUIManager : MonoBehaviour
{
    [Header("메시지 프리팹")]
    [SerializeField] private GameObject playerMessagePrefab = null;
    [SerializeField] private GameObject npcMessagePrefab = null;
    [SerializeField] private GameObject imageMessagePrefab = null;
    [SerializeField] private Transform messageParent = null;

    [Header("선택지 버튼")]
    [SerializeField] private Button[] choiceButtons = new Button[0];

    private GameObject lastMessageObj = null;
    private DialogueProvider dialogueProvider = null;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (playerMessagePrefab == null) Debug.LogError("[DialogueUIManager] playerMessagePrefab이 할당되지 않았습니다.");
        if (npcMessagePrefab == null) Debug.LogError("[DialogueUIManager] npcMessagePrefab이 할당되지 않았습니다.");
        if (messageParent == null) Debug.LogError("[DialogueUIManager] messageParent가 할당되지 않았습니다.");

        // 컨트롤러 참조 캐싱
        dialogueProvider = FindObjectOfType<DialogueProvider>();
        
        ClearChoices();
    }

    /// <summary>
    /// 메시지 표시 (텍스트 혹은 이미지)
    /// </summary>
    public void ShowMessage(string message, bool isPlayer)
    {
        GameObject prefab = null;

        if (isPlayer)
        {
            prefab = playerMessagePrefab;
        }
        else
        {
            // 이미지 메시지 여부 판단 (Resources 로드 시도)
            var sprite = Resources.Load<Sprite>(message);
            prefab = (sprite != null) ? imageMessagePrefab : npcMessagePrefab;
        }

        if (prefab != null && messageParent != null)
        {
            var go = Instantiate(prefab, messageParent);
            lastMessageObj = go;

            // Message_Layout 컴포넌트가 있다면 데이터 설정
            var layout = go.GetComponent<Message_Layout>();
            layout?.SetUp(message);
        }
    }

    /// <summary>
    /// 출력 중인 마지막 메시지 업데이트 (타이핑 효과용)
    /// </summary>
    public void UpdateLastMessage(string updatedText)
    {
        if (lastMessageObj == null) return;

        var layout = lastMessageObj.GetComponent<Message_Layout>();
        layout?.UpdateText(updatedText);
    }

    /// <summary>
    /// 선택지 버튼 표시
    /// </summary>
    public void ShowChoices(List<(string text, int nextId)> choices)
    {
        ClearChoices();

        int max = Mathf.Min(choices.Count, choiceButtons.Length);

        for (int i = 0; i < max; i++)
        {
            var (text, nextId) = choices[i];
            var button = choiceButtons[i];
            var tmp = button.GetComponentInChildren<TMP_Text>();

            if (tmp != null) tmp.text = text;

            button.gameObject.SetActive(true);
            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(() =>
            {
                OnChoiceClicked(text, nextId);
            });
        }
    }

    private void OnChoiceClicked(string text, int nextId)
    {
        if (dialogueProvider != null && dialogueProvider.Controller != null)
        {
            ShowMessage(text, true); // 플레이어의 선택을 대화창에 표시
            ClearChoices();
            dialogueProvider.Controller.ProceedNext(nextId);
        }
    }

    /// <summary>
    /// 모든 선택지 버튼 숨김 및 이벤트 초기화
    /// </summary>
    public void ClearChoices()
    {
        if (choiceButtons == null) return;

        foreach (var button in choiceButtons)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
        }
    }
}
