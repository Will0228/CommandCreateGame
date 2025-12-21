using Domain.Addressable;
using Infra.Addressable;
using VContainer;
using VContainer.Unity;

namespace Application.Base
{
    /// <summary>
    /// LifetimeScopeの基底クラス
    /// </summary>
    public abstract class CommandCreateGameLifetimeScopeBase : LifetimeScope
    {
        protected virtual void Configure(IContainerBuilder builder)
        {
            builder.Register<IAddressableAssetLoader, AddressableAssetLoader>(Lifetime.Singleton);
        }
    }
}