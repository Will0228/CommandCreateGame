using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared.Attributes;

namespace Editor.ClassGenerator
{
    /// <remarks>
    /// VContainerのSingletonで作成するクラスしか対応できません。
    /// Transientのようなクラスがほしい場合は実装をしてください
    /// </remarks>
    internal sealed class ClassGeneratorSimpleDIContainer : IDisposable
    {
        // private readonly Dictionary<Type, object> _registries = new();
        // private readonly List<IDisposable> _disposables = new();
        private readonly HashSet<Type> _registryTypes = new();
        private readonly Dictionary<Type, Func<object>> _compiledFactories = new();
        private readonly Dictionary<Type, Action<object>> _compiledInjectors = new();
        private readonly Dictionary<Type, object> _cachedInstances = new();

        internal ClassGeneratorSimpleDIContainer()
        {
            Register<ClassGeneratorFolderSettingViewContainer>();
            Register<ClassGeneratorWordingSettingViewContainer>();
            Register<ClassGeneratorFolderSettingPresenter>();
            Register<ClassGeneratorPresenter>();
            Register<ClassGeneratorCommonPresenter>();
            Register<ClassGeneratorWordingSettingPresenter>();
            
            Register<ClassGeneratorModel>();
            Register<ClassGeneratorView>();
            Register<ClassGeneratorCommonView>();
            Register<ClassGeneratorCommonModel>();
            Register<ClassGeneratorWordingSettingTextAreaModel>();
            Register<ClassGeneratorWordingSettingModel>();
            Register<ClassGeneratorWordingSettingImplementationDetailsView>();
            Register<ClassGeneratorWordingSettingClassRequirementView>();
            Register<ClassGeneratorWordingSettingTextAreaView>();
            Register<ClassGeneratorWordingSettingApplyTemplateView>();
            
            Register<ClassGeneratorFolderSettingFolderPathView>();
            Register<ClassGeneratorFolderSettingLayerView>();
            Register<ClassGeneratorFolderSettingLayerModel>();
            Register<ClassGeneratorFolderSettingPathModel>();
            
            Register<ClassIdFactory>();
            
            
            WarmUp();
        }
        
        public void Register<TClass>() where TClass : class
        {
            if (!_compiledFactories.ContainsKey(typeof(TClass)))
            {
                _registryTypes.Add(typeof(TClass));
            }
        }
        
        public void Register(Type classType)
        {
            if (!_compiledFactories.ContainsKey(classType))
            {
                _registryTypes.Add(classType);
            }
        }
        
        public void WarmUp()
        {
            foreach (var type in _registryTypes)
            {
                CompileSettings(type);
            }
            
            _registryTypes.Clear();
        }
        
        private void CompileSettings(Type concreateType)
        {
            // 生成ロジックのコンパイル
            if (!_compiledFactories.ContainsKey(concreateType))
            {
                _compiledFactories[concreateType] = Compile(concreateType);
            }

            // 注入ロジックのコンパイル
            if (!_compiledInjectors.ContainsKey(concreateType))
            {
                _compiledInjectors[concreateType] = CompileMethodInject(concreateType);
            }
        }

        /// <summary>
        /// コンパイルロジック(ExpressionTree)
        /// </summary>
        private Func<object> Compile(Type type)
        {
            var constructors = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).ToArray();
            if (constructors.Length == 0)
            {
                throw new Exception($"No public constructors for {type}");
            }
            var selectedConstructor = constructors[0];
            var parameters = selectedConstructor.GetParameters();
            if (parameters.Length == 0)
            {
                return () => Activator.CreateInstance(type);
            }
            else
            {
                return () =>
                {
                    var args = new object[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var pType = parameters[i].ParameterType;
                        if (pType == typeof(ClassGeneratorSimpleDIContainer))
                        {
                            args[i] = this;
                        }
                        else if (_compiledFactories.ContainsKey(pType))
                        {
                            args[i] = ResolveInstance(pType);
                        }
                        else
                        {
                            throw new Exception($"Type not registered: {pType}");
                        }
                    }
                    return selectedConstructor.Invoke(args);
                };
            }
        }
        
        private Action<object> CompileMethodInject(Type type)
        {
            var methods = type.GetMethods();

            var injectInfos = new List<(MethodInfo method, Type[] paramTypes)>();

            foreach (var method in methods)
            {
                // InjectAttributeを持っているかのチェック
                var injectAttribute = Attribute.GetCustomAttribute(method, typeof(EditorInjectAttribute));
                if (injectAttribute == null)
                {
                    continue;
                }

                // そのメソッドの引数があるか確認
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    continue;
                }

                injectInfos.Add((method, parameters.Select(param => param.ParameterType).ToArray()));
            }

            return (target) =>
            {
                foreach (var info in injectInfos)
                {
                    var args = new object[info.paramTypes.Length];
                    for (var i = 0; i < info.paramTypes.Length; i++)
                    {
                        var pType = info.paramTypes[i];

                        // DIコンテナから依存関係を解決
                        if (pType == typeof(ClassGeneratorSimpleDIContainer))
                        {
                            args[i] = this;
                        }
                        else if (_registryTypes.Contains(pType))
                        {
                            // コンテナのResolveメソッドを呼び出す（実行時）
                            args[i] = ResolveInstance(pType);
                        }
                        else
                        {
                            throw new Exception($"Type not registered: {pType}");
                        }
                    }

                    // メソッドを実行
                    info.method.Invoke(target, args);
                }
            };
        }

        public T Resolve<T>() where T : class
        {
            if(_cachedInstances.TryGetValue(typeof(T), out var instance))
            {
                return instance as T;
            }
            return ResolveInstance(typeof(T)) as T;
        }

        private T Resolve<T>(Type type) where T : class
        {
            if(_cachedInstances.TryGetValue(type, out var instance))
            {
                return instance as T;
            }
            return ResolveInstance(type) as T;
        }
        
        private object ResolveInstance(Type type)
        {
            var instance = _compiledFactories[type]();
            _compiledInjectors[type](instance);
            _cachedInstances[type] = instance;
            
            _cachedInstances[type] = instance;

            return instance;
        }

        void IDisposable.Dispose()
        {
            // foreach (var disposable in _disposables)
            // {
            //     disposable.Dispose();
            // }
            //
            // _disposables.Clear();
            // _registries.Clear();
        }
    }
}