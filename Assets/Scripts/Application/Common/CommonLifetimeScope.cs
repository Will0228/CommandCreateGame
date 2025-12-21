using Domain.Addressable;
using Infra.Addressable;
using VContainer;
using VContainer.Unity;

namespace Application.Common
{
    /// <summary>
    /// どこからでも呼べるようにするLifetimeScope
    /// </summary>
    public class CommonLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IAddressableAssetLoader, AddressableAssetLoader>(Lifetime.Singleton);
        }
    }
}