using Root.DI;
using Root.EntryPointInterface;

namespace Application.Base
{
    /// <summary>
    /// ユーザーが操作可能かつ毎フレーム動くステートの基底クラス
    /// </summary>
    public abstract class UserPlayableUpdateStateBase : UserPlayableStateBase, IUpdatable
    {
        protected UserPlayableUpdateStateBase(IResolver resolver) : base(resolver)
        {
        }


        public abstract void ManualUpdate();
    }
}

