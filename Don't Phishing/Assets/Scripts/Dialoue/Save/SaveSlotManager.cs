using UnityEngine;

/// <summary>
/// ���� ���� UI 6���� �ڵ����� �����ϰ� �����ϴ� �Ŵ���
/// ���� ��� �Ǵ� �ҷ����� ��� ��ȯ ����
/// </summary>
public class SaveSlotManager : MonoBehaviour
{
    [Header("���� ������ �� ��ġ ��ġ")]
    [SerializeField] private GameObject slotPrefab;           // SaveSlotUI ������
    [SerializeField] private Transform slotParent;            // ���� ��ġ�� �θ� ������Ʈ

    [Header("���� ��")]
    [SerializeField] private int slotCount = 6;

    [Header("��� ����")]
    [SerializeField] private SaveSlotUI.SlotMode mode = SaveSlotUI.SlotMode.Save;

    [Header("���� ��� ����")]
    [SerializeField] private string currentEventName;
    [SerializeField] private int currentDialogueId;

    private void Start()
    {
        GenerateSlots();
    }


    // ���Ե��� �����ϰ� �ʱ�ȭ�մϴ�.
    private void GenerateSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotParent);
            SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();

            slotUI.Initialize(i, mode, OnSlotClicked);
        }
    }


    // ���� Ŭ�� �� ����: ���� �Ǵ� �ҷ�����

    private void OnSlotClicked(int slotIndex)
    {
        if (mode == SaveSlotUI.SlotMode.Save)
        {
            if (string.IsNullOrEmpty(currentEventName))
            {
                Debug.LogWarning("[SaveSlotManager] ���� ���� - �̺�Ʈ �̸� ����");
                return;
            }

            SMSManager.Instance.SaveDialogueSlot(currentEventName, currentDialogueId, slotIndex);
        }
        else if (mode == SaveSlotUI.SlotMode.Load)
        {
            DialogueSaveData data = SMSManager.Instance.LoadDialogueSlot(slotIndex);
            if (data != null)
            {
                // DialogueEvent �ҷ��ͼ� DialogueController�� ����
                DialogueEvent loadedEvent = Resources.Load<DialogueEvent>($"DialogueEvents/{data.dialogueEventName}");
                if (loadedEvent != null)
                {
                    DialogueController controller = FindObjectOfType<DialogueController>();
                    controller.StartDialogue(loadedEvent);
                    controller.ProceedNext(data.currentId);
                }
                else
                {
                    Debug.LogWarning($"[SaveSlotManager] �̺�Ʈ ���� ã�� �� ����: {data.dialogueEventName}");
                }
            }
        }
    }


    // �ܺο��� ���� ��� ������ �����ϴ� �Լ�
    public void SetCurrentDialogue(string eventName, int dialogueId)
    {
        currentEventName = eventName;
        currentDialogueId = dialogueId;
    }


    // ���� ��� ���� (Save <-> Load)
    public void SetMode(SaveSlotUI.SlotMode newMode)
    {
        mode = newMode;
    }
}
