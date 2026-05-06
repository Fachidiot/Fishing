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

    public void RunApp(string name)
    {
        if (provider == null) return;
        
        OSManager.Instance.RunApp();
        
        if (provider.AppScreenContainer != null)
            provider.AppScreenContainer.SetActive(true);

        foreach (var app in provider.Apps)
        {
            if (app != null && app.name == name + " Screen")
            {
                app.SetActive(true);
                break;
            }
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
}
