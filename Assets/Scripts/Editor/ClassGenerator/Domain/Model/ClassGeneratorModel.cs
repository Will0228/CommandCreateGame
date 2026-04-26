using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorModel
    {
        [Serializable]
        internal class LayerSettings
        {
            internal string Label;
            internal readonly List<string> ClassNames = new();
            internal string Suffix;
            internal ComponentRoleType Type;
            
            public LayerSettings(ComponentRoleType type, string label, string suffix)
            {
                Type = type;
                Label = label;
                Suffix = suffix;
            }
        }
        
        public string NamespaceName = "YourProject.Domain";
        
        private readonly Dictionary<AppLayerType, List<LayerSettings>> _layers;
        public IReadOnlyDictionary<AppLayerType, List<LayerSettings>> Layers => _layers;
        public IReadOnlyList<LayerSettings> LayerSettingsList
            => _layers.Values.SelectMany(layerSettingsList =>layerSettingsList.Where(layerSettings => layerSettings.ClassNames.Any())).ToList();
        
        private readonly Dictionary<ComponentRoleType, bool> _isGeneratedClassDict = new();
        public bool IsExistGeneratedClass(ComponentRoleType componentRoleType) => _isGeneratedClassDict[componentRoleType];

        public ClassGeneratorModel()
        {
            _layers = new Dictionary<AppLayerType, List<LayerSettings>>
            {
                { AppLayerType.Presentation, new List<LayerSettings> { new(ComponentRoleType.Presenter, "Presenter", "Presenter"), new(ComponentRoleType.View, "View", "View") } },
                { AppLayerType.Application, new List<LayerSettings> { new(ComponentRoleType.UseCase, "UseCase", "UseCase"), new(ComponentRoleType.Service, "Service", "Service") } },
                { AppLayerType.Domain, new List<LayerSettings> { new(ComponentRoleType.Entity, "Entity", "Entity"), new(ComponentRoleType.ValueObject, "ValueObject", "Vo"), new(ComponentRoleType.DataTransferObject, "DataTransferObject", "Dto"), new(ComponentRoleType.RepositoryInterface, "Repository Interface", "Repository") } },
                { AppLayerType.Infrastructure, new List<LayerSettings> { new(ComponentRoleType.RepositoryImplementation, "Repository Impl", "Repository") } }
            };
        }
    }
}