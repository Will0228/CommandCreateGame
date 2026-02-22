using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorPresenter
    {
        private readonly ClassGeneratorModel _model;
        
        public ClassGeneratorPresenter(ClassGeneratorModel model)
        {
            _model = model;
        }

        public string Namespace { get => _model.NamespaceName; set => _model.NamespaceName = value; }
        public Dictionary<string, List<ClassGeneratorWindow.LayerSettings>> Layers => _model.Layers;

        public void OnGenerateRequested()
        {
            string path = "Assets";
            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path)) path = Path.GetDirectoryName(path);
                break;
            }
            _model.GenerateFiles(path);
        }
    }
}