using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingTextAreaCellView
    {
        private readonly float _labelWidth;
        private readonly float _viewHeight;
        private readonly string _labelText;
        
        private string _text;
        private Vector2 _scrollPosition;

        private readonly GUILayoutOption _textAreaOption;

        internal ClassGeneratorWordingSettingTextAreaCellView(ClassGeneratorWordingSettingInfo info,
            float labelWidth,
            float viewHeight,
            bool isTextAreaExpandHeight = true)
        {
            _text = info.ContentText;
            _labelText = info.Label;
            _labelWidth = labelWidth;
            _viewHeight = viewHeight;

            _textAreaOption = GUILayout.ExpandHeight(isTextAreaExpandHeight);
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
                
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(_viewHeight));
                {
                    var style = new GUIStyle(EditorStyles.textArea)
                    {
                        wordWrap = true,
                    };
                    _text = EditorGUILayout.TextArea(_text, style, _textAreaOption);
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
    }
}