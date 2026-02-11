using Shared.DI;
using UnityEngine;

namespace Shared
{
    /// <summary>
    /// ゲーム開始時にクラスのレシピを管理するクラスを作成する
    /// DependencyContextが作成されるときにレシピを渡す
    /// </summary>
    internal static class DIInitializer
    {
        public static RegistrationRegistry RegistrationRegistry { get; private set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            RegistrationRegistry = new RegistrationRegistry();
        }
    }
}