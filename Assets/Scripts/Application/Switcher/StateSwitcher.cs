using System.Threading;
using Application.Base;
using Cysharp.Threading.Tasks;
using R3;

namespace Application.Switcher
{
    /// <summary>
    /// ステート管理を行うマネージャークラス
    /// </summary>
    public sealed class StateSwitcher : IStateSwitcher
    {
        private StateBase _currentState;

        private CancellationTokenSource _cts = new();

        /// <summary>
        /// 次のステートに遷移する
        /// </summary>
        private async UniTask SetNextStateAsync(StateBase nextState)
        {
            await _currentState.ExitAsync();
            
            // 前のステートの購読を消すためにCTSを新しくする
            ResetCancellationTokenSource();
            
            // 前ステートの購読処理などをDisposeする
            _currentState.Dispose();
            NextStateSettings(nextState);
        }

        public void SetFirstState(StateBase nextState) => NextStateSettings(nextState);

        private void NextStateSettings(StateBase nextState)
        {
            // ステート更新
            _currentState = nextState;
            _currentState.Configure();
            
            // 次のステートが新しいステートに更新したい場合再度このメソッドを呼ぶ
            _currentState.TransitionToNextStateAsObservable
                .Subscribe(state => SetNextStateAsync(state).Forget())
                .AddTo(_cts.Token);
        }

        private void ResetCancellationTokenSource()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
        }
    }
}