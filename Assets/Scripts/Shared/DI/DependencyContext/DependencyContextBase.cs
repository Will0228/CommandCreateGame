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
        
        // private DIContainer _container;
        private ScopedContainer _container;
        private IRegister _register => _container;
        private IResolver _resolver => _container;
        
        // SetParentで親スコープを定義するためにstaticにしている
        private static DependencyContextBase _parentDependencyContext;

        // このDependencyContextで登録したType
        // 実際にインスタンスを作成するときはレシピから行うが、レシピには別のDependencyContextで登録したクラスも含まれるので
        // このDependencyContextに含まれているかどうかで判断する必要がある（登録していないのに依存注入できた、というケースが存在するため）
        // FIXME
        // 親DependencyContextで既に登録されていないかを、登録時にバリデチェックする必要はある
        private HashSet<Type> _registeredTypes = new HashSet<Type>();
        private readonly Dictionary<Type, InstanceRegistration> _instances = new();

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
            _parentDependencyContext = parent;
            return new ScopeClearer();
        }
        
        protected virtual void Awake()
        {
            _cachedInstances[this.GetType()] = this;
            
            // 親をインスペクターで設定するようにしている場合はシーン上から見つけ出してそれを親とする
            if (!string.IsNullOrEmpty(_typeName))
            {
                var parentType = Type.GetType(_typeName);
                if (parentType != null && _cachedInstances.TryGetValue(parentType, out var parentInstance))
                {
                    _parentDependencyContext = parentInstance;
                }
            }
            
            if (_autoRun)
            {
                Build();
            }
        }
        
        public void ManualBuild() => Build();

        public void Build()
        {
            // コンテナがすでに存在する場合は処理をスキップする
            if (_container != null)
            {
                return;
            }

            _container = new ScopedContainer(DIInitializer.RegistrationRegistry);
            OnRegister(_container);
            _register.WarmUp();
            
            foreach (var initializable in _container.Initializables)
            {
                initializable.ManualInitialize();
            }
        }

        private void Update()
        {
            foreach (var updatable in _container.Updatables)
            {
                updatable.ManualUpdate();
            }
        }

        protected abstract void OnRegister(IRegister register);

        protected virtual void OnDestroy()
        {
            if (_cachedInstances.TryGetValue(this.GetType(), out var instance))
            {
                _cachedInstances.Remove(this.GetType());
            }
        }

        private class ScopeClearer : IDisposable
        {
            public void Dispose(){}
        }
    }
}
