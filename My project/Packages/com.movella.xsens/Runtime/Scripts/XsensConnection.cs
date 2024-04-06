using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.LiveCapture;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Movella.Xsens
{
    [CreateConnectionMenuItem("Xsens Connection")]
    class XsensConnection : Connection
    {
#if UNITY_EDITOR
        // Used by XsensConnectionEditor
        internal static class Names
        {
            public const string Port = nameof(m_Port);
            public const string StartOnPlay = nameof(m_StartOnPlay);
        }
#endif

        [SerializeField]
        int m_Port = 9763; // default port in MVN Studio

        [SerializeField, Tooltip("Automatically start the connection after entering play mode.")]
        bool m_StartOnPlay = true;

        XsensClient m_Client;

        public override string GetName() => "Xsens Connection";
        public XsensClient Client => m_Client;

        public bool IsConnected => m_Client?.IsConnected ?? false;

        protected override void OnEnable()
        {
            base.OnEnable();

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
#endif
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Teardown();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Teardown();
        }

        void Teardown()
        {
            Stop();

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        void PlayModeStateChanged(PlayModeStateChange state)
        {
            if (m_StartOnPlay && state == PlayModeStateChange.EnteredPlayMode)
                Start();
        }
#endif
        public void Start()
        {
            m_Client = m_Client ?? new XsensClient();

            if (m_Client.IsConnected)
                m_Client.Disconnect();

            m_Client.Connect(m_Port);

            OnChanged(false);
        }

        public void Stop()
        {
            m_Client?.Disconnect();
            m_Client = null;
            
            OnChanged(false);
        }

        public override bool IsEnabled()
        {
            return m_Client is { IsStarted: true };
        }

        public override void SetEnabled(bool enabled)
        {
            if (enabled)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }
    }
}
