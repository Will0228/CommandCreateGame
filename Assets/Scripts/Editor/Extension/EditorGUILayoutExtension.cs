using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EditorGUILayoutExtension
    {
        public static EditorGUILayoutScope CreateEditorGUILayoutScope(GUIStyle style, params GUILayoutOption[] options)
        {
            return new EditorGUILayoutScope(style, options);
        }
    }

    public struct EditorGUILayoutScope : IDisposable
    {
        public EditorGUILayoutScope(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
        }

        void IDisposable.Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }
}