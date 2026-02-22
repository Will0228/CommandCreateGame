using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorModel
    {
        public string NamespaceName = "YourProject.Domain";
        public readonly Dictionary<string, List<ClassGeneratorWindow.LayerSettings>> Layers;

        public ClassGeneratorModel()
        {
            Layers = new Dictionary<string, List<ClassGeneratorWindow.LayerSettings>>
            {
                { "Presentation", new List<ClassGeneratorWindow.LayerSettings> { new("Presenter", "Presenter"), new("View", "View") } },
                { "Application", new List<ClassGeneratorWindow.LayerSettings> { new("UseCase", "UseCase"), new("Service", "Service") } },
                { "Domain", new List<ClassGeneratorWindow.LayerSettings> { new("Entity", ""), new("ValueObject", ""), new("Repository Interface", "Repository") } },
                { "Infrastructure", new List<ClassGeneratorWindow.LayerSettings> { new("DataModel", "DataModel"), new("Repository Impl", "Repository") } }
            };
        }
        
        public void GenerateFiles(string outputPath)
        {
            if (string.IsNullOrEmpty(outputPath)) return;

            int count = 0;
            foreach (var layer in Layers.Values)
            {
                foreach (var setting in layer)
                {
                    if (!setting.IsEnabled) continue;
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
            Debug.Log($"[DDD Generator] {count} classes created at {outputPath}");
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