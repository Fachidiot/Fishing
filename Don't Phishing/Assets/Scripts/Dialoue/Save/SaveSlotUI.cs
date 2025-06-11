using UnityEngine;
using TMPro;
using UnityEngine.UI;

// ���̺� ���� UI �ϳ��� �����ϴ� ��ũ��Ʈ
// ����� ������ ǥ�� �� Ŭ�� �̺�Ʈ ó��

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI ���� ���")]
    [SerializeField] private TMP_Text slotTitleText;    // �̺�Ʈ �̸�
    [SerializeField] private TMP_Text timeText;         // ���� �ð�
    [SerializeField] private Button slotButton;         // Ŭ�� ��ư

    private int slotIndex;  // �� ������ ���� ��ȣ

    public enum SlotMode { Save, Load }
    private SlotMode mode;


    // ���� �ʱ�ȭ �Լ�
    public void Initialize(int index, SlotMode mode, System.Action<int> onClickAction)
    {
        this.slotIndex = index;
        this.mode = mode;

        DialogueSaveData data = SMSManager.Instance.LoadDialogueSlot(index);

        if (data != null)
        {
            slotTitleText.text = data.dialogueEventName;
            timeText.text = data.savedTime;
        }
        else
        {
            slotTitleText.text = "�� ����";
            timeText.text = "";
        }

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => onClickAction?.Invoke(slotIndex));
    }
}
