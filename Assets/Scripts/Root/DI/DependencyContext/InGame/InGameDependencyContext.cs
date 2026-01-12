using Root.Bootstrapper;
using Shared.DependencyContext;
using Shared.DI;

namespace Root.DI
{
    public sealed class InGameDependencyContext : DependencyContextBase
    {
        protected override void OnRegister(IRegister register)
        {
            register.Register<InGameBootstrapper>(Lifetime.Transient);
        }
    }
}