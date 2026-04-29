using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorPresenter : IDisposable
    {
        private readonly ClassGeneratorModel _model;
        private readonly ClassGeneratorView _view;
        
        private readonly CompositeDisposable _disposable = new ();

        [EditorInject]
        public ClassGeneratorPresenter(ClassGeneratorSimpleDIContainer container)
        {
            _model = container.Resolve<ClassGeneratorModel>();
            _view = container.Resolve<ClassGeneratorView>();
            
            Bind();
        }

        private void Bind()
        {
            _view.OnAddClassAsObservable
                .Subscribe(_model.AddClass)
                .AddTo(_disposable);
            
            _view.OnRemoveClassAsObservable
                .Subscribe(_model.RemoveClass)
                .AddTo(_disposable);
            
            _view.OnRenameClassAsObservable
                .Subscribe(_model.RenameClass)
                .AddTo(_disposable);
        }
        
        public void Draw(Rect windowPosition)
        {
            _view.Draw(windowPosition, _model.Layers, _model.NamespaceName);
        }
        
        public IReadOnlyList<ClassGeneratorModel.LayerSettings> GetLayerSettingsList => _model.LayerSettingsList;
        void IDisposable.Dispose()
        {
            _disposable.Dispose();
        }
    }
}