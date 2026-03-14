using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingLayerModel
    {
        private const string ABSOLUTE_PATH = "Assets/Scripts/";
        
        private readonly Dictionary<AppLayerType, string> _layerPathDict = new ();
        public IReadOnlyDictionary<AppLayerType, string> LayerPathDict => _layerPathDict;
        
        private readonly Dictionary<ComponentRoleType, string> _componentRolePathDict = new ();
        public IReadOnlyDictionary<ComponentRoleType, string> ComponentRolePathDict => _componentRolePathDict;

        private readonly Dictionary<AppLayerType, bool> _isSeparateSettingsDict = new();
        public IReadOnlyDictionary<AppLayerType, bool> IsSeparateSettingsDict => _isSeparateSettingsDict;
        
        private Enum _selectedLayerType = AppLayerType.None;
        public Enum SelectedLayerType => _selectedLayerType;

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
        
        internal void SetSelectedLayerType(Enum layerType)
        {
            _selectedLayerType = layerType;
        }

        internal void SetFolderPath(string path)
        {
            if (_selectedLayerType is AppLayerType appLayerType && appLayerType != AppLayerType.None)
            {
                _layerPathDict[appLayerType] = ABSOLUTE_PATH + path;
            }
            else if (_selectedLayerType is ComponentRoleType componentRoleType && componentRoleType != ComponentRoleType.None)
            {
                _componentRolePathDict[componentRoleType] = ABSOLUTE_PATH + path;
            }
        }
        
        internal void SetSeparateSettingsDict(AppLayerType layerType)
        {
            _isSeparateSettingsDict[layerType] = !_isSeparateSettingsDict[layerType];
        }

        internal IReadOnlyDictionary<ClassGeneratorModel.LayerSettings, string> GetLayerTypeAndPaths(IReadOnlyList<ClassGeneratorModel.LayerSettings> settingsList)
        {
            var tempDict = new Dictionary<ClassGeneratorModel.LayerSettings, string>();
            foreach (var settings in settingsList)
            {
                var applicableAppLayerType = GetApplicableAppLayerType(settings.Type);
                if (!_isSeparateSettingsDict[applicableAppLayerType])
                {
                    if (!_layerPathDict.TryGetValue(applicableAppLayerType, out var path))
                    {
                        throw new KeyNotFoundException($"{applicableAppLayerType} is not found");
                        return null;
                    }
                    tempDict.Add(settings, path);
                }
                else
                {
                    if (!_componentRolePathDict.TryGetValue(settings.Type, out var path))
                    {
                        throw new KeyNotFoundException($"{applicableAppLayerType} is not found");
                        return null;
                    }
                    tempDict.Add(settings, path);
                }
            }
            
            return tempDict;
        }

        private AppLayerType GetApplicableAppLayerType(ComponentRoleType componentRoleType)
        {
            return componentRoleType switch
            {
                _ when (componentRoleType & ComponentRoleType.PresentationMask) != 0 => AppLayerType.Presentation,
                _ when (componentRoleType & ComponentRoleType.ApplicationMask) != 0 => AppLayerType.Application,
                _ when (componentRoleType & ComponentRoleType.DomainMask) != 0 => AppLayerType.Domain,
                _ when (componentRoleType & ComponentRoleType.InfrastructureMask) != 0 => AppLayerType.Infrastructure,
                _ => throw new ArgumentOutOfRangeException($"{componentRoleType} is not a valid component role")
            };
        }
    }
}