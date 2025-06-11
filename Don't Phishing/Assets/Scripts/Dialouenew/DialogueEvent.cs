using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ��� ������ ��� ��ȭ �̺�Ʈ Ŭ���� (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "NewDialogueEvent", menuName = "Dialogue/DialogueEvent")]
public class DialogueEvent : ScriptableObject
{
    public string eventName;              // �̺�Ʈ �̸� (��: intro_1)
    public List<Dialogue> lines;          // �� �̺�Ʈ�� ���Ե� ��� ���
}
    