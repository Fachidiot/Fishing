using System;

/// <summary>
/// ��ȭ ���� ������ ���� Ŭ����
/// ���� ��ȭ �̺�Ʈ �̸�, ���� ���� ID, ���� �ð��� ����
/// </summary>
[Serializable]
public class DialogueSaveData
{
    public string dialogueEventName;  // ScriptableObject �̸�
    public int currentId;             // ���� ���� ���� ��� ID
    public string savedTime;          // ����� �ð� (ǥ�ÿ�)

    public DialogueSaveData(string eventName, int id)
    {
        dialogueEventName = eventName;
        currentId = id;
        savedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
