using System.Collections.Generic;
using R3;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Presentation.DemoViewTest
{
   internal sealed class DemoViewBaseEditorDrawer
    {
        private readonly Subject<Unit> _onExecuteMethodSubject = new();
        public Observable<Unit> OnExecuteMethodAsObservable => _onExecuteMethodSubject.AsObservable();
        
        private SerializedObject _serializedObject;
        
        public void Configure(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
        }

        public void OnInspectorGUI(IReadOnlyList<DemoViewBaseEditorParameterDefinition.IGuiParameter> commonParameters,
            IReadOnlyList<DemoViewBaseEditorParameterDefinition.IGuiParameter> animatorParameters)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Demo SetUp", EditorStyles.boldLabel);
                    
                _serializedObject.Update();
                
                foreach (var parameter in commonParameters)
                {
                    CreateField(parameter);
                }
                
                EditorGUILayout.Space();

                if (GUILayout.Button("Invoke"))
                {
                    _onExecuteMethodSubject.OnNext(Unit.Default);
                }
                
                _serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Animator SetUp", EditorStyles.boldLabel);
                    
                _serializedObject.Update();
                
                foreach (var parameter in animatorParameters)
                {
                    CreateField(parameter);
                }
                
                EditorGUILayout.Space();

                if (GUILayout.Button("Invoke"))
                {
                    _onExecuteMethodSubject.OnNext(Unit.Default);
                }
                
                _serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndVertical();
        }
        
        private void CreateField(DemoViewBaseEditorParameterDefinition.IGuiParameter parameter)
        {
            if (parameter is DemoViewBaseEditorParameterDefinition.IntParameter intParameter)
            {
                var newValue = EditorGUILayout.IntField(parameter.ParameterName, intParameter.Value);
                if (newValue != intParameter.Value)
                {
                    parameter.SetField(newValue.ToString());
                }
            }
            else if(parameter is DemoViewBaseEditorParameterDefinition.SpriteParameter spriteParameter)
            {
                var newValue = (Sprite)EditorGUILayout.ObjectField(parameter.ParameterName, spriteParameter.Value, typeof(Sprite), false);
                if (newValue != spriteParameter.Value)
                {
                    parameter.SetField(AssetDatabase.GetAssetPath(newValue));
                }
            }
            else if(parameter is DemoViewBaseEditorParameterDefinition.AudioParameter audioParameter)
            {
                var newValue = (AudioClip)EditorGUILayout.ObjectField(parameter.ParameterName, audioParameter.Value, typeof(AudioClip), false);
                if (newValue != audioParameter.Value)
                {
                    parameter.SetField(AssetDatabase.GetAssetPath(newValue));
                }
            }
            else if (parameter is DemoViewBaseEditorParameterDefinition.AnimatorParameter animatorControllerParameter)
            {
                var newValue = (AnimatorController)EditorGUILayout.ObjectField(parameter.ParameterName, animatorControllerParameter.Value, typeof(AnimatorController), false);
                if (newValue != animatorControllerParameter.Value)
                {
                    parameter.SetField(AssetDatabase.GetAssetPath(newValue));
                }
            }
        }
    }
}