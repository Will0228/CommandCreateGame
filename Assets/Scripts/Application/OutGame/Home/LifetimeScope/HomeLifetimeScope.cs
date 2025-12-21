using Application.Base;
using VContainer;
using VContainer.Unity;

namespace Application.Home
{
    public class HomeLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<HomeInitializeState>(Lifetime.Singleton);
            builder.Register<HomeUserPlayableState>(Lifetime.Transient);
        }
    }
}