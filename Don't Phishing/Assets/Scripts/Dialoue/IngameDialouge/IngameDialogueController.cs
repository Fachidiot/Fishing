using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// �ΰ��� ������ ��µǴ� ��� �帧�� �����ϴ� ��Ʈ�ѷ�
/// �÷��̾� ���� �����̼� or �ƾ� ��� � ���
/// </summary>
public class IngameDialogueController : MonoBehaviour
{
    [Header("UI �Ŵ��� ����")]
    [SerializeField] private IngameDialogueUIManager ui;

    [Header("��ȭ �̺�Ʈ ������")]
    [SerializeField] private DialogueEvent eventData;

    private Dictionary<int, Dialogue> map;   // ID -> Dialogue ��
    private int currentId;                   // ���� ��ȭ ID
    private bool readyForNext = false;       // ���� ���� �Ѿ �غ� �Ϸ� ����

    private void Update()
    {
        if (!ui.IsTyping())
        {
            if (Input.GetKeyDown(KeyCode.E) /*|| Input.GetMouseButtonDown(0)*/)
            {
                if (readyForNext)
                {
                    Debug.Log("�Է� ���� ��� ����ؾ���");
                    ProceedNext();
                }
                else
                {
                    // ui.SkipTyping(); // �Է� �� Ÿ���� ��� ���
                }
            }
        }
    }

    /// <summary>
    /// ��ȭ�� �����ϴ� �Լ� - �ܺο��� ȣ���
    /// </summary>
    public void StartDialogue(DialogueEvent e)
    {
        eventData = e;
        map = new Dictionary<int, Dialogue>();
        foreach (var d in e.lines)
            map[d.id] = d;

        currentId = e.lines[0].id;
        Debug.Log($"[StartDialogue] ù ��� ID: {currentId}");
        ShowLine(map[currentId]);
    }

    /// <summary>
    /// �ش� ID�� ��縦 ����ϰ�, ������ �Ǵ� �������� �������� ����
    /// </summary>
    private void ShowLine(Dialogue d)
    {
        Debug.Log($"[ShowLine] ID: {d.id}, speaker: {d.speaker}, text: {d.text}, tag: {d.tag}");
        readyForNext = false;

        string type = d.type?.ToLowerInvariant();      // system, etc.
        string speaker = d.speaker?.ToLowerInvariant();
        string tag = d.tag?.ToLowerInvariant();         // �±� �ҹ��� ó��

        // �޽��� �±� ��ŵ ó��
        if (tag == "app:message_start" || tag == "app:message_end")
        {
            Debug.Log($"[Ingame] �±� {tag} �� ��ŵ�ϰ� ���� ����");
            ProceedToNextLine();  // ���� ���� �ٷ� �̵�
            return;
        }

        // ���� ���� �±� �α� ���
        if (tag == "dialogue_stop" || tag == "app:alarm" || tag == "app:cameraqr")
        {
            Debug.Log($"[IngameDialogueController] ó�� ��� �±� ������: {tag}");
            return;
        }

        // �ý��� �޽��� ���
        if (type == "system")
        {
            ui.ShowSystemMessage(d.text, () =>
            {
                HandleNextStep(d);
            });
            return;
        }

        // �Ϲ� �޽��� ���
        ui.ShowMessage(d.text, () =>
        {
            HandleNextStep(d);
        });
    }


    /// <summary>
    /// ���� ������, ID ���� �б� ���� ó��
    /// </summary>
    private void HandleNextStep(Dialogue d)
    {
        if (!string.IsNullOrEmpty(d.choices))
        {
            ui.ShowChoices(ParseChoices(d.choices), id =>
            {
                ui.HideChoices();
                currentId = id;
                ShowLine(map[id]);
            });
        }
        else if (d.nextId != 0)
        {
            readyForNext = true;
        }
        else
        {
            ui.HideChoices();
        }
    }

    private void ProceedNext()
    {
        if (map.ContainsKey(currentId) && map[currentId].nextId != 0)
        {
            Debug.Log("proceedNext �̵� �Ϸ�");
            currentId = map[currentId].nextId;
            ShowLine(map[currentId]);
        }
    }
    public void ProceedToNextLine()
    {
        if (map.ContainsKey(currentId) && map[currentId].nextId != 0)
        {
            Debug.Log("[IngameDialogueController] ProceedToNextLine() �� ���� ��� ���");
            currentId = map[currentId].nextId;
            ShowLine(map[currentId]);
        }
        else
        {
            Debug.LogWarning("[IngameDialogueController] ���� ��簡 ���ų� ��ȿ���� ����");
        }
    }



    private List<(string, int)> ParseChoices(string raw)
    {
        var list = new List<(string, int)>();
        foreach (var s in raw.Split(','))
        {
            var parts = s.Trim().Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1], out int id))
            {
                string choiceText = parts[0].Trim().Trim('[', ']', '"');
                list.Add((choiceText, id));
            }
        }
        return list;
    }
}
