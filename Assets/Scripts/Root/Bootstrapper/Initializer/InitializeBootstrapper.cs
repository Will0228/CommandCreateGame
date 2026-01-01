using Root.DI;
using Root.Switcher;
using Shared.Bootstrapper;

namespace Root.Bootstrapper
{
    /// <summary>
    /// ゲーム開始時のBootstrapper
    /// </summary>
    public sealed class InitializeBootstrapper : BootstrapperBase
    {
        private readonly ISceneSwitcher _sceneSwitcher;
        
        public InitializeBootstrapper(IResolver resolver) : base(resolver)
        {
            _sceneSwitcher = resolver.Resolve<ISceneSwitcher>();
        }

        public override void ManualInitialize()
        {
            _sceneSwitcher.ChangeScene(SceneType.OutGame);
        }
    }
}