using System;
using Shared.Screen;

namespace Root.Screen
{
    public sealed class InGameReadyScreen : ScreenBase
    {
        public override string AddressKey => "ReadyScreen";
        public override Type DependencyContextType { get; }
        public override Type FirstTransitionStateType { get; }
    }
}