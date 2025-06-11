using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;

/// <summary>
/// CSV ������ �о� DialogueEvent ScriptableObject�� ��ȯ�ϴ� ������ ��ƿ��Ƽ
/// </summary>
public class DialogueParser : MonoBehaviour
{
    [MenuItem("Tools/Import Dialogue To CSV")]
    public static void ImportCSV()
    {
        string path = EditorUtility.OpenFilePanel("CSV Import", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] data = File.ReadAllLines(path);
        if (data.Length <= 1)
        {
            Debug.LogError("CSV ������ ��� �ְų� ����� �ֽ��ϴ�.");
            return;
        }

        DialogueEvent dialogueEvent = ScriptableObject.CreateInstance<DialogueEvent>();
        dialogueEvent.lines = new List<Dialogue>();

        for (int i = 1; i < data.Length; i++) // ù ���� ���
        {
            if (string.IsNullOrWhiteSpace(data[i])) continue;

            var row = SplitCSVLine(data[i]);

            if (row.Length < 7)
            {
                Debug.LogWarning($"�� ���õ� (�� ����): {data[i]}");
                continue;
            }

            if (!int.TryParse(row[0].Trim(), out int id))
            {
                Debug.LogWarning($"�߸��� ID: '{row[0]}' �� �ǳʶ�");
                continue;
            }

            int nextId = int.TryParse(row[4].Trim(), out int parsedNextId) ? parsedNextId : 0;

            Dialogue line = new Dialogue
            {
                id = id,
                speaker = row[1].Trim(),
                text = row[2].Trim(),
                choices = row[3].Trim(),
                nextId = nextId,
                tag = row[5].Trim(),
                type = row[6].Trim()
            };

            dialogueEvent.lines.Add(line);
        }

        string fileName = Path.GetFileNameWithoutExtension(path);
        string assetPath = $"Assets/Resources/DialogueEvents/{fileName}_event.asset";

        Directory.CreateDirectory("Assets/Resources/DialogueEvents");

        AssetDatabase.CreateAsset(dialogueEvent, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"[DialogueParser] DialogueEvent ���� �Ϸ�: {assetPath}");
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = dialogueEvent;
    }

    /// <summary>
    /// �޸� ���� ���ڿ��� �����ϰ� �Ľ��ϴ� ��ƿ��Ƽ (ex: "����1:2,����2:3")
    /// </summary>
    private static string[] SplitCSVLine(string line)
    {
        List<string> result = new();
        bool inQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current);
        return result.ToArray();
    }
}
#endif
