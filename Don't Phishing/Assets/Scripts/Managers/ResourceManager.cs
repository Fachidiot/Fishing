using UnityEngine;
using System;

/// <summary>
/// 소지금, 호감도, 멘탈 등 주요 자원을 관리하는 순수 C# 매니저 클래스입니다.
/// </summary>
public class ResourceManager
{
    private static ResourceManager instance = null;

    // 리소스 값
    private int money = 10000;         // 기본 소지금
    private int favorability = 0;     // 윤세하 호감도
    private int mental = 100;         // 주인공 멘탈 (0~100)

    // 값 변경 이벤트 (UI 갱신용)
    public event Action<int> OnMoneyChanged = null;
    public event Action<int> OnFavorabilityChanged = null;
    public event Action<int> OnMentalChanged = null;

    public static ResourceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ResourceManager();
            }
            return instance;
        }
    }

    private ResourceManager() { }

    #region Money
    public int Money => money;

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log($"[ResourceManager] 소지금 변경: {money} ({amount})");
        OnMoneyChanged?.Invoke(money);
    }

    public bool TrySpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            OnMoneyChanged?.Invoke(money);
            return true;
        }
        return false;
    }
    #endregion

    #region Favorability
    public int Favorability => favorability;

    public void AddFavorability(int amount)
    {
        favorability += amount;
        Debug.Log($"[ResourceManager] 호감도 변경: {favorability} ({amount})");
        OnFavorabilityChanged?.Invoke(favorability);
    }
    #endregion

    #region Mental
    public int Mental => mental;

    public void AddMental(int amount)
    {
        mental = Mathf.Clamp(mental + amount, 0, 100);
        Debug.Log($"[ResourceManager] 멘탈 변경: {mental} ({amount})");
        OnMentalChanged?.Invoke(mental);

        if (mental <= 0)
        {
            OnMentalExhausted();
        }
    }

    private void OnMentalExhausted()
    {
        Debug.LogWarning("[ResourceManager] 멘탈이 소진되었습니다! 게임 오버 로직이 필요합니다.");
    }
    #endregion

    /// <summary>
    /// 모든 리소스를 특정 값으로 초기화 (세이브 데이터 로드 시 등)
    /// </summary>
    public void SetAllResources(int money, int favor, int mental)
    {
        this.money = money;
        this.favorability = favor;
        this.mental = mental;

        OnMoneyChanged?.Invoke(money);
        OnFavorabilityChanged?.Invoke(favorability);
        OnMentalChanged?.Invoke(mental);
    }
}
