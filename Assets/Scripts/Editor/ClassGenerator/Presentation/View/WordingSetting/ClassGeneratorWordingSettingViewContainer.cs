using System;
using System.Collections.Generic;
using UnityEngine;

using CommonView = Editor.ClassGenerator.ClassGeneratorWordingSettingCommonView;
using ImplementationDetailsView = Editor.ClassGenerator.ClassGeneratorWordingSettingImplementationDetailsView;
using ClassRequirementView = Editor.ClassGenerator.ClassGeneratorWordingSettingClassRequirementView;

namespace Editor.ClassGenerator
{
    
    internal sealed class ClassGeneratorWordingSettingViewContainer : IDisposable
    {
        private enum WordingSettingViewType
        {
            None,
            ImplementationDetails, // 実装要望詳細
            RequirementsPerClass, // クラスごとの要望
        }
        
        private readonly CommonView _commonView;
        private readonly ImplementationDetailsView _implementationDetailsView;
        private readonly ClassRequirementView _classRequirementView;
        
        private WordingSettingViewType _viewType = WordingSettingViewType.ImplementationDetails;

        internal ClassGeneratorWordingSettingViewContainer(Rect windowPosition,
            ClassGeneratorWordingSettingInfo info)
        {
            _commonView = new CommonView();
            _implementationDetailsView = new ImplementationDetailsView(info);
            _classRequirementView = new ClassRequirementView(windowPosition);
        }

        internal void UpdateData(IReadOnlyList<ClassGeneratorWordingSettingClassInfo> infos)
        {
            _classRequirementView.UpdateData(infos);
        }
        
        internal void Draw()
        {
            switch (_viewType)
            {
                case WordingSettingViewType.ImplementationDetails:
                    _implementationDetailsView.Draw();
                    break;
                case WordingSettingViewType.RequirementsPerClass:
                    
            }

            ;
        }

        internal void ChangeTab(int index)
        {
            _viewType = (WordingSettingViewType)(index + 1);
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)_commonView).Dispose();
        }
    }
}