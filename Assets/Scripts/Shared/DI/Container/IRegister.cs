using Shared.DependencyContext;
using UnityEngine;

namespace Shared.DI
{
    /// <summary>
    /// DIの登録を行うインタフェース
    /// </summary>
    public interface IRegister
    {
        void WarmUp();
        void Register<TInterface, TClass>(Lifetime lifetime) where TClass : class, TInterface;
        void Register<TClass>(Lifetime lifetime) where TClass : class;
        void RegisterEntryPoint<TClass>(Lifetime lifetime) where TClass : class;
        void RegisterComponent<TClass>(TClass instance) where TClass : MonoBehaviour;
        // RegistrationBuilder Register<TClass>(Lifetime lifetime) where TClass : class;
    }
}