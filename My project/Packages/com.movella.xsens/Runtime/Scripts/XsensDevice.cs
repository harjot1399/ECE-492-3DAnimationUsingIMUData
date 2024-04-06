using UnityEngine;
using Unity.LiveCapture;
using Unity.LiveCapture.Mocap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;

namespace Movella.Xsens
{
    [CreateDeviceMenuItem("Xsens Actor Device")]
    [AddComponentMenu("MVN/Xsens Actor Device")]
    class XsensDevice : MocapDevice<FrameData>
    {
        [SerializeField]
        int m_CharacterID;

        [SerializeField, Tooltip("The global channels of mocap data to apply to this source.")]
        [EnumFlagButtonGroup(60f)]
        JointFlags m_Channels = JointFlags.Position | JointFlags.Rotation;

        [Serializable]
        struct PropInfo
        {
            public GameObject gameObject;
            public XsBodyAnimationSegment segmentType;
        }

        [SerializeField]
        PropInfo[] m_Props = new PropInfo[XsensConstants.MvnPropSegmentCount];

        PropRecorder[] m_PropRecorders;

        [SerializeField, HideInInspector]
        Avatar m_AvatarCache;

        XsensConnection m_Connection;

        bool m_ClientInitialized;
        bool m_ModelInitialized;

        (Transform transform, Vector3 tposePosition, Quaternion tPoseRotation)[] m_Model;  // Model segments with TPose rotation
        (Transform transform, Quaternion rot)[] m_OriginalRotations;

        // If we get no data from the client in the current frame, use the previous frame to avoid hiccups
        FrameData m_PreviousFrame;

        double? m_FirstFrameTime = null;

        int[] m_BodySegmentOrder; // The body segment order
        int[] m_FingerSegmentOrder;  // The finger segment order

        public override bool IsReady() => Animator != null && IsConnected;

        public bool IsConnected => Client?.IsConnected ?? false;

        public int CharacterID => m_CharacterID;

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

            m_CharacterID = Mathf.Clamp(m_CharacterID, 0, XsensConstants.MaxCharacters - 1);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Animator == null)
                Animator = gameObject.GetOrAddComponent<Animator>();
            
            m_ModelInitialized = false;
            m_ClientInitialized = false;

            m_BodySegmentOrder = Enum.GetValues(typeof(XsBodyAnimationSegment)).Cast<int>().ToArray();
            m_FingerSegmentOrder = Enum.GetValues(typeof(XsFingerAnimationSegment)).Cast<int>().ToArray();

            var maxSegments = XsensConstants.MvnBodySegmentCount + XsensConstants.MvnFingerSegmentCount + XsensConstants.MvnPropSegmentCount;

            m_Model = new (Transform, Vector3, Quaternion)[maxSegments];
            m_OriginalRotations = new (Transform, Quaternion)[maxSegments];

            if (Animator != null && Animator.avatar == null && m_AvatarCache != null)
                Animator.avatar = m_AvatarCache;

            m_PropRecorders = new PropRecorder[XsensConstants.MvnPropSegmentCount];

            for (int i = 0; i < m_PropRecorders.Length; i++)
                m_PropRecorders[i] = new PropRecorder();

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

            m_ModelInitialized = false;
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

