using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 인게임 대화 UI를 제어하는 클래스입니다.
/// 대화창, 타이핑 애니메이션, 선택지 버튼 표시/숨김을 담당합니다.
/// </summary>
public class IngameDialogueUIManager : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    [SerializeField] private TMP_Text messageText = null;           // 대화 내용 텍스트
    [SerializeField] private GameObject dialogueBox = null;         // 대화 박스 전체
    [SerializeField] private Button[] choiceButtons = new Button[0]; // 선택지 버튼들

    private bool isTyping = false; // 현재 타이핑 애니메이션 중인지 여부

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (messageText == null) Debug.LogError("[IngameDialogueUIManager] messageText가 할당되지 않았습니다.");
        if (dialogueBox == null) Debug.LogError("[IngameDialogueUIManager] dialogueBox가 할당되지 않았습니다.");
        
        HideDialogueBox();
        HideChoices();
    }

    /// <summary>
    /// 일반 메시지 출력
    /// </summary>
    public void ShowMessage(string text, Action onComplete)
    {
        if (messageText == null) return;

        messageText.color = Color.black;
        dialogueBox.SetActive(true);

        StartCoroutine(TypeLine(text, onComplete));
    }


    /// <summary>
    /// 시스템 메시지 출력
    /// </summary>
    public void ShowSystemMessage(string text, Action onComplete)
    {
        if (messageText == null) return;

        messageText.color = Color.black;
        dialogueBox.SetActive(true);
        StartCoroutine(TypeLine(text, onComplete));
    }

    /// <summary>
    /// 텍스트를 한 글자씩 출력하는 코루틴
    /// </summary>
    private IEnumerator TypeLine(string text, Action onComplete)
    {
        isTyping = true;
        messageText.text = "";

        foreach (char c in text)
        {
            messageText.text += c;
            yield return new WaitForSeconds(0.03f); // 타이핑 속도 조절
        }

        isTyping = false;
        onComplete?.Invoke();
    }

    /// <summary>
    /// 현재 타이핑 중인지 확인
    /// </summary>
    public bool IsTyping() => isTyping;

    /// <summary>
    /// 선택지를 화면에 표시하고, 버튼 클릭 시 콜백 실행
    /// </summary>
    public void ShowChoices(List<(string, int)> choices, Action<int> onChoice)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                var (text, id) = choices[i];
                TMP_Text btnText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
                if (btnText != null) btnText.text = text;

                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => onChoice(id));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 선택지 버튼 숨기기
    /// </summary>
    public void HideChoices()
    {
        if (choiceButtons == null) return;

        foreach (var b in choiceButtons)
        {
            if (b != null) b.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 대화 박스 숨기기
    /// </summary>
    public void HideDialogueBox()
    {
        if (dialogueBox != null) dialogueBox.SetActive(false);
    }
}
