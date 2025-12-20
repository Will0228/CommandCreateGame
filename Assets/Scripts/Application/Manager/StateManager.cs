using System.Threading;
using Application.Base;
using Cysharp.Threading.Tasks;
using R3;

namespace Application.StateManager
{
    /// <summary>
    /// ステート管理を行うマネージャークラス
    /// </summary>
    public sealed class StateManager
    {
        private StateBase _currentState;

        private CancellationTokenSource _cts = new();

        /// <summary>
        /// 次のステートに遷移する
        /// </summary>
        private void SetNextState(StateBase nextState)
        {
            // 前のステートの購読を消すためにCTSを新しくする
            ResetCancellationTokenSource();
            
            // 前ステートの購読処理などをDisposeする
            _currentState.Dispose();
            
            // ステート更新
            _currentState = nextState;
            
            // 次のステートが新しいステートに更新したい場合再度このステートを呼ぶ
            _currentState.TransitionToNextStateAsObservable
                .Subscribe(SetNextState)
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