using System;
using System.Collections.Generic;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    /// <summary>
    /// ClassGeneratorFolderSettingLayerCellViewの機能に加えて
	/// 個別にフォルダを設定するかどうかを決定するボタン機能がついているCellView
    /// </summary>
    internal sealed class ClassGeneratorFolderAvailableSeparateSettingsCellView : IDisposable
    {
        private readonly ClassGeneratorFolderSettingLayerCellView _baseCellView;
        
        // 個別設定をするかどうかの購読
        private readonly Subject<AppLayerType> _onLayerSeparateSettingsSubject = new();
        public Observable<AppLayerType> OnLayerSeparateSettingsAsObservable => _onLayerSeparateSettingsSubject;
        
        public Observable<Enum> OnLayerPathSettingButtonClickedAsObservable => _baseCellView.OnLayerPathSettingButtonClickedAsObservable;
        
        internal ClassGeneratorFolderAvailableSeparateSettingsCellView()
        {
            _baseCellView = new ClassGeneratorFolderSettingLayerCellView();
        }

        internal void Configure(bool isSeparateSettings,
            Color backgroundColor,
            Enum cachedSelectedLayer,
            KeyValuePair<AppLayerType, string> kvp,
            float sectionRectWidth,
            int labelLayerViewWidth,
            int separateSettingViewWidth)
        {
            _baseCellView.Configure(isSeparateSettings, backgroundColor, cachedSelectedLayer, kvp, sectionRectWidth, labelLayerViewWidth, separateSettingViewWidth);
            
            if (GUILayout.Button("個別に設定", GUILayout.Width(separateSettingViewWidth)))
            {
                _onLayerSeparateSettingsSubject.OnNext(kvp.Key);
            }
        }

        public void Dispose()
        {
            _baseCellView?.Dispose();
            _onLayerSeparateSettingsSubject.Dispose();
        }
    }
}