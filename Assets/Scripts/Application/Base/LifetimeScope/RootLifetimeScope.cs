using Application.Home;
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
            builder.Register<HomeScreen>(Lifetime.Transient);
            
            // Loader
            builder.Register<IAddressableAssetLoader, AddressableAssetLoader>(Lifetime.Singleton);
            
            // Switcher
            builder.Register<ISceneSwitcher, SceneSwitcher>(Lifetime.Singleton);
            builder.Register<IScreenSwitcher, ScreenSwitcher>(Lifetime.Singleton);
            builder.Register<IStateSwitcher, StateSwitcher>(Lifetime.Singleton);
        }
    }
}