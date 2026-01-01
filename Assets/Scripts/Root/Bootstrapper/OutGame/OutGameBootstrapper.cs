using Common.ZLogger;
using Root.DI;
using Root.Screen;
using Root.Switcher;
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
            ZLoggerUtility.LogWarning(resolver.GetParentsResolver());
            _resolver = resolver;
            _screenSwitcher = resolver.Resolve<IScreenSwitcher>();
        }
        
        public override void ManualInitialize()
        {
            _screenSwitcher.SetFirstScreenAsync(_resolver.Resolve<HomeScreen>()).Forget();
        }
    }
}