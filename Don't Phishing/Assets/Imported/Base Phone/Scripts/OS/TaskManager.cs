using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 실행 중인 앱들의 기록을 관리하는 순수 C# 매니저 클래스입니다.
/// </summary>
public class TaskManager
{
    private static TaskManager instance = null;

    private TaskProvider provider = null;
    private List<string> tasks = new List<string>();

    public static TaskManager Instance
    {
        get
        {
            if (instance == null) instance = new TaskManager();
            return instance;
        }
    }

    private TaskManager() { }

    public void Initialize(TaskProvider provider)
    {
        this.provider = provider;
        tasks.Clear();
        UpdateTaskBarVisibility();
    }

    public void AddTask(string name)
    {
        if (string.IsNullOrEmpty(name)) return;
        if (!CheckValidate(name)) return;

        tasks.Add(name);
        InstantiateTaskUI(name);
        UpdateTaskBarVisibility();
    }

    private bool CheckValidate(string name)
    {
        return !tasks.Contains(name);
    }

    public void Remove(string name)
    {
        tasks.Remove(name);
        UpdateTaskBarVisibility();
    }

    private void InstantiateTaskUI(string name)
    {
        if (provider == null || provider.TaskLayoutPrefab == null || provider.TaskParent == null) return;

        GameObject go = GameObject.Instantiate(provider.TaskLayoutPrefab, provider.TaskParent);
        var layout = go.GetComponent<Task_Layout>();
        if (layout != null)
        {
            var scrollRect = provider.TaskBar?.GetComponent<ScrollRect>();
            layout.SetTaskLayout(name, null, scrollRect); 
        }
    }

    private void UpdateTaskBarVisibility()
    {
        // 이제 여기서 SetActive를 직접 제어하지 않고, OSManager의 상태에 따라 결정되도록 합니다.
        // (필요 시 TaskManager.Show() 같은 메서드를 따로 호출)
    }

    public void ShowTaskBar(bool show)
    {
        if (provider != null && provider.TaskBar != null)
            provider.TaskBar.SetActive(show); // 리스트가 없어도 일단 창은 띄움
    }

    public void RunningApp()
    {
        if (provider != null && provider.TaskBar != null)
            provider.TaskBar.SetActive(false);
    }
}
