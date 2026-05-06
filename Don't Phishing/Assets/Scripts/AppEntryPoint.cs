using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 프로젝트의 메인 진입점 클래스입니다.
/// MonoBehaviour가 없는 매니저들의 초기화와 업데이트를 관리합니다.
/// </summary>
public class AppEntryPoint : MonoBehaviour
{
    private static AppEntryPoint instance = null;

    [Header("Providers Reference")]
    [SerializeField] private IngameDialogueProvider storyProvider = null;
    [SerializeField] private DialogueProvider messageProvider = null;

    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputActions = null;

    [Header("Dialogue Data")]
    [SerializeField] private List<DialogueEvent> dialogueEvents = new List<DialogueEvent>();

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);

            InitManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitManagers()
    {
        // 입력 매니저 초기화 및 에셋 주입
        if (inputActions != null)
        {
            InputManager.Instance.Initialize(inputActions);
        }
        else
        {
            Debug.LogWarning("[AppEntryPoint] InputActions 자산이 할당되지 않았습니다.");
        }

        // Provider를 통해 생성된 순수 C# 컨트롤러/매니저들을 연결
        if (storyProvider != null && messageProvider != null)
        {
            GameFlowManager.Instance.Initialize(
                storyProvider.Controller,
                messageProvider.Controller,
                dialogueEvents
            );
        }

        Debug.Log("[AppEntryPoint] 모든 매니저가 초기화되었습니다.");
    }

    private void OnDestroy()
    {
        // 순수 C# 매니저들 리소스 해제 (메모리 누수 방지)
        if (InputManager.Instance != null) InputManager.Instance.Dispose();
        if (OSManager.Instance != null) OSManager.Instance.Dispose();
        if (AppManager.Instance != null) AppManager.Instance.Dispose();
        if (SystemSetting.Instance != null) SystemSetting.Instance.Dispose();
        
        // TaskManager는 현재 별도의 Dispose가 필요 없으나 구조적 일관성을 위해 체크 가능
    }
}
