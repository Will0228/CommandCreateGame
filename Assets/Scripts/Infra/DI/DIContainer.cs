using System;
using System.Collections.Generic;
using Application.Attributes;

namespace Infra.DI
{
    /// <summary>
    /// DIを担うクラス
    /// </summary>
    public sealed class DIContainer
    {
        public enum Lifetime
        {
            Singleton,
            Transient
        }

        // ボックス化や頻繁に受け渡しを考えてclassが一番パフォーマンスが良さそう
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
        
        private readonly Dictionary<Type, Type> _container = new();

        // public void Register<TInterface, TClass>(Lifetime lifetime) 
        //     where TClass : TInterface
        public void Register<TInterface, TClass>() 
            where TClass : TInterface
        {
            _container[typeof(TInterface)] = typeof(TClass);
        }

        public void Inject(object target)
        {
            var methods = target.GetType().GetMethods();
            foreach (var method in methods)
            {
                // InjectAttributeを持っているかのチェック
                var type = typeof(InjectAttribute);
                var injectAttribute = Attribute.GetCustomAttribute(method, type);
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
                
                var args = new object[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    var paramType = parameters[i].ParameterType;
                    if (_container.TryGetValue(paramType, out var instance))
                    {
                        // 引数の中に、すでに登録済みのクラスが存在する場合はインスタンス化する
                        args[i] = Activator.CreateInstance(instance);
                    }
                    else
                    {
                        throw new Exception($"対象のクラスが登録されていません：{paramType}");
                    }
                }
                
                // 注入
                method.Invoke(target, args);
            }
        }
    }
}