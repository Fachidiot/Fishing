using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// �ΰ��� ��� UI�� �����ϴ� Ŭ����
/// ��� ���, Ÿ�� �ִϸ��̼�, ������ ��ư ǥ��/���� ó��
/// </summary>
public class IngameDialogueUIManager : MonoBehaviour
{
    [Header("UI ������Ʈ")]
    [SerializeField] private TMP_Text messageText;           // ��� ��� �ؽ�Ʈ
    [SerializeField] private GameObject dialogueBox;         // ��� �ڽ� ��ü
    [SerializeField] private Button[] choiceButtons;         // ������ ��ư��

    private bool isTyping = false; // ���� Ÿ�� �ִϸ��̼� �� ����

    /// <summary>
    /// �Ϲ� �޽��� ��� (�÷��̾�)
    /// </summary>
    public void ShowMessage(string text, Action onComplete)
    {
        messageText.color = Color.white; // �⺻ ��
        dialogueBox.SetActive(true);
        StartCoroutine(TypeLine(text, onComplete));
    }

    /// <summary>
    /// �ý��� �޽��� ���
    /// </summary>
    public void ShowSystemMessage(string text, Action onComplete)
    {
        messageText.color = new Color(1f, 0.85f, 0.2f);
        dialogueBox.SetActive(true);
        StartCoroutine(TypeLine(text, onComplete));
    }

    /// <summary>
    /// �ؽ�Ʈ�� �� ���ھ� ����ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator TypeLine(string text, Action onComplete)
    {
        isTyping = true;
        messageText.text = "";

        foreach (char c in text)
        {
            messageText.text += c;
            yield return new WaitForSeconds(0.03f); // Ÿ�� �ӵ� ����
        }

        isTyping = false;
        onComplete?.Invoke();
    }

    /// <summary>
    /// ���� �޽��� ��� ������ Ȯ��
    /// </summary>
    public bool IsTyping() => isTyping;

    /// <summary>
    /// �������� ȭ�鿡 ǥ���ϰ�, ��ư Ŭ�� �� �ݹ� ����
    /// </summary>
    public void ShowChoices(List<(string, int)> choices, Action<int> onChoice)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                var (text, id) = choices[i];
                choiceButtons[i].GetComponentInChildren<TMP_Text>().text = text;
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
    /// ��� ������ ��ư�� ����ϴ�
    /// </summary>
    public void HideChoices()
    {
        foreach (var b in choiceButtons)
        {
            b.gameObject.SetActive(false);
        }
    }
}
