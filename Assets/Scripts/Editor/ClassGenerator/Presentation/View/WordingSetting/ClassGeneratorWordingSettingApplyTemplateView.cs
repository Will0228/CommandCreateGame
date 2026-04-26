using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingApplyTemplateView
    {
        internal void Draw(float viewWidth)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox,  GUILayout.Width(viewWidth));
            {
                if (GUILayout.Button("Clear"))
                {
                    
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}