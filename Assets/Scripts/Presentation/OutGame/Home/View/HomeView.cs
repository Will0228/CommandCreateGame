using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Presenter.OutGame
{
    public sealed class HomeView : MonoBehaviour
    {
        [SerializeField] private Button _button;

        public Observable<Unit> ButtonClickedAsObservable => _button.OnClickAsObservable();
    }
}