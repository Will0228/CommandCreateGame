using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

using ViewContainer = Editor.ClassGenerator.ClassGeneratorFolderSettingViewContainer;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingPresenter : IDisposable
    {
        private readonly ViewContainer _viewContainer;
        private readonly ClassGeneratorFolderSettingLayerModel _layerModel;
        private readonly ClassGeneratorFolderSettingPathModel _pathModel;
        
        private readonly CompositeDisposable _disposables = new();
        
        internal ClassGeneratorFolderSettingPresenter()
        {
            _viewContainer = new ClassGeneratorFolderSettingViewContainer();
            _layerModel = new ClassGeneratorFolderSettingLayerModel();
            _pathModel = new ClassGeneratorFolderSettingPathModel();

            Bind();
        }

        private void Bind()
        {
            _viewContainer.OnLayerButtonClickedAsObservable
                .Subscribe(_layerModel.SetSelectedLayerType)
                .AddTo(_disposables);
            
            _viewContainer.OnFolderButtonClickedAsObservable
                .Subscribe(index => _layerModel.SetFolderPath(_pathModel.PathInfos[index].Path))
                .AddTo(_disposables);
            
            _viewContainer.OnLayerSeparateSettingsAsObservable
                .Subscribe(_layerModel.SetSeparateSettingsDict)
                .AddTo(_disposables);
        }
        
        internal void Draw(Rect windowPosition)
        {
            _viewContainer.Draw(windowPosition, _layerModel.LayerPathDict, _layerModel.ComponentRolePathDict, _layerModel.IsSeparateSettingsDict, _pathModel.PathInfos.Select(info => new ClassGeneratorFolderSettingPathDto(info)).ToList());
        }

        internal IReadOnlyDictionary<ClassGeneratorModel.LayerSettings, string> GetLayerTypeAndPaths(
            IReadOnlyList<ClassGeneratorModel.LayerSettings> settingsList)
        {
            return _layerModel.GetLayerTypeAndPaths(settingsList);
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)_viewContainer).Dispose();
            _disposables?.Dispose();
        }
    }
}