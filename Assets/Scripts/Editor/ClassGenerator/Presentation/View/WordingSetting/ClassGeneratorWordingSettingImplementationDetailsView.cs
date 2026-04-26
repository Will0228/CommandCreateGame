using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    /// <summary>
    /// 実装要望詳細を設定するView
    /// </summary>
    internal sealed class ClassGeneratorWordingSettingImplementationDetailsView
    {
        private readonly ClassGeneratorWordingSettingTextAreaView _textAreaView;
        private readonly ClassGeneratorWordingSettingApplyTemplateView _applyTemplateView;

        private float _halfWidth;
        
        public ClassGeneratorWordingSettingImplementationDetailsView()
        {
            _textAreaView = new ClassGeneratorWordingSettingTextAreaView();
            _applyTemplateView = new ClassGeneratorWordingSettingApplyTemplateView();
        }

        public void Configure(ClassGeneratorWordingSettingInfo info, Rect windowPosition)
        {
            _halfWidth = windowPosition.width / 2;
            _textAreaView.Configure(info);
        }
        
        public void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _textAreaView.Draw();
                _applyTemplateView.Draw(_halfWidth);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}