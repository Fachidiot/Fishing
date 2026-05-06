using UnityEngine;
using System;

/// <summary>
/// 세이브 및 로드 로직을 관리하는 매니저 클래스입니다.
/// MonoBehaviour를 상속받지 않으며 AppEntryPoint에서 초기화될 수 있습니다.
/// </summary>
public class SaveLoadManager
{
    private static SaveLoadManager instance = null;

    public static SaveLoadManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SaveLoadManager();
            }
            return instance;
        }
    }

    private SaveLoadManager() { }

    /// <summary>
    /// 특정 슬롯에 대화 데이터를 저장합니다.
    /// </summary>
    public void SaveDialogue(string eventName, int currentId, int slotIndex)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("[SaveLoadManager] 저장 실패: 이벤트 이름이 비어있습니다.");
            return;
        }

        // 실제 저장 로직 (SMSManager의 로직을 여기로 점진적으로 이전 가능)
        if (SMSManager.Instance != null)
        {
            SMSManager.Instance.SaveDialogueSlot(eventName, currentId, slotIndex);
            Debug.Log($"[SaveLoadManager] 슬롯 {slotIndex}에 '{eventName}'(ID: {currentId}) 저장 완료.");
        }
        else
        {
            Debug.LogError("[SaveLoadManager] SMSManager가 존재하지 않아 저장할 수 없습니다.");
        }
    }

    /// <summary>
    /// 특정 슬롯에서 대화 데이터를 불러옵니다.
    /// </summary>
    public DialogueSaveData LoadDialogue(int slotIndex)
    {
        if (SMSManager.Instance != null)
        {
            DialogueSaveData data = SMSManager.Instance.LoadDialogueSlot(slotIndex);
            if (data != null)
            {
                Debug.Log($"[SaveLoadManager] 슬롯 {slotIndex} 로드 완료: {data.dialogueEventName}");
                return data;
            }
        }
        
        Debug.LogWarning($"[SaveLoadManager] 슬롯 {slotIndex} 로드 실패.");
        return null;
    }
}
