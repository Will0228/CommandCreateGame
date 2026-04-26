using R3;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorCommonPresenter
    {
        private readonly ClassGeneratorCommonView _view;
        private readonly ClassGeneratorCommonModel _model;
        
        private readonly CompositeDisposable _disposables = new ();
        
        public Observable<Unit> OnCreateButtonClickedAsObservable => _view.OnCreateButtonClickedAsObservable;

        [EditorInject]
        public ClassGeneratorCommonPresenter(ClassGeneratorSimpleDIContainer container)
        {
            _view = container.Resolve<ClassGeneratorCommonView>();
            _model = container.Resolve<ClassGeneratorCommonModel>();
            
            Bind();
        }

        private void Bind()
        {
            _view.OnChangedCategoryIndexAsObservable
                .Subscribe(_model.SaveCategoryIndex)
                .AddTo(_disposables);
        }
        
        public void Draw() => _view.Draw(_model.SelectedCategoryIndex);
    }
}