using Application.Switcher;
using Root.DI;
using Root.Screen;
using Shared.Bootstrapper;

namespace Root.Bootstrapper
{
    /// <summary>
    /// インゲーム開始時のBootstrapper
    /// </summary>
    public sealed class InGameBootstrapper : BootstrapperBase
    {
        private readonly IResolver _resolver;
        private readonly IScreenSwitcher _screenSwitcher;
        
        public InGameBootstrapper(IResolver resolver) : base(resolver)
        {
            _resolver = resolver;
            _screenSwitcher = resolver.Resolve<IScreenSwitcher>();
        }

        public override void ManualInitialize()
        {
            _screenSwitcher.SetFirstScreenAsync(_resolver.Resolve<InGameReadyScreen>()).Forget();
        }
    }
}