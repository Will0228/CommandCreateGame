using System;
using System.Collections.Generic;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingViewContainer : IDisposable
    {
        private readonly ClassGeneratorFolderSettingFolderPathView _folderPathView;
        private readonly ClassGeneratorFolderSettingLayerView _layerView;
        
        public Observable<int> OnFolderButtonClickedAsObservable => _folderPathView.OnFolderButtonClickedAsObservable;
        public Observable<AppLayerType> OnLayerButtonClickedAsObservable => _layerView.OnLayerButtonClickedAsObservable;
        public Observable<ComponentRoleType> OnComponentRoleButtonClickedAsObservable => _layerView.OnComponentRoleButtonClickedAsObservable;
        public Observable<AppLayerType> OnLayerSeparateSettingsAsObservable => _layerView.OnLayerSeparateSettingsAsObservable;

        public ClassGeneratorFolderSettingViewContainer()
        {
            _folderPathView = new ClassGeneratorFolderSettingFolderPathView();
            _layerView = new ClassGeneratorFolderSettingLayerView();
        }
        
        private AppLayerType _selectedLayer = AppLayerType.None;
        
        private readonly Subject<(ComponentRoleType, string)> _onSetFolderPathSubject = new();
        public Observable<(ComponentRoleType, string)> OnSetFolderPathAsObservable => _onSetFolderPathSubject;
        
        internal void Draw(Rect windowPosition,
            IReadOnlyDictionary<AppLayerType, string> layerPathDict,
            IReadOnlyDictionary<ComponentRoleType, string> componentRoleSettingsDict,
            IReadOnlyDictionary<AppLayerType, bool> isSeperateSettingsDict,
            IReadOnlyList<ClassGeneratorFolderSettingPathDto> pathDtos)
        {
            var halfWidth = windowPosition.width / 2f - 2f;
            
            EditorGUILayout.BeginHorizontal();
            _layerView.Draw(layerPathDict, componentRoleSettingsDict, isSeperateSettingsDict, halfWidth);
            _folderPathView.Draw(pathDtos);
            EditorGUILayout.EndHorizontal();
        }
        
        
        void IDisposable.Dispose()
        {
            ((IDisposable)_layerView).Dispose();
            ((IDisposable)_folderPathView).Dispose();
        }
    }
}