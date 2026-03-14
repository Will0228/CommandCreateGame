using System.Collections.Generic;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorPresenter
    {
        private readonly ClassGeneratorModel _model;
        private readonly ClassGeneratorView _view;
        
        public ClassGeneratorPresenter()
        {
            _model = new ClassGeneratorModel();
            _view = new ClassGeneratorView();
        }
        
        public void Draw(Rect windowPosition)
        {
            _view.Draw(windowPosition, _model.Layers, _model.NamespaceName);
        }
        
        public IReadOnlyList<ClassGeneratorModel.LayerSettings> GetLayerSettingsList => _model.LayerSettingsList;
    }
}