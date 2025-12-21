using VContainer;
using VContainer.Unity;

namespace Application.Base
{
    /// <summary>
    /// ユーザーが操作可能なステートの基底クラス
    /// </summary>
    public abstract class UserPlayableStateBase : StateBase
    {
        [Inject]
        public UserPlayableStateBase(IObjectResolver resolver) : base(resolver)
        {
            
        }
        
        /// <summary>
        /// このクラスで完結する購読処理
        /// </summary>
        protected abstract void SetEvent();
        
        /// <summary>
        /// 別クラスが関係する購読処理
        /// </summary>
        protected abstract void Bind();
    }
}