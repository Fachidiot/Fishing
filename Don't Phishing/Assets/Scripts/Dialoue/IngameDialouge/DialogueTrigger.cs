using UnityEngine;

/// <summary>
/// 특정 이벤트 시 대화를 시작시키는 트리거 클래스입니다.
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private IngameDialogueProvider provider = null;
    [SerializeField] private DialogueEvent dialogue = null;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (provider == null)
        {
            provider = FindObjectOfType<IngameDialogueProvider>();
        }

        if (provider == null) Debug.LogError("[DialogueTrigger] IngameDialogueProvider가 할당되지 않았습니다.");
        if (dialogue == null) Debug.LogError("[DialogueTrigger] DialogueEvent가 할당되지 않았습니다.");
    }

    /// <summary>
    /// 대화를 시작합니다. (외부 이벤트 혹은 버튼 등에서 호출)
    /// </summary>
    public void StartDialogue()
    {
        if (provider != null && provider.Controller != null && dialogue != null)
        {
            provider.Controller.StartDialogue(dialogue.name);
        }
    }
}
