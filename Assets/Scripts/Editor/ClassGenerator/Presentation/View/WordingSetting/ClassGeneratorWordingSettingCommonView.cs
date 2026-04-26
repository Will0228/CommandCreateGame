using System;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    /// <summary>
    /// Viewを切り替えるタブなどを管理
    /// </summary>
    internal sealed class ClassGeneratorWordingSettingCommonView : IDisposable
    {
        private readonly string[] _tabLabels = { "Implementation Details", "Class Details" };
        
        private readonly ReactiveProperty<int> _onChangeTabIndexProp = new();
        public ReadOnlyReactiveProperty<int> OnChangeTabIndexProp => _onChangeTabIndexProp;
        
        internal void Draw()
        {
            using(EditorGUILayoutExtension.CreateEditorGUILayoutScope(EditorStyles.toolbar))
            {
                _onChangeTabIndexProp.Value = GUILayout.Toolbar(_onChangeTabIndexProp.Value, _tabLabels, EditorStyles.toolbarButton);
            }
        }

        void IDisposable.Dispose()
        {
            _onChangeTabIndexProp.Dispose();
        }
    }
}