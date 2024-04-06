using UnityEngine;
using UnityEditor;
using Unity.LiveCapture.Editor;
using UnityEngine.UIElements;

namespace Movella.Xsens.Editor
{
    [CustomEditor(typeof(XsensConnection), true)]
    public class XsensConnectionEditor : ConnectionEditor
    {
        static class Contents
        {
            public static readonly GUIContent DiagnosticsLabel = EditorGUIUtility.TrTextContent("Show Diagnostics", "Open the diagnostics window for analyzing the Xsens data stream.");
            public static readonly GUIContent DiagnosticsButton = EditorGUIUtility.TrTextContent("Diagnostics", "Open the diagnostics window for analyzing the Xsens data stream.");
            public static readonly GUILayoutOption[] ButtonOptions = { GUILayout.Width(160), GUILayout.Height(20) };
            public static readonly Color IndicatorConnectionPending = Color.yellow;
        }

        SerializedProperty m_StartOnPlayProperty;
        SerializedProperty m_PortProperty;

        XsensConnection m_Connection;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_StartOnPlayProperty = serializedObject.FindProperty(XsensConnection.Names.StartOnPlay);
            m_PortProperty = serializedObject.FindProperty(XsensConnection.Names.Port);

            m_Connection = target as XsensConnection;
        }

        protected override VisualElement CreateInfoGUI()
        {
            GetToolbar().Indicator.schedule.Execute(UpdateConnectionIndicator).Every(100);
            
            return base.CreateInfoGUI();
        }

        protected override VisualElement CreateSettingsGUI()
        {
            return CreateIMGUIContainer(DrawSettingsGUI);
        }

        void DrawSettingsGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_StartOnPlayProperty);
            DrawPortGUI();
            DrawDiagnosticsButton();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawPortGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(m_PortProperty);

                if (check.changed)
                    m_Connection.Stop();
            }
        }

        void DrawDiagnosticsButton()
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(Contents.DiagnosticsLabel);

                if (GUILayout.Button(Contents.DiagnosticsButton, Contents.ButtonOptions))
                    EditorWindow.GetWindow<XsensDiagnosticsWindow>().Show();
            }
        }

        protected override void OnConnectionChanged()
        {
            base.OnConnectionChanged();

            UpdateConnectionIndicator(); 
        }

        void UpdateConnectionIndicator()
        {
            var indicator = GetToolbar().Indicator;

            indicator.style.backgroundColor = m_Connection.IsEnabled() && !m_Connection.IsConnected ? 
                Contents.IndicatorConnectionPending : 
                StyleKeyword.Null;
        }
    }
}
