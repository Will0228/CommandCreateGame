using System;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    internal sealed class TabToolbarEmitter : IDisposable
    {
        private readonly string[] _labels;
        private readonly GUIStyle _style;

        private readonly Subject<int> _onChangedSubject = new();
        public Observable<int> OnChangedAsObservable => _onChangedSubject.AsObservable();

        public TabToolbarEmitter(string[] labels, GUIStyle style = null)
        {
            _labels = labels;
            _style = style ?? EditorStyles.toolbarButton;
        }

        public void Draw(int currentIndex)
        {
            int newIndex = GUILayout.Toolbar(currentIndex, _labels, _style);

            if (newIndex != currentIndex)
            {
                _onChangedSubject.OnNext(newIndex);
            }
        }

        void IDisposable.Dispose()
        {
            _onChangedSubject.Dispose();
        }
    }
}