using System;
using System.Collections.Generic;
using Root.DI;
using Shared.DI;
using UnityEngine;

namespace Shared.DependencyContext
{
    public abstract class DependencyContextBase : MonoBehaviour
    {
        [Header("自動でDIを行うかどうか")]
        [SerializeField] private bool _autoRun;

        [SerializeField] private string TypeName;
        [SerializeField] private ParentDependencyContextReference _parentDependencyContextReference;
        
        private DIContainer _container;
        public DIContainer Container => _container;

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
        
        private static DependencyContextBase _parentDependencyContext;
        
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
            if (!string.IsNullOrEmpty(TypeName))
            {
                var parentType = Type.GetType(TypeName);
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

        public void Build()
        {
            // コンテナがすでに存在する場合は処理をスキップする
            if (_container != null)
            {
                return;
            }
            
            _container = new DIContainer(_parentDependencyContext == null ? null : _parentDependencyContext.Container);
            OnRegister(_container);
            _container.WarmUp();
            
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
            public void Dispose() => _parentDependencyContext = null;
        }
    }
}
