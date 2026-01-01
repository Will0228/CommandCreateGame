using System;
using R3;
using Root.DI;

namespace Shared.Screen
{
    public interface IScreen : IDisposable
    {
        public IResolver Resolver { get; }
        
        /// <summary>
        /// オブジェクトを生成するのに必要なAddressKey
        /// </summary>
        public string AddressKey { get; }
        
        /// <summary>
        /// そのスクリーンクラスが管理すべきLifetimeScopeのType
        /// </summary>
        public Type DependencyContextType { get; }
        
        public Type FirstTransitionStateType { get; }

        /// <summary>
        /// 次のスクリーンに遷移することを伝えるObservable
        /// </summary>
        public Observable<ScreenBase> NextScreenTransition { get; }
        
        public void SetDependencyContext(IResolver resolver);
    }
}