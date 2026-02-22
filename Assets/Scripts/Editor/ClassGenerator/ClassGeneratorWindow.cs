using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWindow : EditorWindow
    {
        [Serializable]
        internal class LayerSettings
        {
            internal string Label;
            internal bool IsEnabled;
            internal readonly List<string> ClassNames = new(){"test"};
            internal string Suffix;
            
            public LayerSettings(string label, string suffix)
            {
                Label = label;
                Suffix = suffix;
            }
        }
        
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
            // 依存関係を組み立てる
            _model = new ClassGeneratorModel();
            _presenter = new ClassGeneratorPresenter(_model);
            _view = new ClassGeneratorView();
        }

        private void OnGUI()
        {
            // Viewに描画を委譲し、操作データはPresenterを介して扱う
            _view.Draw(position, _presenter);
        }
    }
}