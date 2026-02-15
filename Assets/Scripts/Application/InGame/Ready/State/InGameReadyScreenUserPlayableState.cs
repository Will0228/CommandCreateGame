using System.Threading;
using Application.Base;
using Cysharp.Threading.Tasks;
using Root.DI;

namespace Application.InGame
{
    public sealed class InGameReadyScreenUserPlayableState : UserPlayableStateBase
    {
        public InGameReadyScreenUserPlayableState(IResolver resolver) : base(resolver)
        {
        }

#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        protected override async UniTask ConfigureAsync(CancellationToken token)
#pragma warning restore CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        {
            
        }

        protected override void SetEvent()
        {
        }

        protected override void Bind()
        {
        }
    }
}