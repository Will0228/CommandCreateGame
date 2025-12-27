using Application.Base;
using Application.Home;
using Application.Switcher;
using VContainer;

namespace Application.OutGame
{
    /// <summary>
    /// アウトゲーム開始時のBootstrapper
    /// </summary>
    public sealed class OutGameBootstrapper : BootstrapperBase
    {
        private readonly IObjectResolver _resolver;
        private readonly IScreenSwitcher _screenSwitcher;
        
        public OutGameBootstrapper(IObjectResolver resolver) : base(resolver)
        {
            _resolver = resolver;
            _screenSwitcher = resolver.Resolve<IScreenSwitcher>();
        }
        
        public override void PostInitialize()
        {
            _screenSwitcher.SetFirstScreenAsync(_resolver.Resolve<HomeScreen>()).Forget();
        }
    }
}