using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWindow : EditorWindow
    {
        private ClassGeneratorModel _model;
        private ClassGeneratorPresenter _presenter;
        private ClassGeneratorView _view;

        [MenuItem("Tools/Class Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<ClassGeneratorWindow>("Class Generator");
            window.minSize = new Vector2(700, 500);
        }

        private void OnEnable()
        {
            _model = new ClassGeneratorModel();
            _view = new ClassGeneratorView();
            _presenter = new ClassGeneratorPresenter(_model, _view);
        }
        
        private void OnDisable() => _presenter.Dispose();

        private void OnGUI()
        {
            _presenter.Draw(position);
        }
    }
}