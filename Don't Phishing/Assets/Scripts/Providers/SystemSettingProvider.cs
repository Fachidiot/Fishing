using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI 요소의 이벤트를 순수 C# SystemSetting 매니저로 전달하는 브릿지 컴포넌트입니다.
/// </summary>
public class SystemSettingProvider : MonoBehaviour
{
    [Header("Media Sliders")]
    [SerializeField] private Slider[] m_VolumeSliders = new Slider[0];
    [SerializeField] private Slider[] m_BrightnessSliders = new Slider[0];
    [SerializeField] private Image m_BrightnessOverlay = null;

    [Header("Toggles")]
    [SerializeField] private StatusBar m_StatusBar = null;
    [SerializeField] private Toggle_Button m_AirplaneToggle = null;
    [SerializeField] private Toggle_Button m_WiFiToggle = null;
    [SerializeField] private Toggle_Button m_TextBoldToggle = null;

    public StatusBar StatusBar => m_StatusBar;

    [Header("Font Assets")]
    [SerializeField] private TMP_FontAsset[] m_FontAssets = new TMP_FontAsset[0];

    public Slider[] VolumeSliders => m_VolumeSliders;
    public Slider[] BrightnessSliders => m_BrightnessSliders;
    public Image BrightnessOverlay => m_BrightnessOverlay;
    public Toggle_Button AirplaneToggle => m_AirplaneToggle;
    public Toggle_Button WiFiToggle => m_WiFiToggle;
    public Toggle_Button TextBoldToggle => m_TextBoldToggle;
    public TMP_FontAsset[] FontAssets => m_FontAssets;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        SystemSetting.Instance.Initialize(this);
    }

    private void OnDestroy()
    {
        if (SystemSetting.Instance != null)
            SystemSetting.Instance.Dispose();
    }

    #region UI Bridge Methods (인스펙터에서 호출용)
    /// <summary>
    /// 슬라이더 이벤트에서 호출: 볼륨 조절
    /// </summary>
    public void SetVolume(float volume)
    {
        SystemSetting.Instance.SetVolume(volume);
    }

    /// <summary>
    /// 슬라이더 이벤트에서 호출: 밝기 조절
    /// </summary>
    public void SetBrightness(float brightness)
    {
        SystemSetting.Instance.SetBrightness(brightness);
    }

    /// <summary>
    /// 토글 버튼 등에서 호출: 각종 시스템 설정 변경
    /// </summary>
    /// <param name="type">VirtualPhoneSettingType (정수값으로 전달 가능)</param>
    public void UpdateSetting(int type, bool value, int sort)
    {
        SystemSetting.Instance.UpdateSetting((VirtualPhoneSettingType)type, value, (short)sort);
    }

    // 오버로드 (단일 파라미터용)
    public void UpdateSettingSimple(int type, bool value)
    {
        SystemSetting.Instance.UpdateSetting((VirtualPhoneSettingType)type, value, 0);
    }
    #endregion
}
