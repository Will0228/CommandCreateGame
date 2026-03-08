using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingLayerModel
    {
        private readonly Dictionary<AppLayerType, string> _layerPathDict = new ();
        public IReadOnlyDictionary<AppLayerType, string> LayerPathDict => _layerPathDict;
        
        private readonly Dictionary<ComponentRoleType, string> _componentRolePathDict = new ();
        public IReadOnlyDictionary<ComponentRoleType, string> ComponentRolePathDict => _componentRolePathDict;

        private readonly Dictionary<AppLayerType, bool> _isSeparateSettingsDict = new();
        public IReadOnlyDictionary<AppLayerType, bool> IsSeparateSettingsDict => _isSeparateSettingsDict;
        
        private AppLayerType _selectedLayerType;
        public AppLayerType SelectedLayerType => _selectedLayerType;
        
        private ComponentRoleType _selectedComponentRoleType;
        public ComponentRoleType SelectedComponentRoleType => _selectedComponentRoleType;

        public ClassGeneratorFolderSettingLayerModel()
        {
            foreach (var type in Enum.GetValues(typeof(AppLayerType)))
            {
                _layerPathDict.Add((AppLayerType)type, string.Empty);
                _isSeparateSettingsDict.Add((AppLayerType)type, false);
            }

            foreach (var type in Enum.GetValues(typeof(ComponentRoleType)))
            {
                _componentRolePathDict.Add((ComponentRoleType)type, string.Empty);
            }
        }
        
        internal void SetSelectedLayerType(AppLayerType layerType)
        {
            _selectedLayerType = layerType;
        }

        internal void SetSelectedComponentRoleType(ComponentRoleType componentRoleType)
        {
            _selectedComponentRoleType = componentRoleType;
        }

        internal void SetFolderPath(string path)
        {
            _layerPathDict[_selectedLayerType] = path;
        }
        
        internal void SetSeparateSettingsDict(AppLayerType layerType)
        {
            _isSeparateSettingsDict[layerType] = !_isSeparateSettingsDict[layerType];
        }
    }
}