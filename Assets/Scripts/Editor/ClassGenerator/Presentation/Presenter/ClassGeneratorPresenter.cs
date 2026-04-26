using System.Collections.Generic;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorPresenter
    {
        private readonly ClassGeneratorModel _model;
        private readonly ClassGeneratorView _view;
        
        [EditorInject]
        public ClassGeneratorPresenter(ClassGeneratorSimpleDIContainer container)
        {
            _model = container.Resolve<ClassGeneratorModel>();
            _view = container.Resolve<ClassGeneratorView>();
        }
        
        public void Draw(Rect windowPosition)
        {
            _view.Draw(windowPosition, _model.Layers, _model.NamespaceName);
        }
        
        public IReadOnlyList<ClassGeneratorModel.LayerSettings> GetLayerSettingsList => _model.LayerSettingsList;
    }
}