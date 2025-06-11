using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� �� �±׸� �Ľ��ϰ� ���������� ó���ϴ� �±� ó����
/// WAIT, FLAG, CHECK_FLAG, ANIM, Image ���� ����
/// </summary>
public class DialogueTagProcessor
{
    private Dictionary<string, bool> flags = new();
    private Animator animator;
    private MonoBehaviour coroutineHost;

    /// <summary>
    /// �±� ó���� ������
    /// </summary>
    /// <param name="animator">�ִϸ��̼� �±׿� Animator</param>
    /// <param name="host">�ڷ�ƾ ����� MonoBehaviour</param>
    public DialogueTagProcessor(Animator animator = null, MonoBehaviour host = null)
    {
        this.animator = animator;
        this.coroutineHost = host;
    }

    /// <summary>
    /// �±� ���ڿ� ��ü ó�� ����
    /// </summary>
    public void Process(string tagString, Action onComplete)
    {
        if (string.IsNullOrWhiteSpace(tagString))
        {
            onComplete?.Invoke();
            return;
        }

        var tags = tagString.Split(',');
        ProcessTagsSequentially(tags, 0, onComplete);
    }

    /// <summary>
    /// �±׵��� ���������� ó�� (WAIT �� ���)
    /// </summary>
    private void ProcessTagsSequentially(string[] tags, int index, Action onComplete)
    {
        if (index >= tags.Length)
        {
            onComplete?.Invoke();
            return;
        }

        var parts = tags[index].Split(':');
        string tagType = parts[0].Trim();
        string value = parts.Length > 1 ? parts[1].Trim() : "";

        switch (tagType)
        {
            case "WAIT":
                if (float.TryParse(value, out float waitTime))
                    coroutineHost.StartCoroutine(WaitAndNext(waitTime, () => ProcessTagsSequentially(tags, index + 1, onComplete)));
                else
                    ProcessTagsSequentially(tags, index + 1, onComplete);
                break;

            case "FLAG":
                SetFlag(value, true);
                ProcessTagsSequentially(tags, index + 1, onComplete);
                break;

            case "CHECK_FLAG":
                if (!CheckFlag(value))
                    onComplete?.Invoke(); // ���� ���� �� �ߴ�
                else
                    ProcessTagsSequentially(tags, index + 1, onComplete);
                break;

            case "ANIM":
                animator?.SetTrigger(value);
                ProcessTagsSequentially(tags, index + 1, onComplete);
                break;

            case "Image":
                SMSManager.Instance?.SaveMessage(value, false); // �̹��� �޽��� ����
                ProcessTagsSequentially(tags, index + 1, onComplete);
                break;

            default:
                Debug.LogWarning($"[TagProcessor] �������� �ʴ� �±�: {tagType}");
                ProcessTagsSequentially(tags, index + 1, onComplete);
                break;
        }
    }

    /// <summary>
    /// ��� �ð� �� ���� �±׷� �̵��ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator WaitAndNext(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }

    /// <summary>
    /// ���� �÷��� �� ����
    /// </summary>
    private void SetFlag(string name, bool value) => flags[name] = value;

    /// <summary>
    /// ���� �÷��� �� Ȯ��
    /// </summary>
    private bool CheckFlag(string name) => flags.TryGetValue(name, out bool v) && v;
}
