namespace Application.DI
{
    /// <summary>
    /// DI用クラスの依存解決を行うインタフェース
    /// </summary>
    public interface IResolver
    {
        T Resolve<T>() where T : class;
    }
}