using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingLayerView
    {
        private AppLayerType _selectedLayer = AppLayerType.None;
        
        internal void Draw(IReadOnlyDictionary<AppLayerType, string> layerPathDict,
            float sectionRectWidth)
        {
            EditorGUILayout.BeginVertical();
            
            foreach (var dict in layerPathDict)
            {
                if (dict.Key == AppLayerType.None)
                {
                    continue;
                }
                
                var sectionRect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(sectionRectWidth));
                {
                    // 選択されている場合の枠の強調表示
                    if (_selectedLayer == dict.Key)
                    {
                        var originalColor = GUI.color;
                        GUI.color = Color.yellow;
                        GUI.Box(sectionRect, GUIContent.none, EditorStyles.selectionRect);
                        GUI.color = originalColor;
                    }
                    
                    EditorGUILayout.LabelField($"{dict.Key} Layer", GUILayout.Width(120));
                    var buttonLabel = dict.Value;
                    if (GUILayout.Button(buttonLabel, GUILayout.Width(80)))
                    {
                        _selectedLayer = dict.Key;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(10);
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}