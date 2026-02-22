using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal enum LayerType
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
            internal LayerType Type;
            
            public LayerSettings(LayerType type, string label, string suffix)
            {
                Type = type;
                Label = label;
                Suffix = suffix;
            }
        }
        
        public string NamespaceName = "YourProject.Domain";
        
        private readonly Dictionary<string, List<LayerSettings>> _layers;
        public IReadOnlyDictionary<string, List<LayerSettings>> Layers => _layers;

        private readonly Dictionary<LayerType, bool> _isGeneratedClassDict = new();
        public bool IsExistGeneratedClass(LayerType layerType) => _isGeneratedClassDict[layerType];

        public ClassGeneratorModel()
        {
            _layers = new Dictionary<string, List<LayerSettings>>
            {
                { "Presentation", new List<LayerSettings> { new(LayerType.Presenter, "Presenter", "Presenter"), new(LayerType.View, "View", "View") } },
                { "Application", new List<LayerSettings> { new(LayerType.UseCase, "UseCase", "UseCase"), new(LayerType.Service, "Service", "Service") } },
                { "Domain", new List<LayerSettings> { new(LayerType.Entity, "Entity", "Entity"), new(LayerType.ValueObject, "ValueObject", "Vo"), new(LayerType.DataTransferObject, "DataTransferObject", "Dto"), new(LayerType.RepositoryInterface, "Repository Interface", "Repository") } },
                { "Infrastructure", new List<LayerSettings> { new(LayerType.RepositoryImplementation, "Repository Impl", "Repository") } }
            };
        }
        
        public void GenerateFiles(string outputPath)
        {
            if (string.IsNullOrEmpty(outputPath)) return;

            int count = 0;
            foreach (LayerType type in Enum.GetValues(typeof(LayerType)))
            {
                if (type == LayerType.None || !_isGeneratedClassDict[type])
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