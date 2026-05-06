using UnityEngine;
using System;

/// <summary>
/// 보안 검사 및 피싱 대응 기능을 담당하는 순수 C# 매니저 클래스입니다.
/// </summary>
public class SecurityAppManager
{
    private static SecurityAppManager instance = null;

    private float securityScore = 100f; // 0~100 (높을수록 안전)
    private bool isScanning = false;

    public event Action<float> OnSecurityScoreChanged = null;
    public event Action OnScanStarted = null;
    public event Action<bool> OnScanFinished = null; // 성공 여부

    public static SecurityAppManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SecurityAppManager();
            }
            return instance;
        }
    }

    private SecurityAppManager() { }

    public float SecurityScore => securityScore;

    /// <summary>
    /// 보안 검사를 시작합니다.
    /// </summary>
    public void StartSecurityScan()
    {
        if (isScanning) return;

        isScanning = true;
        OnScanStarted?.Invoke();
        Debug.Log("[SecurityAppManager] 보안 검사 시작...");

        // 실제로는 코루틴이나 타이머가 필요하므로 UI 단에서 시간을 끌거나 
        // AppEntryPoint를 통해 업데이트를 받아야 함. 
        // 여기서는 즉시 완료로 처리 (프로토타입용)
        FinishScan(true);
    }

    private void FinishScan(bool success)
    {
        isScanning = false;
        if (success)
        {
            securityScore = Mathf.Min(securityScore + 5f, 100f);
        }
        OnSecurityScoreChanged?.Invoke(securityScore);
        OnScanFinished?.Invoke(success);
        Debug.Log($"[SecurityAppManager] 보안 검사 완료. 현재 점수: {securityScore}");
    }

    /// <summary>
    /// 피싱 공격 발생 시 점수 차감
    /// </summary>
    public void NotifyPhishingAttack(float damage)
    {
        securityScore = Mathf.Max(securityScore - damage, 0f);
        OnSecurityScoreChanged?.Invoke(securityScore);
        
        // 멘탈에도 영향을 줌
        ResourceManager.Instance.AddMental(-(int)(damage / 2f));
        
        Debug.LogWarning($"[SecurityAppManager] 피싱 피해 발생! 보안 점수 감소: {securityScore}");
    }
}
