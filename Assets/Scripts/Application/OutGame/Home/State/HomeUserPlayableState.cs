using System.Threading;
using Application.Base;
using Application.Switcher;
using Cysharp.Threading.Tasks;
using Root.DI;

using R3;
using UnityEngine;

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
        }

        protected override void Bind(){}

#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        protected override async UniTask ConfigureAsync(CancellationToken token){}
#pragma warning restore CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます

        public override void Dispose()
        {
        }
    }
}

