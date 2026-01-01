using System;
using Application.Base;
using Application.Home;
using Shared.DI;
using Shared.Screen;

namespace Root.Screen
{
    public sealed class HomeScreen : ScreenBase
    {
        public override string AddressKey => "HomeScreen";
        public override Type DependencyContextType => typeof(HomeDependencyContext);
        public override Type FirstTransitionStateType => typeof(HomeInitializeState);
    }
}