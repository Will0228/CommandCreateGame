using Application.Test;
using Infra.DI;
using Infra.Installer;
using UnityEngine;

namespace Infra
{
    /// <summary>
    /// ゲームの起点
    /// DIの準備などを行う
    /// </summary>
    public sealed class CompositionRoot
    {
        private static readonly DIContainer _container = new();
        private static TestInstaller _testInstaller;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            _testInstaller = new();
            _testInstaller.Install(_container);

            var testUseCase = new TestUseCase();
            _container.Inject(testUseCase);
            
            Debug.Log($"useCaseが持っているDIされた型は : {testUseCase.Repository}");
        }
    }
}