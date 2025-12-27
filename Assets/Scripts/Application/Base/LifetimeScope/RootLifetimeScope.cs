using Application.Home;
using Application.Manager;
using Application.Switcher;
using Domain.Addressable;
using Infra.Addressable;
using VContainer;
using VContainer.Unity;

namespace Application.Base
{
    /// <summary>
    /// LifetimeScopeの基底クラス
    /// </summary>
    public sealed class RootLifetimeScope : LifetimeScope
    {
        protected override void Awake()
        {
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            
            base.Awake();
        }
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ScreenManager>(Lifetime.Singleton);
            builder.Register<StateManager.StateManager>(Lifetime.Singleton);
            builder.Register<HomeScreen>(Lifetime.Transient);
            
            // Loader
            builder.Register<IAddressableAssetLoader, AddressableAssetLoader>(Lifetime.Singleton);
            
            // Switcher
            builder.Register<ISceneSwitcher, SceneSwitcher>(Lifetime.Singleton);
        }
    }
}