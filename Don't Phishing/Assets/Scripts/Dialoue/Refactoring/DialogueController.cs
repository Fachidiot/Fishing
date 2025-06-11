using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ȭ �帧�� �����ϴ� ��Ʈ�ѷ� Ŭ����
/// DialogueEvent �����͸� ������� ���� ��� ���, ������ ó��, �±� ó�� ���� ���
/// UI ��� �� �±� ������ �ܺ� �Ŵ����� �и���
/// </summary>
public class DialogueController : MonoBehaviour
{
    [SerializeField] private DialogueUIManager uiManager;
    [SerializeField] private DialogueTagProcessor tagProcessor;

    private DialogueEvent currentEvent;
    private Dictionary<int, Dialogue> dialogueMap;
    private int currentId;

    [SerializeField] private float delayAfterLine = 2f; // �޽��� ��� �� ���� ������

    private void Awake()
    {
        if (uiManager == null)
            uiManager = FindObjectOfType<DialogueUIManager>();
        if (tagProcessor == null)
            tagProcessor = new DialogueTagProcessor();
    }

    /// <summary>
    /// ��ȭ ���� �Լ� - DialogueEvent�� �޾� ��ȭ�� �ʱ�ȭ�ϰ� ù ��� ���
    /// </summary>
    public void StartDialogue(DialogueEvent dialogueEvent)
    {
        currentEvent = dialogueEvent;
        BuildDialogueMap(dialogueEvent);
        ProceedNext(dialogueEvent.lines[0].id); // ���� ID�� 0��° ���� ����
    }

    /// <summary>
    /// ���� ���� �����ϴ� �Լ� - ID ������� ��� �˻� �� ��� ����
    /// </summary>
    public void ProceedNext(int id)
    {
        if (!dialogueMap.TryGetValue(id, out Dialogue line))
        {
            Debug.LogWarning($"Dialogue ID {id} not found.");
            return;
        }

        currentId = id;

        bool isPlayer = line.speaker.ToLower().Contains("player") || line.speaker == "��";
        uiManager.ShowMessage("", isPlayer); // �� �޽��� ���� ���� (Ÿ�� ȿ�� ����)

        StartCoroutine(TypeLineRoutine(line, isPlayer));
    }

    /// <summary>
    /// ��� ��� �ڷ�ƾ - �� ���ھ� ��� �� �±� ó�� �� ���� �б� ����
    /// </summary>
    private IEnumerator TypeLineRoutine(Dialogue line, bool isPlayer)
    {
        string typed = "";
        foreach (char c in line.text)
        {
            typed += c;
            uiManager.UpdateLastMessage(typed);
            yield return new WaitForSeconds(0.03f);
        }

        // �±� ó�� �� 2�� ���
        bool tagComplete = false;
        tagProcessor.Process(line.tag, () => tagComplete = true);
        yield return new WaitUntil(() => tagComplete);

        yield return new WaitForSeconds(delayAfterLine);

        // ���� �б� ó��
        if (!string.IsNullOrWhiteSpace(line.choices))
            uiManager.ShowChoices(ParseChoices(line.choices));
        else if (line.nextId != 0)
            ProceedNext(line.nextId);
        else
            EndDialogue();
    }

    /// <summary>
    /// ��ȭ ���� ó�� �Լ� - ������ UI ���� ��
    /// </summary>
    private void EndDialogue()
    {
        uiManager.ClearChoices();
        Debug.Log("[DialogueController] Dialogue ended.");
    }

    /// <summary>
    /// DialogueEvent�� ��� ��� ID�� ��ųʸ��� ����
    /// </summary>
    private void BuildDialogueMap(DialogueEvent dialogueEvent)
    {
        dialogueMap = new Dictionary<int, Dialogue>();
        foreach (var line in dialogueEvent.lines)
        {
            if (!dialogueMap.ContainsKey(line.id))
                dialogueMap[line.id] = line;
            else
                Debug.LogWarning($"Duplicate dialogue ID: {line.id}");
        }
    }

    /// <summary>
    /// ������ ���ڿ� �Ľ� �Լ� (ex: "��:2,�ƴϿ�:3")
    /// </summary>
    private List<(string, int)> ParseChoices(string raw)
    {
        var choices = new List<(string, int)>();
        var parts = raw.Split(',');

        foreach (var part in parts)
        {
            var split = part.Split(':');
            if (split.Length == 2 && int.TryParse(split[1], out int nextId))
                choices.Add((split[0].Trim(), nextId));
        }

        return choices;
    }

    /// <summary>
    /// ���� ��縦 ��ȯ�ϴ� �Լ� (�ܺ� ������)
    /// </summary>
    public Dialogue GetCurrentLine()
    {
        return dialogueMap.ContainsKey(currentId) ? dialogueMap[currentId] : null;
    }
}