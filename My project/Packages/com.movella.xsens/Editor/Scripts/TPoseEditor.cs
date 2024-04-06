using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Movella.Xsens.Editor
{
    [CustomEditor(typeof(TPose))]
    class TPoseEditor : UnityEditor.Editor
    {
        static readonly GUIContent m_SaveButtonContent = new GUIContent("Save as TPose");
        static readonly GUIContent m_RestoreButtonContent = new GUIContent("Restore TPose");

        SerializedProperty m_Position; 
        SerializedProperty m_Rotation;
        SerializedProperty m_Scale;

        private void OnEnable()
        {
            m_Position = serializedObject.FindProperty("m_Position");
            m_Rotation = serializedObject.FindProperty("m_Rotation");
            m_Scale = serializedObject.FindProperty("m_Scale"); 
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var tpose = (TPose)target; 

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(m_Position);
                EditorGUILayout.PropertyField(m_Rotation);
                EditorGUILayout.PropertyField(m_Scale);

                if (check.changed)
                {
                    Undo.RecordObject(tpose, "TPose changed"); 
                    tpose.RefreshTPose();
                }
            }

            if (GUILayout.Button(m_SaveButtonContent))
                tpose.SaveTPose();

            if (GUILayout.Button(m_RestoreButtonContent))
                tpose.RestoreTPose(); 
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}