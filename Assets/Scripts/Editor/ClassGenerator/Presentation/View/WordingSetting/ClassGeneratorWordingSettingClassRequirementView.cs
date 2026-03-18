using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CellView = Editor.ClassGenerator.ClassGeneratorWordingSettingTextAreaCellView;

namespace Editor.ClassGenerator
{
    /// <summary>
    /// クラスごとの要望を設定するView
    /// </summary>
    internal sealed class ClassGeneratorWordingSettingClassRequirementView
    {
        private Vector2 _scrollPosition;
        
        private const float LABEL_WIDTH = 200f;

        private readonly float _halfWidth;
        private readonly float _halfHeight;
        // 配列はレイヤー層のため
        // 0 = プレゼンテーション層, 1 = アプリケーション層, 2 = ドメイン層, 3 = インフラ層
        private readonly List<CellView>[] _cellViewsArray = new List<CellView>[4];

        internal ClassGeneratorWordingSettingClassRequirementView(Rect windowPosition)
        {
            _halfWidth = windowPosition.width / 2f;
            _halfHeight = windowPosition.height / 2f;
        }

        /// <summary>
        /// 別のタブに遷移している間に入った変更を適用させる
        /// </summary>
        /// <remarks>
        /// TODO
        /// Cellを作り直しているのでパフォーマンスは最適ではないです
        /// これによって重くなるのであれば実装を見直してください
        /// </remarks>
        internal void UpdateData(IReadOnlyList<ClassGeneratorWordingSettingClassInfo> infos)
        {
            foreach (var cellViews in _cellViewsArray)
            {
                cellViews.Clear();
            }
            
            foreach (var info in infos)
            {
                var targetList = info.ComponentRoleType switch
                {
                    ComponentRoleType.PresentationMask => _cellViewsArray[0],
                    ComponentRoleType.ApplicationMask => _cellViewsArray[1],
                    ComponentRoleType.DomainMask => _cellViewsArray[2],
                    ComponentRoleType.InfrastructureMask => _cellViewsArray[3],
                    _ => throw new ArgumentOutOfRangeException($"{info.ComponentRoleType} is not a valid component role"),
                };
                
                targetList.Add(new ClassGeneratorWordingSettingTextAreaCellView(info.Info, LABEL_WIDTH, 50, false));
            }
        }
        
        public void Draw()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            // 上段プレゼンテーション層とアプリケーション層を実装
            EditorGUILayout.BeginHorizontal(GUILayout.Height(_halfHeight));
            DrawLayerArea("Presentation", _cellViewsArray[0], _halfWidth);
            DrawVerticalLine();
            DrawLayerArea("Application", _cellViewsArray[1], _halfWidth);
            EditorGUILayout.EndHorizontal();

            DrawHorizontalLine();

            // 下段ドメイン層とインフラ層を実装
            EditorGUILayout.BeginHorizontal(GUILayout.Height(_halfHeight));
            DrawLayerArea("Domain", _cellViewsArray[2], _halfWidth);
            DrawVerticalLine();
            DrawLayerArea("Infrastructure", _cellViewsArray[3], _halfWidth);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar(string nameSpace)
        {
            EditorGUILayout.BeginVertical(EditorStyles.toolbar);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Class Generator", EditorStyles.boldLabel, GUILayout.Width(120));
                    EditorGUILayout.TextField("Namespace", nameSpace);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 層ごとの設定項目を列挙
        /// </summary>
        private void DrawLayerArea(string title, List<ClassGeneratorModel.LayerSettings> settings, float width)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(width), GUILayout.ExpandHeight(true));
            GUI.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField(title, EditorStyles.whiteBoldLabel);
            EditorGUILayout.EndVertical();

            foreach (var setting in settings)
            {
                DrawSettingCard(setting);
                EditorGUILayout.Space(2);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSettingCard(ClassGeneratorModel.LayerSettings setting)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                setting.ClassNames.Add(""); GUI.FocusControl(null);
            }
            EditorGUILayout.LabelField(setting.Type.GetName(), GUILayout.Width(150));
            // 余白を埋める（これを入れると、以降の要素が左寄せになります）
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (setting.ClassNames.Count > 0)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < setting.ClassNames.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    setting.ClassNames[i] = EditorGUILayout.TextField(setting.ClassNames[i]);

                    if (GUILayout.Button("-", GUILayout.Width(25)))
                    {
                        setting.ClassNames.RemoveAt(i); 
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// 層分割の縦線
        /// </summary>
        private void DrawVerticalLine()
        {
            var r = GUILayoutUtility.GetRect(1, 0, GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(r, new Color(0.12f, 0.12f, 0.12f));
        }

        /// <summary>
        /// 層分割の横線
        /// </summary>
        private void DrawHorizontalLine()
        {
            var r = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(r, new Color(0.12f, 0.12f, 0.12f));
        }
        
    }
}