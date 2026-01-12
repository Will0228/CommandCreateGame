using System.Threading;
using Application.Base;
using Cysharp.Threading.Tasks;
using Domain.Addressable;
using R3;
using Root.DI;
using Shared.Attributes;
using Shared.DependencyContext;
using Shared.Screen;

namespace Application.Switcher
{
    /// <summary>
    /// スクリーンを管理するマネージャークラス
    /// </summary>
    public sealed class ScreenSwitcher : IScreenSwitcher
    {
        private readonly IAddressableAssetLoader _assetLoader;
        private readonly IStateSwitcher _stateSwitcher;
        
        private IScreen _currentScreen;
        private CancellationTokenSource _cts = new();
        
        // シーンの基底DependencyContext
        // SceneSwitcherからSceneの切り替え後に更新される
        private DependencyContextBase _sceneRootDependencyContext;

        [Inject]
        public ScreenSwitcher(IResolver resolver)
        {
            _assetLoader = resolver.Resolve<IAddressableAssetLoader>();
            _stateSwitcher = resolver.Resolve<IStateSwitcher>();
        }
        
        public async UniTaskVoid SetNextScreenAsync(IScreen nextScreen)
        {
            ResetCancellationTokenSource();
            
            _currentScreen.Dispose();
            
            NextScreenSettingsAsync(nextScreen).Forget();
        }

        public async UniTaskVoid SetFirstScreenAsync(IScreen nextScreen) => NextScreenSettingsAsync(nextScreen).Forget(); 
        

        private async UniTask NextScreenSettingsAsync(IScreen nextScreen)
        {
            _currentScreen = nextScreen;
            // 次のスクリーンを作成
            var screenEntity = await _assetLoader.InstantiateAssetAsync(_currentScreen.AddressKey);
            DependencyContextBase _dependencyContext;
            using (DependencyContextBase.SetParent(_sceneRootDependencyContext))
            {
                _dependencyContext = (DependencyContextBase)screenEntity.AddComponent(nextScreen.DependencyContextType);
            }
            _currentScreen.SetDependencyContext(_dependencyContext.Resolver);
            
            _currentScreen.NextScreenTransition
                .Subscribe(screen => SetNextScreenAsync(screen).Forget())
                .AddTo(_cts.Token);
            
            _stateSwitcher.SetFirstState(_currentScreen.Resolver.Resolve(_currentScreen.FirstTransitionStateType) as StateBase);
        }
        
        private void ResetCancellationTokenSource()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
        }
        
        /// <summary>
        /// シーンの基底DependencyContextをセットする
        /// </summary>
        void IScreenSwitcher.SetSceneRootDependencyContext(DependencyContextBase sceneRootDependencyContext)
            => _sceneRootDependencyContext = sceneRootDependencyContext;
    }
}

