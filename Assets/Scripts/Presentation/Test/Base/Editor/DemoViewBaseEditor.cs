using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using R3;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#if UNITY_EDITOR

namespace Presentation.DemoViewTest
{
    [CustomEditor(typeof(DemoViewBase), true)]
    internal sealed class DemoViewBaseEditor : UnityEditor.Editor
    {
        internal enum ParameterType
        {
            None,
            Common,
            Animator
        }

        private DemoViewBase _target;
        private DemoViewBaseEditorParameterStore _parameterStore;
        private DemoViewBaseEditorDrawer _drawer;
        private SerializedObject _serializedObject;

        // 一旦試しに
        private readonly Dictionary<Transform, (Vector3 pos, Quaternion rot, Vector3 scale)> _transformCache = new();

        private CompositeDisposable _disposables = new();

        public void Awake()
        {
            _target = (DemoViewBase)target;
            _parameterStore = new DemoViewBaseEditorParameterStore();
            _drawer = new DemoViewBaseEditorDrawer();
            _serializedObject = new SerializedObject(_target);

            _parameterStore.SetupCommonParameters(GetDemoSetupMethodParameters(ParameterType.Common));
            _parameterStore.SetupAnimatorParameters(GetDemoSetupMethodParameters(ParameterType.Animator));
            _drawer.Configure(_serializedObject);

            SetEvent();
        }

        private void SetEvent()
        {
            // 通常のSetupメソッドを呼び出す場合
            _drawer.OnExecuteMethodAsObservable
                .Subscribe(_ =>
                {
                    var (demoSetupMethod, parameters) = InjectVariables<DemoViewBaseEditorAttribute.DemoSetupAttribute, DemoViewBase>(_target);
                    if (demoSetupMethod == null || parameters.Length == 0)
                    {
                        Debug.LogError("DemoViewBaseEditor: DemoSetup method is null");
                        return;
                    }

                    var commonParameters = _parameterStore.CommonParameters;
                    var parameterValues = new object[parameters.Length];
                    for (int i = 0; i < commonParameters.Count; i++)
                    {
                        if (commonParameters[i] is DemoViewBaseEditorParameterDefinition.IntParameter integerParameter)
                        {
                            parameterValues[i] = integerParameter.Value;
                        }
                        else if (commonParameters[i] is DemoViewBaseEditorParameterDefinition.SpriteParameter spriteParameter)
                        {
                            parameterValues[i] = spriteParameter.Value;
                        }
                        else if (commonParameters[i] is DemoViewBaseEditorParameterDefinition.AudioParameter audioParameter)
                        {
                            parameterValues[i] = audioParameter.Value;
                        }
                    }

                    MethodInvoke(demoSetupMethod, parameterValues);
                })
                .AddTo(_disposables);

            _drawer.OnExecuteAnimationAsObservable
                .Subscribe(clip =>
                {
                    var animator = _target.GetComponent<Animator>();
                    if (animator == null)
                    {
                        Debug.LogError("Animator component is not found on the target GameObject.");
                        return;
                    }

                    var startTime = EditorApplication.timeSinceStartup;
                    var isPlaying = true;
                    _transformCache.Clear();
                    foreach (var t in animator.GetComponentsInChildren<Transform>())
                    {
                        // localPosition と localRotation を保存
                        _transformCache[t] = (t.localPosition, t.localRotation, t.localScale);
                    }

                    AnimationMode.StartAnimationMode();
                    EditorApplication.update += OnEditorUpdate;

                    void StopPreview()
                    {
                        if (!isPlaying)
                        {
                            return;
                        }

                        EditorApplication.update -= OnEditorUpdate;
                        AnimationMode.StopAnimationMode();
                        foreach (var kvp in _transformCache)
                        {
                            if (kvp.Key != null) // ループ中にオブジェクトが削除されていた場合の安全ガード
                            {
                                kvp.Key.localPosition = kvp.Value.pos;
                                kvp.Key.localRotation = kvp.Value.rot;
                                kvp.Key.localScale = kvp.Value.scale;
                            }
                        }
                        EditorUtility.SetDirty(animator.gameObject);
                        SceneView.RepaintAll();
                        isPlaying = false;
                    }

                    void OnEditorUpdate()
                    {
                        if (animator == null || clip == null)
                        {
                            StopPreview();
                            return;
                        }

                        var elapsedTime = EditorApplication.timeSinceStartup - startTime;

                        if (elapsedTime >= clip.length)
                        {
                            StopPreview();
                            return;
                        }

                        var targetTime = (float)(elapsedTime % clip.length);

                        AnimationMode.BeginSampling();
                        AnimationMode.SampleAnimationClip(animator.gameObject, clip, targetTime);
                        AnimationMode.EndSampling();
                    }
                })
                .AddTo(_disposables);
        }

        public void OnDisable()
        {
            _parameterStore.Dispose();
            _disposables.Dispose();
        }

        private MethodInfo GetDemoSetupMethod<TAttribute>()
            where TAttribute : Attribute
        {
            var method = _target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(m => m.GetCustomAttribute<TAttribute>() != null);
            if (method == null)
            {
                Debug.LogError("DemoSetupAttributeをつけたメソッドが存在しません");
            }

            return method;
        }

        private ParameterInfo[] GetDemoSetupMethodParameters(ParameterType type)
        {
            return type switch
            {
                ParameterType.None => Array.Empty<ParameterInfo>(),
                ParameterType.Common => GetDemoSetupMethod<DemoViewBaseEditorAttribute.DemoSetupAttribute>().GetParameters(),
                ParameterType.Animator => GetDemoSetupMethod<DemoViewBaseEditorAttribute.DemoAnimatorSetupAttribute>().GetParameters()
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _drawer.OnInspectorGUI(_parameterStore.CommonParameters, _parameterStore.AnimatorParameters);
        }

        // 派生クラスで使用するメソッドに変数を注入するためのメソッド
        protected (MethodInfo? methodInfo, ParameterInfo[] parameterInfos) InjectVariables<TAttribute, TDemoViewBase>(TDemoViewBase target)
            where TAttribute : Attribute
            where TDemoViewBase : DemoViewBase
        {
            var demoSetupMethod = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(m => m.GetCustomAttribute<TAttribute>() != null);

            if (demoSetupMethod == null)
            {
                Debug.LogError("DemoSetupAttributeが付与されたメソッドが見つかりませんでした。");
                return (null, null);
            }

            var parameters = demoSetupMethod.GetParameters();
            if (parameters.Length == 0)
            {
                Debug.LogError("引数がないためデモのチェックができません");
                return (null, null);
            }
            return (demoSetupMethod, parameters);
        }

        /// <summary>
        /// 派生クラスのメソッド呼び出し
        /// </summary>
        protected void MethodInvoke(MethodInfo methodInfo, object[] methodParameters)
        {
            methodInfo.Invoke(_target, methodParameters);
            EditorUtility.SetDirty(_target);
            InternalEditorUtility.RepaintAllViews();
        }
    }
}

#endif