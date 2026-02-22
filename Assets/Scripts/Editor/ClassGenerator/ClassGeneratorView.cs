using System;
using System.Collections.Generic;
using R3;
using UnityEditor;
using UnityEngine;

using LayerSettings = Editor.ClassGenerator.ClassGeneratorModel.LayerSettings;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorView : IDisposable
    {
        private Vector2 _scrollPosition;

        private readonly Subject<Unit> _onGenerateRequestedSubject = new();
        public Observable<Unit> OnGenerateRequestedAsObservable => _onGenerateRequestedSubject;

        public void Draw(Rect windowPosition, 
            IReadOnlyDictionary<string, List<LayerSettings>> layers,
            string nameSpace)
        {
            DrawToolbar(nameSpace);

            float halfWidth = windowPosition.width / 2f - 2f;
            float halfHeight = (windowPosition.height - 35f) / 2f;

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // 上段プレゼンテーション層とアプリケーション層を実装
            EditorGUILayout.BeginHorizontal(GUILayout.Height(halfHeight));
            DrawLayerArea("Presentation", layers["Presentation"], halfWidth);
            DrawVerticalLine();
            DrawLayerArea("Application", layers["Application"], halfWidth);
            EditorGUILayout.EndHorizontal();

            DrawHorizontalLine();

            // 下段ドメイン層とインフラ層を実装
            EditorGUILayout.BeginHorizontal(GUILayout.Height(halfHeight));
            DrawLayerArea("Domain", layers["Domain"], halfWidth);
            DrawVerticalLine();
            DrawLayerArea("Infrastructure", layers["Infrastructure"], halfWidth);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar(string nameSpace)
        {
            EditorGUILayout.BeginVertical(EditorStyles.toolbar);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Class Generator", EditorStyles.boldLabel, GUILayout.Width(120));
            EditorGUILayout.TextField("Namespace", nameSpace);

            if (GUILayout.Button("Generate All", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                // presenter.OnGenerateRequested();
                _onGenerateRequestedSubject.OnNext(Unit.Default);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 層ごとの設定項目を列挙
        /// </summary>
        private void DrawLayerArea(string title, List<LayerSettings> settings, float width)
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

        private void DrawSettingCard(LayerSettings setting)
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
        private void DrawVerticalLine() { Rect r = GUILayoutUtility.GetRect(1, 0, GUILayout.ExpandHeight(true)); EditorGUI.DrawRect(r, new Color(0.12f, 0.12f, 0.12f)); }
        
        /// <summary>
        /// 層分割の横線
        /// </summary>
        private void DrawHorizontalLine() { Rect r = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true)); EditorGUI.DrawRect(r, new Color(0.12f, 0.12f, 0.12f)); }

        public void Dispose()
        {
            _onGenerateRequestedSubject?.Dispose();
        }
    }
}