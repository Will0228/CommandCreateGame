using System.Threading;
using Application.Base;
using Cysharp.Threading.Tasks;
using Root.DI;

namespace Application.Home
{
    public sealed class HomeInitializeState : InitializeStateBase
    {
        public HomeInitializeState(IResolver resolver) : base(resolver)
        {
        }

        protected override　async UniTask ConfigureAsync(CancellationToken token)
        {
            SetNextState(Resolver.Resolve<HomeUserPlayableState>());
        }
    }
}

