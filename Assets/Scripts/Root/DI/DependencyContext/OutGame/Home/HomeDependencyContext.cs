using Application.Home;
using Shared.DependencyContext;

namespace Shared.DI
{
    public sealed class HomeDependencyContext : DependencyContextBase
    {
        protected override void OnRegister(IRegister register)
        {
            register.Register<HomeInitializeState>(Lifetime.Transient);
            register.Register<HomeUserPlayableState>(Lifetime.Transient);
        }
    }
}