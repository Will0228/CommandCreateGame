using VContainer;
using VContainer.Unity;

namespace Application.Initializer
{
    public sealed class InitializerLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterEntryPoint<InitializerBootstrapper>();
        }
    }
}