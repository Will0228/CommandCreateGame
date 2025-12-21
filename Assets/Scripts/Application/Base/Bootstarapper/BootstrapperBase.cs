using Unity.VisualScripting;
using VContainer;
using VContainer.Unity;

namespace Application.Base
{
    /// <summary>
    /// 該当シーンの始目の行動を行うBootstrapperの基底クラス
    /// あくまで最初にどのスクリーンから始めるかだけを宣言してManagerクラスに情報を渡す
    /// </summary>
    public abstract class BootstrapperBase : IPostInitializable
    {
        [Inject]
        protected BootstrapperBase(IObjectResolver resolver)
        {
        }

        public abstract void PostInitialize();
    }
}