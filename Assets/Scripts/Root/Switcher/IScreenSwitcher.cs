using Cysharp.Threading.Tasks;
using Shared.Screen;

namespace Root.Switcher
{
    public interface IScreenSwitcher
    {
        UniTaskVoid SetNextScreenAsync(IScreen nextScreen);
        UniTaskVoid SetFirstScreenAsync(IScreen nextScreen);
    }
}