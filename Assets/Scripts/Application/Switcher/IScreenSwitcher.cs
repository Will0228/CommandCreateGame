using Application.Base;
using Cysharp.Threading.Tasks;

namespace Application.Switcher
{
    public interface IScreenSwitcher
    {
        UniTaskVoid SetNextScreenAsync(IScreen nextScreen);
        UniTaskVoid SetFirstScreenAsync(IScreen nextScreen);
    }
}