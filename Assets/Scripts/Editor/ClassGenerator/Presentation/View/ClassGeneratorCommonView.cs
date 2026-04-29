using System;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorCommonView : IDisposable
    {
        private readonly TabToolbarEmitter _categorySelectToolbarEmitter;
        private readonly string[] _labels = { "Generator", "Folder Settings", "Wording Settings" };

        private readonly CompositeDisposable _disposables = new ();

        private readonly Subject<int> _onCategoryIndexChangedSubject = new();
        public Observable<int> OnChangedCategoryIndexAsObservable => _onCategoryIndexChangedSubject;

        private readonly Subject<Unit> _onCreateButtonClickedSubject = new();
        public Observable<Unit> OnCreateButtonClickedAsObservable => _onCreateButtonClickedSubject.AsObservable();
        
        public ClassGeneratorCommonView()
        {
            // var labels = new string[] { "Generator", "Folder Settings", "Wording Settings" };
            // _categorySelectToolbarEmitter = new(labels);
        }

        public void Draw(int currentCategoryIndex)
        {
            // EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            // _categorySelectToolbarEmitter.Draw(currentCategoryIndex);
            // GUILayout.FlexibleSpace();
            // if (GUILayout.Button("Create Files", EditorStyles.toolbarButton, GUILayout.Width(80)))
            // {
            //     _onCreateButtonClickedSubject.OnNext(Unit.Default);
            // }
            //
            // EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            {
                DrawTabButton(currentCategoryIndex);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawTabButton(int currentCategoryIndex)
        {
            for(int i = 0; i < _labels.Length; i++)
            {
                // 現在選択されているタブかどうかでスタイルを変える
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.alignment = TextAnchor.MiddleCenter;
                style.fixedHeight = 120;
                style.fixedWidth = 120;
                style.margin = new RectOffset(5, 5, 2, 2);

                if (i == currentCategoryIndex)
                {
                    GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f);
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                }

                if (GUILayout.Button(_labels[i], style))
                {
                    _onCategoryIndexChangedSubject.OnNext(i);
                }
                
                // 色をリセット
                GUI.backgroundColor = Color.white;
            }
        }

        void IDisposable.Dispose()
        {
            _disposables.Dispose();
            _onCategoryIndexChangedSubject.Dispose();
        }
    }
}