        bool InitModel()
        {
            var animator = Animator;

            if (animator == null)
                return false;

            // If animator doesn't have an avatar, and we don't have a cached avatar, we have nothing to work with
            if (animator.avatar == null)
            {
                Debug.LogError($"{nameof(XsensDevice)}: {Animator.name} is missing an avatar. An avatar is required to bind incoming Xsens data to their correct bone destinations.");
                return false;
            }

            //zero out model before populating it
            for (int i = 0; i < m_Model.Length; ++i)
                m_Model[i] = (null, Vector3.zero, Quaternion.identity);

            //the model must be zeroed out before calling this so props don't get set with bad values
            RestoreTPose();

            //go through the model's body segments and store values
            for (int i = 0; i < XsensConstants.MvnBodySegmentCount; i++)
            {
                var segID = m_BodySegmentOrder[i];
                HumanBodyBones boneID = XsensConstants.BodyMecAnimBones[(XsBodyAnimationSegment)m_BodySegmentOrder[i]];

                try
                {
                    if (boneID == HumanBodyBones.LastBone)
                        continue;

                    Vector3 tempPos = animator.transform.position;
                    Quaternion tempRot = animator.transform.rotation;

                    animator.transform.position = Vector3.zero;
                    animator.transform.rotation = Quaternion.identity;

                    var bone = animator.GetBoneTransform(boneID);

                    if (bone != null)
                        m_Model[segID] = (bone, bone.position, bone.rotation);

                    animator.transform.position = tempPos;
                    animator.transform.rotation = tempRot;
                }
                catch (Exception e)
                {
                    Debug.Log($"{nameof(XsensDevice)}: Error processing [{boneID}] in the model. Exception: {e}");
                }
            }

            //go through the model's finger segments and store values
            for (int i = 0; i < XsensConstants.MvnFingerSegmentCount; i++)
            {
                var segID = m_FingerSegmentOrder[i] + XsensConstants.MvnBodySegmentCount + XsensConstants.MvnPropSegmentCount;
                HumanBodyBones boneID = XsensConstants.FingerMecAnimBones[(XsFingerAnimationSegment)m_FingerSegmentOrder[i]];

                try
                {
                    if (boneID == HumanBodyBones.LastBone)
                        continue;

                    Vector3 tempPos = animator.transform.position;
                    Quaternion tempRot = animator.transform.rotation;

                    animator.transform.position = Vector3.zero;
                    animator.transform.rotation = Quaternion.identity;

                    var bone = animator.GetBoneTransform(boneID);

                    if (bone != null)
                        m_Model[segID] = (bone, bone.position, bone.rotation);

                    animator.transform.position = tempPos;
                    animator.transform.rotation = tempRot;
                }
                catch (Exception e)
                {
                    Debug.Log($"{nameof(XsensDevice)}: Error processing [{boneID}] in the model. Exception: {e}");
                }
            }

            // go through props and store values
            for (int i = 0; i < XsensConstants.MvnPropSegmentCount; ++i)
            {
                var propID = XsensConstants.MvnBodySegmentCount + i;

                var pinfo = m_Props[i]; 

                if (pinfo.gameObject != null)
                {
                    var tpose = pinfo.gameObject.GetOrAddComponent<TPose>();
                    tpose.RefreshTPose(); 
                    m_Model[propID] = (pinfo.gameObject.transform, tpose.Position, tpose.Rotation);
                }
                else
                {
                    m_Model[propID] = (null, Vector3.zero, Quaternion.identity); 
                }
            }

            return true;
        }

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

        protected override void UpdateDevice()
        {
            var animator = Animator;

            if (animator == null)
                return;

            if (animator.avatar != null)
                m_AvatarCache = animator.avatar;

            if (!m_ModelInitialized)
                m_ModelInitialized = InitModel();

            if (!m_ClientInitialized)
                m_ClientInitialized = InitClient();

            if (m_ModelInitialized && m_ClientInitialized && !SyncBuffer.IsSynchronized && IsReady())
            {
                var client = Client;

                client.FrameRate = TakeRecorder.FrameRate;

                var frame = client.GetFrame(m_CharacterID);

                if (frame.SegmentCount == 0)
                    frame = m_PreviousFrame;

                if (frame.SegmentCount != 0)
                    AddFrame(frame, new FrameTimeWithRate(frame.FrameRate, frame.TC.ToFrameTime(frame.FrameRate)));
            }
        }

        bool OnFrameDataReceivedAsync(int characterID, FrameData frame)
        {
            if (SyncBuffer.IsSynchronized && characterID == m_CharacterID)
            {
                // Timecode steps backwards on things like a looping animation
                if (frame.TC < m_PreviousFrame.TC)
                    ResetSyncBuffer();

                AddFrame(frame, new FrameTimeWithRate(frame.FrameRate, frame.TC.ToFrameTime(frame.FrameRate)));
                return true; // frame was consumed
            }

            return false; // frame was not consumed
        }

        void OnClientDisconnected()
        {
            RestoreTPose();

            m_ClientInitialized = false;
        }

        protected override void OnRecordingChanged()
        {
            m_FirstFrameTime = null;

            if (IsRecording)
            {
                var frameRate = TakeRecorder.FrameRate;

                for (int i = 0; i < m_Props.Length; i++)
                {
                    var recorder = m_PropRecorders[i];
                    recorder.Prepare(m_Props[i].gameObject.GetOrAddComponent<Animator>(), frameRate);
                }
            }
        }

        protected override void LiveUpdate()
        {
            base.LiveUpdate();

            for (int i = 0; i < m_Props.Length; ++i)
                m_PropRecorders[i].ApplyFrame(m_Props[i].gameObject.GetOrAddComponent<Animator>());
        }

