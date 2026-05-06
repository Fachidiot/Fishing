using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// 핸드폰의 각종 시스템 설정(볼륨, 밝기, 폰트 등)을 관리하는 순수 C# 매니저 클래스입니다.
/// </summary>
public class SystemSetting : IDisposable
{
    private static SystemSetting instance = null;

    private SystemSettingProvider provider = null;
    private VirtualPhoneSetting setting;
    private List<TMP_Text> styleableTexts = new List<TMP_Text>();
    private bool isDisposed = false;

    public event Action<float> OnVolumeChanged = null;
    public event Action<float> OnBrightnessChanged = null;

    private const string OptionDataFileName = "SystemSetting.json";

    public static SystemSetting Instance
    {
        get
        {
            if (instance == null)
                instance = new SystemSetting();
            return instance;
        }
    }

    private SystemSetting()
    {
        LoadOptionData();
    }

    public void Initialize(SystemSettingProvider provider)
    {
        this.provider = provider;
        ApplyAllSettings();
        Debug.Log("[SystemSetting] 초기화 및 설정 적용 완료");
    }

    #region Style Registry
    public void RegisterText(TMP_Text text)
    {
        if (text == null || styleableTexts.Contains(text)) return;
        styleableTexts.Add(text);
        ApplyStyleToText(text);
    }

    public void UnregisterText(TMP_Text text)
    {
        if (text != null) styleableTexts.Remove(text);
    }

    private void ApplyAllStyles()
    {
        foreach (var text in styleableTexts) ApplyStyleToText(text);
    }

    private void ApplyStyleToText(TMP_Text text)
    {
        if (text == null) return;
        if (provider != null && provider.FontAssets != null && setting.TextFont < provider.FontAssets.Length)
            text.font = provider.FontAssets[setting.TextFont];
        
        text.fontStyle = setting.TextBold ? FontStyles.Bold : FontStyles.Normal;
        text.fontSize = setting.TextSize > 0 ? setting.TextSize : text.fontSize;
    }
    #endregion

    #region Media Controls
    public void SetVolume(float volume)
    {
        setting.Volume = volume;
        SaveOptionData();
        UpdateVolumeUI();
        OnVolumeChanged?.Invoke(volume);
    }

    public void SetBrightness(float brightness)
    {
        setting.Brightness = brightness;
        SaveOptionData();
        UpdateBrightnessUI();
        OnBrightnessChanged?.Invoke(brightness);
    }

    private void UpdateVolumeUI()
    {
        if (provider == null) return;
        foreach (var slider in provider.VolumeSliders)
            if (slider != null) slider.value = setting.Volume;
    }

    private void UpdateBrightnessUI()
    {
        if (provider == null) return;
        foreach (var slider in provider.BrightnessSliders)
            if (slider != null) slider.value = setting.Brightness;
        
        if (provider.BrightnessOverlay != null)
        {
            float alpha = 1f - Mathf.Clamp(setting.Brightness, 0.1f, 1f);
            provider.BrightnessOverlay.color = new Color(0, 0, 0, alpha);
        }
    }
    #endregion

    public void UpdateSetting(VirtualPhoneSettingType type, bool value, short sort = 0)
    {
        switch (type)
        {
            case VirtualPhoneSettingType.Airplane: 
                setting.AirplaneMode = value; 
                provider?.StatusBar?.AirplaneMode(value);
                break;
            case VirtualPhoneSettingType.WiFi: 
                setting.WiFi = value; 
                provider?.StatusBar?.WiFiMode(value);
                break;
            case VirtualPhoneSettingType.Cellular:
                setting.Cellular = value;
                provider?.StatusBar?.CellularMode(value);
                break;
            case VirtualPhoneSettingType.Battery:
                if (sort == 0)
                {
                    setting.Battery_Percent = value;
                    provider?.StatusBar?.BatteryPercent(value);
                }
                else if (sort == 1) setting.Battery_LowMode = value;
                break;
            case VirtualPhoneSettingType.Display:
                if (sort == 0) setting.DarkMode = value;
                else if (sort == 1) setting.AutoMode = value;
                else if (sort == 2)
                {
                    setting.TextBold = value;
                    ApplyAllStyles();
                }
                break;
            case VirtualPhoneSettingType.Sound:
                if (sort == 0) setting.Haptic = value;
                else if (sort == 1) setting.LockSound = value;
                break;
            case VirtualPhoneSettingType.General:
                if (sort == 0) setting.TimeFormat = value ? TimeFormat.Army : TimeFormat.Normal;
                break;
        }
        SaveOptionData();
    }

    private void ApplyAllSettings()
    {
        UpdateVolumeUI();
        UpdateBrightnessUI();
        ApplyAllStyles();
        
        if (provider != null)
        {
            if (provider.StatusBar != null)
            {
                provider.StatusBar.AirplaneMode(setting.AirplaneMode);
                provider.StatusBar.WiFiMode(setting.WiFi);
                provider.StatusBar.CellularMode(setting.Cellular);
                provider.StatusBar.BatteryPercent(setting.Battery_Percent);
            }
            provider.AirplaneToggle?.SetValue(setting.AirplaneMode);
            provider.WiFiToggle?.SetValue(setting.WiFi);
            provider.TextBoldToggle?.SetValue(setting.TextBold);
        }
    }

    #region Persistence
    private void LoadOptionData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, OptionDataFileName);
        if (File.Exists(filePath)) setting = JsonUtility.FromJson<VirtualPhoneSetting>(File.ReadAllText(filePath));
        else { setting = new VirtualPhoneSetting(); setting.Brightness = 1f; setting.Volume = 0.5f; setting.TextSize = 36f; }
    }

    public void SaveOptionData() => File.WriteAllText(Path.Combine(Application.persistentDataPath, OptionDataFileName), JsonUtility.ToJson(setting, true));
    #endregion

    public void Dispose()
    {
        if (isDisposed) return;
        styleableTexts.Clear();
        isDisposed = true;
    }

    public VirtualPhoneSetting GetSetting() => setting;
    public float GetTextSize() => setting.TextSize;
    public float GetCurrentVolume() => setting.Volume;
    public float GetCurrentBrightness() => setting.Brightness;
    public TMP_FontAsset GetTextFont() => (provider != null && provider.FontAssets != null && setting.TextFont < provider.FontAssets.Length) ? provider.FontAssets[setting.TextFont] : null;
    public FontStyles GetTextBold() => setting.TextBold ? FontStyles.Bold : FontStyles.Normal;
    public TimeFormat GetTimeSetting() => setting.TimeFormat;
}

[Serializable]
public struct VirtualPhoneSetting
{
    public bool AirplaneMode, WiFi, Cellular, Battery_Percent, Battery_LowMode, VPN, DarkMode, AutoMode, TextBold, QR, GuideLine, NotificationShow, Haptic, LockSound, UsingPassword;
    public string[] WiFiList, BluetoothList, BackgroundList;
    public TimeFormat TimeFormat;
    public Language SystemLanguage;
    public short TextFont;
    public float TextSize, Brightness, Volume;
    public ScreenResolution VideoRecordResolution, CameraCaptureResolution;
    public string CallRing, SMSRing, Password;
    public short[] BatteryGraph;
}

public enum VirtualPhoneSettingType { None, Airplane, WiFi, Bluetooth, Cellular, Battery, Vpn, General, Display, Background, ControlCenter, Camera, HomeScreen, Notification, Sound, Password }
public enum ScreenResolution { UHD, QHD, FHD, HD, SD }
public enum TimeFormat { Normal, Army }