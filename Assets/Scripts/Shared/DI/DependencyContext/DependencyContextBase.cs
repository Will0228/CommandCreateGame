using System;
using System.Collections.Generic;
using Root.DI;
using Shared.DI;
using UnityEngine;

namespace Shared.DependencyContext
{
    internal class InstanceRegistration
    {
        public object Instance;
        public bool ShouldDispose;
    }
    
    public abstract class DependencyContextBase : MonoBehaviour
    {
        [Header("自動でDIを行うかどうか")]
        [SerializeField] private bool _autoRun;

        // DependencyContextEditorでインスペクター改造を行っています
        [HideInInspector] 
        [SerializeField] private string _typeName;
        
        private ScopedContainer _container;
        private IRegister _register => _container;
        private IResolver _resolver => _container;

        // このDependencyContextで登録したType
        // 実際にインスタンスを作成するときはレシピから行うが、レシピには別のDependencyContextで登録したクラスも含まれるので
        // このDependencyContextに含まれているかどうかで判断する必要がある（登録していないのに依存注入できた、というケースが存在するため）
        // FIXME
        // 親DependencyContextで既に登録されていないかを、登録時にバリデチェックする必要はある
        // private HashSet<Type> _registeredTypes = new HashSet<Type>();
        // private readonly Dictionary<Type, InstanceRegistration> _instances = new();

        private static ScopedContainer _cachedParentContainer;

        public IResolver Resolver
        {
            get
            {
                if (_container == null)
                {
                    Build();
                }
                return _container;
            }
        }
        
        
        // 過去のDependencyContextのキャッシュ
        // 親として指定したいインスタンスがあれば即座に発見できるように
        private static readonly Dictionary<Type, DependencyContextBase> _cachedInstances = new();
        
        // 親スコープを設定する
        public static IDisposable SetParent(DependencyContextBase parent)
        {
            _cachedParentContainer = parent._container;
            return new ScopeClearer();
        }
        
        protected virtual void Awake()
        {
            _cachedInstances[GetType()] = this;
            
            // 親をインスペクターで設定するようにしている場合はシーン上から見つけ出してそれを親とする
            if (!string.IsNullOrEmpty(_typeName))
            {
                var parentType = Type.GetType(_typeName);
                if (parentType != null && _cachedInstances.TryGetValue(parentType, out var parentInstance))
                {
                    _cachedParentContainer = parentInstance._container;
                }
            }
            
            if (_autoRun)
            {
                Build();
            }
        }
        
        public void ManualBuild() => Build();

        private void Build()
        {
            // コンテナがすでに存在する場合は処理をスキップする
            if (_container != null)
            {
                return;
            }

            _container = new ScopedContainer(DIInitializer.RegistrationRegistry, _cachedParentContainer);
            _cachedParentContainer = null;
            OnRegister();
            _register.WarmUp();
            
            foreach (var initializable in _container.InitializableClasses)
            {
                initializable.ManualInitialize();
            }
        }

        private void Update()
        {
            foreach (var updatable in _container.UpdatableClasses)
            {
                updatable.ManualUpdate();
            }
        }

        private void OnRegister()
        {
            PreRegisterInternal();
            OnRegister(_register);
        }

        private void PreRegisterInternal()
        {
            _register.RegisterComponent(this, typeof(DependencyContextBase));
        }

        protected virtual void OnRegister(IRegister register){}

        protected virtual void OnDestroy()
        {
            if (_cachedInstances.TryGetValue(GetType(), out var instance))
            {
                _cachedInstances.Remove(GetType());
            }
            
            ((IDisposable)_container).Dispose();
        }

        private class ScopeClearer : IDisposable
        {
            public void Dispose() {}
        }
    }
}
