using System;
using Application.Home;
using Application.Scene;

namespace MyNamespace
{
    /// <summary>
    /// アウトゲームシーン
    /// </summary>
    public sealed class OutGameScene : IScene
    {
        public string SceneName => "OutGameScene";
        
        public Type FirstScreenType => typeof(HomeScreen);
    }
}