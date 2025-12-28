namespace Application.DI
{
    /// <summary>
    /// DIの登録を行うインタフェース
    /// </summary>
    public interface IRegister
    {
        void Register<TInterface, TClass>() where TClass : TInterface;
    }
}