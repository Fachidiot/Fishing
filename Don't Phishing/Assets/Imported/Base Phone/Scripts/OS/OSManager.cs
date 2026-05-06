using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public enum Language { English, Korean, Japanese }
public enum Status { Idle, Notification, Control, RunApp }

/// <summary>
/// 핸드폰 OS의 핵심 로직을 담당하는 순수 C# 매니저 클래스입니다.
/// </summary>
public class OSManager : IDisposable
{
    private static OSManager instance = null;

    private OSProvider provider = null;
    private Language language = Language.Korean;
    private Status currentStatus = Status.Idle;
    private bool isLocked = true;
    private bool isDisposed = false;
    
    private Profile profile = null;
    private ScrollSnap lockSnap = null;
    private ScrollSnap controlSnap = null;

    public event Action OnLanguageChanged = null;

    public static OSManager Instance
    {
        get
        {
            if (instance == null) instance = new OSManager();
            return instance;
        }
    }

    private OSManager()
    {
        profile = new Profile("User", "Sprites/Icons/channels4_profile");
    }

    public void Initialize(OSProvider provider)
    {
        this.provider = provider;
        if (provider.LockScreen != null) lockSnap = provider.LockScreen.GetComponent<ScrollSnap>();
        if (provider.ControlScreen != null) controlSnap = provider.ControlScreen.GetComponent<ScrollSnap>();
        SetDate(language);
        Debug.Log("[OSManager] 초기화 완료");
    }

    public void Dispose()
    {
        if (isDisposed) return;
        OnLanguageChanged = null;
        isDisposed = true;
    }

    public void OnUpdate()
    {
        if (provider == null) return;
        CheckLockStatus();
        CheckStatus();

        if (provider.MainAnimator != null)
            provider.MainAnimator.SetBool("IsLocked", isLocked);

        UpdateUIByStatus();
    }

    private void UpdateUIByStatus()
    {
        if (provider == null) return;
        switch (currentStatus)
        {
            case Status.Idle:
                if (provider.BottomBar != null) provider.BottomBar.SetActive(true);
                provider.MainScreen.SetActive(true);
                break;
            case Status.Notification:
            case Status.Control:
                if (provider.BottomBar != null) provider.BottomBar.SetActive(true);
                provider.MainScreen.SetActive(false);
                break;
            case Status.RunApp:
                if (provider.BackgroundCaptureCamera != null) provider.BackgroundCaptureCamera.gameObject.SetActive(true);
                if (provider.BottomBar != null) provider.BottomBar.SetActive(true);
                provider.MainScreen.SetActive(false);
                break;
        }
    }

    private void CheckLockStatus()
    {
        if (lockSnap != null && isLocked && lockSnap.GetCurrentItem() == 1)
            isLocked = false;
    }

    private void CheckStatus()
    {
        if (controlSnap != null && controlSnap.GetCurrentItem() == 2)
        {
            currentStatus = Status.Control;
            return;
        }
        else if (lockSnap != null && lockSnap.GetCurrentItem() == 2)
        {
            currentStatus = Status.Notification;
            return;
        }
        if (currentStatus == Status.RunApp) return;
        currentStatus = Status.Idle;
    }

    public void RunApp() => currentStatus = Status.RunApp;
    public void EndApp() => currentStatus = Status.Idle;
    public Language GetLanguage() => language;

    public void SetLanguage(int langIndex)
    {
        language = (Language)langIndex;
        SetDate(language);
        OnLanguageChanged?.Invoke();
    }

    public void ChangeBackground(int index) => provider.Background?.UpdateBackground(index);
    public Profile GetProfile() => profile;

    public string GetTime()
    {
        return DateTime.Now.ToString("HH:mm");
    }

    private void SetDate(Language lang)
    {
        if (provider != null && provider.TDate != null)
            provider.TDate.text = TimeUtils.GetDate(GetCulture(lang));
    }

    private CultureInfo GetCulture(Language lang) => lang switch
    {
        Language.English => new CultureInfo("en-US"),
        Language.Korean => new CultureInfo("ko-KR"),
        Language.Japanese => new CultureInfo("ja-JP"),
        _ => new CultureInfo("en-US")
    };
}

public static class TimeUtils
{
    public static string GetDate(CultureInfo cultureInfo)
    {
        return cultureInfo.TwoLetterISOLanguageName switch
        {
            "en" => $"{DateTime.Now:dddd, MMMM dd}",
            "ja" => $"{DateTime.Now:MM}月{DateTime.Now:dd}일{GetJapaneseDayOfWeek(DateTime.Now.DayOfWeek)}",
            "ko" => $"{DateTime.Now:MM}월 {DateTime.Now:dd}일 {GetKoreanDayOfWeek(DateTime.Now.DayOfWeek)}",
            _ => null
        };
    }

    private static string GetKoreanDayOfWeek(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "월요일", DayOfWeek.Tuesday => "화요일", DayOfWeek.Wednesday => "수요일",
        DayOfWeek.Thursday => "목요일", DayOfWeek.Friday => "금요일", DayOfWeek.Saturday => "토요일",
        DayOfWeek.Sunday => "일요일", _ => null
    };

    private static string GetJapaneseDayOfWeek(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "月曜日", DayOfWeek.Tuesday => "火曜日", DayOfWeek.Wednesday => "水曜日",
        DayOfWeek.Thursday => "木曜日", DayOfWeek.Friday => "金曜日", DayOfWeek.Saturday => "土曜日",
        DayOfWeek.Sunday => "日요일", _ => null
    };

    public static string GetHour() => DateTime.Now.ToString("HH");
    public static string GetMinute() => DateTime.Now.ToString("mm");
}