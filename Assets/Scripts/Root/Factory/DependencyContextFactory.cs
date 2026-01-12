using System;
using Root.DI;
using Shared.Attributes;
using Shared.DependencyContext;
using UnityEngine;

namespace Root.Factory
{
    /// <summary>
    /// DependencyContextを作成するFactory
    /// </summary>
    public sealed class DependencyContextFactory
    {
        // ゲームの基底のDependencyContext
        // ゲームが終了するまで破棄されません
        private readonly DependencyContextBase _rootDependencyContext;
        
        [Inject]
        public DependencyContextFactory(IResolver resolver)
        {
            _rootDependencyContext = resolver.Resolve<DependencyContextBase>();
        }

        /// <summary>
        /// シーンの規定となるDependencyContextを作成
        /// これによって作られるDependencyContextはシーンが破棄されるまで破棄されません
        /// </summary>
        /// <param name="dependencyContextType"></param>
        public DependencyContextBase CreateSceneDependencyContext(Type dependencyContextType)
        {
            var dependencyContextEntity = new GameObject(dependencyContextType.Name);
            DependencyContextBase sceneDependencyContext;
            using (DependencyContextBase.SetParent(_rootDependencyContext))
            {
                sceneDependencyContext = (DependencyContextBase)dependencyContextEntity.AddComponent(dependencyContextType);
            }

            return sceneDependencyContext;
        }
    }
}