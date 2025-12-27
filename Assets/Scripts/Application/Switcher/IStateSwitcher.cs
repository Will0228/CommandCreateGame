using Application.Base;

namespace Application.Switcher
{
    public interface IStateSwitcher
    {
        void SetFirstState(StateBase nextState);
    }
}