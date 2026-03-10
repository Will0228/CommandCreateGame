using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace Editor.ClassGenerator
{
    /// <summary>
    /// 設定されたデータをもとにファイルを作成するクラス
    /// </summary>
    internal sealed class ClassGeneratorCreateFilesService
    {
        internal void CreateFiles(IReadOnlyDictionary<ClassGeneratorModel.LayerSettings, string> dict)
        {
            foreach (var kvp in dict)
            {
                foreach (var name in kvp.Key.ClassNames)
                {
                    var settings = kvp.Key;
                    
                    var fullClassName = name + settings.Suffix;
                    var filePath = Path.Combine(kvp.Value, fullClassName + ".cs");

                    if (File.Exists(filePath))
                    {
                        continue;
                    }

                    File.WriteAllText(filePath, GetTemplate(fullClassName, settings.Label));
                    
                }
            }
            
            AssetDatabase.Refresh();
        }
        
        private string GetTemplate(string className, string layer)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            if (layer == "State") sb.AppendLine("using System;");
            sb.AppendLine("");
            if (!string.IsNullOrEmpty("NamespaceName"))
            {
                sb.AppendLine($"namespace {"NamespaceName"}");
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

            if (!string.IsNullOrEmpty("NamespaceName"))
            {
                sb.AppendLine("}");
            }

            return sb.ToString();
        }
    }
}