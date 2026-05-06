using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 대화 흐름을 제어하는 로직 클래스입니다.
/// MonoBehaviour를 상속받지 않으며 UI 및 태그 프로세서와 상호작용합니다.
/// </summary>
public class DialogueController
{
    private DialogueUIManager uiManager = null;
    private DialogueTagProcessor tagProcessor = null;
    private MonoBehaviour coroutineHost = null;

    private DialogueEvent currentEvent = null;
    private Dictionary<int, Dialogue> dialogueMap = new Dictionary<int, Dialogue>();
    private int currentId = 0;
    private float delayAfterLine = 2f;

    public void Initialize(DialogueUIManager ui, DialogueTagProcessor processor, MonoBehaviour host)
    {
        this.uiManager = ui;
        this.tagProcessor = processor;
        this.coroutineHost = host;
        Debug.Log("[DialogueController] 로직 초기화 완료");
    }

    public void StartDialogue(DialogueEvent dialogueEvent)
    {
        if (dialogueEvent == null) return;

        currentEvent = dialogueEvent;
        BuildDialogueMap(dialogueEvent);
        ProceedNext(dialogueEvent.lines[0].id);
    }

    public void ProceedNext(int id)
    {
        if (dialogueMap == null || !dialogueMap.TryGetValue(id, out Dialogue line))
        {
            Debug.LogWarning($"Dialogue ID {id}를 찾을 수 없습니다.");
            return;
        }

        currentId = id;
        bool isPlayer = line.speaker != null && (line.speaker.ToLower().Contains("player") || line.speaker == "나");
        
        uiManager.ShowMessage("", isPlayer);
        coroutineHost.StartCoroutine(TypeLineRoutine(line, isPlayer));
    }

    private IEnumerator TypeLineRoutine(Dialogue line, bool isPlayer)
    {
        string typed = "";
        foreach (char c in line.text)
        {
            typed += c;
            uiManager.UpdateLastMessage(typed);
            yield return new WaitForSeconds(0.03f);
        }

        bool tagComplete = false;
        if (tagProcessor != null)
        {
            tagProcessor.Process(line.tag, () => tagComplete = true);
            yield return new WaitUntil(() => tagComplete);
        }
        else
        {
            tagComplete = true;
        }

        yield return new WaitForSeconds(delayAfterLine);

        if (!string.IsNullOrWhiteSpace(line.choices))
        {
            uiManager.ShowChoices(ParseChoices(line.choices));
        }
        else if (line.nextId != 0)
        {
            ProceedNext(line.nextId);
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        uiManager.ClearChoices();
        Debug.Log("[DialogueController] 대화 종료");
    }

    private void BuildDialogueMap(DialogueEvent dialogueEvent)
    {
        dialogueMap.Clear();
        foreach (var line in dialogueEvent.lines)
        {
            if (!dialogueMap.ContainsKey(line.id))
                dialogueMap[line.id] = line;
        }
    }

    private List<(string, int)> ParseChoices(string raw)
    {
        var choices = new List<(string, int)>();
        if (string.IsNullOrEmpty(raw)) return choices;

        var parts = raw.Split(',');
        foreach (var part in parts)
        {
            var split = part.Split(':');
            if (split.Length == 2 && int.TryParse(split[1], out int nextId))
                choices.Add((split[0].Trim(), nextId));
        }
        return choices;
    }

    public Dialogue GetCurrentLine()
    {
        return (dialogueMap != null && dialogueMap.ContainsKey(currentId)) ? dialogueMap[currentId] : null;
    }
}