using Root.DI;
using Root.EntryPointInterface;
using Shared.Attributes;

namespace Shared.Bootstrapper
{
    /// <summary>
    /// 該当シーンの始目の行動を行うBootstrapperの基底クラス
    /// あくまで最初にどのスクリーンから始めるかだけを宣言してSwitcherクラスに情報を渡す
    /// </summary>
    public abstract class BootstrapperBase : IInitializable
    {
        [Inject]
        protected BootstrapperBase(IResolver resolver)
        {
        }

        public virtual void ManualInitialize()
        {
            
        }
    }
}