using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Root.DI;
using Root.EntryPointInterface;
using Shared.Attributes;

namespace Shared.DI
{
    public enum Lifetime
    {
        Singleton,
        Transient,
        SelfDestruct
    }
    
    /// <summary>
    /// インスタンスを生成するためのレシピを管理するクラス
    /// </summary>
    public sealed class RegistrationRegistry
    {
        private readonly HashSet<Type> _registryTypes = new();
        private readonly HashSet<Type> _entryPointRegistryTypes = new();
        private readonly Dictionary<Type, Func<IResolver, object>> _compiledFactories = new();
        private readonly Dictionary<Type, Action<object, IResolver>> _compiledInjectors = new();
        
        public void Register<TClass>(Lifetime lifetime) where TClass : class
        {
            if (!_compiledFactories.ContainsKey(typeof(TClass)))
            {
                _registryTypes.Add(typeof(TClass));
            }
        }
        
        public void Register(Type classType, Lifetime lifetime)
        {
            if (!_compiledFactories.ContainsKey(classType))
            {
                _registryTypes.Add(classType);
            }
        }

        public void RegisterEntryPoint<TClass>(Lifetime lifetime) where TClass : class
        {
            _entryPointRegistryTypes.Add(typeof(TClass));
        }
        
        /// <summary>
        /// ローディング中に一括コンパイル
        /// Expression Treeを使用して2度目以降の生成を高速化
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns>EntryPointで登録されたインスタンス</returns>
        public IReadOnlyList<Object> WarmUp(IResolver resolver)
        {
            foreach (var type in _registryTypes)
            {
                CompileSettings(type);
            }
            
            var entryPointInstances = new List<Object>();
            foreach (var type in _entryPointRegistryTypes)
            {
                CompileSettings(type);
                entryPointInstances.Add(ResolveInternal(type,  resolver));
            }
            
            _registryTypes.Clear();
            _entryPointRegistryTypes.Clear();
            
            return entryPointInstances;
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
            var ctor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();
            
            if (ctor == null)
            {
                throw new Exception($"{type.Name} を持つコンストラクタが見つかりませんでした");
            }
            
            // ラムダ式の引数定義
            var resolverParam = Expression.Parameter(typeof(IResolver), "resolver");
            var parameters = ctor.GetParameters();
            
            // コンストラクタ呼び出し式
            NewExpression newExpression;
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
                    if (_registryTypes.TryGetValue(paramType, out var instance))
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
        
        /// <summary>
        /// EntryPointで登録したクラスを特殊ルートでインスタンス化したい場合
        /// </summary>
        private object ResolveInternal(Type type, IResolver resolver)
        {
            if (_entryPointRegistryTypes.TryGetValue(type, out var value))
            {
                return ResolveInstance(value, resolver);
            }
            throw new Exception($"{type.Name} is not registered.");
        }
        
        public object ResolveInstance(Type type, IResolver resolver)
        {
            var instance = _compiledFactories[type](resolver);
            _compiledInjectors[type](instance, resolver);

            return instance;
        }
    }
}