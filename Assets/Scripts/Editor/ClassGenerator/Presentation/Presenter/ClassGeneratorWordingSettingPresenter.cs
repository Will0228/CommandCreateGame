using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingPresenter
    {
        private readonly ClassGeneratorWordingSettingViewContainer _viewContainer;
        private readonly ClassGeneratorWordingSettingTextAreaModel _textAreaModel;

        internal ClassGeneratorWordingSettingPresenter()
        {
            _viewContainer = new ClassGeneratorWordingSettingViewContainer();
            _textAreaModel = new ClassGeneratorWordingSettingTextAreaModel();
            
            Configure();
        }
        
        private void Configure()
        {
            _textAreaModel.Initialize();
            _viewContainer.Configure(_textAreaModel.ImplementationText);
        }

        internal void Draw(Rect windowPosition)
        {
            _viewContainer.Draw(windowPosition);
        }
    }
}