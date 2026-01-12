using Application.Switcher;
using Common.ZLogger;
using Root.DI;
using Root.Screen;
using Shared.Bootstrapper;

namespace Root.Bootstrapper
{
    /// <summary>
    /// アウトゲーム開始時のBootstrapper
    /// </summary>
    public sealed class OutGameBootstrapper : BootstrapperBase
    {
        private readonly IResolver _resolver;
        private readonly IScreenSwitcher _screenSwitcher;
        
        public OutGameBootstrapper(IResolver resolver) : base(resolver)
        {
            _resolver = resolver;
            _screenSwitcher = resolver.Resolve<IScreenSwitcher>();
        }
        
        public override void ManualInitialize()
        {
            _screenSwitcher.SetFirstScreenAsync(_resolver.Resolve<HomeScreen>()).Forget();
        }
    }
}