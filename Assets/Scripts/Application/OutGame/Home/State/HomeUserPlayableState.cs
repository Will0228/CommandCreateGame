using System.Threading;
using Application.Base;
using Application.Switcher;
using Cysharp.Threading.Tasks;
using Root.DI;

using R3;

namespace Application.Home
{
    public sealed class HomeUserPlayableState : UserPlayableStateBase
    {
        private readonly IHomePresenter _presenter;
        private readonly ISceneSwitcher _sceneSwitcher;
        
        public HomeUserPlayableState(IResolver resolver) : base(resolver)
        {
            _presenter = resolver.Resolve<IHomePresenter>();
            _sceneSwitcher = resolver.Resolve<ISceneSwitcher>();
            
            SetEvent();
        }

        protected override void SetEvent()
        {
            Subscribe(_presenter.ButtonClickedAsObservable, _ =>_sceneSwitcher.ChangeSceneAsync(SceneType.InGame));
            _presenter.ButtonClickedAsObservable
                .Subscribe(_ => _sceneSwitcher.ChangeSceneAsync(SceneType.InGame))
                .AddTo(CompositeDisposable);
        }

        protected override void Bind(){}

        protected override async UniTask ConfigureAsync(CancellationToken token)
        {
            
        }
    }
}

