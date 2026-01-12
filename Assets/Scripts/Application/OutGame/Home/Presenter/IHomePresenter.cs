using R3;

namespace Application.Home
{
    public interface IHomePresenter
    {
        Observable<Unit> ButtonClickedAsObservable { get; }
    }
}