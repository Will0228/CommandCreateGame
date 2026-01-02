using Shared.DependencyContext;

namespace Shared.DI
{
    /// <summary>
    /// DIの登録を行うインタフェース
    /// </summary>
    public interface IRegister
    {
        void Register<TInterface, TClass>(Lifetime lifetime) where TClass : TInterface;
        void Register<TClass>(Lifetime lifetime) where TClass : class;
        void RegisterEntryPoint<TClass>(Lifetime lifetime) where TClass : class;
        void RegisterParentDependencyContext(DependencyContextBase instance);
    }
}