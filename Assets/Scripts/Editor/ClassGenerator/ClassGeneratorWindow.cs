using System;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWindow : EditorWindow
    {
        private ClassGeneratorModel _model;
        private ClassGeneratorPresenter _presenter;
        private ClassGeneratorView _view;
        
        private ClassGeneratorFolderSettingModel _folderSettingModel;
        private ClassGeneratorFolderSettingPresenter _folderSettingPresenter;
        private ClassGeneratorFolderSettingViewContainer _folderSettingViewContainer;
        
        // タブの状態管理
        private int _selectedTabIndex = 0;
        private readonly string[] _tabLabels = { "Generator", "Folder Settings", "History" };


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

            _folderSettingModel = new ClassGeneratorFolderSettingModel();
            _folderSettingViewContainer = new ClassGeneratorFolderSettingViewContainer();
            _folderSettingPresenter = new ClassGeneratorFolderSettingPresenter(_folderSettingViewContainer, _folderSettingModel);
        }
        
        private void OnDisable()
        {
            _presenter.Dispose();
            ((IDisposable)_folderSettingPresenter).Dispose();
        }

        private void OnGUI()
        {
            DrawTabs();
            
            switch (_selectedTabIndex)
            {
                case 0: // Generator
                    _presenter.Draw(position);
                    break;
                case 1: // Settings
                    _folderSettingPresenter.Draw(position);
                    break;
                case 2: // History
                    DrawHistory();
                    break;
            }
        }
        
        private void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            _selectedTabIndex = GUILayout.Toolbar(_selectedTabIndex, _tabLabels, EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            EditorGUILayout.LabelField("Global Template Settings", EditorStyles.boldLabel);
            _model.NamespaceName = EditorGUILayout.TextField("Default Namespace", _model.NamespaceName);
            // その他、共通設定などをここに記述
            EditorGUILayout.EndVertical();
        }

        private void DrawHistory()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            EditorGUILayout.LabelField("Generation History", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("No recent logs found.", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
    }
}