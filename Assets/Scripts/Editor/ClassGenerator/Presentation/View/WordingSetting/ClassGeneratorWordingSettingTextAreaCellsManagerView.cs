using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CellView = Editor.ClassGenerator.ClassGeneratorWordingSettingTextAreaCellView;
using WordingSettingInfo = Editor.ClassGenerator.ClassGeneratorWordingSettingInfo;
namespace Editor.ClassGenerator
{
    /// <summary>
    /// クラス毎の設定用の全てのTextAreaを管理するViewクラス
    /// </summary>
    internal sealed class ClassGeneratorWordingSettingTextAreaCellsManagerView
    {
        private readonly float _labelWidth;
        private readonly Dictionary<ComponentRoleType, CellView> _cellViews = new();
        
        private Vector2 _scrollPosition;

        internal ClassGeneratorWordingSettingTextAreaCellsManagerView(float labelWidth)
        {
            _labelWidth = labelWidth;
        }
        
        /// <summary>
        /// 別のタブに遷移している間に入った変更を適用させる
        /// </summary>
        /// <remarks>
        /// TODO
        /// Cellを作り直しているのでパフォーマンスは最適ではないです
        /// これによって重くなるのであれば実装を見直してください
        /// </remarks>
        internal void UpdateData(IReadOnlyDictionary<ComponentRoleType, WordingSettingInfo> dict)
        {
            _cellViews.Clear();
            foreach (var kvp in dict)
            {
                _cellViews[kvp.Key] = new CellView(kvp.Value, _labelWidth);
            }
        }

        internal void Draw()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    GUILayout.Label("クラス毎の要望", EditorStyles.boldLabel, GUILayout.Width(_labelWidth));
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("All Clear"))
                    {
                        // FIXME
                        // 実装する
                        
                        GUI.FocusControl(null);
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(300));
                {
                    foreach (var kvp in _cellViews)
                    {
                        kvp.Value.Draw();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
    }
}