using System;
using System.Linq;
using R3;
using UnityEngine;

using ViewContainer = Editor.ClassGenerator.ClassGeneratorFolderSettingViewContainer;
using Model = Editor.ClassGenerator.ClassGeneratorFolderSettingModel;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingPresenter : IDisposable
    {
        private readonly ViewContainer _viewContainer;
        private readonly Model _model;
        
        private readonly CompositeDisposable _disposables = new();
        
        internal ClassGeneratorFolderSettingPresenter(ViewContainer viewContainer, Model model)
        {
            _viewContainer = viewContainer;
            _model = model;

            Bind();
        }

        private void Bind()
        {
            _viewContainer.OnLayerButtonClickedAsObservable
                .Subscribe(_model.SetSelectedLayerType)
                .AddTo(_disposables);
            
            _viewContainer.OnFolderButtonClickedAsObservable
                .Where(_ => _model.SelectedLayerType != AppLayerType.None)
                .Subscribe(_model.SetFolderPath)
                .AddTo(_disposables);
        }
        
        internal void Draw(Rect windowPosition) => _viewContainer.Draw(windowPosition, _model.LayerPathDict, _model.PathInfos.Select(info => new ClassGeneratorFolderSettingPathDto(info)).ToList());

        void IDisposable.Dispose()
        {
            ((IDisposable)_viewContainer).Dispose();
            _disposables?.Dispose();
        }
    }
}