using System;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWindow : EditorWindow
    {
        private ClassGeneratorPresenter _presenter;
        private ClassGeneratorFolderSettingPresenter _folderSettingPresenter;
        
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
            _presenter = new ClassGeneratorPresenter();

            _folderSettingPresenter = new ClassGeneratorFolderSettingPresenter();
        }
        
        private void OnDisable()
        {
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
            }
        }
        
        private void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            _selectedTabIndex = GUILayout.Toolbar(_selectedTabIndex, _tabLabels, EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Files", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                var settingsFiles = _presenter.GetLayerSettingsList;
                var hasPathSettingFiles = _folderSettingPresenter.GetLayerTypeAndPaths(settingsFiles);
                var createFilesService = new ClassGeneratorCreateFilesService();
                createFilesService.CreateFiles(hasPathSettingFiles);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}