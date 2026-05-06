using UnityEngine;
using TMPro;
using UnityEngine.UI;

// 세이브 슬롯 UI 하나를 제어하는 스크립트
// 저장된 데이터 표시 및 클릭 이벤트 처리

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    [SerializeField] private TMP_Text slotTitleText = null;    // 이벤트 이름
    [SerializeField] private TMP_Text timeText = null;         // 저장 시간
    [SerializeField] private Button slotButton = null;         // 클릭 버튼

    private int slotIndex = 0;  // 슬롯 인덱스 번호

    public enum SlotMode { Save, Load }
    private SlotMode mode = SlotMode.Save;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (slotTitleText == null) Debug.LogError("[SaveSlotUI] slotTitleText가 할당되지 않았습니다.");
        if (timeText == null) Debug.LogError("[SaveSlotUI] timeText가 할당되지 않았습니다.");
        if (slotButton == null) Debug.LogError("[SaveSlotUI] slotButton이 할당되지 않았습니다.");
    }

    // 슬롯 초기화 함수
    public void Initialize(int index, SlotMode mode, System.Action<int> onClickAction)
    {
        this.slotIndex = index;
        this.mode = mode;

        // 비-Mono 매니저를 통해 데이터 로드
        DialogueSaveData data = SaveLoadManager.Instance.LoadDialogue(index);

        if (data != null)
        {
            slotTitleText.text = data.dialogueEventName;
            timeText.text = data.savedTime;
        }
        else
        {
            slotTitleText.text = "빈 슬롯";
            timeText.text = "";
        }

        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => onClickAction?.Invoke(slotIndex));
        }
    }
}
