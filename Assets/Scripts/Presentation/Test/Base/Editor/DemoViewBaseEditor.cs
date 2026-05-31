using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Presentation.Test;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#if UNITY_EDITOR

namespace Presentation.DemoViewTest
{
    [CustomEditor(typeof(DemoViewBase), true)]
    internal partial class DemoViewBaseEditor : Editor
    {
        private DemoViewBase _target;
        private readonly HashSet<string> _serializedPropertyNames = new();
        private readonly List<IGuiParameter> _cachedParameters = new();
        
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

            var methodParameters = GetDemoSetupMethodParameters();
            foreach (var parameter in methodParameters)
            {
                IGuiParameter param = null;
                if(parameter.ParameterType == typeof(int))
                {
                    param = new IntParameter();
                    param.Initialize(parameter.Name, "0");
                }
                else if(parameter.ParameterType == typeof(Sprite))
                {
                    param = new SpriteParameter();
                    param.Initialize(parameter.Name, null);
                }
                _cachedParameters.Add(param);
            }
        }

        private MethodInfo GetDemoSetupMethod()
        {
            var method = _target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(m => m.GetCustomAttribute<DemoSetupAttribute>() != null);
            if (method == null)
            {
                Debug.LogError("DemoSetupAttributeをつけたメソッドが存在しません");
            }
            
            return method;
        }
        
        private ParameterInfo[] GetDemoSetupMethodParameters() => GetDemoSetupMethod().GetParameters();

        private void CreateField(IGuiParameter parameter)
        {
            if (parameter is IntParameter intParameter)
            {
                var newValue = EditorGUILayout.IntField(parameter.ParameterName, intParameter.Value);
                if (newValue != intParameter.Value)
                {
                    parameter.SetField(newValue.ToString());
                }
            }
            else if(parameter is SpriteParameter spriteParameter)
            {
                var newValue = (Sprite)EditorGUILayout.ObjectField(parameter.ParameterName, spriteParameter.Value, typeof(Sprite), false);
                if (newValue != spriteParameter.Value)
                {
                    parameter.SetField(AssetDatabase.GetAssetPath(newValue));
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Demo SetUp", EditorStyles.boldLabel);
                    
                serializedObject.Update();
                
                var iterator = serializedObject.GetIterator();
                var enterChildren = true;

                while (iterator.NextVisible(enterChildren))
                {
                    if(iterator.name == "m_Script")
                    {
                        continue;
                    }
                    
                    
                    // EditorGUILayout.PropertyField(iterator, true);
                    enterChildren = false;
                }
                
                // var tests = GetDemoSetupMethodParameters();
                foreach (var parameter in _cachedParameters)
                {
                    CreateField(parameter);
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
                    for (int i = 0; i < _cachedParameters.Count; i++)
                    {
                        if (_cachedParameters[i] is IntParameter integerParameter)
                        {
                            parameterValues[i] = integerParameter.Value;
                        }
                        else if (_cachedParameters[i] is SpriteParameter spriteParameter)
                        {
                            parameterValues[i] = spriteParameter.Value;
                        }
                    }

                    demoSetupMethod.Invoke(_target, parameterValues);
                    EditorUtility.SetDirty(_target);
                    InternalEditorUtility.RepaintAllViews();
                }
                
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndVertical();
        }
    }
}

#endif
