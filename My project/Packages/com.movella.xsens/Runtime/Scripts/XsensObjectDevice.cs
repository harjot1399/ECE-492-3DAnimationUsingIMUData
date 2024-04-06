using UnityEngine;
using Unity.LiveCapture;
using Unity.LiveCapture.Mocap;

namespace Movella.Xsens
{
    [CreateDeviceMenuItem("Xsens Object Device")]
    [AddComponentMenu("MVN/Xsens Object Device")]
    class XsensObjectDevice : MocapDevice<FrameData>
    {
        [SerializeField]
        int m_StreamID; 

        [SerializeField]
        int m_ObjectID;

        public override bool IsReady() => Animator != null && IsConnected;

        public bool IsConnected => Client?.IsConnected ?? false;

        public int StreamID => m_StreamID;
        public int ObjectID => m_ObjectID;

        XsensConnection m_Connection;
        bool m_ClientInitialized;
        FrameData m_PreviousFrame; 

        XsensClient Client
        {
            get
            {
                if (m_Connection == null)
                    ConnectionManager.Instance.TryGetConnection(out m_Connection);

                return m_Connection?.Client;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            m_StreamID = Mathf.Clamp(m_StreamID, 0, XsensConstants.NumStreams - 1);
            m_ObjectID = Mathf.Max(0, m_ObjectID); 
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Animator == null)
                Animator = gameObject.GetOrAddComponent<Animator>();

            m_ClientInitialized = false; 

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.update -= EditorUpdate;
                UnityEditor.EditorApplication.update += EditorUpdate;
            }
#endif
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_ClientInitialized = false;

            var client = Client;

            if (client != null)
            {
                client.FrameDataReceivedAsync -= OnFrameDataReceivedAsync;
                client.Disconnected -= OnClientDisconnected;
            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= EditorUpdate;
#endif
        }

#if UNITY_EDITOR
        void EditorUpdate()
        {
            // Force the editor to update views while running in edit mode
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif

        bool InitClient()
        {
            var client = Client;

            if (client != null)
            {
                client.FrameDataReceivedAsync -= OnFrameDataReceivedAsync;
                client.FrameDataReceivedAsync += OnFrameDataReceivedAsync;
                client.Disconnected -= OnClientDisconnected;
                client.Disconnected += OnClientDisconnected;

                return true;
            }

            return false;
        }

        void OnClientDisconnected()
        {
            RestoreTPose();

            m_ClientInitialized = false;
        }

        protected override void UpdateDevice()
        {
            if (!m_ClientInitialized)
                m_ClientInitialized = InitClient(); 

            if (m_ClientInitialized && !SyncBuffer.IsSynchronized && IsReady())
            {
                var client = Client;

                client.FrameRate = TakeRecorder.FrameRate;

                var frame = client.GetFrame(m_StreamID);

                if (frame.SegmentCount == 0)
                    frame = m_PreviousFrame;

                if (frame.SegmentCount != 0)
                    AddFrame(frame, new FrameTimeWithRate(frame.FrameRate, frame.TC.ToFrameTime(frame.FrameRate)));
            }
        }

        bool OnFrameDataReceivedAsync(int streamID, FrameData frame)
        {
            if (SyncBuffer.IsSynchronized && m_StreamID == streamID)
            {
                // Timecode steps backwards on things like a looping animation
                if (frame.TC < m_PreviousFrame.TC)
                    ResetSyncBuffer();

                AddFrame(frame, new FrameTimeWithRate(frame.FrameRate, frame.TC.ToFrameTime(frame.FrameRate)));
                return true; // frame was consumed
            }

            return false; // frame was not consumed
        }

        protected override void ProcessFrame(FrameData frame)
        {
            var animator = Animator;

            if (animator == null)
                return;

            if (m_ObjectID >= frame.SegmentCount)
                return;

            var newPosition = frame.Positions[m_ObjectID]; 

            Vector3? pos = animator.applyRootMotion ? newPosition : 
                new Vector3(animator.transform.localPosition.x, newPosition.y, animator.transform.localPosition.z);

            Quaternion? rot = frame.Orientations[m_ObjectID];

            Present(animator.transform, pos, rot, null);

            m_PreviousFrame = frame;
        }

        internal void RestoreTPose()
        {
            var animator = Animator; 

            if (animator != null)
            {
                animator.transform.localPosition = Vector3.zero;
                animator.transform.localRotation = Quaternion.identity;
            }
        }
    }
}