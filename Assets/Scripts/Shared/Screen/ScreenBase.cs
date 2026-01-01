using System;
using R3;
using Root.DI;

namespace Shared.Screen
{
    /// <summary>
    /// スクリーンの基底クラス
    /// </summary>
    public abstract class ScreenBase : IScreen
    {
        public IResolver Resolver { get; private set; }

        /// <summary>
        /// スクリーンを生成するのに必要なAddressKey
        /// </summary>
        public abstract string AddressKey { get; }
        
        public abstract Type DependencyContextType { get; }
        public abstract Type FirstTransitionStateType { get; }

        private readonly Subject<ScreenBase> _onNextScreenTransitionSubject = new();
        public Observable<ScreenBase> NextScreenTransition => _onNextScreenTransitionSubject;
        
        public void SetDependencyContext(IResolver resolver) => Resolver = resolver;


        public void Dispose()
        {
            _onNextScreenTransitionSubject?.Dispose();
        }
    }
}