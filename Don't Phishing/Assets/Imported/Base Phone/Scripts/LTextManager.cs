using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 다국어 텍스트 데이터를 관리하는 직렬화 가능 클래스입니다.
/// </summary>
[Serializable]
public class LText
{
    [SerializeField] private string m_English;
    [SerializeField] private string m_Korean;
    [SerializeField] private string m_Japanese;

    public string GetText(Language language)
    {
        return language switch
        {
            Language.English => m_English,
            Language.Korean => m_Korean,
            Language.Japanese => m_Japanese,
            _ => m_English
        };
    }
}

/// <summary>
/// SystemSetting의 스타일 관리 리스트에 자신을 등록하여 중앙 제어를 받는 텍스트 컴포넌트입니다.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class LTextManager : MonoBehaviour
{
    private TMP_Text m_text = null;

    private void Awake()
    {
        m_text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (SystemSetting.Instance != null && m_text != null)
        {
            SystemSetting.Instance.RegisterText(m_text);
        }
    }

    private void OnDisable()
    {
        if (SystemSetting.Instance != null && m_text != null)
        {
            SystemSetting.Instance.UnregisterText(m_text);
        }
    }
}
