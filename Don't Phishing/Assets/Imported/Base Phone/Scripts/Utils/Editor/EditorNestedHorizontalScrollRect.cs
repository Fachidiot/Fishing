using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NestedHorizontalScrollRect))]
public class EditorNestedHorizontalScrollRect : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // �⺻ �ν����� ǥ��

        NestedHorizontalScrollRect scrollRect = (NestedHorizontalScrollRect)target;
        serializedObject.Update();

        // ���� �Ӽ� ǥ��
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ScrollRect"));

        serializedObject.ApplyModifiedProperties();
    }
}
