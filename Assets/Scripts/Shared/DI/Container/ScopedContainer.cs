using System;
using System.Collections.Generic;
using System.Text;
using R3;
using Root.DI;
using Root.EntryPointInterface;
using UnityEngine;
using Object = System.Object;

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
        private readonly InstanceStorage _instanceStorage;
        
        private readonly RegistrationRegistry _registrationRegistry;
        private readonly ScopedContainer _parentContainer;
        private IResolver _parentResolver => _parentContainer;
        
        private CompositeDisposable _compositeDisposable = new();

        public IReadOnlyList<IInitializable> InitializableClasses => _instanceStorage.InitializableClasses;
        public IReadOnlyList<IUpdatable> UpdatableClasses => _instanceStorage.UpdatableClasses;
        
        public ScopedContainer(RegistrationRegistry registrationRegistry, ScopedContainer parentContainer = null)
        {
            _registrationRegistry = registrationRegistry;
            _parentContainer = parentContainer;
            _instanceStorage = new InstanceStorage();
        }

        #region Resolve

        T IResolver.Resolve<T>()
            where T : class
        {
            if (_instanceStorage.MultiRegisteredTypes.TryGetValue(typeof(T), out var values))
            {
                foreach (var value in values)
                {
                    if (value.IsEqualType(typeof(T)))
                    {
                        return ResolveInstance(value) as T;
                    }
                }
            }
            if (_instanceStorage.RegisteredTypes.TryGetValue(typeof(T), out var instance))
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
            if (_instanceStorage.MultiRegisteredTypes.TryGetValue(type, out var values))
            {
                foreach (var value in values)
                {
                    if (value.IsEqualType(type))
                    {
                        return ResolveInstance(value);
                    }
                }
            }
            if (_instanceStorage.RegisteredTypes.TryGetValue(type, out var instance))
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
            CheckSelfDestructibleClass(instance);
            
            // もしシングルトンの場合はインスタンスを保存
            if (value.Lifetime == Lifetime.Singleton)
            {
                value.Instance = instance;
            }
            
            return instance;
        }

        private void CheckSelfDestructibleClass(Object instance)
        {
            if (instance is SelfDestructibleBaseClass selfDestructibleClass)
            {
                if (_instanceStorage.AddSelfDestructibleClass(selfDestructibleClass))
                {
                    selfDestructibleClass.DestroyAsObservable
                        .Take(1)
                        .Subscribe(obj => _instanceStorage.RemoveSelfDestructibleClass(obj))
                        .AddTo(_compositeDisposable);
                }
                else
                {
                    throw new InvalidOperationException($"すでに{instance.GetType()}が登録されています");
                }
            }
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
                    _instanceStorage.AddInitializableClass(initializable);
                }
                if (instance is IUpdatable updatable)
                {
                    _instanceStorage.AddUpdatableClass(updatable);
                }
            }
        }

        void IRegister.Register<TInterface, TClass>(Lifetime lifetime)
        {
            if (_instanceStorage.AddRegisteredType(typeof(TInterface), new Value(lifetime, typeof(TClass))))
            {
                _registrationRegistry.Register<TClass>(lifetime);
                return;
            }
            throw new InvalidOperationException($"すでに{typeof(TInterface)}が登録されています");
        }

        void IRegister.Register<TClass>(Lifetime lifetime)
        {
            if (_instanceStorage.AddRegisteredType(typeof(TClass), new Value(lifetime, typeof(TClass))))
            {
                _registrationRegistry.Register<TClass>(lifetime);
                return;
            }
            throw new InvalidOperationException($"すでに{typeof(TClass)}が登録されています");
        }

        void IRegister.RegisterEntryPoint<TClass>(Lifetime lifetime)
        {
            if (_instanceStorage.AddRegisteredType(typeof(TClass),  new Value(lifetime, typeof(TClass))))
            {
                _registrationRegistry.RegisterEntryPoint<TClass>(lifetime);
                return;
            }
            throw new InvalidOperationException($"すでに{typeof(TClass)}が登録されています");
        }

        void IRegister.RegisterComponent<TClass>(TClass instance)
        {
            if (_instanceStorage.AddRegisterComponent(instance))
            {
                return;
            }
            throw new InvalidOperationException($"すでに{typeof(TClass)}が登録されています");
        }

        public void RegisterComponent<TClass>(TClass instance, Type type) where TClass : MonoBehaviour
        {
            if (_instanceStorage.AddRegisterComponent(instance, type))
            {
                return;
            }
            throw new InvalidOperationException($"すでに{typeof(TClass)}が登録されています");
        }

        #endregion

        void IDisposable.Dispose()
        {
            ((IDisposable)_instanceStorage).Dispose();
        }
    }
}
