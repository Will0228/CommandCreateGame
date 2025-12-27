using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Application.Switcher
{
    public enum SceneType
    {
        None,
        OutGame,
        InGame,
    }
    
    /// <summary>
    /// シーンの切り替えをするSwitcher
    /// </summary>
    public sealed class SceneSwitcher : ISceneSwitcher
    {
        private Dictionary<SceneType, string> _sceneDict = new()
        {
            { SceneType.OutGame, "OutGameScene" },
        };

        public void ChangeScene(SceneType sceneType)
        {
            if (_sceneDict.TryGetValue(sceneType, out var sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}