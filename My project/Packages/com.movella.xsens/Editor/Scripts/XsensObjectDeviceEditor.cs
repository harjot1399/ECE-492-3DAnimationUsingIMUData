using System;
using UnityEngine;
using UnityEditor;
using Unity.LiveCapture.Mocap.Editor;
using Unity.LiveCapture.Editor;

namespace Movella.Xsens.Editor
{
    [CustomEditor(typeof(XsensObjectDevice))]
    class XsensObjectDeviceEditor : MocapDeviceEditor
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
            public static readonly GUIContent Connections = EditorGUIUtility.TrTextContent("Connections", "Show the connections window.");
            public static readonly GUIContent Diagnostics = EditorGUIUtility.TrTextContent("Diagnostics", "Open the diagnostics window for analyzing the Xsens data stream.");
            public static readonly GUIContent StreamID = EditorGUIUtility.TrTextContent("Stream ID", "The stream ID this device should be associated with.");
            public static readonly GUIContent ObjectID = EditorGUIUtility.TrTextContent("Object ID", "The object ID this device should be associated with.");
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

        XsensObjectDevice m_Device;

        SerializedProperty m_StreamID;
        SerializedProperty m_ObjectID;

        Texture m_Logo;
        GUIContent[] m_StreamIDs;
        int[] m_StreamIDValues;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Logo = (Texture)AssetDatabase.LoadAssetAtPath($"{IconPath}/XsensLogo.PNG", typeof(Texture));

            m_Device = (XsensObjectDevice)target;

            m_StreamID = serializedObject.FindProperty("m_StreamID");
            m_ObjectID = serializedObject.FindProperty("m_ObjectID");

            m_StreamIDs = new GUIContent[XsensConstants.NumStreams];
            m_StreamIDValues = new int[XsensConstants.NumStreams];

            for (int i = 0; i < m_StreamIDs.Length; i++)
            {
                m_StreamIDValues[i] = i;
                m_StreamIDs[i] = new GUIContent((i + 1).ToString());
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawLogo();

            if (!m_Device.IsConnected)
                EditorGUILayout.HelpBox(k_NotConnectedText, MessageType.Warning);

            DrawButtons();

            DrawStreamID();
            DrawObjectID(); 

            DrawPropertiesExcluding(serializedObject, "m_Script", "m_StreamID", "m_ObjectID");

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

        void DrawStreamID()
        {
            EditorGUILayout.IntPopup(m_StreamID, m_StreamIDs, m_StreamIDValues, Contents.StreamID);
        }

        void DrawObjectID()
        {
            var objectValue = m_ObjectID.intValue + 1;

            var newValue = EditorGUILayout.IntField(Contents.ObjectID, objectValue);

            if (newValue != objectValue)
                m_ObjectID.intValue = Mathf.Clamp(newValue - 1, 0, XsensConstants.MvnBodySegmentCount-1);
        }
    }
}