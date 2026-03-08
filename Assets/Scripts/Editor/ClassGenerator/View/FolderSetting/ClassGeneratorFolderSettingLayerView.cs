using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingLayerView : IDisposable
    {
        private const int SEPARATE_SETTING_VIEW_WIDTH = 100;
        private const int COMPONENT_ROLE_START_VIEW_SPACE_WIDTH = 20;
        private const int LABEL_LAYER_VIEW_WIDTH = 120;
        private readonly Color LAYER_VIEW_DISABLED_COLOR = new(0.1f, 0.1f, 0.1f);
        
        private AppLayerType _selectedLayer = AppLayerType.None;
        private ComponentRoleType _selectedComponentRole = ComponentRoleType.None;
        
        private readonly Subject<AppLayerType> _onLayerButtonClickedSubject = new();
        public Observable<AppLayerType> OnLayerButtonClickedAsObservable => _onLayerButtonClickedSubject;
        
        private readonly Subject<ComponentRoleType> _onComponentRoleButtonClickedSubject = new();
        public Observable<ComponentRoleType> OnComponentRoleButtonClickedAsObservable => _onComponentRoleButtonClickedSubject;

        // 個別設定をするかどうかの購読
        private readonly Subject<AppLayerType> _onLayerSeparateSettingsSubject = new();
        public Observable<AppLayerType> OnLayerSeparateSettingsAsObservable => _onLayerSeparateSettingsSubject;
        
        internal void Draw(IReadOnlyDictionary<AppLayerType, string> appLayerPathDict,
            IReadOnlyDictionary<ComponentRoleType, string> componentRoleSettingsDict,
            IReadOnlyDictionary<AppLayerType, bool> isSeperateSettingsDict,
            float sectionRectWidth)
        {
            EditorGUILayout.BeginVertical();
            
            foreach (var dict in appLayerPathDict)
            {
                if (dict.Key == AppLayerType.None)
                {
                    continue;
                }
                
                // Layerの設定と個別設定ボタンの描画
                EditorGUILayout.BeginHorizontal();
                {
                    var originalBgColor = GUI.backgroundColor;
                    // 個別設定にしている際のAppLayerViewは設定できない見た目にする
                    if (isSeperateSettingsDict[dict.Key])
                    {
                        GUI.backgroundColor = LAYER_VIEW_DISABLED_COLOR;
                    }
                    
                    var appLayerSectionRect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(sectionRectWidth - SEPARATE_SETTING_VIEW_WIDTH));
                    {
                        LayerFrameEmphasis(dict, _selectedLayer, appLayerSectionRect);
                        
                        EditorGUILayout.LabelField($"{dict.Key} Layer", GUILayout.Width(LABEL_LAYER_VIEW_WIDTH));
                        EditorGUI.BeginDisabledGroup(isSeperateSettingsDict[dict.Key]);
                        {
                            ButtonSettings(dict, ref _selectedLayer);
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUI.backgroundColor = originalBgColor;
                    
                    if (GUILayout.Button("個別に設定", GUILayout.Width(SEPARATE_SETTING_VIEW_WIDTH)))
                    {
                        _onLayerSeparateSettingsSubject.OnNext(dict.Key);
                    }
                }
                EditorGUILayout.EndHorizontal();

                // 個別設定を行う場合は具体クラスを列挙する
                if (isSeperateSettingsDict[dict.Key])
                {
                    foreach (var componentRoleTypeKvp in componentRoleSettingsDict.Where(kvp => ((int)kvp.Key & (int)dict.Key) != 0 && ((int)kvp.Key & 0xFF) != 0))
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(COMPONENT_ROLE_START_VIEW_SPACE_WIDTH);

                            var componentRoleSectionRect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(sectionRectWidth - COMPONENT_ROLE_START_VIEW_SPACE_WIDTH));
                            {
                                LayerFrameEmphasis(componentRoleTypeKvp, _selectedComponentRole, componentRoleSectionRect);
                            
                                EditorGUILayout.LabelField($"{componentRoleTypeKvp.Key} Class", GUILayout.Width(LABEL_LAYER_VIEW_WIDTH));
                                ButtonSettings(componentRoleTypeKvp, ref _selectedComponentRole);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.Space(10);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void ButtonSettings<T>(KeyValuePair<T, string> dict, ref T selectedLayer)
            where T : Enum
        {
            if (GUILayout.Button(dict.Value))
            {
                selectedLayer = dict.Key;
                _onLayerButtonClickedSubject.OnNext(_selectedLayer);
            }
        }

        /// <summary>
        /// 選択されている場合の枠の強調表示
        /// </summary>
        private void LayerFrameEmphasis<T>(KeyValuePair<T, string> dict, T selectedLayer, Rect sectionRect)
            where T : Enum
        {
            if (EqualityComparer<T>.Default.Equals(selectedLayer, dict.Key))
            {
                var originalColor = GUI.color;
                GUI.color = Color.yellow;
                GUI.Box(sectionRect, GUIContent.none, EditorStyles.selectionRect);
                GUI.color = originalColor;
            }
        }

        void IDisposable.Dispose()
        {
            _onLayerButtonClickedSubject.Dispose();
            _onComponentRoleButtonClickedSubject.Dispose();
            _onLayerSeparateSettingsSubject.Dispose();
        }
    }
}