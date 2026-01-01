namespace Root.EntryPointInterface
{
    /// <summary>
    /// DIされるクラスかつ、Awakeメソッドを呼び出したいクラス
    /// </summary>
    public interface IInitializable
    {
        void ManualInitialize();
    }
}