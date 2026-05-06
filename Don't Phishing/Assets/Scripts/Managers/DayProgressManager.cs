using UnityEngine;
using System;

/// <summary>
/// 일차(Day) 진행과 시간 흐름을 관리하는 순수 C# 매니저 클래스입니다.
/// </summary>
public class DayProgressManager
{
    private static DayProgressManager instance = null;

    private int currentDay = 1;
    private bool isDayTransitioning = false;

    public event Action<int> OnDayChanged = null;

    public static DayProgressManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DayProgressManager();
            }
            return instance;
        }
    }

    private DayProgressManager() { }

    public int CurrentDay => currentDay;

    /// <summary>
    /// 다음 날로 진행합니다.
    /// </summary>
    public void AdvanceDay()
    {
        if (isDayTransitioning) return;
        
        isDayTransitioning = true;
        currentDay++;

        Debug.Log($"[DayProgressManager] 다음 날로 전환: Day {currentDay}");
        
        // 날짜 전환 시 필요한 정산 로직 (예: 이자 계산, 멘탈 회복 등)
        ProcessDayEnd();

        OnDayChanged?.Invoke(currentDay);
        isDayTransitioning = false;
    }

    private void ProcessDayEnd()
    {
        // 예: 매일 소량의 멘탈 회복
        ResourceManager.Instance.AddMental(10);
    }

    /// <summary>
    /// 특정 일차로 강제 설정 (세이브 데이터 로드 시 등)
    /// </summary>
    public void SetDay(int day)
    {
        currentDay = day;
        OnDayChanged?.Invoke(currentDay);
    }
}
