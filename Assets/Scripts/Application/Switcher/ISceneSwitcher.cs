using Cysharp.Threading.Tasks;

namespace Application.Switcher
{
    public enum SceneType
    {
        None,
        OutGame,
        InGame,
    }
    
    public interface ISceneSwitcher
    {
        UniTask ChangeSceneAsync(SceneType sceneType);
    }
}