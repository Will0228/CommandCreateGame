using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using WordingSettingInfo = Editor.ClassGenerator.ClassGeneratorWordingSettingInfo;
using CellView = Editor.ClassGenerator.ClassGeneratorWordingSettingTextAreaCellView;
using CellsManagerView = Editor.ClassGenerator.ClassGeneratorWordingSettingTextAreaCellsManagerView;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingTextAreaView
    {
        private readonly float _viewWidth;
        private const float LABEL_WIDTH = 200f;
        
        private Vector2 _scrollPosition;
        
        private readonly CellView _cachedImplementationDetailsTextAreaCellView;
        private readonly CellsManagerView _cachedClassSettingsCellsManagerView;
        
        internal ClassGeneratorWordingSettingTextAreaView(ClassGeneratorWordingSettingInfo info)
        {
            _cachedImplementationDetailsTextAreaCellView = new CellView(info, LABEL_WIDTH);
            _cachedClassSettingsCellsManagerView = new CellsManagerView(LABEL_WIDTH);
        }

        internal void UpdateData(IReadOnlyDictionary<ComponentRoleType, WordingSettingInfo> dict)
        {
            _cachedClassSettingsCellsManagerView.UpdateData(dict);
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