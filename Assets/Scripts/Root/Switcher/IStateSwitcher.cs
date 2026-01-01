using Application.Base;

namespace Root.Switcher
{
    public interface IStateSwitcher
    {
        void SetFirstState(StateBase nextState);
    }
}