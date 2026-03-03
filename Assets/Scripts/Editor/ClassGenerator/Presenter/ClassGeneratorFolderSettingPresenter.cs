using UnityEngine;
using Model = Editor.ClassGenerator.ClassGeneratorFolderSettingModel;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingPresenter
    {
        private readonly ClassGeneratorFolderSettingViewContainer _viewContainer;
        private readonly Model _model;
        
        internal ClassGeneratorFolderSettingPresenter(ClassGeneratorFolderSettingViewContainer viewContainer, Model model)
        {
            _viewContainer = viewContainer;
            _model = model;
        }
        
        internal void Draw(Rect windowPosition) => _viewContainer.Draw(windowPosition, _model.LayerPathDict, _model.FolderPathDict);
    }
}