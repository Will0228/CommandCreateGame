using Application.Base;
using Application.Home;
using Application.Manager;
using Domain.Addressable;
using Infra.Addressable;
using VContainer;
using VContainer.Unity;

namespace Application.OutGame
{
    /// <summary>
    /// アウトゲームで共通として使えるLifetimeScope
    /// </summary>
    public class OutGameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterEntryPoint<OutGameBootstrapper>();

            builder.Register<ScreenManager>(Lifetime.Singleton);
            builder.Register<StateManager.StateManager>(Lifetime.Singleton);
            builder.Register<IAddressableAssetLoader, AddressableAssetLoader>(Lifetime.Singleton);
            
            // Screen
            builder.Register<HomeScreen>(Lifetime.Transient);
        }
    }
}
