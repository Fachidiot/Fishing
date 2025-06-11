using System.Collections;
using UnityEngine;
using TMPro; // UI �ؽ�Ʈ ǥ�ÿ�

public class Player : MonoBehaviour
{
    [SerializeField]
    private DialogueEvent m_DialogueEvent; // ��ȭ �����͸� ��� �ִ� ScriptableObject

    [SerializeField]
    private TMP_Text playerTextUI; // ���� ȭ�鿡 ����� �ؽ�Ʈ UI (TextMeshPro ���)

    private int currentId = 2000; // ������ ��� ID
    private bool isTyping = false; // ���� Ÿ���� ������ ����
    private bool isReadyForNext = false; // ���� ���� �Ѿ �غ� �Ǿ����� ����

    [SerializeField]
    private bool introEnd = false;      //ó�� ��Ʈ�� ����

    private Coroutine typeCoroutine; // ���� ���� ���� �ڷ�ƾ ����
    private string currentFullText = ""; // ���� ����� ��ü �ؽ�Ʈ
    private string currentDisplayedText = ""; // ������� ��µ� �ؽ�Ʈ

    

    // �� �����Ӹ��� Ű �Է� üũ
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                // Ÿ���� ���� �� E�� ������ ��� ��ü �ؽ�Ʈ�� ���
                isTyping = false;
                StopCoroutine(typeCoroutine);
                ShowFullTextImmediately();
            }
            else if (isReadyForNext)
            {
                // �ؽ�Ʈ�� �� ��µ� �� E�� ������ ���� ���� ����
                ProceedNext();
            }
        }
    }

    // ���� ���� �� ��Ʈ�� Ȯ�� ���� �� �ؽ�Ʈ ���
    private void Start()
    {
        if (introEnd == true) ProceedNext();
    }

    // ���� ID�� ��縦 �ҷ��� Ÿ���� ����
    private void ProceedNext()
    {
        isReadyForNext = false;

        // ID�� �ش��ϴ� ��� ã��
        var dialogue = m_DialogueEvent.lines.Find(d => d.id == currentId);
        if (dialogue == null)
        {
            Debug.LogWarning($"ID {currentId}�� �ش��ϴ� ��簡 �����ϴ�.");
            return;
        }

        // �ؽ�Ʈ �ʱ�ȭ
        currentFullText = dialogue.text;
        currentDisplayedText = "";

        // �ڷ�ƾ���� Ÿ���� ȿ�� ����
        typeCoroutine = StartCoroutine(TypeTextCoroutine(dialogue.text, dialogue.nextId));
    }

    // Ÿ���� ȿ�� ���� �ڷ�ƾ
    private IEnumerator TypeTextCoroutine(string text, int nextId)
    {
        isTyping = true;

        // �� ���ھ� ���
        for (int i = 0; i < text.Length; i++)
        {
            if (!isTyping)
                yield break; // �߰��� ����Ǹ� �ڷ�ƾ ����

            currentDisplayedText += text[i];
            playerTextUI.text = currentDisplayedText; // UI�� �ؽ�Ʈ �ݿ�

            yield return new WaitForSeconds(0.03f); // Ÿ���� �ӵ� ����
        }

        isTyping = false;
        isReadyForNext = true;
        currentId = nextId; // ���� ��� ID ����
    }

    // �ؽ�Ʈ ��ü ��� ǥ��
    private void ShowFullTextImmediately()
    {
        currentDisplayedText = currentFullText;
        playerTextUI.text = currentDisplayedText; // UI�� �ؽ�Ʈ �ݿ�
        isReadyForNext = true;
    }

    public void introEvent()
    {
        Debug.Log("��Ʈ�� �ؽ�Ʈ ");
        introEnd = true;
    }
}
