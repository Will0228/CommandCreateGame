using System;
using System.Collections.Generic;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    /// <summary>
    /// 設定の項目名やフォルダパス設定ができるセル
    /// </summary>
    internal sealed class ClassGeneratorFolderSettingLayerCellView : IDisposable
    {
        private readonly Subject<Enum> _onLayerPathSettingButtonClickedSubject = new();
        public Observable<Enum> OnLayerPathSettingButtonClickedAsObservable => _onLayerPathSettingButtonClickedSubject;
        
        internal void Configure<T>(bool isSeparateSettings,
            Color backgroundColor,
            Enum cachedSelectedLayer,
            KeyValuePair<T, string> kvp,
            float sectionRectWidth,
            int labelLayerViewWidth,
            int separateSettingViewWidth) where T : Enum
        {
            var originalBgColor = GUI.backgroundColor;
            // 個別設定にしている際のAppLayerViewは設定できない見た目にする
            if (isSeparateSettings)
            {
                GUI.backgroundColor = backgroundColor;
            }
                    
            var appLayerSectionRect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(sectionRectWidth - separateSettingViewWidth));
            {
                LayerFrameEmphasis(kvp.Key, cachedSelectedLayer, appLayerSectionRect);
                        
                EditorGUILayout.LabelField($"{kvp.Key}", GUILayout.Width(labelLayerViewWidth));
                EditorGUI.BeginDisabledGroup(isSeparateSettings);
                {
                    ButtonSettings(kvp);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = originalBgColor;
        }
        
        /// <summary>
        /// 選択されている場合の枠の強調表示
        /// </summary>
        private void LayerFrameEmphasis<T>(T currentSelectedLayer, T cachedSelectedLayer, Rect sectionRect)
            where T : Enum
        {
            if (EqualityComparer<T>.Default.Equals(cachedSelectedLayer, currentSelectedLayer))
            {
                var originalColor = GUI.color;
                GUI.color = Color.yellow;
                GUI.Box(sectionRect, GUIContent.none, EditorStyles.selectionRect);
                GUI.color = originalColor;
            }
        }
        
        private void ButtonSettings<T>(KeyValuePair<T, string> kvp)
            where T : Enum
        {
            if (GUILayout.Button(kvp.Value))
            {
                _onLayerPathSettingButtonClickedSubject.OnNext(kvp.Key);
            }
        }

        public void Dispose()
        {
            _onLayerPathSettingButtonClickedSubject?.Dispose();
        }
    }
}