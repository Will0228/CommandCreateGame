using System;
using System.Collections.Generic;
using Root.EntryPointInterface;

namespace Shared.DI
{
    /// <summary>
    /// インスタンスを管理する
    /// </summary>
    internal sealed class InstanceStorage : IDisposable
    {
        // Disposableを継承したクラスを管理
        // DependencyContextが破棄されたときにDisposeを呼び出すため
        private List<IDisposable> _disposableInstances = new();

        private readonly Dictionary<Type, Value> _registeredTypes = new();
        public IReadOnlyDictionary<Type, Value> RegisteredTypes => _registeredTypes;
        // 同じインタフェースで別クラスを一度に登録したい場合など
        private readonly Dictionary<Type, List<Value>> _multiRegisteredTypes = new();
        public IReadOnlyDictionary<Type, List<Value>> MultiRegisteredTypes => _multiRegisteredTypes;
        
        private readonly HashSet<object> _selfDestructibleClasses = new();
        public HashSet<object> SelfDestructibleClasses => _selfDestructibleClasses;
        
        // EntryPointで登録されたオブジェクトが管理されている
        internal readonly List<IInitializable> _initializable = new();
        public IReadOnlyList<IInitializable> InitializableClasses => _initializable;
        
        private readonly List<IUpdatable> _updatableClasses = new();
        public IReadOnlyList<IUpdatable> UpdatableClasses =>  _updatableClasses;
        
        public bool AddRegisteredType(Type type, Value value)
        {
            if (_registeredTypes.TryAdd(type, value))
            {
                return true;
            }

            return false;
        }

        public bool AddRegisterComponent<TClass>(TClass instance)
        {
            var type = instance.GetType();
            return AddRegisterComponent(instance, type);
        }

        public bool AddRegisterComponent<TClass>(TClass instance, Type type)
        {
            if (_registeredTypes.TryAdd(type, new Value(Lifetime.Singleton, type,  instance)))
            {
                return true;
            }
            return false;
        }

        public void AddInitializableClass(IInitializable value) => _initializable.Add(value);
        public void AddUpdatableClass(IUpdatable value) => _updatableClasses.Add(value);
        public bool AddSelfDestructibleClass(SelfDestructibleBaseClass value)
        {
            if (_selfDestructibleClasses.Add(value))
            {
                return true;
            }
            return false;
        }
        public void RemoveSelfDestructibleClass(Object obj) => _selfDestructibleClasses.Remove(obj);
        
        void IDisposable.Dispose()
        {
            foreach (var disposable in _disposableInstances)
            {
                disposable.Dispose();
            }
            _disposableInstances.Clear();
            _registeredTypes.Clear();
            _multiRegisteredTypes.Clear();
            _initializable.Clear();
            _updatableClasses.Clear();
        }
    }
}