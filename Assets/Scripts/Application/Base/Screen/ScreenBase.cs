using System;
using R3;
using VContainer;

namespace Application.Base
{
    /// <summary>
    /// スクリーンの基底クラス
    /// </summary>
    public abstract class ScreenBase : IScreen
    {
        public IObjectResolver Resolver { get; private set; }

        /// <summary>
        /// スクリーンを生成するのに必要なAddressKey
        /// </summary>
        public abstract string AddressKey { get; }
        
        public abstract Type LifetimeScopeType { get; }
        public abstract Type FirstTransitionStateType { get; }

        private readonly Subject<ScreenBase> _onNextScreenTransitionSubject = new();
        public Observable<ScreenBase> NextScreenTransition => _onNextScreenTransitionSubject;
        
        public void SetLifetimeContainer(IObjectResolver resolver) => Resolver = resolver;


        public void Dispose()
        {
            _onNextScreenTransitionSubject?.Dispose();
        }
    }
}