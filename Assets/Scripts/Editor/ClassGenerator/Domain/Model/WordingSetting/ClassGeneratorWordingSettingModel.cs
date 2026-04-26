using System;
using R3;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingModel : IDisposable
    {
        private readonly ReactiveProperty<int> _currentSelectedTabIndexProp = new();
        public ReadOnlyReactiveProperty<int>  CurrentSelectedTabIndex => _currentSelectedTabIndexProp;

        internal void UpdateTab(int selectedTabIndex)
        {
            _currentSelectedTabIndexProp.Value = selectedTabIndex;
        }


        void IDisposable.Dispose()
        {
            _currentSelectedTabIndexProp.Dispose();
        }
    }
}