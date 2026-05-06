using System.Collections;
using UnityEngine;
using TMPro; // UI 텍스트 표시용

public class Player : MonoBehaviour
{
    [SerializeField]
    private DialogueEvent m_DialogueEvent = null; // 대화 데이터를 담고 있는 ScriptableObject

    [SerializeField]
    private TMP_Text playerTextUI = null; // 대화창에 띄울 텍스트 UI (TextMeshPro 사용)

    private int currentId = 2000; // 현재 대사 ID
    private bool isTyping = false; // 현재 타이핑 중인지 여부
    private bool isReadyForNext = false; // 다음 대사로 넘어갈 준비가 되었는지 여부

    [SerializeField]
    private bool introEnd = false;      // 초반 인트로 종료 여부

    private Coroutine typeCoroutine = null; // 대화 타이핑 효과용 코루틴 참조
    private string currentFullText = ""; // 현재 대사의 전체 텍스트
    private string currentDisplayedText = ""; // 현재까지 출력된 텍스트

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        if (introEnd == true) ProceedNext();
    }

    private void OnEnable()
    {
        BindInput();
    }

    private void OnDisable()
    {
        UnbindInput();
    }

    private void Init()
    {
        if (playerTextUI == null)
        {
            playerTextUI = GetComponentInChildren<TMP_Text>();
        }

        if (playerTextUI == null)
        {
            Debug.LogError("[Player] playerTextUI가 할당되지 않았습니다.");
        }
    }

    private void BindInput()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPressed += OnInteractPressed;
        }
    }

    private void UnbindInput()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPressed -= OnInteractPressed;
        }
    }

    private void OnInteractPressed()
    {
        if (isTyping)
        {
            SkipTyping();
        }
        else if (isReadyForNext)
        {
            ProceedNext();
        }
    }

    private void SkipTyping()
    {
        isTyping = false;
        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        ShowFullTextImmediately();
    }

    private void ProceedNext()
    {
        if (m_DialogueEvent == null) return;

        isReadyForNext = false;

        var dialogue = m_DialogueEvent.lines.Find(d => d.id == currentId);
        if (dialogue == null)
        {
            Debug.LogWarning($"ID {currentId}에 해당하는 대사가 없습니다.");
            return;
        }

        currentFullText = dialogue.text;
        currentDisplayedText = "";

        typeCoroutine = StartCoroutine(TypeTextCoroutine(dialogue.text, dialogue.nextId));
    }

    private IEnumerator TypeTextCoroutine(string text, int nextId)
    {
        isTyping = true;

        for (int i = 0; i < text.Length; i++)
        {
            if (!isTyping) yield break;

            currentDisplayedText += text[i];
            playerTextUI.text = currentDisplayedText;

            yield return new WaitForSeconds(0.03f);
        }

        isTyping = false;
        isReadyForNext = true;
        currentId = nextId;
    }

    private void ShowFullTextImmediately()
    {
        currentDisplayedText = currentFullText;
        if (playerTextUI != null) playerTextUI.text = currentDisplayedText;
        isReadyForNext = true;
    }

    public void introEvent()
    {
        Debug.Log("인트로 이벤트 호출");
        introEnd = true;
    }
}
