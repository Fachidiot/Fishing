using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Time;
    [SerializeField] private GameObject m_airplane;
    [SerializeField] private GameObject m_connection;
    [SerializeField] private Slider m_connnectionSlider;
    [SerializeField] private GameObject m_wifi;
    [SerializeField] private Slider m_wifiSlider;
    [SerializeField] private GameObject m_cellular;
    [SerializeField] private GameObject m_batteryPercent;
    [SerializeField] private Slider m_batterySlider;

    void Update()
    {
        if (m_Time.text != OSManager.Instance.GetTime())
            SetTime();
    }

    private void SetTime()
    {
        m_Time.text = OSManager.Instance.GetTime();
    }
    public void AirplaneMode(bool _airplane)
    {
        m_airplane.SetActive(_airplane);
        m_connection.SetActive(!_airplane);
    }

    public void WiFiMode(bool _wifi)
    {
        m_wifi.SetActive(_wifi);
        if (_wifi)  // WiFi�� Active��ų�� Cellular�� Off����� �Ѵ�.
            m_cellular.SetActive(false);
    }

    public void CellularMode(bool _cellular)
    {
        if (!m_wifi.activeSelf) // Cellular�� WiFi�� Off�� ���¿����� Active�Ҽ� �ִ�.
            m_cellular.SetActive(_cellular);
    }

    public void BatteryPercent(bool _value)
    {
        m_batteryPercent.SetActive(_value);
    }
}
