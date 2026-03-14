using System;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWindow : EditorWindow, IDisposable
    {
        private ClassGeneratorPresenter _presenter;
        private ClassGeneratorFolderSettingPresenter _folderSettingPresenter;
        private ClassGeneratorWordingSettingPresenter _wordingSettingPresenter;
        
        // タブの状態管理
        private int _selectedTabIndex = 0;
        private readonly ReactiveProperty<int> _selectedTabIndexProp = new();
        private readonly string[] _tabLabels = { "Generator", "Folder Settings", "Wording Settings" };
        
        private readonly CompositeDisposable _disposable = new();
        
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
            _wordingSettingPresenter = new ClassGeneratorWordingSettingPresenter(_disposable);
            
            SetEvent();
        }

        private void SetEvent()
        {
            _selectedTabIndexProp
                .Subscribe(UpdateData)
                .AddTo(_disposable);
        }
        
        private void UpdateData(int index)
        {
            switch (_selectedTabIndex)
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
            DrawTabs();
            
            switch (_selectedTabIndex)
            {
                case 0:
                    _presenter.Draw(position);
                    break;
                case 1:
                    _folderSettingPresenter.Draw(position);
                    break;
                case 2:
                    _wordingSettingPresenter.Draw(position);
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
        
        private void OnDisable() => Dispose();

        public void Dispose()
        {
            ((IDisposable)_folderSettingPresenter)?.Dispose();
            ((IDisposable)_wordingSettingPresenter)?.Dispose();
            _selectedTabIndexProp?.Dispose();
            _disposable?.Dispose();
        }
    }
}