using Application.Base;
using Application.Switcher;
using VContainer;

namespace Application.Initializer
{
    /// <summary>
    /// ゲーム開始時のBootstrapper
    /// </summary>
    public sealed class InitializerBootstrapper : BootstrapperBase
    {
        private readonly ISceneSwitcher _sceneSwitcher;
        
        public InitializerBootstrapper(IObjectResolver resolver) : base(resolver)
        {
            _sceneSwitcher = resolver.Resolve<ISceneSwitcher>();
        }

        public override void PostInitialize()
        {
            _sceneSwitcher.ChangeScene(SceneType.OutGame);
        }
    }
}