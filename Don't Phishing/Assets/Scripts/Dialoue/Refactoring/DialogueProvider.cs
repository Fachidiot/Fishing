using UnityEngine;

/// <summary>
/// DialogueController(순수 C#)를 씬에서 사용하기 위한 프록시 클래스입니다.
/// </summary>
public class DialogueProvider : MonoBehaviour
{
    [SerializeField] private DialogueUIManager uiManager = null;
    [SerializeField] private float delayAfterLine = 2f;

    private DialogueController controller = null;
    private DialogueTagProcessor tagProcessor = null;

    public DialogueController Controller => controller;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        tagProcessor = new DialogueTagProcessor(null, this);
        controller = new DialogueController();
        controller.Initialize(uiManager, tagProcessor, this);
    }
}
