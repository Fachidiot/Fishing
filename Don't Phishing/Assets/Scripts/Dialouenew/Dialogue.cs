using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �ϳ��� ��� ������ ��� Ŭ����
/// </summary>

[System.Serializable]
public class Dialogue
{
    public int id;                 // ����� ���� ID
    public string speaker;         // ���ϴ� ĳ���� �̸�
    [TextArea] public string text; // ��� ����
    public string choices;         // ������ �ؽ�Ʈ: "������1:3,������2:4"
    public int nextId;             // ���� ��� ID (������ ���� ��)
    public string tag;             // ��翡 ���Ե� �±� (WAIT, FLAG_ ��)
    public string type;            // System, Player ��

}
