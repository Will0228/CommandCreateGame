using Application.InGame;
using Common.ZLogger;
using Presenter.OutGame;
using Shared.DependencyContext;
using Shared.DI;

namespace Root.DI
{
    public sealed class InGameReadyScreenDependencyContext : DependencyContextBase
    {
        protected override void OnRegister(IRegister register)
        {
            register.Register<InGameReadyScreenInitializeState>(Lifetime.Transient);
            register.Register<InGameReadyScreenUserPlayableState>(Lifetime.Transient);
        }
    }
}