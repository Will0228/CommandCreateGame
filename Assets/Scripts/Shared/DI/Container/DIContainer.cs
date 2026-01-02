using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Root.DI;
using Root.EntryPointInterface;
using Shared.Attributes;
using Shared.DependencyContext;

namespace Shared.DI
{
    public enum Lifetime
    {
        Singleton,
        Transient
    }
    
    /// <summary>
    /// DIを担うクラス
    /// </summary>
    public sealed class DIContainer : IResolver, IRegister
    {
        /// <remarks>
        /// ボックス化や頻繁に受け渡しを考えてclassが一番パフォーマンスが良さそう
        /// </remarks>
        private class Value
        {
            public Lifetime Lifetime { get; private set; }
            public Type ConcreteType { get; private set; }
            // Singletonの場合に必要
            public object Instance { get; set; } 

            public Value(Lifetime lifetime, Type concreteType)
            {
                Lifetime = lifetime;
                ConcreteType = concreteType;
            }
        }

        // 現在のDIContainerの親
        private DIContainer _parent;
        
        private readonly Dictionary<Type, Value> _registries = new();
        private readonly Dictionary<Type, Value> _entryPointMappings = new();
        private readonly Dictionary<Type, Func<IResolver, object>> _compiledFactories = new();
        private readonly Dictionary<Type, Action<object, IResolver>> _compiledInjectors = new();
        private readonly List<IInitializable> _initializables = new();
        public IReadOnlyList<IInitializable> Initializables => _initializables;
        private readonly List<IUpdatable> _updatables = new();
        public IReadOnlyList<IUpdatable> Updatables => _updatables;
        
        private DependencyContextBase _parentDependencyContext;

        /// <summary>
        /// 親Containerを所持することも可能
        /// </summary>
        public DIContainer(DIContainer parent = null)
        {
            _parent = parent;
        }

        
        public void Register<TInterface, TClass>(Lifetime lifetime) where TClass : TInterface
        {
            _registries[typeof(TInterface)] = new Value(lifetime, typeof(TClass));
        }

        public void Register<TClass>(Lifetime lifetime) where TClass : class
        {
            _registries[typeof(TClass)] = new Value(lifetime, typeof(TClass));
        }

        public void RegisterEntryPoint<TClass>(Lifetime lifetime) where TClass : class
        {
            _entryPointMappings[typeof(TClass)] = new Value(lifetime, typeof(TClass));
        }

        public void RegisterParentDependencyContext(DependencyContextBase instance)
        {
            _parentDependencyContext = instance;
        }

