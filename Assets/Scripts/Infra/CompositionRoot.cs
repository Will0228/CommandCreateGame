
using UnityEngine;

namespace Infra
{
    /// <summary>
    /// ゲームの起点
    /// DIの準備などを行う
    /// </summary>
    public sealed class CompositionRoot
    {
        // private static readonly DIContainer _container = new();
        // private static TestInstaller _testInstaller;
        //
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        // private static void OnBeforeSceneLoad()
        // {
        //     _testInstaller = new();
        //     _testInstaller.Install(_container);
        //
        //     _container.WarmUp();
        //     var test = _container.Resolve<TestUseCase>();
        // }
    }
}