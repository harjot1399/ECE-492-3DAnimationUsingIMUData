using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movella.Xsens
{
    class TPose : MonoBehaviour
    {
        [SerializeField]
        Vector3 m_Position;

        [SerializeField]
        Quaternion m_Rotation = Quaternion.identity;

        [SerializeField]
        Vector3 m_Scale;

        [SerializeField, HideInInspector]
        bool m_Initialized;

        public Vector3 Position => m_Position;
        public Quaternion Rotation => m_Rotation;
        public Vector3 Scale => m_Scale;

        public void RefreshTPose()
        {
            if (m_Initialized)
            {
                RestoreTPose();
            }
            else
            {
                SaveTPose(); 
            }
        }

        public void RestoreTPose()
        {
            transform.localPosition = m_Position;
            transform.localRotation = m_Rotation;
            transform.localScale = m_Scale;
        }

        public void SaveTPose()
        {
            m_Position = transform.localPosition;
            m_Rotation = transform.localRotation;
            m_Scale = transform.localScale;

            m_Initialized = true;
        }
    }
}