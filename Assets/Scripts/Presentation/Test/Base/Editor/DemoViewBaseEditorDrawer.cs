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

        private readonly Subject<AnimationClip> _onExecuteAnimationSubject = new();
        public Observable<AnimationClip> OnExecuteAnimationAsObservable => _onExecuteAnimationSubject.AsObservable();

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
                    var animationClips = CreateAnimatorControllerField(parameter);
                    if (animationClips != null)
                    {
                        foreach (var clip in animationClips)
                        {
                            EditorGUILayout.Space();
                            if (GUILayout.Button($"{clip.name} Invoke"))
                            {
                                _onExecuteAnimationSubject.OnNext(clip);
                            }
                        }
                    }
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
            else if (parameter is DemoViewBaseEditorParameterDefinition.SpriteParameter spriteParameter)
            {
                var newValue = (Sprite)EditorGUILayout.ObjectField(parameter.ParameterName, spriteParameter.Value, typeof(Sprite), false);
                if (newValue != spriteParameter.Value)
                {
                    parameter.SetField(AssetDatabase.GetAssetPath(newValue));
                }
            }
            else if (parameter is DemoViewBaseEditorParameterDefinition.AudioParameter audioParameter)
            {
                var newValue = (AudioClip)EditorGUILayout.ObjectField(parameter.ParameterName, audioParameter.Value, typeof(AudioClip), false);
                if (newValue != audioParameter.Value)
                {
                    parameter.SetField(AssetDatabase.GetAssetPath(newValue));
                }
            }
        }

        private IReadOnlyList<AnimationClip> CreateAnimatorControllerField(DemoViewBaseEditorParameterDefinition.IGuiParameter parameter)
        {
            if (parameter is DemoViewBaseEditorParameterDefinition.AnimatorParameter animatorControllerParameter)
            {
                var newAnimationControllerValue = (AnimatorController)EditorGUILayout.ObjectField(parameter.ParameterName, animatorControllerParameter.AnimatorControllerValue, typeof(AnimatorController), false);
                if (newAnimationControllerValue != animatorControllerParameter.AnimatorControllerValue)
                {
                    parameter.SetField($"{DemoViewBaseEditorParameterDefinition.AnimatorParameter.ANIMATOR_CONTROLLER_TAG}\\{AssetDatabase.GetAssetPath(newAnimationControllerValue)}");
                }

                var animationClipValues = animatorControllerParameter.AnimationClipValues;
                for (int i = 0; i < animationClipValues.Count; i++)
                {
                    var newAnimationClipValue = (AnimationClip)EditorGUILayout.ObjectField(animationClipValues[i].name, animationClipValues[i], typeof(AnimationClip), false);
                    if (newAnimationClipValue != animationClipValues[i])
                    {
                        parameter.SetField($"{DemoViewBaseEditorParameterDefinition.AnimatorParameter.ANIMATION_CLIP_TAG}\\{newAnimationClipValue.name}\\{i}");
                    }
                }

                return animationClipValues;
            }

            return null;
        }
    }
}