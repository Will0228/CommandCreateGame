using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using WordingSettingInfo = Editor.ClassGenerator.ClassGeneratorWordingSettingInfo;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingViewContainer
    {
        private readonly ClassGeneratorWordingSettingTextAreaView _textAreaView;
        private readonly ClassGeneratorWordingSettingApplyTemplateView _applyTemplateView;

        internal ClassGeneratorWordingSettingViewContainer(ClassGeneratorWordingSettingInfo info)
        {
            _textAreaView = new ClassGeneratorWordingSettingTextAreaView(info);
            _applyTemplateView = new ClassGeneratorWordingSettingApplyTemplateView();
        }

        internal void UpdateData(IReadOnlyDictionary<ComponentRoleType, WordingSettingInfo> dict)
        {
            _textAreaView.UpdateData(dict);
        }
        
        internal void Draw(Rect windowPosition)
        {
            var halfWidth = windowPosition.width / 2f;

            EditorGUILayout.BeginHorizontal();
            {
                _textAreaView.Draw();
                _applyTemplateView.Draw(halfWidth);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}