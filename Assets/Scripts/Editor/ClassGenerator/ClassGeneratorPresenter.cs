using System;
using System.Collections.Generic;
using System.IO;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorPresenter : IDisposable
    {
        private readonly ClassGeneratorModel _model;
        private readonly ClassGeneratorView _view;
        private readonly CompositeDisposable _compositeDisposables = new();
        
        public ClassGeneratorPresenter(ClassGeneratorModel model, ClassGeneratorView view)
        {
            _model = model;
            _view = view;
            
            SetEvent();
        }

        private void SetEvent()
        {
            _view.OnGenerateRequestedAsObservable
                .Subscribe(OnGenerateRequested)
                .AddTo(_compositeDisposables);
        }

        public void Draw(Rect windowPosition)
        {
            _view.Draw(windowPosition, _model.Layers, _model.NamespaceName);
        }
        
        public void OnGenerateRequested(Unit _)
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

        public void Dispose()
        {
            _view.Dispose();
            _compositeDisposables.Dispose();
        }
    }
}