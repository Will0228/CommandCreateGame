using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

#if UNITY_EDITOR

namespace Presentation.DemoViewTest
{
    internal sealed class DemoViewBaseEditorParameterDefinition
    {
        internal interface IGuiParameter
        {
            public string ParameterName { get; }

            void Initialize(string parameterName, string defaultValue);
            void SetField(string value);
        }

        internal sealed class IntParameter : IGuiParameter
        {
            public int Value { get; private set; }

            private string _parameterName;
            string IGuiParameter.ParameterName => _parameterName;

            void IGuiParameter.Initialize(string parameterName, string defaultValue)
            {
                _parameterName = parameterName;
                ((IGuiParameter)this).SetField(defaultValue);
            }

            void IGuiParameter.SetField(string value) => Value = int.Parse(value);
        }

        internal sealed class AnimatorParameter : IGuiParameter
        {
            internal const string ANIMATOR_CONTROLLER_TAG = "AnimatorController";
            internal const string ANIMATION_CLIP_TAG = "AnimationClip";

            private string _animatorControllerPath;
            public AnimatorController AnimatorControllerValue { get; private set; }
            public readonly List<AnimationClip> AnimationClipValues = new();

            private string _parameterName;
            string IGuiParameter.ParameterName => _parameterName;

            void IGuiParameter.Initialize(string parameterName, string defaultValue)
            {
                _parameterName = parameterName;
                ((IGuiParameter)this).SetField($"{ANIMATOR_CONTROLLER_TAG}\\{defaultValue}");
            }

            // valueには '\\' で異なる中身が渡される
            // 0番目にはAnimatorControllerかAnimationClipが渡される
            // 1番目に該当のkeyが渡される
            // 2番目はAnimationClipの場合のみ入り、何番目のStateを変更するか選べる
            void IGuiParameter.SetField(string value)
            {
                var splitResults = value.Split('\\');
                if (splitResults[0] == ANIMATOR_CONTROLLER_TAG)
                {
                    var guids = AssetDatabase.FindAssets($"{Path.GetFileNameWithoutExtension(splitResults[1])} t:AnimatorController");
                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        if (_animatorControllerPath == path)
                        {
                            return;
                        }

                        _animatorControllerPath = path;
                        AnimationClipValues.Clear();
                        AnimatorControllerValue = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

                        foreach (var layer in AnimatorControllerValue.layers)
                        {
                            var stateMachine = layer.stateMachine;
                            DumpStateMachine(stateMachine);
                        }
                    }
                    else
                    {
                        Debug.LogError($"AnimatorController parameter {value} not found");
                    }
                }
                else if (splitResults[0] == ANIMATION_CLIP_TAG)
                {
                    var guids = AssetDatabase.FindAssets($"{splitResults[1]} t:AnimationClip");
                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);

                        if (path.EndsWith(".controller", System.StringComparison.OrdinalIgnoreCase))
                        {
                            var targetClip = AssetDatabase.LoadAllAssetsAtPath(path)
                                .Where(asset => asset is AnimationClip)
                                .FirstOrDefault(clip => clip.name == splitResults[1]);
                            if (targetClip != null)
                            {
                                AnimationClipValues[int.Parse(splitResults[2])] = targetClip as AnimationClip;
                                break;
                            }
                        }
                        else if (path.EndsWith(".anim", System.StringComparison.OrdinalIgnoreCase))
                        {
                            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                            AnimationClipValues[int.Parse(splitResults[2])] = clip;
                            break;
                        }
                    }
                }
                else
                {
                    Debug.LogError("想定していないタグが\\で区切られたテキストの0番目に入っています");
                }
            }

            void DumpStateMachine(AnimatorStateMachine stateMachine)
            {
                foreach (var childState in stateMachine.states)
                {
                    var state = childState.state;

                    if (state.motion != null)
                    {
                        if (state.motion is AnimationClip animationClip)
                        {
                            AnimationClipValues.Add(animationClip);
                        }
                        // else if(state.motion is BlendTree)
                    }

                    foreach (var transition in state.transitions)
                    {
                        var destinationState = transition.isExit ? "Exit" :
                            (transition.destinationState != null ? transition.destinationState.name : "SubStateMachine");
                        Debug.Log($"遷移先 : {destinationState}");
                    }
                }
            }
        }

        internal sealed class AudioParameter : IGuiParameter
        {
            public AudioClip Value { get; private set; }

            private string _parameterName;
            string IGuiParameter.ParameterName => _parameterName;

            void IGuiParameter.Initialize(string parameterName, string defaultValue)
            {
                _parameterName = parameterName;
                ((IGuiParameter)this).SetField(defaultValue);
            }

            void IGuiParameter.SetField(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    Value = null;
                    return;
                }

                // 1. もし送られてくる string が「GUID（一意の文字列）」の場合
                string assetPath = AssetDatabase.GUIDToAssetPath(value);

                // 2. もし送られてくる string が GUID ではなく直接の「パス（Assets/.../sound.wav）」だった場合のフォールバック
                if (string.IsNullOrEmpty(assetPath))
                {
                    assetPath = value;
                }

                // 3. 確定したパスから AudioClip をロードして保持する
                Value = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

                if (Value == null)
                {
                    Debug.LogWarning($"[AudioParameter] オーディオアセットが見つかりませんでした: {value}");
                }
            }
        }

        internal class SpriteParameter : IGuiParameter
        {
            public Sprite Value { get; private set; }
            private string _parameterName;
            string IGuiParameter.ParameterName => _parameterName;

            void IGuiParameter.Initialize(string parameterName, string defaultValue)
            {
                _parameterName = parameterName;
                ((IGuiParameter)this).SetField(defaultValue);
            }

            void IGuiParameter.SetField(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    Value = null;
                    return;
                }

                // 1. 文字列（GUID）からアセットのパス（"Assets/.../image.png"）を検索
                string assetPath = AssetDatabase.GUIDToAssetPath(value);

                // もし入力された文字列がGUIDではなく直接の「パス」だった場合のフォールバック
                if (string.IsNullOrEmpty(assetPath))
                {
                    assetPath = value;
                }

                // 2. パスからSpriteとしてアセットをロードして保持
                Value = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

                if (Value == null)
                {
                    Debug.LogWarning($"[SpriteParameter] アセットが見つかりませんでした: {value}");
                }
            }
        }
    }
}

#endif