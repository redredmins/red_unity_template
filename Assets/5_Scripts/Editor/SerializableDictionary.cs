using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RedMinS
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new List<TKey>();
        [SerializeField] private List<TValue> values = new List<TValue>();

        // Expose for drawers
        internal List<TKey> KeysList => keys;
        internal List<TValue> ValuesList => values;

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var kv in this)
            {
                keys.Add(kv.Key);
                values.Add(kv.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            var count = Math.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++)
            {
                // Avoid crashing on duplicate keys in serialized data
                if (!ContainsKey(keys[i]))
                    Add(keys[i], values[i]);
            }
        }
    }

    [Serializable]
    public class SerializableStringGameObjectDictionary : SerializableDictionary<string, GameObject>
    {
        [SerializeField] public bool autoSort = true;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SerializableStringGameObjectDictionary))]
    public class SerializableStringGameObjectDictionaryDrawer : PropertyDrawer
    {
        const float Pad = 2f;
        const float RemoveBtnWidth = 22f;
        const float KeyMinWidth = 100f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var keys = property.FindPropertyRelative("keys");
            int rows = (keys != null ? keys.arraySize : 0) + 2; // header + toggle + items
            float line = EditorGUIUtility.singleLineHeight + Pad;
            return rows * line + Pad; // some bottom space
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var lineH = EditorGUIUtility.singleLineHeight;

            var keysProp = property.FindPropertyRelative("keys");
            var valsProp = property.FindPropertyRelative("values");
            var sortProp = property.FindPropertyRelative("autoSort");
            if (keysProp == null || valsProp == null || sortProp == null)
            {
                EditorGUI.HelpBox(position, "Internal lists not found.", MessageType.Error);
                EditorGUI.EndProperty();
                return;
            }

            // Header line with label + Add button
            var header = new Rect(position.x, position.y, position.width, lineH);
            DrawHeader(header, label, keysProp);

            // Toggle line for auto-sort
            var toggleRect = new Rect(position.x, position.y + lineH + Pad, position.width, lineH);
            sortProp.boolValue = EditorGUI.ToggleLeft(toggleRect, "Auto-sort keys alphabetically", sortProp.boolValue);

            // Auto-sort keys alphabetically (and sync values) if enabled
            if (sortProp.boolValue)
            {
                AutoSort(keysProp, valsProp);
            }

            float y = toggleRect.y + lineH + Pad;
            for (int i = 0; i < keysProp.arraySize; i++)
            {
                var rowRect = new Rect(position.x, y, position.width, lineH);
                DrawRow(rowRect, keysProp, valsProp, i);
                y += lineH + Pad;
            }

            EditorGUI.EndProperty();
        }

        private void DrawHeader(Rect rect, GUIContent label, SerializedProperty keysProp)
        {
            var left = rect; left.width -= 60f;
            var right = new Rect(rect.xMax - 60f, rect.y, 60f, rect.height);

            EditorGUI.LabelField(left, label, EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(false))
            {
                if (GUI.Button(right, "+", EditorStyles.miniButton))
                {
                    int idx = keysProp.arraySize;
                    keysProp.arraySize++;
                    var valsProp = keysProp.serializedObject.FindProperty(keysProp.propertyPath.Replace("keys", "values"));
                    if (valsProp != null) valsProp.arraySize = keysProp.arraySize;

                    var newKeyProp = keysProp.GetArrayElementAtIndex(idx);
                    newKeyProp.stringValue = MakeUniqueKey(keysProp, "key");

                    keysProp.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawRow(Rect rect, SerializedProperty keysProp, SerializedProperty valsProp, int index)
        {
            var keyProp = keysProp.GetArrayElementAtIndex(index);
            var valProp = valsProp.GetArrayElementAtIndex(index);

            float removeW = RemoveBtnWidth;
            float valueW = rect.width * 0.5f;
            float keyW = Mathf.Max(rect.width - valueW - removeW - Pad * 2, KeyMinWidth);

            var keyRect = new Rect(rect.x, rect.y, keyW, rect.height);
            var valueRect = new Rect(keyRect.xMax + Pad, rect.y, valueW, rect.height);
            var removeRect = new Rect(valueRect.xMax + Pad, rect.y, removeW, rect.height);

            // Duplicate key detection
            bool isDup = IsDuplicateKey(keysProp, index);
            var prevColor = GUI.color;
            if (isDup) GUI.color = Color.Lerp(Color.white, Color.red, 0.35f);
            keyProp.stringValue = EditorGUI.TextField(keyRect, keyProp.stringValue);
            GUI.color = prevColor;

            EditorGUI.PropertyField(valueRect, valProp, GUIContent.none);

            if (GUI.Button(removeRect, "-", EditorStyles.miniButton))
            {
                keysProp.DeleteArrayElementAtIndex(index);
                valsProp.DeleteArrayElementAtIndex(index);
                keysProp.serializedObject.ApplyModifiedProperties();
                return;
            }
        }

        private static bool IsDuplicateKey(SerializedProperty keysProp, int index)
        {
            string key = keysProp.GetArrayElementAtIndex(index).stringValue;
            if (string.IsNullOrEmpty(key)) return false;
            for (int i = 0; i < keysProp.arraySize; i++)
            {
                if (i == index) continue;
                if (keysProp.GetArrayElementAtIndex(i).stringValue == key)
                    return true;
            }
            return false;
        }

        private static string MakeUniqueKey(SerializedProperty keysProp, string baseKey)
        {
            string candidate = baseKey;
            int suffix = 1;
            bool exists;
            do
            {
                exists = false;
                for (int i = 0; i < keysProp.arraySize; i++)
                {
                    if (keysProp.GetArrayElementAtIndex(i).stringValue == candidate)
                    {
                        exists = true; break;
                    }
                }
                if (exists) candidate = baseKey + (++suffix).ToString();
            } while (exists);
            return candidate;
        }

        private static void AutoSort(SerializedProperty keysProp, SerializedProperty valsProp)
        {
            int count = keysProp.arraySize;
            var pairs = new List<(string key, SerializedProperty keyProp, SerializedProperty valProp)>();

            for (int i = 0; i < count; i++)
            {
                var k = keysProp.GetArrayElementAtIndex(i).stringValue;
                pairs.Add((k, keysProp.GetArrayElementAtIndex(i), valsProp.GetArrayElementAtIndex(i)));
            }

            pairs.Sort((a, b) => string.Compare(a.key, b.key, StringComparison.Ordinal));

            for (int i = 0; i < count; i++)
            {
                keysProp.GetArrayElementAtIndex(i).stringValue = pairs[i].keyProp.stringValue;
                valsProp.GetArrayElementAtIndex(i).objectReferenceValue = pairs[i].valProp.objectReferenceValue;
            }
        }
    }
#endif
}
