using VContainer;
using VContainer.Unity;

namespace Application.Base
{
    /// <summary>
    /// ユーザーが操作可能かつ毎フレーム動くステートの基底クラス
    /// </summary>
    public abstract class UserPlayableUpdateStateBase : UserPlayableStateBase, ITickable
    {
        protected UserPlayableUpdateStateBase(IObjectResolver resolver) : base(resolver)
        {
        }


        public abstract void Tick();
    }
}

