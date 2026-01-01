using VContainer;
using VContainer.Unity;

namespace Application.OutGame
{
    /// <summary>
    /// アウトゲームで共通として使えるLifetimeScope
    /// </summary>
    public class OutGameLifetimeScope : LifetimeScope
    {
        // protected override void Configure(IContainerBuilder builder)
        // {
        //     base.Configure(builder);
        //     
        //     builder.RegisterEntryPoint<OutGameBootstrapper>();
        // }
    }
}
