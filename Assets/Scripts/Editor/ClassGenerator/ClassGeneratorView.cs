using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorView
    {
        private Vector2 _scrollPosition;

        public void Draw(Rect windowPosition, ClassGeneratorPresenter presenter)
        {
            DrawToolbar(presenter);

            float halfWidth = windowPosition.width / 2f - 2f;
            float halfHeight = (windowPosition.height - 35f) / 2f;

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // 上段プレゼンテーション層とアプリケーション層を実装
            EditorGUILayout.BeginHorizontal(GUILayout.Height(halfHeight));
            DrawLayerArea("Presentation", presenter.Layers["Presentation"], halfWidth);
            DrawVerticalLine();
            DrawLayerArea("Application", presenter.Layers["Application"], halfWidth);
            EditorGUILayout.EndHorizontal();

            DrawHorizontalLine();

            // 下段ドメイン層とインフラ層を実装
            EditorGUILayout.BeginHorizontal(GUILayout.Height(halfHeight));
            DrawLayerArea("Domain", presenter.Layers["Domain"], halfWidth);
            DrawVerticalLine();
            DrawLayerArea("Infrastructure", presenter.Layers["Infrastructure"], halfWidth);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar(ClassGeneratorPresenter presenter)
        {
            EditorGUILayout.BeginVertical(EditorStyles.toolbar);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Class Generator", EditorStyles.boldLabel, GUILayout.Width(120));
            presenter.Namespace = EditorGUILayout.TextField("Namespace", presenter.Namespace);

            if (GUILayout.Button("Generate All", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                presenter.OnGenerateRequested();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 層ごとの設定項目を列挙
        /// </summary>
        private void DrawLayerArea(string title, List<ClassGeneratorWindow.LayerSettings> settings, float width)
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

        private void DrawSettingCard(ClassGeneratorWindow.LayerSettings setting)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            setting.IsEnabled = EditorGUILayout.ToggleLeft($" Generate {setting.Label}", setting.IsEnabled, EditorStyles.boldLabel);

            if (setting.IsEnabled)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < setting.ClassNames.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    setting.ClassNames[i] = EditorGUILayout.TextField(setting.ClassNames[i]);
                    if (GUILayout.Button("+", GUILayout.Width(25)))
                    {
                        setting.ClassNames.Insert(i + 1, ""); GUI.FocusControl(null);
                    }

                    if (setting.ClassNames.Count > 1 && GUILayout.Button("-", GUILayout.Width(25)))
                    {
                        setting.ClassNames.RemoveAt(i); 
                        // GUI.FocusControl(null);
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
        private void DrawVerticalLine() { Rect r = GUILayoutUtility.GetRect(1, 0, GUILayout.ExpandHeight(true)); EditorGUI.DrawRect(r, new Color(0.12f, 0.12f, 0.12f)); }
        
        /// <summary>
        /// 層分割の横線
        /// </summary>
        private void DrawHorizontalLine() { Rect r = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true)); EditorGUI.DrawRect(r, new Color(0.12f, 0.12f, 0.12f)); }
    }
}