        protected override void ProcessFrame(FrameData frame)
        {
            var animator = Animator;

            if (animator == null)
                return;

            var inverseActor = Quaternion.Inverse(animator.transform.rotation);

            try
            {
                var flags = m_Channels;

                // Validate prop tpose and check if prop(s) changed; must be done before caching rotations
                for (int i = 0; i < m_Props.Length; ++i)
                {
                    var prop = m_Props[i].gameObject ? m_Props[i].gameObject.transform : null;

                    TPose tpose = null; 

                    // Make sure prop has a TPose component
                    if (prop != null && !prop.TryGetComponent(out tpose))
                    {
                        tpose = prop.gameObject.AddComponent<TPose>();
                        tpose.SaveTPose();
                    }

                    var propID = XsensConstants.MvnBodySegmentCount + i;
                    var model = m_Model[propID].transform;

                    if (model != prop)
                    {
                        if (model != null)
                        {
                            if (model.TryGetComponent<TPose>(out var modelPose))
                                modelPose.RefreshTPose(); 
                        }

                        if (prop != null)
                        {
                            m_Model[propID] = (prop, tpose.Position, tpose.Rotation);
                        }
                        else
                        {
                            m_Model[propID] = (null, Vector3.zero, Quaternion.identity);
                        }
                    }
                }

                // cache original rotations
                for (int i = 0; i < m_Model.Length; ++i)
                {
                    var bone = m_Model[i].transform;
                    m_OriginalRotations[i] = (bone, bone ? bone.localRotation : Quaternion.identity);
                }

                // body segments
                for (int i = 0; i < m_BodySegmentOrder.Length; i++)
                {
                    var bodyID = m_BodySegmentOrder[i];
                    var bone = m_Model[bodyID].transform;

                    if (bone == null)
                        continue;

                    Vector3? localPosition = null;

                    if (flags.HasFlag(JointFlags.Position))
                    {
                        if (XsBodyAnimationSegment.Pelvis == (XsBodyAnimationSegment)bodyID)
                        {
                            var invParent = (bone.parent && bone.parent != animator.transform) ?
                                            Quaternion.Inverse(Quaternion.Inverse(animator.transform.rotation) * bone.parent.transform.rotation) :
                                            Quaternion.identity;

                            var newPosition = invParent * (frame.Positions[bodyID] / bone.lossyScale.y);

                            localPosition = animator.applyRootMotion ? newPosition :
                                new Vector3(bone.localPosition.x, newPosition.y, bone.localPosition.z);
                        }
                    }

                    Quaternion? localRotation = null;

                    if (flags.HasFlag(JointFlags.Rotation))
                    {
                        var parentRotation = bone.parent ? bone.parent.rotation : Quaternion.identity;
                        var inverseParent = Quaternion.Inverse(inverseActor * parentRotation);
                        localRotation = inverseParent * (frame.Orientations[bodyID] * m_Model[i].tPoseRotation);

                        bone.localRotation = localRotation.Value;
                    }

                    Present(bone, localPosition, localRotation, null);
                }

                // finger segments
                if (frame.SegmentCount > XsensConstants.MvnPropSegmentCount + XsensConstants.MvnBodySegmentCount)
                {
                    for (int i = 0; i < m_FingerSegmentOrder.Length; i++)
                    {
                        var boneID = i + XsensConstants.MvnBodySegmentCount + XsensConstants.MvnPropSegmentCount;
                        var bone = m_Model[boneID].transform;

                        if (bone == null)
                            continue;

                        Quaternion? localRotation = null;

                        if (flags.HasFlag(JointFlags.Rotation))
                        {
                            var parentRotation = bone.parent ? bone.parent.rotation : Quaternion.identity;
                            var inverseParent = Quaternion.Inverse(inverseActor * parentRotation);
                            var newOrientation = frame.Orientations[frame.Orientations.Length - XsensConstants.MvnFingerSegmentCount + i];

                            localRotation = inverseParent * (newOrientation * m_Model[boneID].tPoseRotation);

                            bone.localRotation = localRotation.Value;
                        }

                        Present(bone, null, localRotation, null);
                    }
                }

                // props
                for (int i = 0; i < m_Props.Length; ++i)
                {
                    var propID = XsensConstants.MvnBodySegmentCount + i;

                    if (i >= frame.NumProps)
                        continue; 

                    if (propID > frame.SegmentCount)
                        continue;

                    var prop = m_Props[i].gameObject ? m_Props[i].gameObject.transform : null;

                    if (prop != null)
                    {
                        var type = m_Props[i].segmentType;
                        var parent = GetSegmentTransform(type);

                        if (parent == null)
                            continue;

                        var parentRotation = inverseActor * parent.rotation;

                        var oldPos = prop.position;
                        var oldRot = prop.rotation; 

                        // world space position & rotation
                        var rot = Quaternion.Inverse(parentRotation) * frame.Orientations[propID] * m_Model[propID].tPoseRotation;

                        var propOffset = (frame.Positions[propID] - frame.Positions[(int)type]) * animator.transform.localScale.x;
                        var handRot = Quaternion.Inverse(parentRotation) * frame.Orientations[(int)type] * m_Model[(int)type].tPoseRotation;
                        propOffset = animator.transform.rotation * handRot * propOffset;
                        var pos = parent.position + propOffset;

                        // apply world space to prop then extract local space from it for the recorder
                        prop.position = pos;
                        prop.rotation = parent.rotation * rot;

                        var recorder = m_PropRecorders[i];

                        recorder.Present(prop.localPosition, prop.localRotation);

                        prop.transform.position = oldPos;
                        prop.transform.rotation = oldRot; 

                        if (IsRecording)
                        {
                            var time = frame.TC.ToSeconds(frame.FrameRate);
                            m_FirstFrameTime ??= time;

                            recorder.Record(time - m_FirstFrameTime.Value);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                // restore original rotations from cache
                for (int i = 0; i < m_OriginalRotations.Length; ++i)
                {
                    var bone = m_OriginalRotations[i].transform;

                    if (bone != null)
                        bone.localRotation = m_OriginalRotations[i].rot;
                }
            }

            m_PreviousFrame = frame;
        }

        internal void RestoreTPose()
        {
            var animator = Animator;

            if (animator == null || animator.avatar == null)
                return;

            //body and fingers t-pose can be restored from the avatar skeleton
            var skeletons = animator.avatar.humanDescription.skeleton;

            if (skeletons == null)
                return;

            var tfs = animator.GetComponentsInChildren<Transform>();
            var dir = new Dictionary<string, Transform>(tfs.Count());

            foreach (var tf in tfs)
            {
                if (!dir.ContainsKey(tf.name))
                    dir.Add(tf.name, tf);
            }

            foreach (var skeleton in skeletons)
            {
                if (!dir.TryGetValue(skeleton.name, out var bone))
                    continue;

                bone.localPosition = skeleton.position;
                bone.localRotation = skeleton.rotation;
                bone.localScale = skeleton.scale;
            }

            for (int i = 0; i < XsensConstants.MvnPropSegmentCount; ++i)
            {
                var propID = XsensConstants.MvnBodySegmentCount + i;
                var prop = m_Model[propID].transform;

                if (prop != null)
                {
                    var tpose = prop.GetComponent<TPose>();

                    if (tpose != null)
                        tpose.RefreshTPose(); 
                }
            }
        }

        public Transform GetSegmentTransform(XsBodyAnimationSegment segmentType)
        {
            var index = (int)segmentType;

            if (index >= 0 && index < m_Model.Length)
                return m_Model[index].transform;

            return null;
        }

        public override void Write(ITakeBuilder takeBuilder)
        {
            if (Animator == null)
                return;

            base.Write(takeBuilder);

            // Add avatar track
            var animatorBinding = new AnimatorTakeBinding();
            animatorBinding.SetName(Animator.name);

            var track = takeBuilder.CreateTrack<AvatarTrack>("Avatar Track", animatorBinding);
            var clip = track.CreateDefaultClip();

            clip.displayName = "Avatar";
            clip.start = takeBuilder.ContextStartTime;

            var tracks = track.timelineAsset.GetRootTracks();
            double duration = 0;

            foreach (var t in tracks)
            {
                if (t == track)
                    continue;

                if (t.duration > duration)
                    duration = t.duration;
            }

            clip.duration = duration;

            var avatar = Animator.avatar;

            if (avatar == null)
                avatar = m_AvatarCache;

            if (avatar != null)
            {
                var director = GetComponentInParent<PlayableDirector>();

                if (director != null)
                {
                    var asset = clip.asset as AvatarPlayableAsset;
                    asset.Avatar.exposedName = UnityEditor.GUID.Generate().ToString();

                    director.SetReferenceValue(asset.Avatar.exposedName, avatar);
                }
            }

            // Add prop tracks
            for (int i = 0; i < m_Props.Length; ++i)
            {
                var propAnimator = m_Props[i].gameObject.GetOrAddComponent<Animator>();

                if (propAnimator != null)
                {
                    takeBuilder.CreateAnimationTrack(propAnimator.name, propAnimator, m_PropRecorders[i].Bake(), alignTime: m_FirstFrameTime);
                }
            }
        }
    }
}
