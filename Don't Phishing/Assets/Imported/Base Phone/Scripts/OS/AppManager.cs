using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 핸드폰 앱들의 실행과 관리를 담당하는 순수 C# 매니저 클래스입니다.
/// </summary>
public class AppManager : IDisposable
{
    private static AppManager instance = null;

    private AppProvider provider = null;
    private bool isDisposed = false;

    // 앱 ID별 알림 개수 관리
    private Dictionary<string, int> appNotifications = new Dictionary<string, int>();
    public event Action<string, int> OnNotificationChanged = null;

    public static AppManager Instance
    {
        get
        {
            if (instance == null) instance = new AppManager();
            return instance;
        }
    }

    private AppManager() { }

    public void Initialize(AppProvider provider)
    {
        this.provider = provider;
        ResetApps();
        
        // 초기화 시 컨테이너 비활성화
        if (provider != null && provider.AppScreenContainer != null)
            provider.AppScreenContainer.SetActive(false);
            
        Debug.Log("[AppManager] 초기화 완료");
    }

    public void Dispose()
    {
        if (isDisposed) return;
        isDisposed = true;
    }

    public string GetCurrentApp()
    {
        if (provider == null || provider.Apps == null) return string.Empty;
        foreach (var app in provider.Apps)
        {
            if (app != null && app.activeSelf)
                return app.GetComponent<BaseAppManager>()?.GetName() ?? string.Empty;
        }
        return string.Empty;
    }

    public void RunApp(string appID)
    {
        if (provider == null) return;
        
        Debug.Log($"[AppManager] 앱 실행 시도: {appID}");
        
        OSManager.Instance.RunApp();
        
        if (provider.AppScreenContainer != null)
            provider.AppScreenContainer.SetActive(true);

        bool found = false;
        foreach (var app in provider.Apps)
        {
            if (app != null && (app.name.Equals(appID, StringComparison.OrdinalIgnoreCase) || 
                               app.name.Equals(appID + " Screen", StringComparison.OrdinalIgnoreCase)))
            {
                app.SetActive(true);
                found = true;
                Debug.Log($"[AppManager] '{app.name}' 활성화 완료");
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning($"[AppManager] '{appID}' 또는 '{appID} Screen' 이라는 이름의 앱 화면 오브젝트를 찾을 수 없습니다.");
        }
    }

    public void ResetApps()
    {
        if (provider == null) return;

        if (provider.AppScreenContainer != null)
            provider.AppScreenContainer.SetActive(false);

        foreach (var app in provider.Apps)
        {
            if (app != null) app.SetActive(false);
        }
    }

    public void RefreshApp(string name)
    {
        if (provider == null || provider.Apps == null) return;
        foreach (var app in provider.Apps)
        {
            if (app != null && app.name == name)
            {
                app.GetComponent<BaseAppManager>()?.ResetApp();
                return;
            }
        }
    }

    #region Notifications
    public void SetNotification(string appID, int count)
    {
        appNotifications[appID] = count;
        OnNotificationChanged?.Invoke(appID, count);
    }

    public void AddNotification(string appID, int addCount)
    {
        int current = GetNotification(appID);
        SetNotification(appID, current + addCount);
    }

    public int GetNotification(string appID)
    {
        return appNotifications.TryGetValue(appID, out int count) ? count : 0;
    }
    #endregion
}
