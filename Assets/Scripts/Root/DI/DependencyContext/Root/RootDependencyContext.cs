using Domain.Addressable;
using Infra.Addressable;
using Root.Bootstrapper;
using Root.DI;
using Root.Screen;
using Root.Switcher;
using Shared.DependencyContext;
using Shared.DI;

namespace Root.DependencyContext
{
    /// <summary>
    /// プロジェクト全体で使用可能なDependencyContext
    /// </summary>
    public sealed class RootDependencyContext : DependencyContextBase
    {
        protected override void Awake()
        {
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            
            base.Awake();
        }

        protected override void OnRegister(IRegister register)
        {
            // TODO
            // 試しに、やり方が間違っている可能性もあるので要検討
            register.RegisterParentDependencyContext(this);

            register.Register<HomeScreen>();
            
            // Loader
            register.Register<IAddressableAssetLoader, AddressableAssetLoader>();
            
            // Switcher
            register.Register<ISceneSwitcher, SceneSwitcher>();
            register.Register<IScreenSwitcher, ScreenSwitcher>();
            register.Register<IStateSwitcher, StateSwitcher>();
            
            // Bootstrapper
            register.RegisterEntryPoint<InitializeBootstrapper>();
        }
    }
}