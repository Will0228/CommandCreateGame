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
            IReadOnlyDictionary<string, int> folderPathDict)
        {
            var halfWidth = windowPosition.width / 2f - 2f;
            
            EditorGUILayout.BeginHorizontal();
            _layerView.Draw(layerPathDict, halfWidth);
            _folderPathView.Draw(folderPathDict);
            EditorGUILayout.EndHorizontal();
        }
        
        
        void IDisposable.Dispose()
        {
            
        }
    }
}