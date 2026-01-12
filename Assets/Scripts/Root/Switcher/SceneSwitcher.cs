using System;
using System.Collections.Generic;
using Application.Switcher;
using Cysharp.Threading.Tasks;
using Root.DI;
using Root.Factory;
using Shared.Attributes;
using Shared.DependencyContext;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Root.Switcher
{
    public struct SceneInfo
    {
        public string SceneName;
        public Type SceneDependencyContextType;

        public SceneInfo(string sceneName, Type sceneDependencyContextType)
        {
            SceneName = sceneName;
            SceneDependencyContextType = sceneDependencyContextType;
        }
    }
    
    /// <summary>
    /// シーンの切り替えをするSwitcher
    /// </summary>
    public sealed class SceneSwitcher : ISceneSwitcher
    {
        private readonly DependencyContextFactory _dependencyContextFactory;
        private readonly IScreenSwitcher _screenSwitcher;
        
        [Inject]
        public SceneSwitcher(IResolver resolver)
        {
            _dependencyContextFactory = resolver.Resolve<DependencyContextFactory>();
            _screenSwitcher = resolver.Resolve<IScreenSwitcher>();
        }
        
        private Dictionary<SceneType, SceneInfo> _sceneDict = new()
        {
            { SceneType.OutGame, new SceneInfo("OutGameScene", typeof(OutGameDependencyContext)) },
            { SceneType.InGame , new SceneInfo("InGameScene", typeof(InGameDependencyContext)) }
        };

        async UniTask ISceneSwitcher.ChangeSceneAsync(SceneType sceneType)
        {
            if (_sceneDict.TryGetValue(sceneType, out var sceneInfo))
            {
                await SceneManager.LoadSceneAsync(sceneInfo.SceneName).ToUniTask();
                var sceneRootDependencyContext = _dependencyContextFactory.CreateSceneDependencyContext(sceneInfo.SceneDependencyContextType);
                _screenSwitcher.SetSceneRootDependencyContext(sceneRootDependencyContext);
                sceneRootDependencyContext.ManualBuild();
            }
        }
    }
}