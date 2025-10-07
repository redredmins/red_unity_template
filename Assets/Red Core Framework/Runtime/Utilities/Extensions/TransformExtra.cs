using System;
using UnityEngine;

namespace RedMinS
{
    public static class TranstormExtention
    {

        public static void RemoveAllChildren(this Transform parent, ObjectPool pool)
        {
            foreach(Transform t in parent)
            {
                pool.RemoveObject(t.gameObject);
            }
        }

        public static void RemoveAllChildren(this RectTransform parent, ObjectPool pool)
        {
            foreach(RectTransform t in parent)
            {
                pool.RemoveObject(t.gameObject);
            }
        }
    }
}