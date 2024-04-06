using UnityEngine;
using UnityEditor;
using Unity.LiveCapture;

namespace Movella.Xsens.Editor
{
    class XsensDiagnosticsWindow : EditorWindow
    {
        static readonly Vector2 k_WindowSize = new Vector2(300f, 100f);

        static class Contents
        {
            public static readonly GUIContent WindowTitle = EditorGUIUtility.TrTextContent("Xsens Diagnostics");
            public static readonly GUIContent NoConnection = EditorGUIUtility.TrTextContent("Not connected.");
            public static readonly GUIContent Stream = EditorGUIUtility.TrTextContent("Stream");
            public static readonly GUIContent Position = EditorGUIUtility.TrTextContent("Position");
            public static readonly GUIContent Rotation = EditorGUIUtility.TrTextContent("Rotation");
            public static readonly GUIContent Timecode = EditorGUIUtility.TrTextContent("Timecode");
            public static readonly GUIContent Segments = EditorGUIUtility.TrTextContent("Segments");
            public static readonly GUIContent Positions = EditorGUIUtility.TrTextContent("Positions");
            public static readonly GUIContent Orientations = EditorGUIUtility.TrTextContent("Orientations");
            public static readonly GUIContent Body = EditorGUIUtility.TrTextContent("Body");
            public static readonly GUIContent Props = EditorGUIUtility.TrTextContent("Props");
            public static readonly GUIContent Fingers = EditorGUIUtility.TrTextContent("Fingers");

            public static readonly GUIContent NoSegments = EditorGUIUtility.TrTextContent("This frame contains no segments.");
            public static readonly GUIContent NoPositions = EditorGUIUtility.TrTextContent("This frame contains no positions.");
            public static readonly GUIContent NoOrientations = EditorGUIUtility.TrTextContent("This frame contains no orientations.");
            public static readonly GUIContent MismatchedCount = EditorGUIUtility.TrTextContent("This frame has a mismatched count of segments, positions, and orientations.");
        }

        XsensConnection m_Connection;

        [SerializeField]
        Vector2 m_Scroll;

        (bool foldoutStream, bool foldoutSegments, bool foldoutBody, bool foldoutProps, bool foldoutFingers)[] m_Foldouts = new (bool, bool, bool, bool, bool)[XsensConstants.NumStreams];

        GUIStyle m_NormalBackgroundColor;
        GUIStyle m_AltBackgroundColor; 

        void OnEnable()
        {
            titleContent = Contents.WindowTitle;
            minSize = k_WindowSize;

            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;

            m_NormalBackgroundColor = new GUIStyle(); 

            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.gray);
            texture.Apply();

            m_AltBackgroundColor = new GUIStyle();
            m_AltBackgroundColor.normal.background = texture;
        }

        void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
        }

        void EditorUpdate()
        {
            if (m_Connection != null && m_Connection.IsConnected)
                Repaint();
        }

        void OnGUI()
        {
            EditorGUIUtility.wideMode = true;

            if ((m_Connection == null && !ConnectionManager.Instance.TryGetConnection(out m_Connection)) || 
                !m_Connection.IsConnected)
            {
                m_Scroll = Vector2.zero; 

                EditorGUILayout.HelpBox(Contents.NoConnection.text, MessageType.Info);
                return;
            }

            using var scrollView = new EditorGUILayout.ScrollViewScope(m_Scroll);
            m_Scroll = scrollView.scrollPosition; 

            DrawClientGUI(); 
        }

        void DrawClientGUI()
        {
            var client = m_Connection.Client;

            if (client != null && client.IsConnected)
            {
                for (int i = 0; i < XsensConstants.NumStreams; ++i)
                {
                    var frame = client.GetFrame(i);
                    DoFrameGUI(i, frame);
                }
            }
        }

        void DoFrameGUI(int index, FrameData frame)
        {
            m_Foldouts[index].foldoutStream = EditorGUILayout.Foldout(m_Foldouts[index].foldoutStream, $"Stream {index + 1}", EditorStyles.foldoutHeader);

            if (!m_Foldouts[index].foldoutStream)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                bool frameValid = true;

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(Contents.Timecode);
                    EditorGUILayout.LabelField(frame.TC.ToString());
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(Contents.Segments);
                    EditorGUILayout.LabelField(frame.SegmentCount.ToString());
                }

                if (frame.SegmentCount == 0)
                {
                    EditorGUILayout.HelpBox(Contents.NoSegments.text, MessageType.Error);
                    frameValid = false;
                }

                var numPositions = frame.Positions?.Length ?? 0;

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(Contents.Positions);
                    EditorGUILayout.LabelField(numPositions.ToString());
                }

                if (numPositions == 0)
                {
                    EditorGUILayout.HelpBox(Contents.NoPositions.text, MessageType.Error);
                    frameValid = false;
                }

                var numOrientations = frame.Orientations?.Length ?? 0;

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(Contents.Orientations);
                    EditorGUILayout.LabelField(numOrientations.ToString());
                }

                if (numOrientations == 0)
                {
                    EditorGUILayout.HelpBox(Contents.NoOrientations.text, MessageType.Error);
                    frameValid = false;
                }

                if (!(frame.SegmentCount == numPositions && frame.SegmentCount == numOrientations))
                {
                    EditorGUILayout.HelpBox(Contents.MismatchedCount.text, MessageType.Error);
                    frameValid = false;
                }

                if (!frameValid)
                    return;

                m_Foldouts[index].foldoutBody = EditorGUILayout.Foldout(m_Foldouts[index].foldoutBody, Contents.Body);

                if (m_Foldouts[index].foldoutBody)
                {
                    DrawFrameRange(frame,
                        0,
                        XsensConstants.MvnBodySegmentCount);
                }

                m_Foldouts[index].foldoutProps = EditorGUILayout.Foldout(m_Foldouts[index].foldoutProps, Contents.Props);

                if (m_Foldouts[index].foldoutProps && frame.NumProps > 0)
                {
                    DrawFrameRange(frame,
                        XsensConstants.MvnBodySegmentCount,
                        XsensConstants.MvnBodySegmentCount + Mathf.Min(XsensConstants.MvnPropSegmentCount, frame.NumProps));
                }

                m_Foldouts[index].foldoutFingers = EditorGUILayout.Foldout(m_Foldouts[index].foldoutFingers, Contents.Fingers);

                if (m_Foldouts[index].foldoutFingers)
                {
                    DrawFrameRange(frame,
                        XsensConstants.MvnBodySegmentCount + XsensConstants.MvnPropSegmentCount,
                        XsensConstants.MvnBodySegmentCount + XsensConstants.MvnPropSegmentCount + XsensConstants.MvnFingerSegmentCount);
                }
            }
        }

        void DrawFrameRange(FrameData frame, int start, int end)
        {
            if (start >= frame.SegmentCount)
                return;

            using (new EditorGUI.IndentLevelScope()) 
            using (new EditorGUI.DisabledScope(true))
            {
                for (int i = start; i < end && i < frame.SegmentCount; ++i)
                {
                    var style = (i % 2) == 0 ? m_NormalBackgroundColor : m_AltBackgroundColor;

                    using (new EditorGUILayout.VerticalScope(style))
                    {
                        var position = frame.Positions[i];
                        var rotation = frame.Orientations[i];

                        EditorGUILayout.Vector3Field(i.ToString(), position);
                        EditorGUILayout.Vector3Field(" ", rotation.eulerAngles);
                    }
                }
            }
        }
    }
}
