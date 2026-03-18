using UnityEditor;
using UnityEngine;

using CellView = Editor.ClassGenerator.ClassGeneratorWordingSettingTextAreaCellView;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingTextAreaView
    {
        private readonly float _viewWidth;
        private const float LABEL_WIDTH = 200f;
        private const float VIEW_HEIGHT = 200f;
        
        private Vector2 _scrollPosition;
        
        private readonly CellView _cachedImplementationDetailsTextAreaCellView;
        
        internal ClassGeneratorWordingSettingTextAreaView(ClassGeneratorWordingSettingInfo info)
        {
            _cachedImplementationDetailsTextAreaCellView = new CellView(info, LABEL_WIDTH, VIEW_HEIGHT);
        }
        
        internal void Draw()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    _cachedImplementationDetailsTextAreaCellView.Draw();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}