using Root.DI;
using Shared.Attributes;

namespace Application.Base
{
    /// <summary>
    /// その画面に遷移して初めて何かを行うステートの基底クラス
    /// </summary>
    public abstract class InitializeStateBase : StateBase
    {
        [Inject]
        public InitializeStateBase(IResolver resolver) : base(resolver)
        {
        }
    }
}
