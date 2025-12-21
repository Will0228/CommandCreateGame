using System.Threading;
using Application.Base;
using Application.Home;
using Application.Manager;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Application.OutGame
{
    /// <summary>
    /// アウトゲーム開始時のBootstrapper
    /// </summary>
    public sealed class OutGameBootstrapper : BootstrapperBase
    {
        private readonly IObjectResolver _resolver;
        private readonly ScreenManager _screenManager;
        
        public OutGameBootstrapper(IObjectResolver resolver) : base(resolver)
        {
            _resolver = resolver;
            _screenManager = resolver.Resolve<ScreenManager>();
        }
        
        public override void PostInitialize()
        {
            _screenManager.SetFirstScreenAsync(_resolver.Resolve<HomeScreen>()).Forget();
        }
    }
}