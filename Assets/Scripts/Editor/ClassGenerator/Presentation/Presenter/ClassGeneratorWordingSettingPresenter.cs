using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingPresenter : IDisposable
    {
        private readonly ClassGeneratorWordingSettingViewContainer _viewContainer;
        private readonly ClassGeneratorWordingSettingTextAreaModel _textAreaModel;
        private readonly ClassGeneratorWordingSettingModel _model;
        private readonly ClassIdFactory _classIdFactory;
        
        
        private readonly CompositeDisposable _disposable = new();
        
        [EditorInject]
        public ClassGeneratorWordingSettingPresenter(ClassGeneratorSimpleDIContainer container)
        {
            _textAreaModel = container.Resolve<ClassGeneratorWordingSettingTextAreaModel>();
            _viewContainer = container.Resolve<ClassGeneratorWordingSettingViewContainer>();
            _model = container.Resolve<ClassGeneratorWordingSettingModel>();
            _classIdFactory = container.Resolve<ClassIdFactory>();

            Bind();
        }

        internal void Configure(Rect windowPosition)
        {
            _viewContainer.Configure(windowPosition, _textAreaModel.ImplementationDetailsInfo);
        }

        private void Bind()
        {
            _textAreaModel.UpdateClassInfosAsObservable
                .Subscribe(_viewContainer.UpdateData)
                .AddTo(_disposable);
            
            _model.CurrentSelectedTabIndex
                .Subscribe(_viewContainer.ChangeTab)
                .AddTo(_disposable);
        }

        /// <summary>
        /// 作成したいクラスが増えている可能性があるためタブを表示するたびに更新を行う
        /// </summary>
        internal void UpdateData(IReadOnlyList<ClassGeneratorModel.LayerSettings> settingsList)
        {
            _textAreaModel.UpdateData(settingsList.ToDictionary(settings => _classIdFactory.Create(settings), settings => settings.Type));
        }

        internal void Draw()
        {
            _viewContainer.Draw();
        }

        void IDisposable.Dispose()
        {
            _disposable.Dispose();
        }
    }
}