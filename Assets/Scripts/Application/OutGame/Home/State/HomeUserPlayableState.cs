using System.Threading;
using Application.Base;
using Cysharp.Threading.Tasks;
using Root.DI;

namespace Application.Home
{
    public sealed class HomeUserPlayableState : UserPlayableStateBase
    {
        public HomeUserPlayableState(IResolver resolver) : base(resolver)
        {
        }

        protected override void SetEvent()
        {
            
        }

        protected override void Bind()
        {
        }

        protected override async UniTask ConfigureAsync(CancellationToken token)
        {
            
        }
    }
}

