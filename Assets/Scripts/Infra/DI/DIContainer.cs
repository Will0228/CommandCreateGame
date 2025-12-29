using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Application.Attributes;
using Application.DI;

namespace Infra.DI
{
    /// <summary>
    /// DIを担うクラス
    /// </summary>
    public sealed class DIContainer : IResolver, IRegister
    {
        public enum Lifetime
        {
            Singleton,
            Transient
        }

        /// <remarks>
        /// ボックス化や頻繁に受け渡しを考えてclassが一番パフォーマンスが良さそう
        /// </remarks>
        private class Value
        {
            public Lifetime Lifetime { get; private set; }
            public object Instance { get; private set; }

            public Value(Lifetime lifetime, object instance)
            {
                Lifetime = lifetime;
                Instance = instance;
            }
        }
        
        private readonly Dictionary<Type, Type> _mappings = new();
        private readonly Dictionary<Type, Func<IResolver, object>> _compiledFactories = new();
        private readonly Dictionary<Type, Action<object, IResolver>> _compiledInjectors = new();

        // public void Register<TInterface, TClass>(Lifetime lifetime) 
        //     where TClass : TInterface
        public void Register<TInterface, TClass>() where TClass : TInterface
        {
            _mappings[typeof(TInterface)] = typeof(TClass);
        }

        public void Register<TClass>() where TClass : class
        {
            _mappings[typeof(TClass)] = typeof(TClass);
        }

        /// <summary>
        /// ローディング中に一括コンパイル
        /// Expression Treeを使用して2度目以降の生成を高速化
        /// </summary>
        public void WarmUp()
        {
            foreach (var mapping in _mappings)
            {
                var concreateType = mapping.Value;

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
        }

        /// <summary>
        /// コンパイルロジック(ExpressionTree)
        /// </summary>
        private Func<IResolver, object> Compile(Type type)
        {
            // IResolverを渡しているコンストラクタを探す
            var constructorWithResolver = type.GetConstructor(new[] { typeof(IResolver) });
            var parameterlessConstructor = type.GetConstructor(Type.EmptyTypes);

            if (constructorWithResolver == null && parameterlessConstructor == null)
            {
                throw new Exception($"{type.Name} を持つコンストラクタが見つかりませんでした");
            }
            
            // ラムダ式の引数定義
            var resolverParam = Expression.Parameter(typeof(IResolver), "resolver");
            
            // コンストラクタ呼び出し式
            // new T(resolver)
            NewExpression newExpression;
            if (constructorWithResolver != null)
            {
                newExpression = Expression.New(constructorWithResolver, resolverParam);
            }
            else
            {
                newExpression = Expression.New(parameterlessConstructor);
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
                    if (_mappings.TryGetValue(paramType, out var instance))
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
            if (_mappings.TryGetValue(typeof(T), out var concreateType))
            {
                var instance = _compiledFactories[concreateType](this);
                _compiledInjectors[concreateType](instance, this);
                return instance as T;
            }
            else
            {
                throw new Exception($"対象のクラスが登録されていません：{typeof(T)}");
            }
        }
    }
}