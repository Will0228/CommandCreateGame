namespace Root.EntryPointInterface
{
    /// <summary>
    /// DIされるクラスかつ、Updateメソッドを呼び出したいクラス
    /// </summary>
    public interface IUpdatable
    {
        void ManualUpdate();
    }
}