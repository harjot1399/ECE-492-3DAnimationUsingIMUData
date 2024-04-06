using System;
using UnityEngine;
using Unity.LiveCapture; 

namespace Movella.Xsens
{
    [Serializable]
    class PropRecorder
    {
        [SerializeField]
        [Tooltip("The relative tolerance, in percent, for reducing position keyframes")]
        float m_PositionError = 0.5f;

        [SerializeField]
        [Tooltip("The tolerance, in degrees, for reducing rotation keyframes")]
        float m_RotationError = 0.5f;

        Vector3 m_Position = Vector3.zero;
        Quaternion m_Rotation = Quaternion.identity;

#if LIVE_CAPTURE_4_PRE5
        Vector3Curve m_PositionCurve = new Vector3Curve();
        EulerCurve m_RotationCurve = new EulerCurve(); 
#else
        Vector3Curve m_PositionCurve = new Vector3Curve(string.Empty, "m_LocalPosition", typeof(Transform));
        EulerCurve m_RotationCurve = new EulerCurve(string.Empty, "m_LocalEulerAngles", typeof(Transform));
#endif

        public float PositionError
        {
            get => m_PositionError;
            set => m_PositionError = value;
        }

        public float RotationError
        {
            get => m_RotationError;
            set => m_RotationError = value;
        }

        public void Validate()
        {
            m_PositionError = Mathf.Clamp(m_PositionError, 0f, 100f);
            m_RotationError = Mathf.Clamp(m_RotationError, 0f, 10f);
        }

        public void Prepare(Animator prop, FrameRate frameRate)
        {
            if (prop != null)
            {
                m_Position = prop.transform.localPosition;
                m_Rotation = prop.transform.localRotation;
            }

            m_PositionCurve.Clear(); 
            m_PositionCurve.FrameRate = frameRate;
            m_PositionCurve.MaxError = PositionError / 100f;

            m_RotationCurve.Clear(); 
            m_RotationCurve.FrameRate = frameRate;
            m_RotationCurve.MaxError = RotationError;
        }

        public AnimationClip Bake()
        {
            var clip = new AnimationClip();

#if LIVE_CAPTURE_4_PRE5
            m_PositionCurve.SetToAnimationClip(new PropertyBinding(string.Empty, "m_LocalPosition", typeof(Transform)), clip);
            m_RotationCurve.SetToAnimationClip(new PropertyBinding(string.Empty, "m_LocalEulerAngles", typeof(Transform)), clip); 
#else
            m_PositionCurve.SetToAnimationClip(clip);
            m_RotationCurve.SetToAnimationClip(clip);
#endif

            m_PositionCurve.Clear();
            m_RotationCurve.Clear(); 

            return clip;
        }

        public void Present(Vector3 position, Quaternion rotation)
        {
            m_Position = position;
            m_Rotation = rotation;
        }

        public void ApplyFrame(Animator prop)
        {
            if (prop != null)
            {
                prop.transform.localPosition = m_Position;
                prop.transform.localRotation = m_Rotation; 
            }
        }

        public void Record(double elapsedTime)
        {
            m_PositionCurve.AddKey(elapsedTime, m_Position);
            m_RotationCurve.AddKey(elapsedTime, m_Rotation);
        }
    }
}
