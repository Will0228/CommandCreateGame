using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal enum AppLayerType
    {
        None,
        Presentation, // プレゼンテーション層
        Application, // アプリケーション層
        Domain, // ドメイン層
        Infrastructure, // インフラ層
    }
    
    internal enum ComponentRoleType
    {
        None,
        
        // プレゼンテーション層
        Presenter,
        View,
        
        // アプリケーション層
        UseCase,
        Service,
        
        // ドメイン層
        Entity,
        ValueObject,
        DataTransferObject,
        RepositoryInterface,
        
        // インフラ層
        RepositoryImplementation,
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
        
        public void GenerateFiles(string outputPath)
        {
            if (string.IsNullOrEmpty(outputPath)) return;

            int count = 0;
            foreach (ComponentRoleType type in Enum.GetValues(typeof(ComponentRoleType)))
            {
                if (type == ComponentRoleType.None || !_isGeneratedClassDict[type])
                {
                    continue;
                }
                
                
                foreach (var layer in _layers.Values)
                {
                    foreach (var setting in layer)
                    {
                        foreach (var name in setting.ClassNames)
                        {
                            if (string.IsNullOrWhiteSpace(name)) continue;
                        
                            string fullClassName = name + setting.Suffix;
                            string filePath = Path.Combine(outputPath, fullClassName + ".cs");
                        
                            if (File.Exists(filePath)) continue;

                            File.WriteAllText(filePath, GetTemplate(fullClassName, setting.Label));
                            count++;
                        }
                    }
                }
                AssetDatabase.Refresh();
                Debug.Log($"[Class Generator] {count} classes created at {outputPath}");
            }
        }
        
        private string GetTemplate(string className, string layer)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            if (layer == "State") sb.AppendLine("using System;");
            sb.AppendLine("");
            if (!string.IsNullOrEmpty(NamespaceName))
            {
                sb.AppendLine($"namespace {NamespaceName}");
                sb.AppendLine("{");
            }

            switch (layer)
            {
                case "View":
                    sb.AppendLine($"    public class {className} : MonoBehaviour");
                    sb.AppendLine("    {");
                    sb.AppendLine("    }");
                    break;
                case "Presenter":
                    sb.AppendLine($"    public class {className}");
                    sb.AppendLine("    {");
                    sb.AppendLine("        public void Initialize()");
                    sb.AppendLine("        {");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    break;
                default:
                    sb.AppendLine($"    public class {className}");
                    sb.AppendLine("    {");
                    sb.AppendLine("    }");
                    break;
            }

            if (!string.IsNullOrEmpty(NamespaceName))
            {
                sb.AppendLine("}");
            }

            return sb.ToString();
        }
    }
}