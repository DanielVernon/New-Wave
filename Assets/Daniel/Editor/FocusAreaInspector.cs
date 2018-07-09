using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FocusArea))]
public class FocusAreaInspector : Editor {


    FocusArea obj;
    SerializedProperty getProperty(string name)
    {
        return serializedObject.FindProperty(name);
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, new string[] { getProperty("m_Script").name, getProperty("circular").name, getProperty("box").name, getProperty("xPos").name});

        switch (obj.focusType)
        {
            case FocusArea.FocusType.Circular:
                DoCircular();
                break;
            case FocusArea.FocusType.XPos:
                DoXPos();
                break;
            case FocusArea.FocusType.Box:
                DoBox();
                break;
            default:
                break;
        }
        serializedObject.ApplyModifiedProperties();
        EditorGUI.EndChangeCheck();
    }


    void DoCircular()
    { 
        EditorGUILayout.PropertyField(getProperty("circular"), true, new GUILayoutOption[0]);
    }

    void DoXPos()
    {
        EditorGUILayout.PropertyField(getProperty("xPos"), true, new GUILayoutOption[0]);
    }
    void DoBox()
    {
        EditorGUILayout.PropertyField(getProperty("box"), true, new GUILayoutOption[0]);
    }

    private void OnEnable()
    {
        obj = (FocusArea)target;
    }
}