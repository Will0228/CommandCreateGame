using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;

namespace Application.Base
{
    /// <summary>
    /// 全てのスクリーンの基底クラス
    /// </summary>
    public abstract class StateBase : IState
    {
        protected readonly IObjectResolver Resolver; 
        
        protected CompositeDisposable CompositeDisposables;
        protected CancellationTokenSource CancellationTokenSource = new();

        // 別のステートに遷移して画面が閉じられるときに発火される
        private readonly Subject<StateBase> _onTransitionToNextStateSubject = new();
        public Observable<StateBase> TransitionToNextStateAsObservable => _onTransitionToNextStateSubject.AsObservable();

        [Inject]
        public StateBase(IObjectResolver resolver)
        {
            Resolver = resolver;
        }

        /// <summary>
        /// 次のステートに遷移する
        /// </summary>
        protected void SetNextState(StateBase nextState)
        {
            _onTransitionToNextStateSubject.OnNext(nextState);
        }

        /// <summary>
        /// マネージャークラスから呼ばれる初期化メソッド
        /// </summary>
        public void Configure() => ConfigureAsync(CancellationTokenSource.Token).Forget();
        
        /// <summary>
        /// 実際に派生クラスが初期化するのに必要な初期化メソッド
        /// </summary>
        protected abstract UniTask ConfigureAsync(CancellationToken token);

        /// <summary>
        /// ステートを抜けるときに呼ばれるメソッド
        /// </summary>
        public virtual async UniTask ExitAsync(){}

        /// <summary>
        /// 購読処理を共通化
        /// </summary>
        protected void Subscribe<T>(Observable<T> source, Action<T> onNext)
        {
            source.Subscribe(onNext).AddTo(CompositeDisposables);
        }

        public void Dispose()
        {
            CompositeDisposables?.Dispose();
            _onTransitionToNextStateSubject?.Dispose();
            
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
        }
    }
}