using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(NestedVerticalScrollRect))]
public class EditorNestedVerticalScrollRect : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // �⺻ �ν����� ǥ��

        NestedVerticalScrollRect scrollRect = (NestedVerticalScrollRect)target;
        serializedObject.Update();

        // ���� �Ӽ� ǥ��
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ScrollRect"));

        serializedObject.ApplyModifiedProperties();
    }
}
