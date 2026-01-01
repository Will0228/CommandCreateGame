using System.Collections.Generic;
using System.Linq;
using Shared.DependencyContext;
using UnityEditor;

namespace Shared.DI.Editor
{
    [CustomEditor(typeof(DependencyContextBase), true)]
    public class DependencyContextEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var script = (DependencyContextBase)target;
            var typeNameProp = serializedObject.FindProperty("TypeName");
            
            // 1. 編集中のオブジェクト（target）自身の型のアセンブリを取得
            var targetAssembly = target.GetType().Assembly;

            // 2. そのアセンブリ内だけで DependencyContextBase を継承している型を探す
            var allContextTypes = targetAssembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DependencyContextBase)) && !t.IsAbstract)
                .ToList();

            var displayNames = new List<string> { "None (Root)" };
            displayNames.AddRange(allContextTypes.Select(t => t.Name));

            int currentIndex = allContextTypes.FindIndex(t => t.AssemblyQualifiedName == typeNameProp.stringValue) + 1;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Parent Selection ({targetAssembly.GetName().Name})", EditorStyles.boldLabel);
            int nextIndex = EditorGUILayout.Popup("Parent Context Type", currentIndex, displayNames.ToArray());

            if (nextIndex != currentIndex)
            {
                if (nextIndex == 0)
                    typeNameProp.stringValue = "";
                else
                    typeNameProp.stringValue = allContextTypes[nextIndex - 1].AssemblyQualifiedName;

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}