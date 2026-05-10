using UnityEngine;

/// <summary>
/// 개별 앱의 정적 데이터를 보관하는 ScriptableObject입니다.
/// </summary>
[CreateAssetMenu(fileName = "NewAppData", menuName = "Phone/AppData")]
public class AppData : ScriptableObject
{
    [SerializeField] private string m_AppID;       // 앱 실행 ID (예: SMS, Calculator)
    [SerializeField] private LText m_AppName;     // 다국어 이름
    [SerializeField] private Sprite m_Icon;       // 앱 아이콘 이미지

    public string AppID => m_AppID;
    public LText AppName => m_AppName;
    public Sprite Icon => m_Icon;
}

/// <summary>
/// 모든 앱 데이터를 리스트로 관리하는 데이터베이스 ScriptableObject입니다.
/// </summary>
[CreateAssetMenu(fileName = "AppDatabase", menuName = "Phone/AppDatabase")]
public class AppDatabase : ScriptableObject
{
    [SerializeField] private AppData[] m_AppList;

    public AppData GetAppData(string id)
    {
        foreach (var data in m_AppList)
        {
            if (data.AppID == id) return data;
        }
        return null;
    }

    public AppData[] AllApps => m_AppList;
}
