using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Presentation.Test
{
    /// <summary>
    /// Viewのテストを行うための基底クラス
    /// </summary>
    internal abstract class DemoViewBase : MonoBehaviour
    {
        
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DemoSetupAttribute : Attribute
    {
        
    }
    
# if UNITY_EDITOR
    [CustomEditor(typeof(DemoViewBase), true)]
    internal class DemoViewBaseEditor : Editor
    {
        private DemoViewBase _target;
        private readonly HashSet<string> _serializedPropertyNames = new();
        
        public void Awake()
        {
            _target = (DemoViewBase)target;
            
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                if(iterator.name == "m_Script")
                {
                    continue;
                }
                
                _serializedPropertyNames.Add(iterator.name);
                enterChildren = false;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                if(iterator.name == "m_Script")
                {
                    continue;
                }
                
                EditorGUILayout.PropertyField(iterator, true);
                enterChildren = false;
            }
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Invoke"))
            {
                var demoSetupMethod = _target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .FirstOrDefault(m => m.GetCustomAttribute<DemoSetupAttribute>() != null);

                if (demoSetupMethod == null)
                {
                    Debug.LogError("DemoSetupAttributeが付与されたメソッドが見つかりませんでした。");
                    return;
                }
                
                var parameters = demoSetupMethod.GetParameters();
                if (parameters.Length == 0)
                {
                    Debug.LogError("引数がないためデモのチェックができません");
                    return;
                }
                
                var parameterValues = new object[parameters.Length];
                iterator = serializedObject.GetIterator();
                enterChildren = true;
                var idx = 0;
                while (iterator.NextVisible(enterChildren))
                {
                    if(iterator.name == "m_Script")
                    {
                        continue;
                    }
                
                    parameterValues[idx] = iterator.boxedValue;
                    idx++;
                    enterChildren = false;
                }

                demoSetupMethod.Invoke(_target, parameterValues);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}