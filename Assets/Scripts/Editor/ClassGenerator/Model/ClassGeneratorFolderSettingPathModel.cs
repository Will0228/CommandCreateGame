using System.Collections.Generic;
using System.IO;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingPathModel
    {
        private readonly string _absoluteBase;
        
        private readonly List<ClassGeneratorFolderSettingPathInfo> _pathInfos = new();
        public IReadOnlyList<ClassGeneratorFolderSettingPathInfo> PathInfos => _pathInfos;

        public ClassGeneratorFolderSettingPathModel()
        {
            _absoluteBase = Path.Combine(UnityEngine.Device.Application.dataPath, "Scripts").Replace("\\", "/");
            RefreshFolderPaths();
        }
        
        private void RefreshFolderPaths()
        {
            _pathInfos.Clear();
            
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
                _pathInfos.Add(new ClassGeneratorFolderSettingPathInfo(relativePath, depth));
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