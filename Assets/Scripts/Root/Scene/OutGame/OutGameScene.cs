using System;
using Application.Scene;
using Root.Screen;

namespace Root.Scene
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