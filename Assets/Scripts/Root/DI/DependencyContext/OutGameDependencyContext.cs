using Root.Bootstrapper;
using Shared.DependencyContext;
using Shared.DI;

namespace Root.DI
{
    public sealed class OutGameDependencyContext : DependencyContextBase
    {
        protected override void OnRegister(IRegister register)
        {
            register.RegisterEntryPoint<OutGameBootstrapper>(Lifetime.Transient);
        }
    }
}