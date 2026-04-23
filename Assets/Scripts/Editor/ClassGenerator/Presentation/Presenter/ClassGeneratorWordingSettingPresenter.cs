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
        private readonly ClassGeneratorWordingSettingCommonModel _commonModel;
        private readonly ClassIdFactory _classIdFactory;

        internal ClassGeneratorWordingSettingPresenter(Rect windowPosition, CompositeDisposable disposable)
        {
            _textAreaModel = new ();
            _viewContainer = new (windowPosition, _textAreaModel.ImplementationDetailsInfo);
            _commonModel = new();
            _classIdFactory = new ();

            Bind(disposable);
        }

        private void Bind(CompositeDisposable disposable)
        {
            _textAreaModel.UpdateClassInfosAsObservable
                .Subscribe(_viewContainer.UpdateData)
                .AddTo(disposable);
            
            _commonModel.CurrentSelectedTabIndex
                .Subscribe(_viewContainer.ChangeTab)
                .AddTo(disposable);
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
            ((IDisposable)_textAreaModel).Dispose();
            ((IDisposable)_commonModel).Dispose();
        }
    }
}