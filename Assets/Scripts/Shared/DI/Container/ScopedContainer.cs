using System;
using System.Collections.Generic;
using System.Text;
using Root.DI;
using Root.EntryPointInterface;
using Shared.DependencyContext;
using Shared.DI;

namespace Shared.DI
{
    /// <remarks>
    /// ボックス化や頻繁に受け渡しを考えてclassが一番パフォーマンスが良さそう
    /// </remarks>
    public class Value
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

        public Value(Lifetime lifetime, Type concreteType, object instance)
        {
            Lifetime = lifetime;
            ConcreteType = concreteType;
            Instance = instance;
        }
            
        public bool IsEqualType(Type type) => ConcreteType == type;
    }
    
    /// <summary>
    /// DependencyContext別に持っておくインスタンス管理Container
    /// </summary>
    public sealed class ScopedContainer : IResolver, IRegister, IDisposable
    {
        
        
        
        private readonly RegistrationRegistry _registrationRegistry;
        private readonly ScopedContainer _parentContainer;
        private IResolver _parentResolver => _parentContainer;
        // このコンテナを作成したDependencyContext
        private readonly DependencyContextBase _dependencyContext;
        
        // Disposableを継承したクラスを管理
        // DependencyContextが破棄されたときにDisposeを呼び出すため
        private List<IDisposable> _disposableInstances;

        private readonly Dictionary<Type, Value> _registeredTypes = new();
        // 同じインタフェースで別クラスを一度に登録したい場合など
        private readonly Dictionary<Type, List<Value>> _multiRegisteredTypes = new();
        
        // EntryPointで登録されたオブジェクトが管理されている
        private readonly List<IInitializable> _initializables = new();
        private readonly List<IUpdatable> _updatables = new();
        
        public ScopedContainer(RegistrationRegistry registrationRegistry)
        {
            _registrationRegistry = registrationRegistry;
        }

        #region Resolve

        T IResolver.Resolve<T>()
            where T : class
        {
            if (typeof(T) == typeof(DependencyContextBase))
            {
                return _dependencyContext as T;
            }
            if (_multiRegisteredTypes.TryGetValue(typeof(T), out var values))
            {
                foreach (var value in values)
                {
                    if (value.IsEqualType(typeof(T)))
                    {
                        return ResolveInstance(value) as T;
                    }
                }
            }
            if (_registeredTypes.TryGetValue(typeof(T), out var instance))
            {
                return ResolveInstance(instance) as T;
            }
            if (_parentContainer != null)
            {
                return _parentResolver.Resolve<T>();
            }
            
            throw new Exception($"対象のクラスが登録されていません：{typeof(T)}");
        }

        object IResolver.Resolve(Type type)
        {
            if (type == typeof(DependencyContextBase))
            {
                return _dependencyContext;
            }
            if (_multiRegisteredTypes.TryGetValue(type, out var values))
            {
                foreach (var value in values)
                {
                    if (value.IsEqualType(type))
                    {
                        return ResolveInstance(value);
                    }
                }
            }
            if (_registeredTypes.TryGetValue(type, out var instance))
            {
                return ResolveInstance(instance);
            }
            if (_parentContainer != null)
            {
                return _parentResolver.Resolve(type);
            }
            
            throw new Exception($"対象のクラスが登録されていません：{type}");
        }

#if UNITY_EDITOR
        // 確認用として現在の親のResolverとして何が存在するのかを取得する
        string IResolver.GetParentsResolver()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(GetDependencyContextName());
            
            if (_parentContainer != null)
            {
                stringBuilder.Append(_parentResolver.GetParentsResolver());
            }
            
            return stringBuilder.ToString();
        }

        private string GetDependencyContextName()
        {
            return _parentContainer + "¥n";
        }
#endif
        
        private object ResolveInstance(Value value)
        {
            // シングルトンかつインスタンスがすでに存在する場合
            if (value.Lifetime == Lifetime.Singleton && value.Instance != null)
            {
                return value.Instance;
            }
                
            var instance = _registrationRegistry.ResolveInstance(value.ConcreteType, this);

            // もしシングルトンの場合はインスタンスを保存
            if (value.Lifetime == Lifetime.Singleton)
            {
                value.Instance = instance;
            }
            
            return instance;
        }
        
        #endregion

        #region Register

        void IRegister.WarmUp()
        {
            var entryPointInstances = _registrationRegistry.WarmUp(this);

            foreach (var instance in entryPointInstances)
            {
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

        void IRegister.Register<TInterface, TClass>(Lifetime lifetime)
        {
            if (_registeredTypes.TryGetValue(typeof(TInterface), out var value))
            {
                throw new InvalidOperationException($"すでに{typeof(TInterface)}が登録されています");
            }
            _registeredTypes.Add(typeof(TInterface), new Value(lifetime, typeof(TClass)));
            _registrationRegistry.Register<TClass>();
        }

        void IRegister.Register<TClass>(Lifetime lifetime)
        {
            if (_registeredTypes.TryAdd(typeof(TClass), new Value(lifetime, typeof(TClass))))
            {
                _registrationRegistry.Register<TClass>();
                return;
            }
            throw new InvalidOperationException($"すでに{typeof(TClass)}が登録されています");
        }

        void IRegister.RegisterEntryPoint<TClass>(Lifetime lifetime)
        {
            if (_registeredTypes.TryAdd(typeof(TClass),  new Value(lifetime, typeof(TClass))))
            {
                _registrationRegistry.RegisterEntryPoint<TClass>();
                return;
            }
            throw new InvalidOperationException($"すでに{typeof(TClass)}が登録されています");
        }

        void IRegister.RegisterComponent<TClass>(TClass instance)
        {
            var type = instance.GetType();
            _registeredTypes[type] = new Value(Lifetime.Singleton, type,  instance);
        }
        
        public RegistrationBuilder RegisterTest<TClass>(Lifetime lifetime) where TClass : class
        {
            var implementationType = typeof(TClass);
        
            // 1. 実体のコンパイル（作り方のレシピを作成）
            var factory = Compile(implementationType);

            // ※ ここで Singleton の場合はキャッシュロジックでラップする処理が入る（後述）

            // 2. 実体型そのものをキーとして登録
            if (!_compiledFactories.TryAdd(implementationType, factory))
            {
                throw new InvalidOperationException($"{implementationType.Name} は既に登録されています。");
            }

            // 3. ★重要★ ビルダーを作って返す
            // これにより、後ろに .As() を繋げられるようになる
            return new RegistrationBuilder(implementationType, factory);
        }

        #endregion
        
        

        public void Dispose()
        {
            foreach (var disposable in _disposableInstances)
            {
                disposable.Dispose();
            }
        }
    }
}
