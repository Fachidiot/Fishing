using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 핸드폰 내 앱 아이콘 및 실행을 담당하는 컴포넌트입니다.
/// </summary>
public class App : MonoBehaviour
{
    [SerializeField] private string m_AppName = "";
    [SerializeField] private TMP_Text m_TMPName = null;

    private void Awake()
    {
        if (m_TMPName == null) m_TMPName = GetComponentInChildren<TMP_Text>();
        
        // 언어 변경 이벤트 구독
        if (OSManager.Instance != null)
        {
            OSManager.Instance.OnLanguageChanged += UpdateLanguage;
        }

        UpdateLanguage();
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        if (OSManager.Instance != null)
        {
            OSManager.Instance.OnLanguageChanged -= UpdateLanguage;
        }
    }

    public void RunApp()
    {
        if (AppManager.Instance != null)
        {
            AppManager.Instance.RunApp(m_AppName);
        }
    }

    private void UpdateLanguage()
    {
        if (m_TMPName == null) return;
        
        // OSManager의 현재 언어에 따라 이름 업데이트 (필요 시 로직 확장 가능)
        m_TMPName.text = m_AppName; 
    }
}
