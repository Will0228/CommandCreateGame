using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingTextAreaCellView
    {
        private readonly float _labelWidth;
        private readonly string _labelText;
        
        private string _text;
        private Vector2 _scrollPosition;

        internal ClassGeneratorWordingSettingTextAreaCellView(ClassGeneratorWordingSettingInfo info,
            float labelWidth)
        {
            _text = info.ContentText;
            _labelText = info.Label;
            _labelWidth = labelWidth;
        }

        internal void Draw()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    GUILayout.Label(_labelText, EditorStyles.boldLabel, GUILayout.Width(_labelWidth));
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Clear"))
                    {
                        _text = string.Empty;
                        GUI.FocusControl(null);
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(300));
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