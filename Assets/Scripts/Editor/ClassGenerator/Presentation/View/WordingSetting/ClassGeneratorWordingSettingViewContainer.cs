using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingViewContainer
    {
        private readonly ClassGeneratorWordingSettingTextAreaView _textAreaView;
        private readonly ClassGeneratorWordingSettingApplyTemplateView _applyTemplateView;

        internal ClassGeneratorWordingSettingViewContainer()
        {
            _textAreaView = new ClassGeneratorWordingSettingTextAreaView();
            _applyTemplateView = new ClassGeneratorWordingSettingApplyTemplateView();
        }

        internal void Configure(string defaultText)
        {
            _textAreaView.Configure(defaultText);
        }
        
        internal void Draw(Rect windowPosition)
        {
            var halfWidth = windowPosition.width / 2f;
            
            EditorGUILayout.BeginHorizontal();
            {
                _textAreaView.Draw(halfWidth);
                _applyTemplateView.Draw(halfWidth);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}