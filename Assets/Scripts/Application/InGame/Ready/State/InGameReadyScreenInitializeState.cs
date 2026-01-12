using System.Threading;
using Application.Base;
using Cysharp.Threading.Tasks;
using Root.DI;

namespace Application.InGame
{
    public sealed class InGameReadyScreenInitializeState : InitializeStateBase
    {
        public InGameReadyScreenInitializeState(IResolver resolver) : base(resolver)
        {
        }

        protected override async UniTask ConfigureAsync(CancellationToken token)
        {
            SetNextState(Resolver.Resolve<InGameReadyScreenUserPlayableState>());
        }
    }
}