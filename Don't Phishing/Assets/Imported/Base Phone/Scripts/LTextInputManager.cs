using TMPro;
using UnityEngine;

/// <summary>
/// 인풋 필드와 그 내부 텍스트들에 대해 중앙 집중식 스타일 시스템을 적용하는 컴포넌트입니다.
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class LTextInputManager : MonoBehaviour
{
    [SerializeField] private TMP_Text m_placeholderText = null;
    [SerializeField] private TMP_Text m_text = null;
    
    private TMP_InputField m_inputField = null;

    private void Awake()
    {
        m_inputField = GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        if (SystemSetting.Instance == null) return;

        // 개별 텍스트 컴포넌트 등록
        if (m_text != null) SystemSetting.Instance.RegisterText(m_text);
        if (m_placeholderText != null) SystemSetting.Instance.RegisterText(m_placeholderText);
        
        // 인풋 필드 자체의 폰트 에셋 설정 동기화
        SyncInputFieldFont();
    }

    private void OnDisable()
    {
        if (SystemSetting.Instance == null) return;

        if (m_text != null) SystemSetting.Instance.UnregisterText(m_text);
        if (m_placeholderText != null) SystemSetting.Instance.UnregisterText(m_placeholderText);
    }

    private void SyncInputFieldFont()
    {
        if (m_inputField != null && SystemSetting.Instance != null)
        {
            m_inputField.fontAsset = SystemSetting.Instance.GetTextFont();
        }
    }
}
