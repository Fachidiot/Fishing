using System.Collections.Generic;
using System.IO;
using System;
using TMPro;
using UnityEngine;
using static Message;
using System.Collections;
using UnityEngine.UI;

[Serializable]
public class Message
{
    public string name;
    public string message;
    public MsgType type;
    public string date;

    public enum MsgType
    {
        None,
        Mine
    }

    public Message(string name, string message, MsgType type, string date)
    {
        this.name = name;
        this.message = message;
        this.type = type;
        this.date = date;
    }
}
public class MessageDB
{
    public List<Message> messages;
}

public class SMSManager : BaseAppManager
{
    private static SMSManager m_Instance;
    public static SMSManager Instance { get { return m_Instance; } }


    [Header("View & Bar")]
    [SerializeField]
    private GameObject m_MainBar;
    [SerializeField]
    private GameObject m_MessageBar;
    [SerializeField]
    private GameObject m_PopupView;
    [SerializeField]
    private GameObject m_HorizontalSnapScrollView;
    [SerializeField]
    private Profile_Layout m_SMSProfile;
    [Space]
    [Header("Text")]
    [SerializeField]
    private TMP_Text m_TMPSMS;
    [SerializeField]
    private LText m_SMSText;
    [SerializeField]
    private TMP_Text m_TMPEdit;
    [SerializeField]
    private LText m_EditText;
    [SerializeField]
    private SearchBar[] m_SearchBars;
    [Header("Messages")]
    [SerializeField]
    private bool m_AutoSave = true;
    [SerializeField]
    private GameObject m_MessagePreviewParent;
    [SerializeField]
    private GameObject m_MessagePreviewPrefab;
    [SerializeField]
    private GameObject m_MessageParent;
    [SerializeField]
    private GameObject m_NPCMessagePrefab;
    [SerializeField]
    private GameObject m_IMGMessagePrefab;
    [SerializeField]
    private GameObject m_PlayerMessagePrefab;

    private Message_Layout _lastNPCLayout;

    [SerializeField] private Button[] m_ChoiceButtonsFixed;
    [SerializeField] private DialogueController dialogueController;



    // TODO : ���� ���� ��ü(�޴��� ����) Localization�� ���� Messages-en, Messages-kr, Messages-jp�̷������� �����ϸ� ������ ����.
    private string m_FileName = "Messages.json";
    private string m_Path = Application.dataPath + "/Resources/Json/Messages/";

