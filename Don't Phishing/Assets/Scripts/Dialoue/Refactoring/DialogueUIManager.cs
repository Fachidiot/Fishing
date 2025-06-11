using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� ���, ������ ǥ�� �� UI ó�� ��� �Ŵ���
/// </summary>
public class DialogueUIManager : MonoBehaviour
{
    [Header("Message Prefabs")]
    [SerializeField] private GameObject playerMessagePrefab;
    [SerializeField] private GameObject npcMessagePrefab;
    [SerializeField] private GameObject imageMessagePrefab;
    [SerializeField] private Transform messageParent;

    [Header("Choice Buttons")]
    [SerializeField] private Button[] choiceButtons;

    private GameObject lastMessageObj;

    /// <summary>
    /// �޽��� ��� (�ؽ�Ʈ or �̹���)
    /// </summary>
    public void ShowMessage(string message, bool isPlayer)
    {
        GameObject prefab;

        if (isPlayer)
        {
            prefab = playerMessagePrefab;
        }
        else
        {
            // �̹��� �޽������� �Ǵ�
            var sprite = Resources.Load<Sprite>(message);
            prefab = (sprite != null) ? imageMessagePrefab : npcMessagePrefab;
        }

        var go = Instantiate(prefab, messageParent);
        lastMessageObj = go;

        var layout = go.GetComponent<Message_Layout>();
        layout?.SetUp(message);
    }

    /// <summary>
    /// ������ ��µ� �޽����� ������Ʈ (Ÿ�� ȿ����)
    /// </summary>
    public void UpdateLastMessage(string updatedText)
    {
        if (lastMessageObj == null) return;

        var layout = lastMessageObj.GetComponent<Message_Layout>();
        layout?.UpdateText(updatedText);
    }

    /// <summary>
    /// ������ ��ư ǥ��
    /// </summary>
    public void ShowChoices(List<(string text, int nextId)> choices)
    {
        ClearChoices(); // ���� ��ư �ʱ�ȭ

        int max = Mathf.Min(choices.Count, choiceButtons.Length);

        for (int i = 0; i < max; i++)
        {
            var (text, nextId) = choices[i];
            var button = choiceButtons[i];
            var tmp = button.GetComponentInChildren<TMP_Text>();

            if (tmp != null)
                tmp.text = text;

            button.gameObject.SetActive(true);
            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(() =>
            {
                DialogueController controller = FindObjectOfType<DialogueController>();
                ShowMessage(text, true); // ������ �޽����� �÷��̾� ��ǳ������ ���
                ClearChoices();
                controller.ProceedNext(nextId);
            });
        }
    }

    /// <summary>
    /// ������ ��ư ����
    /// </summary>
    public void ClearChoices()
    {
        foreach (var button in choiceButtons)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }
    }
}
