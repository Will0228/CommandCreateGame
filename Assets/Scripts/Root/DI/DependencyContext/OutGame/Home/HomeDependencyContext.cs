using Application.Home;
using Common.ZLogger;
using Presenter.OutGame;
using Shared.DependencyContext;

namespace Shared.DI
{
    public sealed class HomeDependencyContext : DependencyContextBase
    {
        protected override void OnRegister(IRegister register)
        {
            if (!gameObject.TryGetComponent<HomeView>(out var view))
            {
                ZLoggerUtility.LogError("HomeViewがアタッチされていません");
            }
            
            register.Register<HomeInitializeState>(Lifetime.Transient);
            register.Register<HomeUserPlayableState>(Lifetime.Transient);
            register.Register<IHomePresenter, HomePresenter>(Lifetime.Singleton);
            register.RegisterComponent(view);
        }
    }
}