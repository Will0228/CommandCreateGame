using System.Threading;
using Application.Base;
using Cysharp.Threading.Tasks;
using Domain.Addressable;
using R3;
using VContainer;
using VContainer.Unity;

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
        
        private readonly LifetimeScope _rootLifetimeScope;

        [Inject]
        public ScreenSwitcher(IObjectResolver resolver)
        {
            _assetLoader = resolver.Resolve<IAddressableAssetLoader>();
            _stateSwitcher = resolver.Resolve<IStateSwitcher>();
            _rootLifetimeScope = resolver.Resolve<LifetimeScope>();
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
            // 次のシーンを作成
            var screenEntity = await _assetLoader.InstantiateAssetAsync(_currentScreen.AddressKey);
            LifetimeScope scope;
            using (LifetimeScope.EnqueueParent(_rootLifetimeScope))
            {
                scope = (LifetimeScope)screenEntity.AddComponent(nextScreen.LifetimeScopeType);
            }
            _currentScreen.SetLifetimeContainer(scope.Container);
            
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
    }
}

