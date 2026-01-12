using Cysharp.Threading.Tasks;
using Shared.DependencyContext;
using Shared.Screen;

namespace Application.Switcher
{
    public interface IScreenSwitcher
    {
        UniTaskVoid SetNextScreenAsync(IScreen nextScreen);
        UniTaskVoid SetFirstScreenAsync(IScreen nextScreen);

        void SetSceneRootDependencyContext(DependencyContextBase sceneRootDependencyContext);
    }
}