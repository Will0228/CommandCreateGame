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

#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        protected override　async UniTask ConfigureAsync(CancellationToken token)
#pragma warning restore CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        {
            SetNextState(Resolver.Resolve<HomeUserPlayableState>());
        }
    }
}

