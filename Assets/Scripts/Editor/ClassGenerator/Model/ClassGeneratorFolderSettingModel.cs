using System;
using System.IO;
using System.Collections.Generic;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingModel
    {
        private readonly string _absoluteBase;

        private readonly Dictionary<AppLayerType, string> _layerPathDict = new ();
        public IReadOnlyDictionary<AppLayerType, string> LayerPathDict => _layerPathDict;
        
        // valueはどこの階層にいるかを調べるために使用する
        // Viewの見た目を良くするためだけです
        private readonly Dictionary<string, int> _folderPathDict = new();
        public IReadOnlyDictionary<string, int> FolderPathDict => _folderPathDict;
        
        public ClassGeneratorFolderSettingModel()
        {
            _absoluteBase = Path.Combine(UnityEngine.Device.Application.dataPath, "Scripts").Replace("\\", "/");

            foreach (var type in Enum.GetValues(typeof(AppLayerType)))
            {
                _layerPathDict.Add((AppLayerType)type, string.Empty);
            }
            RefreshFolderPaths();
        }

        private void RefreshFolderPaths()
        {
            _folderPathDict.Clear();
            
            if (Directory.Exists(_absoluteBase))
            {
                TraverseDirectory(_absoluteBase);
            }
        }
        
        /// <summary>
        /// 再帰的にディレクトリを探索する（深さ優先）
        /// </summary>
        private void TraverseDirectory(string currentPath, int depth = 0)
        {
            var relativePath = ToRelativePath(currentPath);
            if (!string.IsNullOrEmpty(relativePath))
            {
                _folderPathDict.Add(relativePath, depth);
            }

            // 直下のサブディレクトリを取得
            var subDirectories = Directory.GetDirectories(currentPath);

            foreach (var subDir in subDirectories)
            {
                TraverseDirectory(subDir.Replace('\\', '/'), depth + 1);
            }
        }

        private string ToRelativePath(string absolutePath)
        {
            if (!absolutePath.StartsWith(_absoluteBase))
            {
                return "";
            }

            // プレフィックス部分を削除
            string relative = absolutePath.Substring(_absoluteBase.Length);

            // 先頭が '/' で始まる場合は削除
            if (relative.StartsWith("/"))
            {
                relative = relative.Substring(1);
            }

            return relative;
        }
    }
}