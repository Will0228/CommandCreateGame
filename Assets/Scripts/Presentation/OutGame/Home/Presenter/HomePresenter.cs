using Application.Home;
using R3;
using Root.DI;
using Shared.Attributes;

namespace Presenter.OutGame
{
    public sealed class HomePresenter : IHomePresenter
    {
        private readonly HomeView _view;
        
        Observable<Unit> IHomePresenter.ButtonClickedAsObservable => _view.ButtonClickedAsObservable;
        
        [Inject]
        public HomePresenter(IResolver resolver)
        {
            _view = resolver.Resolve<HomeView>();
        }
    }
}