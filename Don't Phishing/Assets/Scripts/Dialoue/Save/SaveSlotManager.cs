using UnityEngine;

/// <summary>
/// 저장 슬롯 UI 6개를 자동으로 생성하고 제어하는 매니저
/// 저장 모드 또는 불러오기 모드 전환 가능
/// </summary>
public class SaveSlotManager : MonoBehaviour
{
    [Header("UI 요소 및 프리팹")]
    [SerializeField] private GameObject slotPrefab = null;      // SaveSlotUI 프리팹
    [SerializeField] private Transform slotParent = null;       // 슬롯이 배치될 부모 트랜스폼

    [Header("설정")]
    [SerializeField] private int slotCount = 6;
    [SerializeField] private SaveSlotUI.SlotMode mode = SaveSlotUI.SlotMode.Save;

    [Header("현재 데이터 상태 (저장용)")]
    [SerializeField] private string currentEventName = null;
    [SerializeField] private int currentDialogueId = 0;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        GenerateSlots();
    }

    private void Init()
    {
        if (slotPrefab == null) Debug.LogError("[SaveSlotManager] slotPrefab이 할당되지 않았습니다.");
        if (slotParent == null) Debug.LogError("[SaveSlotManager] slotParent가 할당되지 않았습니다.");
    }

    // 슬롯 생성 및 초기화
    private void GenerateSlots()
    {
        if (slotPrefab == null || slotParent == null) return;

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotParent);
            SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();

            if (slotUI != null)
            {
                slotUI.Initialize(i, mode, OnSlotClicked);
            }
        }
    }

    // 슬롯 클릭 이벤트 핸들러
    private void OnSlotClicked(int slotIndex)
    {
        if (mode == SaveSlotUI.SlotMode.Save)
        {
            SaveLoadManager.Instance.SaveDialogue(currentEventName, currentDialogueId, slotIndex);
        }
        else if (mode == SaveSlotUI.SlotMode.Load)
        {
            DialogueSaveData data = SaveLoadManager.Instance.LoadDialogue(slotIndex);
            if (data != null)
            {
                LoadDialogueEvent(data);
            }
        }
    }

    private void LoadDialogueEvent(DialogueSaveData data)
    {
        // DialogueEvent 로드 및 컨트롤러 연동
        DialogueEvent loadedEvent = Resources.Load<DialogueEvent>($"DialogueEvents/{data.dialogueEventName}");
        if (loadedEvent != null)
        {
            // 리팩토링된 DialogueProvider를 통해 컨트롤러에 접근
            DialogueProvider provider = FindObjectOfType<DialogueProvider>();
            if (provider != null && provider.Controller != null)
            {
                provider.Controller.StartDialogue(loadedEvent);
                provider.Controller.ProceedNext(data.currentId);
            }
        }
        else
        {
            Debug.LogWarning($"[SaveSlotManager] 이벤트를 로드할 수 없습니다: {data.dialogueEventName}");
        }
    }

    public void SetCurrentDialogue(string eventName, int dialogueId)
    {
        currentEventName = eventName;
        currentDialogueId = dialogueId;
    }

    public void SetMode(SaveSlotUI.SlotMode newMode)
    {
        mode = newMode;
    }
}
