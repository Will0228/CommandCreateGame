using UnityEngine;
using View = Editor.ClassGenerator.ClassGeneratorFolderSettingView;
using Model = Editor.ClassGenerator.ClassGeneratorFolderSettingModel;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingPresenter
    {
        private readonly View _view;
        private readonly Model _model;
        
        internal ClassGeneratorFolderSettingPresenter(View view, Model model)
        {
            _view = view;
            _model = model;
        }
        
        internal void Draw(Rect windowPosition) => _view.Draw(windowPosition, _model.LayerPathDict, _model.FolderPathDict);
    }
}