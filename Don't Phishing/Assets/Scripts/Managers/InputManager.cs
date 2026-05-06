using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// 자동 생성 클래스 없이 InputActionAsset을 직접 사용하는 InputManager입니다.
/// </summary>
public class InputManager : IDisposable
{
    private static InputManager instance = null;

    private InputActionAsset inputAsset = null;
    private InputActionMap playerMap = null;
    private bool isDisposed = false;

    // 입력 이벤트 정의
    public event Action OnInteractPressed = null;
    public event Action OnSubmitPressed = null;
    public event Action OnCancelPressed = null;
    public event Action<Vector2> OnNavigate = null;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InputManager();
            }
            return instance;
        }
    }

    private InputManager() { }

    /// <summary>
    /// 외부(AppEntryPoint 등)에서 에셋을 주입하며 초기화합니다.
    /// </summary>
    public void Initialize(InputActionAsset asset)
    {
        if (asset == null)
        {
            Debug.LogError("[InputManager] InputActionAsset이 null입니다.");
            return;
        }

        this.inputAsset = asset;
        playerMap = inputAsset.FindActionMap("Player");

        if (playerMap == null)
        {
            Debug.LogError("[InputManager] 'Player' 액션 맵을 찾을 수 없습니다.");
            return;
        }

        // 액션 바인딩 (문자열 이름으로 찾기)
        playerMap.FindAction("Interact").performed += ctx => OnInteractPressed?.Invoke();
        playerMap.FindAction("Submit").performed += ctx => OnSubmitPressed?.Invoke();
        playerMap.FindAction("Cancel").performed += ctx => OnCancelPressed?.Invoke();
        playerMap.FindAction("Navigate").performed += ctx => OnNavigate?.Invoke(ctx.ReadValue<Vector2>());

        EnableInput();
        Debug.Log("[InputManager] 자산 기반 초기화 및 입력 활성화 완료");
    }

    public void EnableInput()
    {
        playerMap?.Enable();
    }

    public void DisableInput()
    {
        playerMap?.Disable();
    }

    public void Dispose()
    {
        if (isDisposed) return;
        DisableInput();
        isDisposed = true;
    }
}
