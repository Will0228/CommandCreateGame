using System;
using Application.Base;
using VContainer;

namespace Application.Home
{
    public sealed class HomeScreen : ScreenBase
    {
        public override string AddressKey => "HomeScreen";
        public override Type LifetimeScopeType => typeof(HomeLifetimeScope);
        public override Type FirstTransitionStateType => typeof(HomeInitializeState);
    }
}