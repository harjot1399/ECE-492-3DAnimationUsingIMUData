using System;
using UnityEngine;
using UnityEditor;
using Unity.LiveCapture.Mocap.Editor;
using Unity.LiveCapture.Editor;

namespace Movella.Xsens.Editor
{
    [CustomEditor(typeof(XsensDevice))]
    class XsensDeviceEditor : MocapDeviceEditor
    {
        static readonly string IconPath = "Packages/com.movella.xsens/Editor/Icons";

        static readonly string k_NotConnectedText = L10n.Tr(
            "Not connected." + Environment.NewLine + Environment.NewLine +
            "Some reasons for this are:" + Environment.NewLine +
            "• Unable to find an Xsens connection" + Environment.NewLine +
            "• The Xsens connection is not active" + Environment.NewLine +
            "• The port is invalid"
            );

        static class Contents
        {
            public static readonly GUIContent Props = EditorGUIUtility.TrTextContent("Props");
            public static readonly GUIContent Prop = EditorGUIUtility.TrTextContent("Prop");
            public static readonly GUIContent Connections = EditorGUIUtility.TrTextContent("Connections", "Show the connections window.");
            public static readonly GUIContent Diagnostics = EditorGUIUtility.TrTextContent("Diagnostics", "Open the diagnostics window for analyzing the Xsens data stream.");
            public static readonly GUIContent CharacterID = EditorGUIUtility.TrTextContent("Character ID", "The character ID this device should be associated with.");
            public static readonly GUIContent RestoreTPose = EditorGUIUtility.TrTextContent("Restore TPose");
            public static readonly GUILayoutOption[] ButtonOptions = { GUILayout.Width(160), GUILayout.Height(24) };
        }

        static class Styles
        {
            public static GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                margin = new RectOffset(0, 0, 6, 6)
            };
        }

        XsensDevice m_Device;

        SerializedProperty m_CharacterID;
        SerializedProperty m_Props;

        bool m_PropsFoldout;

        Texture m_Logo;

        GUIContent[] m_CharacterIDs;
        int[] m_CharacterIDValues;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Logo = (Texture)AssetDatabase.LoadAssetAtPath($"{IconPath}/XsensLogo.PNG", typeof(Texture));

            m_Device = (XsensDevice)target;

            m_CharacterID = serializedObject.FindProperty("m_CharacterID");
            m_Props = serializedObject.FindProperty("m_Props");

            m_CharacterIDs = new GUIContent[XsensConstants.MaxCharacters];
            m_CharacterIDValues = new int[XsensConstants.MaxCharacters];

            for (int i = 0; i < m_CharacterIDs.Length; i++)
            {
                m_CharacterIDValues[i] = i;
                m_CharacterIDs[i] = new GUIContent((i+1).ToString());
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawLogo();

            if (!m_Device.IsConnected)
                EditorGUILayout.HelpBox(k_NotConnectedText, MessageType.Warning);

            DrawButtons();

            DrawCharacterID();

            DrawPropertiesExcluding(serializedObject, "m_Script", "m_CharacterID", "m_Props");

            DrawProps();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawLogo()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_Logo, GUILayout.MinWidth(1));
                GUILayout.FlexibleSpace();
            }
        }

        void DrawButtons()
        {
            GUILayout.Space(5);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(Contents.Connections, Contents.ButtonOptions))
                    ConnectionsWindow.ShowWindow();

                if (GUILayout.Button(Contents.Diagnostics, Contents.ButtonOptions))
                    EditorWindow.GetWindow<XsensDiagnosticsWindow>().Show();

                if (GUILayout.Button(Contents.RestoreTPose, Contents.ButtonOptions))
                    m_Device.RestoreTPose();

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(5);
        }

        void DrawCharacterID()
        {
            EditorGUILayout.IntPopup(m_CharacterID, m_CharacterIDs, m_CharacterIDValues);
        }

        void DrawProps()
        {
            m_PropsFoldout = EditorGUILayout.Foldout(m_PropsFoldout, Contents.Props);

            if (m_PropsFoldout)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    for (int i = 0; i < XsensConstants.MvnPropSegmentCount; ++i)
                    {
                        var prop = m_Props.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(prop, new GUIContent($"{Contents.Prop} {i+1}"));
                    }
                }
            }
        }
    }
}
