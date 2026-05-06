using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TMP_FontChanger : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset fontAsset = null;

    public TMP_FontAsset GetFontAsset() => fontAsset;
}

#if UNITY_EDITOR
[CustomEditor(typeof(TMP_FontChanger))]
public class TMP_FontChangerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Change Font!"))
        {   
            TMP_FontChanger changer = (TMP_FontChanger)target;
            TMP_FontAsset fontAsset = changer.GetFontAsset();

            if (fontAsset == null)
            {
                Debug.LogWarning("[TMP_FontChanger] FontAsset이 설정되지 않았습니다.");
                return;
            }

            foreach (TextMeshPro textMeshPro3D in GameObject.FindObjectsOfType<TextMeshPro>(true))
            {
                textMeshPro3D.font = fontAsset;
            }
            foreach (TextMeshProUGUI textMeshProUi in GameObject.FindObjectsOfType<TextMeshProUGUI>(true))
            {
                textMeshProUi.font = fontAsset;
            }
        }
    }
}
#endif