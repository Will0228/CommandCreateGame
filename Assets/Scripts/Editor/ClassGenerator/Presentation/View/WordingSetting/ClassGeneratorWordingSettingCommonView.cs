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
        
        private readonly ReactiveProperty<int> _onChangeTabIndexSubject = new();
        public ReadOnlyReactiveProperty<int> OnChangeTabIndexAsObservable => _onChangeTabIndexSubject;
        
        internal void Draw()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                _onChangeTabIndexSubject.Value = GUILayout.Toolbar(_onChangeTabIndexSubject, _tabLabels, EditorStyles.toolbarButton);
            }
        }

        void IDisposable.Dispose()
        {
            _onChangeTabIndexSubject.Dispose();
        }
    }
}