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
        
        private Enum _selectedLayerType;
        
        private CompositeDisposable _disposable = new();
        
        private readonly Subject<Enum> _onLayerPathSettingButtonClickedSubject = new();
        public Observable<Enum> OnLayerPathSettingButtonClickedAsObservable => _onLayerPathSettingButtonClickedSubject;

        // 個別設定をするかどうかの購読
        private readonly Subject<AppLayerType> _onLayerSeparateSettingsSubject = new();
        public Observable<AppLayerType> OnLayerSeparateSettingsAsObservable => _onLayerSeparateSettingsSubject;

        private readonly Dictionary<AppLayerType, ClassGeneratorFolderAvailableSeparateSettingsCellView> _cachedAvailableSeparateSettingsCellViews = new();
        private readonly Dictionary<ComponentRoleType, ClassGeneratorFolderSettingLayerCellView> _cachedSettingLayerCellViews = new();
        
        internal void Draw(IReadOnlyDictionary<AppLayerType, string> appLayerPathDict,
            IReadOnlyDictionary<ComponentRoleType, string> componentRoleSettingsDict,
            IReadOnlyDictionary<AppLayerType, bool> isSeparateSettingsDict,
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
                    if (!_cachedAvailableSeparateSettingsCellViews.TryGetValue(dict.Key, out var cell))
                    {
                        cell = new ClassGeneratorFolderAvailableSeparateSettingsCellView();
                        CellViewSetEvent(cell);
                        _cachedAvailableSeparateSettingsCellViews[dict.Key] = cell;
                    }
                    
                    cell.Configure(isSeparateSettingsDict[dict.Key],
                        LAYER_VIEW_DISABLED_COLOR,
                        _selectedLayerType,
                        dict,
                        sectionRectWidth,
                        LABEL_LAYER_VIEW_WIDTH,
                        SEPARATE_SETTING_VIEW_WIDTH);
                }
                EditorGUILayout.EndHorizontal();

                // // 個別設定を行う場合は具体クラスを列挙する
                if (isSeparateSettingsDict[dict.Key])
                {
                    foreach (var componentRoleTypeKvp in componentRoleSettingsDict.Where(kvp => ((int)kvp.Key & (int)dict.Key) != 0 && ((int)kvp.Key & 0xFF) != 0))
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(COMPONENT_ROLE_START_VIEW_SPACE_WIDTH);
                            
                            if (!_cachedSettingLayerCellViews.TryGetValue(componentRoleTypeKvp.Key, out var settingLayerCell))
                            {
                                settingLayerCell = new ClassGeneratorFolderSettingLayerCellView();
                                CellViewSetEvent(settingLayerCell);
                                _cachedSettingLayerCellViews[componentRoleTypeKvp.Key] = settingLayerCell;
                            }
                    
                            settingLayerCell.Configure(false,
                                LAYER_VIEW_DISABLED_COLOR,
                                _selectedLayerType,
                                componentRoleTypeKvp,
                                sectionRectWidth,
                                LABEL_LAYER_VIEW_WIDTH,
                                SEPARATE_SETTING_VIEW_WIDTH);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.Space(10);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void CellViewSetEvent(ClassGeneratorFolderSettingLayerCellView cell)
        {
            cell.OnLayerPathSettingButtonClickedAsObservable
                .Subscribe(LayerPathSettingButtonClicked)
                .AddTo(_disposable);
        }

        private void CellViewSetEvent(ClassGeneratorFolderAvailableSeparateSettingsCellView cell)
        {
            cell.OnLayerPathSettingButtonClickedAsObservable
                .Subscribe(LayerPathSettingButtonClicked)
                .AddTo(_disposable);
            
            cell.OnLayerSeparateSettingsAsObservable
                .Subscribe(_onLayerSeparateSettingsSubject.OnNext)
                .AddTo(_disposable);
        }

        private void LayerPathSettingButtonClicked(Enum selectedLayerType)
        {
            _selectedLayerType = selectedLayerType;
            _onLayerPathSettingButtonClickedSubject.OnNext(selectedLayerType);
        }

        void IDisposable.Dispose()
        {
            _onLayerPathSettingButtonClickedSubject.Dispose();
            _onLayerSeparateSettingsSubject.Dispose();
        }
    }
}