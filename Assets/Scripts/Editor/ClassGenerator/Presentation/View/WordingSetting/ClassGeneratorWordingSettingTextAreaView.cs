using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingTextAreaView
    {
        private string _text;
        private Vector2 _scrollPosition;

        internal void Configure(string defaultText)
        {
            _text = defaultText;
        }
        
        internal void Draw(float viewWidth)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(viewWidth));
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    GUILayout.Label("実装してほしい内容", EditorStyles.boldLabel, GUILayout.Width(200));
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Clear"))
                    {
                        _text = string.Empty;
                        GUI.FocusControl(null);
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                {
                    var style = new GUIStyle(EditorStyles.textArea)
                    {
                        wordWrap = true,
                    };
                    _text = EditorGUILayout.TextArea(_text, style, GUILayout.ExpandHeight(true));
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
    }
}