using System;

namespace Root.DI
{
    /// <summary>
    /// DI用クラスの依存解決を行うインタフェース
    /// </summary>
    public interface IResolver
    {
        T Resolve<T>() where T : class;
        object Resolve(Type type);
        
#if UNITY_EDITOR
        string GetParentsResolver();
#endif
    }
}