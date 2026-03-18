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

        private readonly float _halfWidth;
        
        internal ClassGeneratorWordingSettingImplementationDetailsView(ClassGeneratorWordingSettingInfo info, Rect windowPosition)
        {
            _textAreaView = new ClassGeneratorWordingSettingTextAreaView(info);
            _applyTemplateView = new ClassGeneratorWordingSettingApplyTemplateView();
            _halfWidth = windowPosition.width / 2;
        }
        
        internal void Draw()
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