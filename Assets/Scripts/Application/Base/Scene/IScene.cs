using System;

namespace Application.Scene
{
    /// <summary>
    /// シーンインタフェース
    /// </summary>
    public interface IScene
    {
        string SceneName { get; }
        
        Type FirstScreenType { get; }
    }
}