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
        public Observable<Enum> OnLayerButtonClickedAsObservable => _layerView.OnLayerPathSettingButtonClickedAsObservable;
        public Observable<AppLayerType> OnLayerSeparateSettingsAsObservable => _layerView.OnLayerSeparateSettingsAsObservable;

        [EditorInject]
        public ClassGeneratorFolderSettingViewContainer(ClassGeneratorSimpleDIContainer container)
        {
            _folderPathView = container.Resolve<ClassGeneratorFolderSettingFolderPathView>();
            _layerView = container.Resolve<ClassGeneratorFolderSettingLayerView>();
        }
        
        private AppLayerType _selectedLayer = AppLayerType.None;
        
        private readonly Subject<(ComponentRoleType, string)> _onSetFolderPathSubject = new();
        public Observable<(ComponentRoleType, string)> OnSetFolderPathAsObservable => _onSetFolderPathSubject;
        
        public void Draw(Rect windowPosition,
            IReadOnlyDictionary<AppLayerType, string> layerPathDict,
            IReadOnlyDictionary<ComponentRoleType, string> componentRoleSettingsDict,
            IReadOnlyDictionary<AppLayerType, bool> isSeparateSettingsDict,
            IReadOnlyList<ClassGeneratorFolderSettingPathDto> pathDtos)
        {
            var halfWidth = windowPosition.width / 2f - 2f;
            
            EditorGUILayout.BeginHorizontal();
            _layerView.Draw(layerPathDict, componentRoleSettingsDict, isSeparateSettingsDict, halfWidth);
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