using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EditorGUILayoutExtension
    {
        public static EditorGUIVerticalLayoutScope CreateEditorGUIVerticalLayoutScope(GUIStyle style = null, params GUILayoutOption[] options)
        {
            return new EditorGUIVerticalLayoutScope(style, options);
        }
        
        public static EditorGUIHorizontalLayoutScope CreateEditorGUIHorizontalLayoutScope(GUIStyle style = null, params GUILayoutOption[] options)
        {
            return new EditorGUIHorizontalLayoutScope(style, options);
        }
    }

    public struct EditorGUIVerticalLayoutScope : IDisposable
    {
        public EditorGUIVerticalLayoutScope(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
        }

        void IDisposable.Dispose()
        {
            EditorGUILayout.EndVertical();
        }
    }

    public struct EditorGUIHorizontalLayoutScope : IDisposable
    {
        public EditorGUIHorizontalLayoutScope(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
        }

        void IDisposable.Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }
}