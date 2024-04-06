using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movella.Xsens
{
    public static class Utilities
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go == null) return null;

            if (!go.TryGetComponent<T>(out T comp))
            {
                comp = go.AddComponent<T>();
            }

            return comp;
        }
    }
}