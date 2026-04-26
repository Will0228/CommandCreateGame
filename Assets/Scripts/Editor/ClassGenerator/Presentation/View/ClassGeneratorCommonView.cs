using System;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorCommonView : IDisposable
    {
        private readonly TabToolbarEmitter _categorySelectToolbarEmitter;

        private readonly CompositeDisposable _disposables = new ();

        public Observable<int> OnChangedCategoryIndexAsObservable => _categorySelectToolbarEmitter.OnChangedAsObservable;

        private readonly Subject<Unit> _onCreateButtonClickedSubject = new();
        public Observable<Unit> OnCreateButtonClickedAsObservable => _onCreateButtonClickedSubject.AsObservable();
        
        public ClassGeneratorCommonView()
        {
            var labels = new string[] { "Generator", "Folder Settings", "Wording Settings" };
            _categorySelectToolbarEmitter = new(labels);
        }

        public void Draw(int currentCategoryIndex)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            _categorySelectToolbarEmitter.Draw(currentCategoryIndex);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Files", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                _onCreateButtonClickedSubject.OnNext(Unit.Default);
            }

            EditorGUILayout.EndHorizontal();
        }

        void IDisposable.Dispose()
        {
            _disposables.Dispose();
        }
    }
}