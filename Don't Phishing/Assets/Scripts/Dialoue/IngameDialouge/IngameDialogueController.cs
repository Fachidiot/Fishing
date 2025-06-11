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
        // ��� ���� ���� �ƴϰ�, ����ڰ� �Է��� ��� �������� ����
        if (readyForNext && !ui.IsTyping() &&
           (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
        {
            ProceedNext();
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
        ShowLine(map[currentId]);
    }

    /// <summary>
    /// �ش� ID�� ��縦 ����ϰ�, ������ �Ǵ� �������� �������� ����
    /// </summary>
    private void ShowLine(Dialogue d)
    {
        readyForNext = false;

        // type ������� �޽��� ���� �б�
        string type = d.type?.ToLowerInvariant(); // null-safe �ҹ��� ó��

        // ���� ��� ���� �Ǵ� ������ ó���� �ݹ�
        Action onComplete = () =>
        {
            if (!string.IsNullOrEmpty(d.choices))
            {
                // ������ �ִ� ��� ��ư ǥ��
                ui.ShowChoices(ParseChoices(d.choices), id =>
                {
                    ui.HideChoices();
                    currentId = id;
                    ShowLine(map[id]);
                });
            }
            else if (d.nextId != 0)
            {
                // ���� ��簡 �ִ� ���: ����� �Է� ���
                readyForNext = true;
            }
            else
            {
                // ������ ���� ���� ó��
                ui.HideChoices();
            }
        };

        // �ý��� �޽������� Ȯ���ϰ� �б�
        if (type == "system")
        {
            ui.ShowSystemMessage(d.text, onComplete); // �ý��� ��Ÿ�� ���
        }
        else
        {
            ui.ShowMessage(d.text, onComplete);       // �Ϲ� ��� ���
        }
    }

    /// <summary>
    /// ���� ID�� ���� (����� �Է¿� ���� ȣ��)
    /// </summary>
    private void ProceedNext()
    {
        if (map.ContainsKey(currentId) && map[currentId].nextId != 0)
        {
            currentId = map[currentId].nextId;
            ShowLine(map[currentId]);
        }
    }

    /// <summary>
    /// ������ �ؽ�Ʈ�� �Ľ��Ͽ� (�ؽ�Ʈ, ���� ID) ����Ʈ�� ��ȯ
    /// ex: "��:2,�ƴϿ�:3"
    /// </summary>
    private List<(string, int)> ParseChoices(string raw)
    {
        var list = new List<(string, int)>();
        foreach (var s in raw.Split(','))
        {
            var parts = s.Trim().Split(':'); // trim���� ���� ����
            if (parts.Length == 2 && int.TryParse(parts[1], out int id))
            {
                string choiceText = parts[0].Trim().Trim('[', ']', '"');
                list.Add((choiceText, id));
            }
        }
        return list;
    }

}