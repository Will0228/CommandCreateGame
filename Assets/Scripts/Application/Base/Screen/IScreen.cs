using System;
using R3;
using VContainer;

namespace Application.Base
{
    public interface IScreen : IDisposable
    {
        public IObjectResolver Resolver { get; }
        
        /// <summary>
        /// オブジェクトを生成するのに必要なAddressKey
        /// </summary>
        public string AddressKey { get; }
        
        /// <summary>
        /// そのスクリーンクラスが管理すべきLifetimeScopeのType
        /// </summary>
        public Type LifetimeScopeType { get; }
        
        public Type FirstTransitionStateType { get; }

        /// <summary>
        /// 次のスクリーンに遷移することを伝えるObservable
        /// </summary>
        public Observable<ScreenBase> NextScreenTransition { get; }
        
        public void SetLifetimeContainer(IObjectResolver resolver);
    }
}