        /// <summary>
        /// ローディング中に一括コンパイル
        /// Expression Treeを使用して2度目以降の生成を高速化
        /// </summary>
        public void WarmUp()
        {
            foreach (var mapping in _registries)
            {
                var concreateType = mapping.Value.ConcreteType;
                CompileSettings(concreateType);
            }
            
            foreach (var mapping in _entryPointMappings)
            {
                var concreateType = mapping.Value.ConcreteType;
                CompileSettings(concreateType);
                var instance = ResolveInternal(concreateType);
                
                if (instance is IInitializable initializable)
                {
                    _initializables.Add(initializable);
                }
                if (instance is IUpdatable updatable)
                {
                    _updatables.Add(updatable);
                }
            }
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
        private Func<IResolver, object> Compile(Type type)
        {
            // IResolverを渡しているコンストラクタを探す
            // var constructorWithResolver = type.GetConstructor(new[] { typeof(IResolver) });
            // var parameterlessConstructor = type.GetConstructor(Type.EmptyTypes);
            var ctor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            // if (constructorWithResolver == null && parameterlessConstructor == null)
            // {
            //     throw new Exception($"{type.Name} を持つコンストラクタが見つかりませんでした");
            // }
            if (ctor == null)
            {
                throw new Exception($"{type.Name} を持つコンストラクタが見つかりませんでした");
            }
            
            // ラムダ式の引数定義
            var resolverParam = Expression.Parameter(typeof(IResolver), "resolver");
            var parameters = ctor.GetParameters();
            
            // コンストラクタ呼び出し式
            // new T(resolver)
            NewExpression newExpression;
            // if (constructorWithResolver != null)
            // {
            //     newExpression = Expression.New(constructorWithResolver, resolverParam);
            // }
            // else
            // {
            //     newExpression = Expression.New(parameterlessConstructor);
            // }
            if (parameters.Length == 0)
            {
                // 引数なしコンストラクタの場合
                newExpression = Expression.New(ctor);
            }
            else
            {
                // 2. コンストラクタの各引数を IResolver.Resolve<T>() で解決する式を生成
                var args = new Expression[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var pType = parameters[i].ParameterType;
            
                    if (pType == typeof(IResolver))
                    {
                        args[i] = resolverParam;
                    }
                    else
                    {
                        // resolver.Resolve<pType>() を呼び出す式を作成
                        var resolveMethod = typeof(IResolver).GetMethod("Resolve").MakeGenericMethod(pType);
                        args[i] = Expression.Call(resolverParam, resolveMethod);
                    }
                }
                newExpression = Expression.New(ctor, args);
            }
            
            // object型へのキャスト
            // newの戻り値がT型で辞書型で一括管理するため用にobject型へアップキャスト
            var castExpression = Expression.Convert(newExpression, typeof(object));
            
            // コンパイル（IResolverを受け取ってobjectを返す関数にする）
            var lambda = Expression.Lambda<Func<IResolver, object>>(castExpression, resolverParam);
            
            return lambda.Compile();
        }

       
        private Action<object, IResolver> CompileMethodInject(Type type)
        {
            var methods = type.GetMethods();
            var expressions = new List<Expression>();

            // 引数定義
            // IResolver以外の値を直で代入したい場合
            var targetParam = Expression.Parameter(typeof(object), "target");
            var castTarget = Expression.Convert(targetParam, type);
            // IResolverをそのまま渡す場合（推奨）
            var resolverParam = Expression.Parameter(typeof(IResolver), "resolver");
            
            foreach (var method in methods)
            {
                // InjectAttributeを持っているかのチェック
                var injectAttribute = Attribute.GetCustomAttribute(method, typeof(InjectAttribute));
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
                
                var argsExpressions = new Expression[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    var paramType = parameters[i].ParameterType;
                    if (_registries.TryGetValue(paramType, out var instance))
                    {
                        // 引数の中に、すでに登録済みのクラスが存在する場合はインスタンス化する
                        var resolverMethod = typeof(IResolver).GetMethod("Resolve").MakeGenericMethod(paramType);
                        argsExpressions[i] = Expression.Call(resolverParam, resolverMethod);
                    }
                    else if (paramType == typeof(IResolver))
                    {
                        argsExpressions[i] = resolverParam;
                    }
                    else
                    {
                        throw new Exception($"対象のクラスが登録されていません：{paramType}");
                    }
                }
                
                expressions.Add(Expression.Call(castTarget, method, argsExpressions));
            }
            
            var block = Expression.Block(expressions);
            var lambda = Expression.Lambda<Action<object, IResolver>>(block, targetParam, resolverParam);
            return lambda.Compile();
        }

        public T Resolve<T>()
            where T : class
        {
            if (typeof(T) == typeof(DependencyContextBase))
            {
                return ResolveParentDependencyContext() as T;
            }
            if (_registries.TryGetValue(typeof(T), out var value))
            {
                return ResolveInstance(value) as T;
            }
            if (_parent != null)
            {
                return _parent.Resolve<T>();
            }
            
            throw new Exception($"対象のクラスが登録されていません：{typeof(T)}");
        }

        public object Resolve(Type type)
        {
            if (type == typeof(DependencyContextBase))
            {
                return ResolveParentDependencyContext();
            }
            if (_registries.TryGetValue(type, out var value))
            {
                return ResolveInstance(value);
            }
            if (_parent != null)
            {
                return _parent.Resolve(type);
            }
            
            throw new Exception($"対象のクラスが登録されていません：{type}");
        }

#if UNITY_EDITOR
        // 確認用として現在の親のResolverとして何が存在するのかを取得する
        public string GetParentsResolver()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(GetDependencyContextName());
            
            if (_parent != null)
            {
                stringBuilder.Append(_parent.GetParentsResolver());
            }
            
            return stringBuilder.ToString();
        }

        public string GetDependencyContextName()
        {
            return _parent + "¥n";
        }
#endif

        /// <summary>
        /// EntryPointで登録したクラスを特殊ルートでインスタンス化したい場合
        /// </summary>
        private object ResolveInternal(Type type)
        {
            if (_entryPointMappings.TryGetValue(type, out var value))
            {
                return ResolveInstance(value);
            }
            throw new Exception($"{type.Name} is not registered.");
        }
        
        private object ResolveParentDependencyContext() => _parentDependencyContext;

        private object ResolveInstance(Value value)
        {
            // シングルトンかつインスタンスがすでに存在する場合
            if (value.Lifetime == Lifetime.Singleton && value.Instance != null)
            {
                return value.Instance;
            }
                
            var instance = _compiledFactories[value.ConcreteType](this);
            _compiledInjectors[value.ConcreteType](instance, this);

            // もしシングルトンの場合はインスタンスを保存
            if (value.Lifetime == Lifetime.Singleton)
            {
                value.Instance = instance;
            }
            
            return instance;
        }
    }
}