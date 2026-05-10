using UnityEngine;

/// <summary>
/// IngameDialogueController(순수 C#)를 씬에서 사용하기 위한 프록시 클래스입니다.
/// 인스펙터 설정과 Unity 이벤트를 담당합니다.
/// </summary>
public class IngameDialogueProvider : MonoBehaviour
{
    [SerializeField] private IngameDialogueUIManager uiManager = null;
    
    private IngameDialogueController controller = null;

    public IngameDialogueController Controller
    {
        get
        {
            if (controller == null) Initialize();
            return controller;
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        controller = new IngameDialogueController();
        controller.Initialize(uiManager);
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPressed += OnInteractPressed;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPressed -= OnInteractPressed;
        }
    }

    private void OnInteractPressed()
    {
        if (uiManager != null && !uiManager.IsTyping())
        {
            if (controller.ReadyForNext)
            {
                controller.ProceedNext();
            }
        }
    }
}
