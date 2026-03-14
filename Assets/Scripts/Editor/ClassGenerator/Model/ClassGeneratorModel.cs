using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    [Flags]
    internal enum AppLayerType
    {
        None = 0,
        Presentation = 1 << 8, // プレゼンテーション層
        Application = 1 << 9, // アプリケーション層
        Domain = 1 << 10, // ドメイン層
        Infrastructure = 1 << 11, // インフラ層
    }
    
    [Flags]
    internal enum ComponentRoleType
    {
        None = 0,
        
        // プレゼンテーション層
        Presenter = (1 << 8) | (1 << 0),
        View  = (1 << 8) | (1 << 1),
        
        // アプリケーション層
        UseCase = (1 << 9) | (1 << 0),
        Service = (1 << 9) | (1 << 1),
        
        // ドメイン層
        Entity = (1 << 10) | (1 << 0),
        ValueObject = (1 << 10) | (1 << 1),
        DataTransferObject = (1 << 10) | (1 << 2),
        RepositoryInterface = (1 << 10) | (1 << 3),
        
        // インフラ層
        RepositoryImplementation = (1 << 11) | (1 << 0),
        
        // 層判定用のマスク
        PresentationMask = (1 << 8),
        ApplicationMask = (1 << 9),
        DomainMask = (1 << 10),
        InfrastructureMask = (1 << 11),
    }
    
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
            => _layers.Values
                .SelectMany(layerSettingsList =>
                {
                    return layerSettingsList
                        .Where(layerSettings => layerSettings.ClassNames.Any());
                }).ToList();
        
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