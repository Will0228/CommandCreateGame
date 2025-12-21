using System.Threading;
using Application.Base;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Application.Home
{
    public sealed class HomeInitializeState : InitializeStateBase
    {
        public HomeInitializeState(IObjectResolver resolver) : base(resolver)
        {
        }

        protected override　async UniTask ConfigureAsync(CancellationToken token)
        {
            SetNextState(Resolver.Resolve<HomeUserPlayableState>());
        }
    }
}