    private MessageDB m_MessageDB;
    private List<GameObject> m_TopMsgList;
    private List<GameObject> m_MessageList;
    private string m_CurrentName;

    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;

    }

    public void Start()
    {
        SetText();
        ResetApp();
        Init();
        ClearFixedButtons();
    }

    private void Init()
    {
        m_MessageDB = new MessageDB();
        m_TopMsgList = new List<GameObject>();
        m_MessageList = new List<GameObject>();
        LoadMessages();
    }

    public void SaveMessage(string message, bool isMine)
    {
        var type = isMine ? MsgType.Mine : MsgType.None;
        Message _message = new Message(m_CurrentName, message, type, DateTime.Now.ToString("yyyy-MM-dd-HH:mm"));

        GameObject go = InstantiateMessage(_message, isMine);

        m_MessageDB.messages.Add(_message);
        if (m_AutoSave)
            SaveMessages();

        // NPC�� ��� ������ �޽��� ĳ��
        if (!isMine && go != null)
            _lastNPCLayout = go.GetComponent<Message_Layout>();
    }

    public void LoadMessage(List<Message> list)
    {
        DeletePrev();

        // View ����
        m_MainBar.SetActive(false);
        m_MessageBar.SetActive(true);
        m_HorizontalSnapScrollView.GetComponent<ScrollSnap>().SetContentPosition(1);

        // �޼��� ����
        for (int i = 0; i < list.Count; i++)
        {
            InstantiateMessage(list[i], list[i].name == "Mine");
        }

        m_CurrentName = list[0].name;
        m_SMSProfile.SetProfile(m_CurrentName);
    }

    #region BASE_APP
    public override void SetText()
    {
        m_TMPSMS.text = m_SMSText.GetText(OSManager.Instance.GetLanguage());
        m_TMPEdit.text = m_EditText.GetText(OSManager.Instance.GetLanguage());
        foreach (SearchBar searchBar in m_SearchBars)
        {
            searchBar.SetText();
        }
    }

    public override void ResetApp()
    {
        m_MainBar.SetActive(true);
        m_MessageBar.SetActive(false);
        m_PopupView.SetActive(false);
    }
    #endregion

    #region JSON
    public void LoadMessages()
    {
        if (m_TopMsgList.Count > 0)
        {
            foreach (var topmsg in m_TopMsgList)
            {
                Destroy(topmsg);
            }
            m_TopMsgList.Clear();
        }

        if (File.Exists(m_Path + m_FileName))
        {
            // 1. JSON �б�
            string data = File.ReadAllText(m_Path + m_FileName);
            Debug.Log("[SMSManager] Raw JSON:\n" + data);

            // 2. �Ľ� �õ�
            m_MessageDB = JsonUtility.FromJson<MessageDB>(data);

            // 3. �Ľ� ����
            if (m_MessageDB == null)
            {
                Debug.LogError("FromJson ����: m_MessageDB == null");
                return;
            }
            if (m_MessageDB.messages == null)
            {
                Debug.LogError("FromJson ����: messages �ʵ尡 null");
                return;
            }

            // 4. ���������� �ε�� ���
            foreach (var message in m_MessageDB.messages)
            {
                InstantiatePreview(message);
            }

            Debug.Log($"[SMSManager] �޽��� {m_MessageDB.messages.Count}�� �ε� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("There is no Messages");
        }
    }


    private void SaveMessages()
    {
        string data = JsonUtility.ToJson(m_MessageDB, true /* prettyPrint */);
        File.WriteAllText(m_Path + m_FileName, data);
        Debug.Log(data);
    }
    #endregion

    #region UTILS
    private void InstantiatePreview(Message message)
    {
        for (int i = 0; i < m_TopMsgList.Count; i++)
        {
            var layout = m_TopMsgList[i].GetComponent<SMS_Layout>();
            if (layout.GetMessage().name == message.name)
            {
                layout.SetUp(message);
                return;
            }
        }

        GameObject go = Instantiate(m_MessagePreviewPrefab, m_MessagePreviewParent.transform);
        go.GetComponent<SMS_Layout>().SetUp(message);
        m_TopMsgList.Add(go);
    }

    private GameObject InstantiateMessage(Message message, bool isMine)
    {
        GameObject go;

        if (isMine)
        {
            go = Instantiate(m_PlayerMessagePrefab, m_MessageParent.transform);
        }
        else
        {
            //  �̹��� ���ҽ��� �����ϴ��� �˻�
            Sprite sprite = Resources.Load<Sprite>(message.message);

            if (sprite != null)
            {
                // �̹��� �޽��� ������
                go = Instantiate(m_IMGMessagePrefab, m_MessageParent.transform);
            }
            else
            {
                // �Ϲ� �ؽ�Ʈ �޽��� ������
                go = Instantiate(m_NPCMessagePrefab, m_MessageParent.transform);
            }
        }

        if (go != null)
        {
            go.GetComponent<Message_Layout>().SetUp(message);
            m_MessageList.Add(go);
        }

        return go;
    }

    #endregion

    private void DeletePrev()
    {
        if (m_MessageList.Count <= 0)
            return;
        foreach (var item in m_MessageList)
        {
            Destroy(item.gameObject);
        }
        m_MessageList.Clear();
    }

    // SMSManager.cs �ȿ� �߰�
    public void UpdateLastNPCMessage(string message)
    {
        if (m_MessageList.Count == 0)
            return;

        GameObject lastMessage = m_MessageList[m_MessageList.Count - 1];
        Message_Layout layout = lastMessage.GetComponent<Message_Layout>();
        layout.SetUp(message);
    }


    #region Dialaouge

    // ��ư ����
    public void DisplayChoiceButtons(List<(string text, int nextId)> choices)
    {
        ClearFixedButtons();

        int max = Mathf.Min(choices.Count, m_ChoiceButtonsFixed.Length);

        for (int i = 0; i < max; i++)
        {
            var (text, nextId) = choices[i];
            var button = m_ChoiceButtonsFixed[i];
            TMP_Text tmp = button.GetComponentInChildren<TMP_Text>();

            if (tmp != null)
                tmp.text = text;

            button.gameObject.SetActive(true);
            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(() =>
            {
                SaveMessage(text, true);
                ClearFixedButtons();

                if (dialogueController != null)
                    dialogueController.ProceedNext(nextId);
                else
                    Debug.LogWarning("DialogueController�� ����Ǿ� ���� �ʽ��ϴ�.");
            });
        }
    }



    public void ClearFixedButtons()
    {
        foreach (var button in m_ChoiceButtonsFixed)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }
    }

    public void SetupChoiceButton(GameObject buttonObj, string text, int nextId)
    {
        if (buttonObj == null)
        {
            Debug.LogWarning("SetupChoiceButton ȣ���: buttonObj�� null�Դϴ�.");
            return;
        }

        TMP_Text tmpText = buttonObj.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
            tmpText.text = text;
        else
            Debug.LogWarning("��ư�� TMP_Text ������Ʈ�� �����ϴ�.");

        Button button = buttonObj.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("��ư�� Button ������Ʈ�� �����ϴ�.");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            SaveMessage(text, true);
            ClearFixedButtons();

            if (dialogueController != null)
                dialogueController.ProceedNext(nextId);
            else
                Debug.LogWarning("DialogueController�� ����Ǿ� ���� �ʽ��ϴ�.");
        });
    }

    // ��ȭ ���� ���� ����
    public void SaveDialogueSlot(string eventName, int currentId, int slotIndex)
    {
        DialogueSaveData saveData = new DialogueSaveData(eventName, currentId);
        string path = Path.Combine(Application.persistentDataPath, $"save_slot_{slotIndex}.json");
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, json);
        Debug.Log($"[SMSManager] ���� �Ϸ�: ���� {slotIndex} / �̺�Ʈ: {eventName} / ID: {currentId}");
    }

    // ��ȭ ���� ���� �ҷ�����
    public DialogueSaveData LoadDialogueSlot(int slotIndex)
    {
        string path = Path.Combine(Application.persistentDataPath, $"save_slot_{slotIndex}.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SMSManager] ���� {slotIndex}�� ����� �����Ͱ� �����ϴ�.");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<DialogueSaveData>(json);
    }


    #endregion
}