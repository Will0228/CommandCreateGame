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

        protected override async UniTask ConfigureAsync(CancellationToken token)
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