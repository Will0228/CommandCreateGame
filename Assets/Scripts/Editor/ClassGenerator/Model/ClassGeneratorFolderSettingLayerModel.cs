using System;
using System.Collections.Generic;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingLayerModel
    {
        private readonly Dictionary<AppLayerType, string> _layerPathDict = new ();
        public IReadOnlyDictionary<AppLayerType, string> LayerPathDict => _layerPathDict;
        
        private AppLayerType _selectedLayerType;
        public AppLayerType SelectedLayerType => _selectedLayerType;

        public ClassGeneratorFolderSettingLayerModel()
        {
            foreach (var type in Enum.GetValues(typeof(AppLayerType)))
            {
                _layerPathDict.Add((AppLayerType)type, string.Empty);
            }
        }
        
        internal void SetSelectedLayerType(AppLayerType layerType)
        {
            _selectedLayerType = layerType;
        }

        internal void SetFolderPath(string path)
        {
            _layerPathDict[_selectedLayerType] = path;
        }
    }
}