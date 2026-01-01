using Shared.DependencyContext;

namespace Shared.DI
{
    /// <summary>
    /// DIの登録を行うインタフェース
    /// </summary>
    public interface IRegister
    {
        void Register<TInterface, TClass>() where TClass : TInterface;
        void Register<TClass>() where TClass : class;
        void RegisterEntryPoint<TClass>() where TClass : class;
        void RegisterParentDependencyContext(DependencyContextBase instance);
    }
}