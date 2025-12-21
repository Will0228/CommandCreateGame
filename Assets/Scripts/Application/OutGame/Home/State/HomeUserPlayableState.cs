using System.Threading;
using Application.Base;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Application.Home
{
    public sealed class HomeUserPlayableState : UserPlayableStateBase
    {
        public HomeUserPlayableState(IObjectResolver resolver) : base(resolver)
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

