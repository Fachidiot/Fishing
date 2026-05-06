using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// Dialogue System의 루아 스크립트와 게임 내 매니저들을 연결하는 브릿지 클래스입니다.
/// </summary>
public class DialogueLuaBridge : MonoBehaviour
{
    private void OnEnable()
    {
        // 루아 함수 등록
        Lua.RegisterFunction("AddMoney", this, SymbolExtensions.GetMethodInfo(() => AddMoney(0)));
        Lua.RegisterFunction("AddFavorability", this, SymbolExtensions.GetMethodInfo(() => AddFavorability(0)));
        Lua.RegisterFunction("AddMental", this, SymbolExtensions.GetMethodInfo(() => AddMental(0)));
        Lua.RegisterFunction("NotifyPhishing", this, SymbolExtensions.GetMethodInfo(() => NotifyPhishing(0)));
    }

    private void OnDisable()
    {
        // 루아 함수 해제
        Lua.UnregisterFunction("AddMoney");
        Lua.UnregisterFunction("AddFavorability");
        Lua.UnregisterFunction("AddMental");
        Lua.UnregisterFunction("NotifyPhishing");
    }

    public void AddMoney(double amount)
    {
        ResourceManager.Instance.AddMoney((int)amount);
    }

    public void AddFavorability(double amount)
    {
        ResourceManager.Instance.AddFavorability((int)amount);
    }

    public void AddMental(double amount)
    {
        ResourceManager.Instance.AddMental((int)amount);
    }

    public void NotifyPhishing(double damage)
    {
        SecurityAppManager.Instance.NotifyPhishingAttack((float)damage);
    }
}
