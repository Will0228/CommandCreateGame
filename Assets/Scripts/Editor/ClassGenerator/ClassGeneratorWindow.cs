using System;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWindow : EditorWindow, IDisposable
    {
        private ClassGeneratorSimpleDIContainer _container;
        
        private ClassGeneratorCommonPresenter _commonPresenter;
        private ClassGeneratorCommonModel _commonModel;
        
        private ClassGeneratorPresenter _presenter;
        private ClassGeneratorFolderSettingPresenter _folderSettingPresenter;
        private ClassGeneratorWordingSettingPresenter _wordingSettingPresenter;
        
        private readonly CompositeDisposable _disposable = new();
        
        [MenuItem("Tools/Class Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<ClassGeneratorWindow>("Class Generator");
            window.minSize = new Vector2(700, 500);
        }

        private void OnEnable()
        {
            _container = new();
            _commonPresenter = _container.Resolve<ClassGeneratorCommonPresenter>();
            _commonModel = _container.Resolve<ClassGeneratorCommonModel>();
            _presenter = _container.Resolve<ClassGeneratorPresenter>();
            _folderSettingPresenter = _container.Resolve<ClassGeneratorFolderSettingPresenter>();
            _wordingSettingPresenter = _container.Resolve<ClassGeneratorWordingSettingPresenter>();
            
            Configure();
            SetEvent();
        }

        private void Configure()
        {
            _wordingSettingPresenter.Configure(position);
        }

        private void SetEvent()
        {
            _commonModel.SelectedCategoryIndexProp
                .Subscribe(UpdateData)
                .AddTo(_disposable);
            
            _commonPresenter.OnCreateButtonClickedAsObservable
                .Subscribe(_ => CreateFiles())
                .AddTo(_disposable);
        }
        
        private void UpdateData(int index)
        {
            switch (index)
            {
                case 2:
                    _wordingSettingPresenter.UpdateData(_presenter.GetLayerSettingsList);
                    break;
                default:
                    break; // 一旦何もしない
            }
        }

        private void OnGUI()
        {
            _commonPresenter.Draw();
            
            switch (_commonModel.SelectedCategoryIndex)
            {
                case 0:
                    _presenter.Draw(position);
                    break;
                case 1:
                    _folderSettingPresenter.Draw(position);
                    break;
                case 2:
                    _wordingSettingPresenter.Draw();
                    break;
            }
        }
        
        private void CreateFiles()
        {
            var settingsFiles = _presenter.GetLayerSettingsList;
            var hasPathSettingFiles = _folderSettingPresenter.GetLayerTypeAndPaths(settingsFiles);
            var createFilesService = new ClassGeneratorCreateFilesService();
            createFilesService.CreateFiles(hasPathSettingFiles);
        }
        
        private void OnDisable() => Dispose();

        public void Dispose()
        {
            _disposable?.Dispose();
            ((IDisposable)_container).Dispose();
        }
    }
}