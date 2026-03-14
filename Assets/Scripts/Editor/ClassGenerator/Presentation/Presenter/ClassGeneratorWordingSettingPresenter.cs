using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingPresenter : IDisposable
    {
        private readonly ClassGeneratorWordingSettingViewContainer _viewContainer;
        private readonly ClassGeneratorWordingSettingTextAreaModel _textAreaModel;

        internal ClassGeneratorWordingSettingPresenter(CompositeDisposable disposable)
        {
            _textAreaModel = new ClassGeneratorWordingSettingTextAreaModel();
            _viewContainer = new ClassGeneratorWordingSettingViewContainer(_textAreaModel.ImplementationDetailsInfo);

            Bind(disposable);
        }

        private void Bind(CompositeDisposable disposable)
        {
            _textAreaModel.UpdateContentTextTypeAsObservable
                .Subscribe()
                .AddTo(disposable);
        }

        /// <summary>
        /// 作成したいクラスが増えている可能性があるためタブを表示するたびに更新を行う
        /// </summary>
        internal void UpdateData(IReadOnlyList<ClassGeneratorModel.LayerSettings> settingsList)
        {
            _textAreaModel.UpdateData(settingsList);
        }

        internal void Draw(Rect windowPosition)
        {
            _viewContainer.Draw(windowPosition);
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)_textAreaModel).Dispose();
        }
    }
}