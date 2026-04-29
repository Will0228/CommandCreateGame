using System.Collections.Generic;

namespace Editor.ClassGenerator
{
    internal sealed class ClassKeyFactory
    {
        private readonly List<ClassKey> _tempList = new();
        
        internal IReadOnlyList<ClassKey> Creates(ClassGeneratorModel.LayerSettings settings)
        {
            _tempList.Clear();
            foreach (var className in settings.ClassNames)
            {
                _tempList.Add(new ClassKey($"{className}{settings.Suffix}", settings.Type));
            }
            return _tempList;
        }
    }
}